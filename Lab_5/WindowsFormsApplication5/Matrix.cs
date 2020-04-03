using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using QuickGraph;

namespace WindowsFormsApplication5
{
    /// <summary>
    /// Створює матриці інцидентності та суміжності на основі графу
    /// у вигляді таблиці DataTable
    /// </summary>
    class Matrix
    {
        /// <summary>
        /// Створює матрицю інцидентності у вигляді таблиці для заданого графу.
        /// Якщо ребро виходить з вершини, в таблицю записується додатнє значення.
        /// Якщо ребро входить у вершини, в таблицю записується від'ємне значення.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static DataTable createIncidenceMatrix(Graph graph)
        {
            DataTable table = new DataTable("IncidenceMatrix");
            //Читаємо з graph у список інформацію про зв'язки та вершини
            List<Vertex> vertices = getVertices(graph);
            List<Edge> edges = getEdges(graph);
            
            /*Формуємо структуру таблиці.
             *Додаємо необхідну кількість стовпців.
             *В стовпцях записані назви ребер.
             */
            table.Columns.Add(new DataColumn());
            foreach (Edge e in edges)
            {
                table.Columns.Add(new DataColumn(e.Name));
            }
            //рядок 0 містить назви ребер
            DataRow row = table.NewRow();
            foreach (Edge e in edges)
            {
                row[e.Name] = e.Name;
            }
            table.Rows.Add(row);

            /*Додаємо необхідну кількість рядків в таблицю,
             *та запам'ятовуємо в якому рядку записана ти чи 
             *інша вершина
             */
            List<Conect> conects = new List<Conect>();
            int k = 1;
            foreach(Vertex v in vertices)
            {
                row = table.NewRow();
                row[0] = v.Name;
                table.Rows.Add(row);
                conects.Add(new Conect(v.ID, k++));
            }

            //Записуємо значення в таблицю
            foreach(Edge e in edges)
            {
                //початок ребра
                int rn = getRowNumber(e.Source.ID, conects);
                table.Rows[rn][e.Name] = -e.Weight;
                //кінець ребра
                rn = getRowNumber(e.Target.ID, conects);
                table.Rows[rn][e.Name] = e.Weight;
            }

            return table;
        }

        private static List<Vertex> getVertices(Graph graph)
        {
            List<Vertex> vertices = new List<Vertex>();
            foreach (Vertex v in graph.Vertices)
            {
                vertices.Add(v);
            }
            vertices.Sort();
            return vertices;
        }

        private static List<Edge> getEdges(Graph graph)
        {
            List<Edge> edges = new List<Edge>();
            foreach (Edge e in graph.Edges)
            {
                edges.Add(e);
            }
            edges.Sort();
            return edges;
        }

        private static int getRowNumber(int id, List<Conect> conects)
        {
            int r = -1;
            foreach (Conect c in conects)
            {
                if (c.ID == id)
                {
                    r = c.RowNumber;
                    break;
                }
            }
            return r;
        }

        /// <summary>
        /// Створія граф на основі матриці інцидентності
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static Graph createGraphByIncidenceMatrix(DataTable table)
        {
            Graph graph = new Graph();
            //Читаємо назви вершин
            List<Vertex> vertices = new List<Vertex>();
            for (int i = 1; i < table.Rows.Count; i++)
            {
                vertices.Add(new Vertex(i, table.Rows[i][0].ToString()));
                graph.AddVertex(vertices[vertices.Count-1]);
            }
            //Читаємо назви ребер і додаємо їх в граф
            for (int i = 1; i < table.Columns.Count; i++)
            {
                int s=-1, t=-1; //source and target
                int w = 0;
                //Шукаємо вершини, що з'єднані ребром
                for (int j = 1; j < table.Rows.Count; j++)
                {
                    try {
                        int value = Convert.ToInt32((String)table.Rows[j][i]);
                        if (value < 0)
                        {
                            s = j;
                        }
                        else
                        {
                            t = j;
                            w = value;
                        }
                    }
                    catch { }
                }
                graph.AddEdge(new Edge(vertices[s-1], vertices[t-1], w, (String)table.Rows[0][i]));
            }

            return graph;
        }

        public static DataTable crateCleanTable(int v, int e)
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn());
            for (int i = 1; i <= e; i++)
            {
                table.Columns.Add(new DataColumn("e"+i.ToString()));
            }
            DataRow row = table.NewRow();
            for (int i = 1; i <= e; i++)
            {
                row[i] = "e" + i.ToString();
            }
            table.Rows.Add(row);
            for (int i = 1; i <= v; i++)
            {
                row = table.NewRow();
                row[0] = "v" + i.ToString();
                table.Rows.Add(row);
            }

            return table;
        }
    }

    struct Conect
    {
        public int ID { get; }
        public int RowNumber { get; }

        public Conect(int id, int rowNumber)
        {
            ID = id;
            RowNumber = rowNumber;
        }
    }
}
