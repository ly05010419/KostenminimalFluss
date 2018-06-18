using namespaceStuktur;
using namespaceUtility;
using System;
using System.Collections;
using System.Collections.Generic;

namespace namespaceAlgorithmus
{
    class Algorithmus
    {
        double balance;
        public List<Node> unVisitList = new List<Node>();

        public void zeitOfAlgorithmus(string path, String methode, bool directed)
        {
            Console.WriteLine(methode);

            Algorithmus algorithmus = new Algorithmus();

            Graph graph = Parse.getGraphByFile(path, directed);

            DateTime befor = System.DateTime.Now;

            if (methode == "cycleCancelling")
            {
                algorithmus.cycleCancelling(graph);
            }
            else
            {
                algorithmus.SuccessiveShortestPath(graph);
            }

            DateTime after = System.DateTime.Now;
            TimeSpan ts = after.Subtract(befor);
            Console.WriteLine("\n\n{0}s \n", ts.TotalSeconds);
        }

        public void SuccessiveShortestPath(Graph graph)
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

            Node source = null;
            Node taget = null;

            Graph residualGraph = createResidualGraph(graph);

            Path shortestPath = getShortestPath(residualGraph, out source, out taget);


            while (shortestPath != null)
            {
                double gamma = Math.Min(shortestPath.capacity, Math.Min(source.balance - source.pseudoBalance, taget.pseudoBalance - taget.balance));

                updateOrignalGraph(graph, shortestPath, gamma, source.id, taget.id);

                residualGraph = createResidualGraph(graph);

                shortestPath = getShortestPath(residualGraph, out source, out taget);
            }

            foreach (Node node in graph.nodeList) {
                if (node.balance!=node.pseudoBalance) {
                    Console.WriteLine("Keine KostenminimalFluss");
                    return;
                }
            }

            double sum = kostenMinimalFluss(graph);

            Console.WriteLine("sum:" + sum);


        }

        public double kostenMinimalFluss(Graph graph)
        {
            double sum = 0;

            foreach (Edge e in graph.edgeList)
            {
                sum = sum + e.flow * e.costs;
            }

            return sum;

        }


        public Path getShortestPath(Graph residualGraph, out Node source, out Node taget)
        {
            List<Node> sourceList = residualGraph.getSource();
            
            source = null;
            taget = null;

            foreach (Node s in sourceList) {
                Graph shortestPathTree = MooreBellmanFord(residualGraph, s.id);

                Node t = getTargetFromBFS(shortestPathTree, s.id);

                if (t != null) {
                    source = s;
                    taget = t;
                    return getShortestPathFromID(shortestPathTree, t.id);
                }
            }

            return null;
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


        public void updateOrignalGraph(Graph graph, Path shortPath, double gamma, int startId, int endId)
        {

            foreach (Edge e in graph.edgeList.ToArray())
            {
                foreach (Edge se in shortPath.edgeList)
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

        


        public Graph MooreBellmanFord(Graph g, int startId)
        {
            Graph graph = cloneGraph(g);
            //init
            foreach (Node node in graph.nodeList)
            {
                if (node.id == startId)
                {
                    node.costs = 0;
                }
                else
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

            //negativeCycle checken!
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








        public void cycleCancelling(Graph g)
        {

            Graph graph = cloneGraph(g);

            Node superSource = createSuperSource(graph);
            balance = superSource.balance;

            Node superSink = createSuperSink(graph);

            List<Path> flussList = fordFulkerson(graph, superSource.id, superSink.id);

            double maxCapacity = getMaxFlussCapacity(flussList);

            if (maxCapacity < superSource.balance)
            {
                Console.WriteLine("Keine KostenminimalFluss");
                return;
            }

            removeSuperSource(graph, superSource);

            removeSuperSink(graph, superSink);


            Cycle negativeCycle = findCycleMooreBellmanFord(graph);


            while (negativeCycle != null)
            {

                graph = updateGraph(graph, negativeCycle);

                negativeCycle = findCycleMooreBellmanFord(graph);

            }

            double sum = capacityFormKostenMinimalFluss(graph, g);

            Console.WriteLine("sum:" + sum);

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

        public double capacityFormKostenMinimalFluss(Graph graph, Graph oldGraph)
        {
            double sum = 0;

            foreach (Edge e in oldGraph.edgeList)
            {
                Edge edge = graph.findEdge(e.startNode, e.endNode);
                if (edge != null)
                {
                    sum = sum + (e.capacity - edge.capacity) * e.costs;
                }
                else
                {
                    sum = sum + e.capacity * e.costs;
                }
            }

            return sum;
        }



        public Graph updateGraph(Graph graph, Cycle negativeCycle)
        {
            foreach (Edge e in negativeCycle.edgeList)
            {

                Edge forwardEdge = graph.findEdge(e.startNode, e.endNode);
                forwardEdge.capacity = forwardEdge.capacity - negativeCycle.capacity;


                if (forwardEdge.capacity == 0)
                {
                    graph.removeEdge(forwardEdge);
                }

                Edge backEdge = graph.findEdge(e.endNode, e.startNode);
                if (backEdge != null)
                {
                    backEdge.capacity = backEdge.capacity + negativeCycle.capacity;

                }
                else
                {
                    graph.createOrUpdateEdge(e.endNode, e.startNode, negativeCycle.capacity, -forwardEdge.costs, negativeCycle.capacity);
                }
            }

            return graph;
        }

        public Cycle findCycleMooreBellmanFord(Graph graph)
        {

            //init
            foreach (Node node in graph.nodeList)
            {
                if (node.id == 0)
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

            Node nodeInNegativeCycle = null;

            //negativeCycle checken!
            foreach (Edge e in graph.edgeList)
            {
                if (e.startNode.costs < double.MaxValue)
                {
                    double costs = e.startNode.costs + e.costs;

                    if (costs < e.endNode.costs)
                    {
                        nodeInNegativeCycle = e.endNode;
                        break;
                    }
                }
            }

            if (nodeInNegativeCycle != null)
            {

                for (int i = 0; i < graph.nodeList.Count; i++)
                {
                    nodeInNegativeCycle = nodeInNegativeCycle.previousNode;
                }

                List<Edge> edges = new List<Edge>();
                for (int i = 0; i < graph.nodeList.Count; i++)
                {
                    Edge e = graph.findEdge(nodeInNegativeCycle.previousNode, nodeInNegativeCycle);

                    if (!edges.Contains(e))
                    {
                        edges.Add(e);
                    }

                    nodeInNegativeCycle = nodeInNegativeCycle.previousNode;
                }

                return new Cycle(edges);
            }
            else
            {
                return null;
            }
        }

        public List<Path> fordFulkerson(Graph graph, int startId, int endId)
        {

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

            Path fluss = getShortestPathFromID(graph, endId);
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

        public Path getShortestPathFromID(Graph graph, int endId)
        {
            List<Edge> edges = new List<Edge>();
            findVater(graph.nodeList[endId], graph, edges);

            Path fluss = null;
            if (edges != null && edges.Count > 0)
            {
                fluss = new Path(edges);
            }
            return fluss;
        }


        public void findVater(Node node, Graph graph, List<Edge> edges)
        {
            if (node.previousNode != null)
            {
                Edge e = graph.findEdge(node.previousNode, node);
                edges.Add(e);

                findVater(node.previousNode, graph, edges);
            }
            else
            {
                return;
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

    }
}