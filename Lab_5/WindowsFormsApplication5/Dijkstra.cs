using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApplication5;

namespace Dijkstra_Algoritm
{

    class Dijkstra
    {
        private IncidenceMatrix _m;
        public Vertex startVertex { get; }

        public Dijkstra(Graph g, Vertex startPoint)
        {
            _m = new IncidenceMatrix(g);
            startVertex = startPoint;
            startAlgoritm();
        }

        private void startAlgoritm()
        {
            //Позначаємо в матриці початкову точку
            _m.setStartVertex(startVertex);
            for (Title_Vertex workV = _m.getMinNoMarkPoit();
                workV!=null;
                workV = _m.getMinNoMarkPoit())
            {
                iteration(workV);
                workV.state = true;
            }

        }

        private void iteration(Title_Vertex workV)
        {
            //доки існують неопрацьовані ребра
            for (Title_Edge workE = _m.getMinNoMarkEdge(workV.index);
                workE != null;
                workE = _m.getMinNoMarkEdge(workV.index))
            {
                Title_Vertex nextV;
                if (workV.index == workE.v1.index) nextV = workE.v2;
                else nextV = workE.v1;

                //Рахуємо шлях до наступної вершини
                if (nextV.weigth > workV.weigth + workE.edge.Weight)
                {
                    //шлях, що розглядається коротший за існуючий
                    nextV.weigth = workV.weigth + workE.edge.Weight;
                    nextV.wayV = new List<Vertex>(workV.wayV);
                    nextV.wayV.Add(nextV.vertex);
                    nextV.wayE = new List<Edge>(workV.wayE);
                    nextV.wayE.Add(workE.edge);
                }
                workE.state = true;
            }         
        }

        public List<Vertex> getWayVetrex(Vertex vertex)
        {
            List<Vertex> result = null;
            foreach(Title_Vertex v in _m._vertices)
            {
                if (vertex.CompareTo(v.vertex) == 0)
                {
                    result = v.wayV;
                    break;
                }
            }
            return result;
        }

        public List<Edge> getWayEdge(Vertex vertex)
        {
            List<Edge> result = null;
            foreach (Title_Vertex v in _m._vertices)
            {
                if (vertex.CompareTo(v.vertex) == 0)
                {
                    result = v.wayE;
                    break;
                }
            }
            return result;
        }
    }

    class Title_Vertex
    {
        public Vertex vertex { get; }
        /// <summary>
        /// Провірені вершини (true)
        /// </summary>
        public bool state { get; set; }
        /// <summary>
        /// Довжина найкоротшого шляху
        /// </summary>
        public int weigth { get; set; }
        /// <summary>
        /// Список вершин, що описує шлях
        /// </summary>
        public List<Vertex> wayV { get; set; }
        public List<Edge> wayE { get; set; }

        public int index { get; set; }

        public Title_Vertex(Vertex v)
        {
            vertex = v;
            weigth = int.MaxValue;
            state = false;
            wayV = new List<Vertex>();
            wayE = new List<Edge>();
        }
    }

    class Title_Edge
    {
        public Edge edge { get; }
        public Title_Vertex v1 { get; set; }
        public Title_Vertex v2 { get; set; }
        public bool state { get; set; }

        public Title_Edge(Edge e)
        {
            edge = e;
            state = false;
        }
    }

    class IncidenceMatrix
    {
        public List<Title_Edge> _edges;
        public List<Title_Vertex> _vertices;
        public int[,] _m;
        public readonly int unmarks = -1;

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
        }

        /// <summary>
        /// Заповнює матрицю інцидентності
        /// </summary>
        private void generateMatrix()
        {
            for (int v = 0; v < _vertices.Count; v++)
            {
                _vertices[v].index = v;
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
                    else {
                        _m[v, e] = unmarks;
                    }
                }
            }
        }

        public void setStartVertex(Vertex startV)
        {
            for (int v = 0; v < _vertices.Count; v++)
            {
                if (startV.CompareTo(_vertices[v].vertex) == 0)
                {
                    _vertices[v].weigth = 0;
                    _vertices[v].wayV.Add(startV);
                    break;
                }
            }
        }

        /// <summary>
        /// Повертає не опрацьовану точку з найменшою вагою
        /// </summary>
        /// <returns></returns>
        public Title_Vertex getMinNoMarkPoit()
        {
            int w = int.MaxValue;
            Title_Vertex result = null;
            foreach(Title_Vertex vertex in _vertices)
            { 
                //якщо точка не опрацьована
                if (!vertex.state)
                {
                    if (vertex.weigth < w)
                    {
                        w = vertex.weigth;
                        result = vertex;
                    }
                }
            }
            return result;
        }

        public Title_Edge getMinNoMarkEdge(int vertexIndex)
        {
            int w = int.MaxValue;
            Title_Edge result = null;

            int v = vertexIndex;
            for (int e = 0; e < _edges.Count; e++)
            {
                //якщо ребро виходить з робочої веришини (vertexIndex)
                if (_m[v, e] != unmarks)
                {
                    //якщо ребро ще не опрацьоване
                    if (!_edges[e].state)
                    {
                        //якщо знайдена вага ребра ще менша
                        if (_m[v, e] < w)
                        {
                            w = _m[v, e];
                            result = _edges[e];
                        }
                    }
                }
            }
            return result;
        }
    }
}
