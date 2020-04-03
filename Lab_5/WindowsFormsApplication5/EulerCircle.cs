using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApplication5;

namespace EulerCircleAlgoritm
{
    /// <summary>
    /// Реалізує методи для пошуку Ейлерового циклу в графі.
    /// Описує виключення, якщо такого циклу не існує.
    /// </summary>
    class EulerCircle
    {
        public Vertex startVertex { get;  }
        private IncidenceMatrix _m;

        public EulerCircle(Graph g, Vertex startVertex)
        {
            _m = new IncidenceMatrix(g);
            this.startVertex = startVertex;
        }
        
        /// <summary>
        /// Повертає список вершин з непарними степенями в неорієнтованому графі.
        /// </summary>
        public List<Vertex> getNoPartVertex()
        {
            List<Vertex> result = new List<Vertex>();
            foreach(Title_Vertex v in _m._vertices)
            {
                int k = 0;
                for(int e = 0; e < _m._edges.Count; e++)
                {
                    if (_m._m[v.index, e] != IncidenceMatrix.unmarks) k++;
                }
                if (k % 2 == 1) result.Add(v.vertex);
            }
            return result;
        }

        /// <summary>
        /// Шукає один цикл в графі
        /// </summary>
        /// <param name="startPoint"></param>
        private Cycles getCycle(Title_Vertex startPoint)
        {
            //позначаємо всі вершини як неопрацьовані
            for(int v = 0; v<_m._vertices.Count; v++)
            {
                _m._vertices[v].state = false;
            }
            //позначаємо ребра, що не ввійшли в цикл як неопрацьовані
            for(int e = 0; e<_m._edges.Count; e++)
            {
                if (!_m._edges[e].inCircle)
                {
                    _m._edges[e].state = false;
                }
            }

            //стек
            List<Title_Edge> result = new List<Title_Edge>();
            List<Title_Vertex> stack = new List<Title_Vertex>();
            stack.Add(startPoint);

            //використовуємо пошук вглибину для знаходження циклу
            while (stack.Count != 0)
            {
                Title_Vertex workV = stack[stack.Count - 1];
                workV.state = true;
                Title_Edge workE = _m.getNoMarkEdge(workV.index);
                if(workE == null)
                {
                    stack.RemoveAt(stack.Count - 1);
                    continue;
                }
                workE.state = true;

                Title_Vertex nextV;
                if (workE.v1.index == workV.index) nextV = workE.v2;
                else nextV = workE.v1;

                //перевіряємо, чи можна добавити точку в стек
                if (nextV.index == startPoint.index)
                {
                    //цикл знайдено
                    result.Add(workE);
                    break;
                }
                if (nextV.state)
                {
                    //точка вже знаходиться в стеку
                    stack.RemoveAt(stack.Count - 1);
                    result.RemoveAt(result.Count - 1);
                }
                else
                {
                    //точку можна добавити в стек
                    stack.Add(nextV);
                    result.Add(workE);
                }              
            }

            //Отриманий стек порожній, якщо цикл не знайдено
            Cycles cycles;
            cycles.edgesCycle = result;
            cycles.vertexCycle = stack;
            return cycles;
        }

        public String getCycle()
        {
            String str = "";
            Cycles cycle = iteration(_m._vertices[0]);
            List<Title_Edge> c = cycle.edgesCycle;
            if (c != null)
            {
                foreach(Title_Edge v in c)
                {
                    str += v.edge.Name + "->";
                }
            }
            else { str = "null"; }
            return str;
        }

        private Cycles iteration(Title_Vertex startPoint)
        {
            Cycles result = getCycle(startPoint);
            //виключаємо з розгляду знайдений цикл
            for (int e = 0; e < result.edgesCycle.Count; e++)
            {
                result.edgesCycle[e].inCircle = true;
            }
            //шукаємо інші цикли 
            for (int v = 0; v < result.vertexCycle.Count; v++)
            {
                Cycles newCycle = iteration(result.vertexCycle[v]);
                result.edgesCycle.InsertRange(v, newCycle.edgesCycle);
                result.vertexCycle.InsertRange(v, newCycle.vertexCycle);
            }
            return result;
        }

        public lab2.Cycles getEulerCycle()
        {
            Cycles c = iteration(_m._vertices[0]);
            lab2.Cycles c2;
            List<Vertex> vs = new List<Vertex>(); 
            foreach(Title_Vertex v in c.vertexCycle)
            {
                vs.Add(v.vertex);
            }
            c2.vertexCycle = vs;

            List<Edge> es = new List<Edge>();
            foreach (Title_Edge e in c.edgesCycle)
            {
                es.Add(e.edge);
            }
            c2.edgesCycle = es;
            return c2;
        }
    }

    struct Cycles
    {
        public List<Title_Edge> edgesCycle;
        public List<Title_Vertex> vertexCycle;
    }

    class Title_Vertex
    {
        public Vertex vertex { get; }
        /// <summary>
        /// Провірені вершини (true)
        /// </summary>
        public bool state { get; set; }
                
        public int index { get; set; }

        public Title_Vertex(Vertex v)
        {
            vertex = v;
            state = false;
        }

    }

    class Title_Edge
    {
        public Edge edge { get; }
        public Title_Vertex v1 { get; set; }
        public Title_Vertex v2 { get; set; }
        public bool state { get; set; }
        public bool inCircle { get; set; }
        public Title_Edge(Edge e)
        {
            edge = e;
            state = false;
            inCircle = false;
        }
    }

    class IncidenceMatrix
    {
        public List<Title_Edge> _edges;
        public List<Title_Vertex> _vertices;
        public int[,] _m;
        public static readonly int unmarks = -1;

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
                    else
                    {
                        _m[v, e] = unmarks;
                    }
                }
            }
        }

        /// <summary>
        /// Повертає не список опрацьованих ребер.
        /// </summary>
        /// <param name="indexV"></param>
        /// <returns></returns>
        public Title_Edge getNoMarkEdge(int indexV)
        {
            Title_Edge result = null;
            for(int e = 0; e <_edges.Count; e++)
            {
                //якщо ребро не опрацьоване
                if (!_edges[e].state)
                {
                    if (_m[indexV, e] != unmarks)
                    {
                        result = _edges[e];
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Повертає true, якщо всі вершини оброблені
        /// </summary>
        /// <returns></returns>
        public bool ifAllVertexTrue()
        {
            bool result = true;
            foreach(Title_Vertex v in _vertices)
            {
                if (!v.state)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
    }

}
