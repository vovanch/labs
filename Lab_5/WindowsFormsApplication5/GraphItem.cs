using System;
using System.Drawing;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using System.IO;
using System.Diagnostics;


/// <summary>
/// Містить опис графу, його вершин та ребер.
/// </summary>
namespace WindowsFormsApplication5
{
    class GraphItem
    {
        public static Graph createTestGraph1()
        {
            Graph graph = new Graph();
            Vertex v1 = new Vertex(1, "A");
            Vertex v2 = new Vertex(2, "B");
            Vertex v3 = new Vertex(3, "C");
            Vertex v4 = new Vertex(4, "D");
            Vertex v5 = new Vertex(5, "E");

            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddVertex(v3);
            graph.AddVertex(v4);
            graph.AddVertex(v5);

            graph.AddEdge(new Edge(v1, v2, 3));
            graph.AddEdge(new Edge(v1, v3, 2));
            graph.AddEdge(new Edge(v1, v4, 5));
            graph.AddEdge(new Edge(v2, v3, 1));
            graph.AddEdge(new Edge(v2, v4, 7));
            graph.AddEdge(new Edge(v3, v4, 4));
            graph.AddEdge(new Edge(v3, v5, 4));
            graph.AddEdge(new Edge(v4, v5, 3));

            return graph;
        }

        public static Graph createTestGraph2()
        {
            Graph graph = new Graph();
            Vertex v1 = new Vertex(1, "F");
            Vertex v2 = new Vertex(2, "T");
            Vertex v3 = new Vertex(3, "S");
            Vertex v4 = new Vertex(4, "P");
            Vertex v5 = new Vertex(5, "Q");

            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddVertex(v3);
            graph.AddVertex(v4);
            graph.AddVertex(v5);

            graph.AddEdge(new Edge(v2, v1, 3));
            graph.AddEdge(new Edge(v1, v3, 2));
            graph.AddEdge(new Edge(v1, v4, 5));
            graph.AddEdge(new Edge(v2, v3, 1));
            graph.AddEdge(new Edge(v4, v2, 7));
            graph.AddEdge(new Edge(v3, v4, 4));
            graph.AddEdge(new Edge(v5, v3, 4));
            graph.AddEdge(new Edge(v4, v5, 3));

            return graph;
        }


    }


    /// <summary>
    /// Опис вершини графу
    /// </summary>
    public class Vertex : IComparable<Vertex>
    {
        public Vertex(int id, String name = null)
        {
            ID = id;
            if (name == null)
            {
                Name = "v" + id.ToString();
            }
            else Name = name;
        }

        public int ID { get; set; }
        public String Name { get; set; }

        public int CompareTo(Vertex other)
        {
            return ID.CompareTo(other.ID);
        }

        public Vertex clone()
        {
            return new Vertex(this.ID, this.Name);
        }
    }

    /// <summary>
    /// Опис ребра графу, що з'єднює дві вершини Vertex
    /// </summary>
    public class Edge : IEdge<Vertex>, IComparable<Edge>
    {
        public Vertex Source { get; set; }
        public Vertex Target { get; set; }
        public String Name { get; set; }
        /// <summary>
        /// Пропускна здатність
        /// </summary>
        public int Weight { get; set; }

        public Edge(Vertex s, Vertex t, int weight = 1, String name = null)
        {
            Source = s;
            Target = t;
            Weight = weight;
            if (name == null)
            {
                Name = "e" + s.ID.ToString() + "-" + t.ID.ToString();
            }
            else Name = name;
        }   

        public int CompareTo(Edge other)
        {
            return Name.CompareTo(other.Name);
        }
      
    }

    /// <summary>
    /// Граф, в якого вершини описуються класом Vertex, 
    /// а дуги за допомогою класу Edge
    /// </summary>
    public class Graph : AdjacencyGraph<Vertex, Edge>
    {
        public Graph clone()
        {
            Graph g = new Graph();
            g.AddVertexRange(this.Vertices);
            g.AddEdgeRange(this.Edges);
            return g;
        }
    }

}
