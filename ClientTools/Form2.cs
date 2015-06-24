using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ODataClientSample
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = Guid.NewGuid().ToString();
            textBox2.Text = Guid.NewGuid().ToString();
            textBox3.Text = Guid.NewGuid().ToString();
            textBox4.Text = Guid.NewGuid().ToString();
            textBox5.Text = Guid.NewGuid().ToString();
            textBox6.Text = Guid.NewGuid().ToString();
            textBox7.Text = Guid.NewGuid().ToString();
            textBox8.Text = Guid.NewGuid().ToString();
            textBox9.Text = Guid.NewGuid().ToString();
            textBox10.Text = Guid.NewGuid().ToString();
        }
    }
}
