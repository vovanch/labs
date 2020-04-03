using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication5
{
    /// <summary>
    /// Шукає максимальний потік в графі.
    /// S - перша точка графу
    /// T - остання точка в графі
    /// </summary>
    class Flow
    {
        private IncidenceMatrix _m;
        private Title_Vertex _s;
        private Title_Vertex _t;

        public Flow(Graph g)
        {
            _m = new IncidenceMatrix(g);
            foreach(Title_Vertex v in _m._vertices)
            {
                if (v.vertex.Name == "S") _s = v;
                if (v.vertex.Name == "T") _t = v;
            }
        }

        /// <summary>
        /// Проводить пошук в глибину і шукає збільшуючий потік
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        private List<Title_Edge> getUpFlow()
        {
            List<Title_Edge> result = new List<Title_Edge>();
            List<Title_Vertex> stack = new List<Title_Vertex>();
            _m.startTitle();
            stack.Add(_s);
            //Цикл припиняється, якщо з розгляду виключається початкова точка
            while (stack.Count != 0)
            {
                Title_Vertex workV = stack[stack.Count - 1];
                workV.state = false;
                Title_Edge workE = _m.getNoMarkEdge(workV.index);
                if (workE == null)
                {
                    //ребро не знайдено. йдемо вверх по стеку
                    workV.state = true;
                    stack.RemoveAt(stack.Count - 1);
                    try {
                        result.RemoveAt(stack.Count - 1);
                    }
                    catch { }
                    continue;
                }

                Title_Vertex nextV = workE.v2;
                //перевіряємо, чи можна добавити точку в стек
                if (nextV.index == _t.index)
                {
                    //шлях знайдено
                    result.Add(workE);
                    break;
                }
                if (!nextV.state)
                {
                    //точка вже знаходиться в стеку
                    stack.RemoveAt(stack.Count - 1);
                    workV.state = true;
                    result.RemoveAt(result.Count - 1);
                }
                else
                {
                    //точку можна добавити в стек
                    stack.Add(nextV);
                    result.Add(workE);
                }
                //Позначаємо ребро як розглянуте
                workE.state = false;
            }
            return result;
        }

        /// <summary>
        /// Повертає значення на скільки можна збільшити потік
        /// </summary>
        /// <param name="upFlow"></param>
        /// <returns></returns>
        private int getMatUpFlow(List<Title_Edge> upFlow)
        {
            int r = int.MaxValue;
            foreach(Title_Edge e in upFlow)
            {
                if (r > e.Weight - e.Flow) r = e.Weight - e.Flow;
            }
            return r;
        }
    
        private void upFlow(List<Title_Edge> flow)
        {
            int r = getMatUpFlow(flow);
            foreach(Title_Edge e in flow)
            {
                e.Flow += r;
            }
        }

        /// <summary>
        /// Встановлює максимальний потік і повертає його значення
        /// </summary>
        /// <returns></returns>
        public int setMaxFlow()
        {
            List<Title_Edge> flow = getUpFlow();
            while (flow.Count != 0)
            {
                upFlow(flow);
                flow = getUpFlow();
            }
            //Рахуємо значення потоку
            int r = 0;
            foreach(Title_Edge e in _m._edges)
            {
                if (e.v2.index == _t.index) r += e.Flow;
            }
            return r;
        }
}

    class Title_Vertex
    {
        public Vertex vertex { get; }
        /// <summary>
        /// true - вершину можна використати
        /// </summary>
        public bool state { get; set; }

        public int index { get; set; }

        public Title_Vertex(Vertex v)
        {
            vertex = v;
            state = true;
        }

    }

    class Title_Edge
    {
        public Edge edge { get; }
        public int Weight {
            get { return edge.Weight; }
            set { edge.Weight = value; }
        }
        public int Flow
        {
            get { return edge.Flow; }
            set { edge.Flow = value; }
        }
        //координати вершин в таблиці
        public Title_Vertex v1 { get; set; }
        public Title_Vertex v2 { get; set; }
        /// <summary>
        /// true - ребро можна використати
        /// Ребро не можна використати, 
        /// якщо через нього проходить максимальний потік
        /// </summary>
        private bool _state;
        public bool state {
            get { return _state; }
            set { if (Flow >= Weight) _state = false;
                else _state = value;
            }
        }

        public Title_Edge(Edge e)
        {
            edge = e;
            state = true;
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
            _vertices = new List<Title_Vertex>();
            _edges = new List<Title_Edge>();

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
                    //Знайдена початкова вершина
                    if (_vertices[v].vertex.CompareTo(
                        _edges[e].edge.Source) == 0)
                    {
                        _edges[e].v1 = _vertices[v];
                        _m[v, e] = -_edges[e].edge.Weight;
                    }
                    //Знайдена кінцева вершина
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
        /// Позначає ребра та вершини як ще не використані.
        /// Готує матрицю до повторного пошуку збільшуючого потоку
        /// </summary>
        public void startTitle() {
            foreach (Title_Edge e in _edges)
            {
                e.state = true;
            }
            foreach(Title_Vertex v in _vertices)
            {
                v.state = true;
            }
        }

        /// <summary>
        /// Повертає список не опрацьованих ребер.
        /// </summary>
        /// <param name="indexV"></param>
        /// <returns></returns>
        public Title_Edge getNoMarkEdge(int indexV)
        {
            Title_Edge result = null;
            for (int e = 0; e < _edges.Count; e++)
            {
                //якщо ребро можна розглядати
                if (_edges[e].state)
                {
                    if (_m[indexV, e] < 0 )
                    {
                        result = _edges[e];
                        break;
                    }
                }
            }
            return result;
        }
    }

}
