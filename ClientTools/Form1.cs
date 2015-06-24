using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ODataClientSample
{
    public partial class Form1 : Form
    {
        static readonly Uri _baseAddress = new Uri("http://localhost:53433/");
        public Form1()
        {
            InitializeComponent();
        }

        //GET
        private void button2_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = _baseAddress;
            HttpResponseMessage response = client.GetAsync(textBox1.Text).Result;
            response.EnsureSuccessStatusCode();
            textBox2.Text+=response.Content.ReadAsStringAsync().Result+"\r\n";
        }

        //POST
        private void button1_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = _baseAddress;
            HttpResponseMessage response = client.PostAsync(textBox1.Text,
            new StringContent(textBox3.Text, Encoding.Default, "application/json")).Result;

            textBox2.Text += response.Content.ReadAsStringAsync().Result + "\r\n";
        }

        //Delete
        private void button3_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = _baseAddress;
            HttpResponseMessage response = client.DeleteAsync(textBox1.Text).Result;
            response.EnsureSuccessStatusCode();
            textBox2.Text += response.Content.ReadAsStringAsync().Result + "\r\n";
        }

        //Put
        private void button4_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = _baseAddress;
            HttpResponseMessage response = client.PutAsync(textBox1.Text, new StringContent(textBox3.Text, Encoding.Default, "application/json")).Result;
            response.EnsureSuccessStatusCode();
            textBox2.Text += response.Content.ReadAsStringAsync().Result + "\r\n";
        }
    }
}
