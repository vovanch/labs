using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApplication5;

namespace MinGamilton
{
    /// <summary>
    /// Вирішує задачу комівояжера
    /// </summary>
    class MinGamilton
    {
        private List<Vertex> _way;

        public MinGamilton()
        {
            _way = new List<Vertex>();
        }

        public List<Vertex> getMinGamWay(Graph g)
        {
            //Початковю є перша вершина
            List<Vertex> vertices = new List<Vertex>();
            foreach(Vertex v in g.Vertices)
            {
                vertices.Add(v);
            }
            _way.Add(vertices[0]);
            vertices.RemoveAt(0);

            while (vertices.Count != 0)
            {
                //шукаємо найперспективніший шлях і додаємо його у _way
                List<SimpleWay> ways = new List<SimpleWay>();
                foreach (Vertex v in vertices)
                {
                    SimpleWay w = new SimpleWay(_way[_way.Count - 1], v);
                    List<Vertex> testWay = new List<Vertex>(_way);
                    testWay.Add(v);
                    w.perspective = getPerstective(g, testWay);
                    ways.Add(w);
                }
                //Вибираємо найперспективніший (найдешевший)
                int minp = Int16.MaxValue;
                int mimi = 0;
                for (int i = 0; i < ways.Count; i++)
                {
                    if (ways[i].perspective < minp)
                    {
                        minp = ways[i].perspective;
                        mimi = i;
                    }
                }
                //додаємо шлях до дерева
                _way.Add(ways[mimi].to);
                for (int i = 0; i < vertices.Count; i++)
                {
                    if (vertices[i].ID == ways[mimi].to.ID)
                    {
                        vertices.RemoveAt(i);
                        break;
                    }
                }
            }

            _way.Add(_way[0]);
            return _way;
        }

        /// <summary>
        /// Вираховує оцінку обраного шляху
        /// </summary>
        /// <param name="g"></param>
        /// <param name="way"></param>
        /// <returns></returns>
        public int getPerstective(Graph g, List<Vertex> way)
        {
            int result = 0;
            AdjacencyMatrix m = new AdjacencyMatrix(g);
            //Викреслюємо пройдений шлях з матриці суміжності
            for(int i = 0; i<way.Count-1; i++)
            {
                result += removeWay(way[i], way[i + 1], m);
            }
            //Оцінюємо невизначений шлях (from)
            int[] fmark = new int[m.from.Length];
            int[] tmark = new int[m.to.Length];
            #region --Віднімаємо від матриці мінімальні значення по рядках --
            for (int f = 0; f<fmark.Length; f++)
            {
                //вершина вже є в дереві
                if (m.from[f].state)
                {
                    fmark[f] = 0;
                    continue;
                }
                else
                {
                    //шукаємо мін в рядку
                    int min = Int16.MaxValue;
                    for (int t = 0; t < tmark.Length; t++)
                    {
                        //якщо вершина to не належить дереву
                        if (!m.to[t].state)
                        {
                            if (min > m.m[f, t]) min = m.m[f, t];
                        }
                    }
                    fmark[f] = min;
                }
            }
            //віднімаємо мінімальні значення в стовпцах від всіх елем. матриці
            for (int f = 0; f < m.from.Length; f++)
            {
                for (int t = 0; t<m.to.Length; t++)
                {
                    m.m[f, t] -= fmark[f];
                }
            }
            #endregion
            #region --Шукаємо мінімальні значення по стовпцях--
            for (int t = 0; t < tmark.Length; t++)
            {
                //вершина вже є в дереві
                if (m.to[t].state)
                {
                    tmark[t] = 0;
                    continue;
                }
                else
                {
                    //шукаємо мін в стовбці
                    int min = Int16.MaxValue;
                    for (int f = 0; f < fmark.Length; f++)
                    {
                        //якщо вершина to не належить дереву
                        if (!m.from[f].state)
                        {
                            if (min > m.m[f, t]) min = m.m[f, t];
                        }
                    }
                    tmark[t] = min;
                }
            }
            #endregion
            for (int f = 0; f< fmark.Length; f++)
            {
                if (!m.from[f].state) result += fmark[f];
            }
            for (int t = 0; t < tmark.Length; t++)
            {
                if (!m.to[t].state) result += tmark[t];
            }
            return result;
        }

        public int removeWay(Vertex v1, Vertex v2, AdjacencyMatrix m)
        {
            int fv1 = 0, tv2 = 0;
            for(int i =0; i<m.to.Length; i++)
            {
                if (v1.ID == m.to[i].vertex.ID) fv1 = i;
                if (v2.ID == m.to[i].vertex.ID) tv2 = i;
            }
            m.from[fv1].state = true;
            m.to[tv2].state = true;
            //Позначаємо шлях з v2 в v1 як неможливий
            m.m[tv2, fv1] = m.maxValue;
            return m.m[fv1, tv2];
        }
    }

    class SimpleWay
    {
        public Vertex from;
        public Vertex to;
        public int perspective;

        public SimpleWay(Vertex from, Vertex to)
        {
            this.from = from;
            this.to = to;
            perspective = Int16.MaxValue;
        }
    }

    class TitleVertex
    {
        public Vertex vertex;
        /// <summary>
        /// true - вершина увійшла в цикл
        /// </summary>
        public bool state;

        public TitleVertex(Vertex v)
        {
            vertex = v;
            state = false;
        }
    }

    class AdjacencyMatrix
    {
        public int maxValue = Int16.MaxValue;
        public TitleVertex[] from;
        public TitleVertex[] to;
        /// <summary>
        /// [from,to]
        /// </summary>
        public int[,] m;

        public AdjacencyMatrix(Graph g)
        {
            from = new TitleVertex[g.VertexCount];
            to = new TitleVertex[g.VertexCount];
            int i = 0;
            foreach(Vertex v in g.Vertices)
            {
                from[i] = new TitleVertex(v);
                to[i++] = new TitleVertex(v);
            }
            generateM(g);
        }

        private void generateM(Graph g)
        {
            m = new Int32[g.VertexCount, g.VertexCount];
            for(int i = 0; i<g.VertexCount; i++)
            {
                for (int j = 0; j < g.VertexCount; j++)
                    m[i, j] = maxValue;
            }
            foreach(Edge e in g.Edges)
            {
                #region -- записуємо в матрицю вагу кожного ребра --
                int f =0, t=0;
                for (int i = 0; i<from.Length; i++)
                {
                    if (from[i].vertex.ID == e.Source.ID)
                    {
                        f = i;
                        break;
                    }
                }
                for (int i = 0; i < to.Length; i++)
                {
                    if (to[i].vertex.ID == e.Target.ID)
                    {
                        t = i;
                        break;
                    }
                }
                m[f, t] = e.Weight;
                m[t, f] = e.Weight;
                #endregion
            }
        }

        public int getWeight(List<Vertex> way)
        {
            int result = 0;
            for (int w = 0; w < way.Count - 1; w++)
            {
                Vertex v1 = way[w];
                Vertex v2 = way[w + 1];
                int fv1 = 0, tv2 = 0;
                for (int i = 0; i < to.Length; i++)
                {
                    if (v1.ID == to[i].vertex.ID) fv1 = i;
                    if (v2.ID == to[i].vertex.ID) tv2 = i;
                }
                result += m[fv1, tv2];
            }
            return result;
        }
    }
}
