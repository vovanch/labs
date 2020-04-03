using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication5
{
    /// <summary>
    /// Перевіряє два графи на ізоморфність
    /// </summary>
    class Isomorph
    {
        private Graph _g1, _g2;
        private IncidenceMatrix _m1, _m2;

        public Isomorph(Graph g1, Graph g2)
        {
            _g1 = g1;
            _g2 = g2;
        }

        public bool test()
        {
            bool r = test1();
            if (r)
            {
                _m1 = new IncidenceMatrix(_g1);
                _m2 = new IncidenceMatrix(_g2);
                r = test2();
                if (r)
                {
                    r = test3(100);
                }
            }
            return r;
        }

        /// <summary>
        /// Перевіряє, чи кількість ребер та вершин рівна
        /// </summary>
        /// <returns></returns>
        private bool test1()
        {
            if (_g1.VertexCount == _g2.VertexCount &&
                _g1.EdgeCount == _g2.EdgeCount)
                return true;
            else return false;
        }

        /// <summary>
        /// Перевіряє, чи співпадають степені вершин
        /// </summary>
        /// <returns></returns>
        private bool test2()
        {
            _m1.sortTitleVertex();
            _m2.sortTitleVertex();
            bool r = true;
            for(int v = 0; v < _m1._vertices.Count; v++)
            {
                if (_m1._vertices[v].weigth != _m2._vertices[v].weigth)
                {
                    r = false;
                    break;
                }
            }
            return r;
        }

        /// <summary>
        /// Переставляє місцями вершини з одинаковими степенями, 
        /// доки не підвердиться ізоморфність,
        /// або не буде перевищена кількість ітерацій
        /// </summary>
        /// <param name="maxIter"></param>
        /// <returns></returns>
        private bool test3(int maxIter)
        {
            bool result = true;

            int v = _m1.compareM(_m2);
            for (int i = 0; i < maxIter; i++)
            {
                if (v != -1)
                {
                    _m2.shiftVertices(v);
                }
                else
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public List<Couple> getCouple()
        {
            List<Couple> couple = new List<Couple>();
            for(int v = 0; v < _m1._vertices.Count; v++)
            {
                couple.Add(new Couple(_m1._vertices[v].vertex,
                    _m2._vertices[v].vertex));
            }
            return couple;
        }
    }

    class Couple
    {
        public Vertex v1;
        public Vertex v2;

        public Couple(Vertex v1, Vertex v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
    }

    class Title_Vertex:IComparable<Title_Vertex>
    {
        public Vertex vertex { get; }
        public int weigth { get; set; }
        public Title_Vertex(Vertex v)
        {
            vertex = v;
            weigth = 0;
        }

        public int CompareTo(Title_Vertex other)
        {
            return -this.weigth.CompareTo(other.weigth);
        }
    }

    class Title_Edge
    {
        public Edge edge { get; }
        public Title_Vertex v1 { get; set; }
        public Title_Vertex v2 { get; set; }
        public Title_Edge(Edge e)
        {
            edge = e;
        }
    }

    class IncidenceMatrix
    {
        public List<Title_Edge> _edges;
        public List<Title_Vertex> _vertices;
        public int[,] _m;
        public static readonly int unmarks = 0;

        public IncidenceMatrix(Graph g)
        {
            this._vertices = new List<Title_Vertex>();
            this._edges = new List<Title_Edge>();

            foreach (Vertex v in g.Vertices)
            {
                _vertices.Add(new Title_Vertex(v));
            }
            foreach (Edge e in g.Edges)
            {
                _edges.Add(new Title_Edge(e));
            }
            _m = new int[_vertices.Count, _edges.Count];
            generateMatrix();
            setWeight();
        }

        /// <summary>
        /// Заповнює матрицю інцидентності
        /// </summary>
        public void generateMatrix()
        {
            for (int v = 0; v < _vertices.Count; v++)
            {
                for (int e = 0; e < _edges.Count; e++)
                {
                    if (_vertices[v].vertex.CompareTo(
                        _edges[e].edge.Source) == 0)
                    {
                        _edges[e].v1 = _vertices[v];
                        _m[v, e] = _edges[e].edge.Weight;
                    }
                    else if (_vertices[v].vertex.CompareTo(
                       _edges[e].edge.Target) == 0)
                    {
                        _edges[e].v2 = _vertices[v];
                        _m[v, e] = _edges[e].edge.Weight;
                    }
                    else
                    {
                        _m[v, e] = unmarks;
                    }
                }
            }
        }

        /// <summary>
        /// Рахує ваги вершин
        /// </summary>
        private void setWeight()
        {
            for(int v = 0; v < _vertices.Count; v++)
            {
                int w = 0;
                for(int e = 0; e < _edges.Count; e++)
                {
                    if (_m[v, e] != unmarks) w++;
                }
                _vertices[v].weigth = w;
            }
        }

        /// <summary>
        /// Сортує вершини в порядку зменшення їх ваги
        /// </summary>
        public void sortTitleVertex()
        {
            setWeight();
            _vertices.Sort();
            generateMatrix();
        }

        public void shiftVertices(int istart)
        {
            Title_Vertex v = _vertices[istart];
            for(int i = istart; i< _vertices.Count-1; i++)
            {
                if (_vertices[i].weigth == v.weigth)
                {
                    _vertices[i] = _vertices[i + 1];
                    _vertices[i + 1] = v;
                }
                else break;
            }
            generateMatrix();
        }

        /// <summary>
        /// Порівнює дану матрицю інцидентності з іншої такої ж розмірності
        /// Ваги і напрям не враховуються. Повертає номер першої вершини що не збігається
        /// </summary>
        /// <param name="m2"></param>
        /// <returns></returns>
        public int compareM(IncidenceMatrix m2)
        {
            int r = -1;

            for (int v = 0; v<_vertices.Count; v++)
            {
                for (int e = 0; e<_edges.Count; e++)
                {
                    if (_m[v, e] != unmarks)
                    {
                        if (m2._m[v, e] == unmarks)
                        {
                            r = v;
                            break;
                        }
                    }
                    else
                    {
                        if (m2._m[v, e] != unmarks)
                        {
                            r = v;
                            break;
                        }
                    }
                }
            }

            return r;
        }
    }



}
