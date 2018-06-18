
using System;
using System.Collections.Generic;


namespace namespaceStuktur
{

    class Graph
    {
        public List<Node> nodeList;

        public List<Edge> edgeList;

        public Graph(List<Node> nodeList, List<Edge> edgeList)
        {
            this.nodeList = nodeList;
            this.edgeList = edgeList;
        }

        public Graph()
        {
            this.nodeList = new List<Node>();
            this.edgeList = new List<Edge>();
        }



        public double getPseudoBalance(Node d)
        {
            double inSume = 0.0;
            double outSume = 0.0;
            foreach (Edge e in edgeList) {
                if (e.endNode.id == d.id) {
                    inSume = inSume + e.flow;
                } else if (e.startNode.id == d.id) {
                    outSume = outSume + e.flow;
                }
            }

            double pseudoBalance = outSume - inSume;

            return pseudoBalance;
        }


        public void reset()
        {
            foreach (Node d in nodeList)
            {
                d.visited = false;
                d.previousNode = null;
            }
        }


        public Edge findEdge(Node startNode, Node endNode)
        {
            foreach (Edge e in edgeList) {
                if (e.startNode.id == startNode.id && e.endNode.id == endNode.id) {
                    return e;
                }
            }

            return null ;
        }

        public Edge findEdge(string edgeId)
        {
            foreach (Edge e in edgeList)
            {
                if (e.id==edgeId)
                {
                    return e;
                }
            }

            return null;
        }

        public Edge createOrUpdateEdge(Node startNode, Node endNode, double capacity, double costs,double flow)
        {
            Edge e = findEdge(startNode, endNode);
            if (e == null)
            {
                e = new Edge(startNode, endNode, capacity, costs,flow);
                startNode.edgeList.Add(e);
                startNode.nodeList.Add(endNode);
                this.edgeList.Add(e);
                //this.array[startNode.id, endNode.id] = e;
            }
            else
            {
                e.capacity = e.capacity + capacity;
               
            }
            return e;
        }

        public void removeEdge(Edge e)
        {
            e.startNode.edgeList.Remove(e);
            e.startNode.nodeList.Remove(e.endNode);
            this.edgeList.Remove(e);
           // this.array[e.startNode.id, e.endNode.id] = null;
        }
        public List<Node> getSource()
        {
            List<Node> sourceList = new List<Node>();

            foreach (Node n in nodeList) {
                if (n.balance>n.pseudoBalance) {
                    sourceList.Add(n);
                }
            }

            return sourceList;
        }

        

    }

    class MST
    {
        public List<Node> nodeList;

        public List<Edge> edgeList;

        public MST(List<Edge> edgeList, int size)
        {

            this.edgeList = edgeList;
            this.nodeList = new List<Node>();

            for (int i = 0; i < size; i++)
            {
                Node node = new Node(i);
                nodeList.Add(node);
            }

            foreach (Edge edge in edgeList)
            {

                Node startNode = nodeList[edge.startNode.id];
                Node endNode = nodeList[edge.endNode.id];

                startNode.edgeList.Add(edge);
                endNode.edgeList.Add(edge);

                startNode.nodeList.Add(endNode);
                endNode.nodeList.Add(startNode);

                startNode.costs = edge.capacity;
                endNode.costs = edge.capacity;
            }

        }

        public void reset()
        {
            foreach (Node d in nodeList)
            {
                d.visited = false;
            }

        }
    }

    class Node : IComparable<Node>
    {
        public int id;

        public List<Edge> edgeList;

        public List<Node> nodeList;

        public bool visited = false;

        public double costs;

        public double balance;

        public double pseudoBalance;

        public Node previousNode;

        public Node(int id)
        {
            this.id = id;
            this.edgeList = new List<Edge>();
            this.nodeList = new List<Node>();
            this.costs = float.MaxValue;
        }

        public int CompareTo(Node other)
        {
            return this.costs.CompareTo(other.costs);
        }

        public override string ToString()
        {
            return "" + id;
        }

    }

    class Edge : IComparable<Edge>
    {
        public Node endNode;
        public Node startNode;
        public double capacity = 0.0;
        public double costs = 0.0;
        public double flow = 0.0;
        public string id;
        public string rId;

        public Edge(Node startNode, Node endNode, double capacity, double costs, double flow)
        {
            this.startNode = startNode;
            this.endNode = endNode;
            this.capacity = capacity;
            this.costs = costs;
            this.flow = flow;

            id = startNode.id + "-" + endNode.id;
            rId = endNode.id + "-" + startNode.id;
        }

        public int CompareTo(Edge other)
        {
            return this.capacity.CompareTo(other.capacity);
        }

        public override string ToString()
        {
            return "["+id+":"+flow+","+capacity+","+costs+"]";
        }
    }

   

    class Path
    {
        public List<Edge> edgeList;
        public double capacity;

        public Path(List<Edge> edgeList)
        {
            this.edgeList = edgeList;

            double minCapacity = Double.MaxValue;
            foreach (Edge e in edgeList)
            {
                if (e.capacity < minCapacity)
                {
                    minCapacity = e.capacity;
                }
            }

            this.capacity = minCapacity;
        }


        public override string ToString()
        {

            string s = "";

            foreach (Edge e in edgeList) {
                s = s + "" + e.id+" ";
            }
            return s;
        }

    }

}