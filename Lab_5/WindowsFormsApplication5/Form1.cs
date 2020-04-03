using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication5
{
    public partial class Form1 : Form
    {
        private Graph _graph1;
        private Graph _graph2;

        public Form1()
        {
            InitializeComponent();
            startValue1();
            startValue2();
        }

        private void createNewGraph1()
        {
            pictureBox1.Image = null;
            try
            {
                int v = Convert.ToInt32(v_count.Text);
                int e = Convert.ToInt32(e_count.Text);
                dataGridView1.DataSource = Matrix.crateCleanTable(v, e);
                errorLabel.Text = "";
            }
            catch
            {
                errorLabel.Text = "Помилка при читанні даних";
            }
        }

        private void createNewGraph2()
        {
            pictureBox2.Image = null;
            try
            {
                int v = Convert.ToInt32(textBox2.Text);
                int e = Convert.ToInt32(textBox1.Text);
                dataGridView2.DataSource = Matrix.crateCleanTable(v, e);
                errorLabel.Text = "";
            }
            catch
            {
                errorLabel.Text = "Помилка при читанні даних";
            }
        }

        private void startValue1()
        {
            _graph1 = GraphItem.createTestGraph1();
            dataGridView1.DataSource = Matrix.createIncidenceMatrix(_graph1);
            v_count.Text = _graph1.VertexCount.ToString();
            e_count.Text = _graph1.EdgeCount.ToString();
            errorLabel.Text = "";
            richTextBox1.Text = "";
            vizGraph1();
        }

        private void startValue2()
        {
            _graph2 = GraphItem.createTestGraph2();
            dataGridView2.DataSource = Matrix.createIncidenceMatrix(_graph2);
            textBox2.Text = _graph2.VertexCount.ToString();
            textBox1.Text = _graph2.EdgeCount.ToString();
            errorLabel.Text = "";
            richTextBox1.Text = "";
            vizGraph2();
        }

        private void updata1()
        {
            try
            {
                _graph1 = Matrix.createGraphByIncidenceMatrix(getTable(dataGridView1));
                vizGraph1();
                errorLabel.Text = "";
                richTextBox1.Text = "";
            }
            catch
            {
                errorLabel.Text = "Матриця інцидентності заповнена невірно.";
            }

        }

        private void updata2()
        {
            try
            {
                _graph2 = Matrix.createGraphByIncidenceMatrix(getTable(dataGridView2));
                vizGraph2();
                errorLabel.Text = "";
                richTextBox1.Text = "";
            }
            catch
            {
                errorLabel.Text = "Матриця інцидентності заповнена невірно.";
            }

        }

        private DataTable getTable(DataGridView dgv)
        {
            var dt = ((DataTable)dgv.DataSource).Copy();
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                if (!column.Visible)
                {
                    dt.Columns.Remove(column.Name);
                }
            }
            return dt;
        }

        private void vizGraph1()
        {
            dataGridView1.DataSource = Matrix.createIncidenceMatrix(_graph1);
            pictureBox1.Image = GraphVisualization.getImage(_graph1);
        }

        private void vizGraph2()
        {
            dataGridView2.DataSource = Matrix.createIncidenceMatrix(_graph2);
            pictureBox2.Image = GraphVisualization.getImage(_graph2);
        }

        private void isomorph()
        {
            Isomorph iso = new Isomorph(_graph1, _graph2);
            bool r = iso.test();
            if (r) printCouples(iso.getCouple());
            else richTextBox1.Text = "Графи не ізоморфні";
        }

        private void printCouples(List<Couple> cop)
        {
            string str = "";
            foreach(Couple c in cop)
            {
                str += c.v1.Name + " -> " + c.v2.Name + "\n";
            }
            richTextBox1.Text = str;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            startValue1();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updata1();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isomorph();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            startValue1();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            createNewGraph1();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            createNewGraph2();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            startValue2();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            updata2();
        }
    }
}
