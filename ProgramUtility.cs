using namespaceStuktur;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace namespaceUtility
{
    class Parse
    {
        public static Graph getGraphByFile(string path, bool directed)
        {

            List<Node> nodeList = new List<Node>();
            List<Edge> edgeList = new List<Edge>();

            StreamReader sr = new StreamReader(path, Encoding.Default);
            String lineStr;

            int nodeCount = 0;
            while ((lineStr = sr.ReadLine()) != null)
            {
                if (nodeCount == 0)
                {
                    initNode(int.Parse(lineStr), nodeList);
                }
                else
                {
                    string[] word = Regex.Split(lineStr, "\t");

                    if (word.Length == 1)
                    {
                        Node n = nodeList[nodeCount - 1];
                        n.balance = double.Parse(word[0]);
                    }else{

                        createNodeAndEdge(int.Parse(word[0]), int.Parse(word[1]), double.Parse(word[2]), double.Parse(word[3]), nodeList, edgeList);

                        if (!directed)
                        {
                            createNodeAndEdge(int.Parse(word[1]), int.Parse(word[0]), double.Parse(word[2]), double.Parse(word[3]), nodeList, edgeList);
                        }
                    }
                }

                nodeCount++;
            }

            Graph g = new Graph(nodeList, edgeList);

            return g;
        }

        static void initNode(int nodeCount, List<Node> nodeList)
        {
            for (int i = 0; i < nodeCount; i++)
            {
                Node node = new Node(i);
                nodeList.Add(node);
            }
        }

        static void createNodeAndEdge(int vaterIndex, int sonIndex,  double costs, double capacity, List<Node> nodeList, List<Edge> edgeList)
        {
            Node vater = nodeList[vaterIndex];
            Node son = nodeList[sonIndex];
            vater.nodeList.Add(son);

            Edge edge = new Edge(vater, son, capacity, costs);
            vater.edgeList.Add(edge);

            edgeList.Add(edge);
        }
        

    }


    class UnionFind
    {

        int[] father;
        int[] rank;

        public void MakeSet(int length)
        {
            father = new int[length];
            rank = new int[length];

            for (int i = 0; i < length; i++)
            {
                father[i] = i;
                rank[i] = 0;
            }
        }

        int Find_Set(int x)
        {
            int root = x;
            while (father[root] != root)
                root = father[root];
            return root;
        }

        public bool Union(int x, int y)
        {
            x = Find_Set(x);
            y = Find_Set(y);
            if (x == y) return false;
            if (rank[x] > rank[y])
            {
                father[y] = x;
            }
            else if (rank[x] < rank[y])
            {
                father[x] = y;
            }
            else
            {
                rank[y]++;
                father[x] = y;
            }
            return true;
        }
    }

}