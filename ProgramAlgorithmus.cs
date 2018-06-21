using namespaceStuktur;
using namespaceUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace namespaceAlgorithmus
{
    class Algorithmus
    {
        double balance;

        public void zeitOfAlgorithmus(string path, String methode, bool directed)
        {
            
            string[] word = Regex.Split(path, "/");

            Console.WriteLine(word[word.LongLength - 1]);

            Algorithmus algorithmus = new Algorithmus();

            Graph graph = Parse.getGraphByFile(path, directed);

            DateTime befor = System.DateTime.Now;

            if (methode == "cycleCancelling")
            {
                algorithmus.cycleCancelling(graph);
            }else{
                algorithmus.successiveShortestPath(graph);
            }

            DateTime after = System.DateTime.Now;
            TimeSpan ts = after.Subtract(befor);
            // Console.WriteLine("\n\n{0}s \n", ts.TotalSeconds);
            Console.WriteLine("\n", ts.TotalSeconds);
        }

       

        public void cycleCancelling(Graph graph)
        {
            Node superSource = createSuperSource(graph);
            balance = superSource.balance;

            Node superSink = createSuperSink(graph);

            List<Path> maximalFlowsList = findMaximalFlows(graph, superSource.id, superSink.id);

            double maxCapacity = getMaxFlussCapacity(maximalFlowsList);

            if (maxCapacity < superSource.balance)
            {
                Console.WriteLine("Keine KostenminimalFluss");
                return;
            }

            udpateGraphWithFlussList(graph, maximalFlowsList);

            removeSuperSource(graph, superSource);

            removeSuperSink(graph, superSink);

            Graph residualGraph =  createResidualGraph(graph);

            Path negativeCycle = findNegativeCycle(residualGraph);

            while (negativeCycle != null)
            {
                updateGraphWithNegativeCycle(graph, negativeCycle);

                residualGraph = createResidualGraph(graph);

                negativeCycle = findNegativeCycle(residualGraph);
            }

            double sum = calculateCosts(graph);

            Console.WriteLine("sum:" + sum);
        }


        public void udpateGraphWithFlussList(Graph graph, List<Path> flussList)
        {
            foreach (Path path in flussList)
            {
                foreach (Edge e in path.edgeList) {
                    Edge edge = graph.findEdge(e.id);
                    edge.flow = edge.flow + path.capacity;
                }
            }
        }


        public void updateGraphWithNegativeCycle(Graph graph, Path negativeCycle)
        {
            foreach (Edge e in negativeCycle.edgeList)
            {
                Edge forwardEdge = graph.findEdge(e.id);

                if (forwardEdge != null) {
                    forwardEdge.flow = forwardEdge.flow + negativeCycle.capacity;
                }

                Edge backwardEdge = graph.findEdge(e.rId);

                if (backwardEdge != null)
                {
                    backwardEdge.flow = backwardEdge.flow - negativeCycle.capacity;
                }

            }
        }

        //MooreBellmanFord
        public Path findNegativeCycle(Graph graph) {

            Path negativeCycle = null;

            foreach (Node node in graph.nodeList)
            {
                negativeCycle = negativeCycleFromMooreBellmanFord(graph,node.id);
                if (negativeCycle != null)
                {
                    break;
                }
            }
            return negativeCycle;
        }


        //MooreBellmanFord
        public Path negativeCycleFromMooreBellmanFord(Graph g,int id)
        {
            Graph graph = cloneGraph(g);
            //init
            foreach (Node node in graph.nodeList)
            {
                if (node.id == id)
                {
                    node.costs = 0;
                }
                else
                {
                    node.costs = Double.MaxValue;
                }
            }

            for (int i = 0; i < graph.nodeList.Count - 1; i++)
            {
                foreach (Edge e in graph.edgeList)
                {
                    double costs = e.startNode.costs + e.costs;

                    if (costs < e.endNode.costs)
                    {
                        e.endNode.previousNode = e.startNode;
                        e.endNode.costs = costs;
                    }
                }
            }

            Node nodeInGraph = null;

            foreach (Edge e in graph.edgeList)
            {
                if (e.startNode.costs < double.MaxValue)
                {
                    double costs = e.startNode.costs + e.costs;

                    if (costs < e.endNode.costs)
                    {
                        nodeInGraph = e.endNode;
                        break;
                    }
                }
            }

            if (nodeInGraph != null)
            {
                for (int i = 0; i < graph.nodeList.Count; i++)
                {
                    nodeInGraph = nodeInGraph.previousNode;
                }

                List<Edge> edges = new List<Edge>();
                for (int i = 0; i < graph.nodeList.Count; i++)
                {
                    Edge e = graph.findEdge(nodeInGraph.previousNode, nodeInGraph);

                    if (!edges.Contains(e))
                    {
                        edges.Add(e);
                    }

                    nodeInGraph = nodeInGraph.previousNode;
                }

                return new Path(edges);
            }

            return null;
        }

        //FordFulkerson
        public List<Path> findMaximalFlows(Graph g, int startId, int endId)
        {
            Graph graph = cloneGraph(g);

            List<Path> flussList = new List<Path>();

            Path fluss = BFS(graph, startId, endId);

            flussList.Add(fluss);

            Graph residualGraph = createResidualGraph(graph, fluss);

            while (fluss != null)
            {
                fluss = BFS(residualGraph, startId, endId);

                if (fluss != null)
                {
                    flussList.Add(fluss);

                    residualGraph = createResidualGraph(residualGraph, fluss);
                }
            }

            return flussList;
        }

        public Graph createResidualGraph(Graph graph, Path fluss)
        {
            foreach (Edge e in graph.edgeList.ToArray())
            {
                if (fluss.edgeList.Contains(e))
                {
                    e.capacity = e.capacity - fluss.capacity;

                    if (e.capacity == 0)
                    {
                        graph.removeEdge(e);
                    }
                    graph.createOrUpdateEdge(e.endNode, e.startNode, fluss.capacity, -e.costs, fluss.capacity);

                }
            }

            return graph;
        }

        
        public Path BFS(Graph graph, int startId, int endId)
        {
            graph.reset();

            Queue<Node> queue = new Queue<Node>();

            Node node = graph.nodeList[startId];
            node.visited = true;
            queue.Enqueue(node);

            while (queue.Count > 0)
            {
                node = queue.Dequeue();

                foreach (Node n in node.nodeList)
                {
                    if (!n.visited)
                    {
                        queue.Enqueue(n);
                        n.visited = true;
                        n.previousNode = node;
                    }
                }
            }

            Path fluss = getShortestPath(graph, endId);

            if (fluss != null)
            {
                if (balance > 0)
                {
                    if (balance < fluss.capacity)
                    {
                        fluss.capacity = balance;
                    }

                    balance = balance - fluss.capacity;
                }
                else { return null; }

            }

            return fluss;
        }








        public void successiveShortestPath(Graph graph)
        {

            foreach (Edge e in graph.edgeList)
            {
                if (e.costs < 0)
                {
                    e.flow = e.capacity;
                }
            }

            foreach (Node d in graph.nodeList)
            {
                d.pseudoBalance = graph.getPseudoBalance(d);
            }

            Graph residualGraph = createResidualGraph(graph);

            Node source = null;
            Node taget = null;
            Path shortestPath = null;

            findShortestPath(residualGraph, out source, out taget, out shortestPath);


            while (shortestPath != null)
            {
                double gamma = Math.Min(shortestPath.capacity, Math.Min(source.balance - source.pseudoBalance, taget.pseudoBalance - taget.balance));

                updateOrignalGraph(graph, shortestPath, gamma, source.id, taget.id);

                residualGraph = createResidualGraph(graph);

                findShortestPath(residualGraph, out source, out taget, out shortestPath);
            }

            foreach (Node node in graph.nodeList)
            {
                if (node.balance != node.pseudoBalance)
                {
                    Console.WriteLine("Keine KostenminimalFluss");
                    return;
                }
            }

            double sum = calculateCosts(graph);

            Console.WriteLine("sum:" + sum);
        }


        public void findShortestPath(Graph graph, out Node source, out Node taget, out Path shortestPath)
        {
            List<Node> sourceList = graph.getSource();

            foreach (Node s in sourceList)
            {
                Graph shortestPathTree = getSPTFromMooreBellmanFord(graph, s.id);

                Node t = getTargetFromBFS(shortestPathTree, s.id);

                if (t != null)
                {
                    source = s;
                    taget = t;
                    shortestPath = getShortestPath(shortestPathTree, t.id);
                    return;
                }
            }

            source = null;
            taget = null;
            shortestPath = null;

        }

        public Node getTargetFromBFS(Graph g, int startId)
        {
            Graph graph = cloneGraph(g);

            graph.reset();

            Queue<Node> queue = new Queue<Node>();

            Node node = graph.nodeList[startId];
            node.visited = true;
            queue.Enqueue(node);

            while (queue.Count > 0)
            {
                node = queue.Dequeue();

                foreach (Node n in node.nodeList)
                {
                    if (!n.visited)
                    {
                        queue.Enqueue(n);
                        n.visited = true;
                        n.previousNode = node;

                        if (n.balance < n.pseudoBalance)
                        {
                            return n;
                        }
                    }
                }
            }

            return null;
        }


        public void updateOrignalGraph(Graph graph, Path shortestPath, double gamma, int startId, int endId)
        {

            foreach (Edge e in graph.edgeList.ToArray())
            {
                foreach (Edge se in shortestPath.edgeList)
                {
                    if (e.id == se.id)
                    {
                        e.flow = e.flow + gamma;
                    }
                    else if (e.rId == se.id)
                    {
                        e.flow = e.flow - gamma;
                    }
                }
            }

            Node startNode = graph.nodeList[startId];
            Node endNode = graph.nodeList[endId];

            startNode.pseudoBalance = startNode.pseudoBalance + gamma;
            endNode.pseudoBalance = endNode.pseudoBalance - gamma;

        }

        //MooreBellmanFord
        public Graph getSPTFromMooreBellmanFord(Graph g, int startId)
        {
            Graph graph = cloneGraph(g);
            //init
            foreach (Node node in graph.nodeList)
            {
                if (node.id == startId)
                {
                    node.costs = 0;
                }else
                {
                    node.costs = Double.MaxValue;
                }
            }

            bool negativeCycle = false;

            for (int i = 0; i < graph.nodeList.Count - 1; i++)
            {
                foreach (Edge e in graph.edgeList)
                {
                    double weight = e.startNode.costs + e.costs;

                    if (weight < e.endNode.costs)
                    {
                        e.endNode.previousNode = e.startNode;
                        e.endNode.costs = weight;
                    }
                }
            }

            foreach (Edge e in graph.edgeList)
            {
                if (e.startNode.costs < double.MaxValue)
                {
                    double weight = e.startNode.costs + e.costs;

                    if (weight < e.endNode.costs)
                    {
                        negativeCycle = true;
                        break;
                    }
                }
            }

            if (negativeCycle)
            {
                Console.WriteLine("negativeCycle!!!");
            }
            return graph;
        }


        public Path getShortestPath(Graph graph, int endId)
        {
            List<Edge> edges = new List<Edge>();

            getPreviousNode(graph.nodeList[endId], graph, edges);

            Path fluss = null;
            if (edges != null && edges.Count > 0)
            {
                fluss = new Path(edges);
            }
            return fluss;
        }


        public void getPreviousNode(Node node, Graph graph, List<Edge> edges)
        {
            if (node.previousNode != null)
            {
                Edge e = graph.findEdge(node.previousNode, node);
                edges.Add(e);

                getPreviousNode(node.previousNode, graph, edges);
            }            
        }


        public Graph cloneGraph(Graph graph)
        {
            Graph g = new Graph();

            foreach (Node n in graph.nodeList)
            {
                Node node = new Node(n.id);
                node.costs = n.costs;

                node.balance = n.balance;
                node.pseudoBalance = n.pseudoBalance;
                g.nodeList.Add(node);
            }


            foreach (Edge e in graph.edgeList)
            {
                g.createOrUpdateEdge(g.nodeList[e.startNode.id], g.nodeList[e.endNode.id], e.capacity, e.costs, e.flow);
            }

            return g;
        }

        public void removeSuperSource(Graph graph, Node superSource)
        {
            foreach (Edge e in graph.edgeList.ToArray())
            {
                if (e.startNode.id == superSource.id || e.endNode.id == superSource.id)
                {
                    graph.removeEdge(e);
                }
            }
            graph.nodeList.Remove(superSource);
        }

        public void removeSuperSink(Graph graph, Node superSink)
        {
            foreach (Edge e in graph.edgeList.ToArray())
            {
                if (e.startNode.id == superSink.id || e.endNode.id == superSink.id)
                {
                    graph.removeEdge(e);
                }
            }
            graph.nodeList.Remove(superSink);
        }

        public double getMaxFlussCapacity(List<Path> flussList)
        {
            double maxCapacity = 0;

            foreach (Path f in flussList)
            {
                maxCapacity = maxCapacity + f.capacity;
            }

            return maxCapacity;
        }

        public Node createSuperSource(Graph graph)
        {
            Node superSource = new Node(graph.nodeList.Count);

            foreach (Node n in graph.nodeList)
            {
                if (n.balance > 0)
                {
                    superSource.balance = superSource.balance + n.balance;
                    graph.createOrUpdateEdge(superSource, n, n.balance, 0, 0);
                }
            }

            graph.nodeList.Add(superSource);

            return superSource;
        }

        public Node createSuperSink(Graph graph)
        {
            Node superSink = new Node(graph.nodeList.Count);

            foreach (Node n in graph.nodeList)
            {
                if (n.balance < 0)
                {
                    superSink.balance = superSink.balance + n.balance;
                    graph.createOrUpdateEdge(n, superSink, -n.balance, 0, 0);
                }
            }

            graph.nodeList.Add(superSink);
            return superSink;
        }

        public double calculateCosts(Graph graph)
        {
            double sum = 0;
            foreach (Edge e in graph.edgeList)
            {
                sum = sum + e.flow * e.costs;
            }
            return sum;
        }

        public Graph createResidualGraph(Graph g)
        {
            Graph graph = cloneGraph(g);

            foreach (Edge e in graph.edgeList.ToArray())
            {
                if (e.flow > 0)
                {
                    if (e.flow == e.capacity)
                    {
                        graph.removeEdge(e);
                    }
                    e.capacity = e.capacity - e.flow;
                    graph.createOrUpdateEdge(e.endNode, e.startNode, e.flow, -e.costs, 0);
                    e.flow = 0;
                }
            }

            return graph;
        }
    }
}