using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
namespace WFCOTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string process_guid = "40c4be7f-2c7d-4391-92a3-32e2dc24f58f";
        private string application_instance_id = "45603";

        #region 进行
        private void button5_Click(object sender, EventArgs e)
        {
            application_instance_id = textBox4.Text == "" ? "0001" : textBox4.Text;

            WfRunner appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr1";
            appRunner.UserName = "usr1";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "fc8c71c5-8786-450e-af27-9f6a9de8560f", UserID = "usr2", UserName = "usr2" });
            var result = WebOpreation.Post("odata/StartProcess", Newtonsoft.Json.JsonConvert.SerializeObject(appRunner));

            s2.BackColor = Color.Green;

            rtext.Text += result + "\r\n";
        }

        private void RunPro(string nextguid, string usr, string nextusr, Button bt)
        {
            application_instance_id = textBox4.Text == "" ? "0001" : textBox4.Text;

            WfRunner appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = usr;
            appRunner.UserName = usr;
            appRunner.NextActivityPerformers.Add(new Point { PathID = nextguid, UserID = nextusr, UserName = nextusr });
            var result = WebOpreation.Post("odata/RunProcess", Newtonsoft.Json.JsonConvert.SerializeObject(appRunner));
            bt.BackColor = Color.Green;
            rtext.Text += result + "\r\n";
        }

        private void button11_Click(object sender, EventArgs e)
        {
            RunPro("39c71004-d822-4c15-9ff2-94ca1068d745", "usr2", "usr3", s3);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            RunPro("b8b1d081-84a6-41ef-be7e-e7e3c76887a1", "usr3", "usr4", s4);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            RunPro("2159ac37-6a49-48dc-82f5-4b97f93e6076", "usr4", "usr5", s5);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            RunPro("41078df3-9024-4a01-9e00-10e41508b0f2", "usr5", "usr6", s6);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            RunPro("0d8656da-909a-4ffa-8d14-46c620425bc9", "usr6", "usr7", s7);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            RunPro("302b2e4c-74ab-4f15-90d0-ca01d53dd54a", "usr7", "usr8", s8);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            RunPro("64010e4d-03e6-46df-b624-5a669b00f1be", "usr8", "usr9", s9);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            RunPro("76167b7d-12ec-4548-a323-4b4ce5356bb4", "usr9", "usr10", s10);
        }

        private void button35_Click(object sender, EventArgs e)
        {
            RunPro("b70e717a-08da-419f-b2eb-7a3d71f054de", "usr10", "", s11);
        }

        private void s9_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region  退回
        private void button10_Click(object sender, EventArgs e)
        {
            BackPro("usr2", s3);
        }

        private void BackPro(string usr, Button bt)
        {
            application_instance_id = textBox4.Text == "" ? "0001" : textBox4.Text;

            WfRunner appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = usr;
            appRunner.UserName = usr;

            var result = WebOpreation.Post("odata/SendBackProcess", Newtonsoft.Json.JsonConvert.SerializeObject(appRunner));
            bt.BackColor = Color.AntiqueWhite;
            rtext.Text += result + "\r\n";
        }

        private void button16_Click(object sender, EventArgs e)
        {
            BackPro("usr4", s5);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            BackPro("usr3", s4);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            BackPro("usr5", s6);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            BackPro("usr6", s7);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            BackPro("usr7", s8);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            BackPro("usr8", s9);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            BackPro("usr9", s10);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            BackPro("usr10", s11);
        }
        #endregion

        #region 撤销
        //撤销  从下一节点撤回到自己
        private void button6_Click(object sender, EventArgs e)
        {
            Withdraw("usr3",s4);
        }

        private void Withdraw(string usr, Button bt)
        {
            application_instance_id = textBox4.Text == "" ? "0001" : textBox4.Text;

            WfRunner appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = usr;
            appRunner.UserName = usr;

            var result = WebOpreation.Post("odata/WithdrawProcess", Newtonsoft.Json.JsonConvert.SerializeObject(appRunner));
            bt.BackColor = Color.AntiqueWhite;
            rtext.Text += result + "\r\n";
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Withdraw("usr4", s5);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Withdraw("usr5", s6);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Withdraw("usr6", s7);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Withdraw("usr7", s8);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Withdraw("usr8", s9);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Withdraw("usr9", s10);
        }

        #endregion

        #region 流程操作
        //跳转 
        private void button4_Click(object sender, EventArgs e)
        {
            application_instance_id = textBox4.Text == "" ? "0001" : textBox4.Text;

            WfRunner appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = textBox2.Text;
            appRunner.UserName = textBox2.Text;
            appRunner.JumpbackActivityGUID = textBox1.Text;
            appRunner.NextActivityPerformers.Add(new Point { PathID = textBox1.Text, UserID = textBox3.Text, UserName = textBox3.Text });
          
            var result = WebOpreation.Post("odata/JumpProcess", Newtonsoft.Json.JsonConvert.SerializeObject(appRunner));

            rtext.Text += result + "\r\n";
        }

        //废弃流程
        private void button10_Click_1(object sender, EventArgs e)
        {
            application_instance_id = textBox4.Text == "" ? "0001" : textBox4.Text;

            WfRunner appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = textBox5.Text;
            appRunner.UserName = textBox5.Text;

            var result = WebOpreation.Post("odata/DiscardProcess", Newtonsoft.Json.JsonConvert.SerializeObject(appRunner));

            rtext.Text += result + "\r\n";
        }

        //反签
        private void button12_Click(object sender, EventArgs e)
        {
            application_instance_id = textBox4.Text == "" ? "0001" : textBox4.Text;

            WfRunner appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = textBox7.Text;
            appRunner.UserName = textBox7.Text;

            var result = WebOpreation.Post("odata/ReverseProcess", Newtonsoft.Json.JsonConvert.SerializeObject(appRunner));

            rtext.Text += "自动回到流程的最后一步";
            rtext.Text += result + "\r\n";
        }
        #endregion
    }


    public static class WebOpreation
    {
        static readonly Uri _baseAddress = new Uri("http://localhost:53433/");
        //static readonly Uri _baseAddress = new Uri("http://172.16.22.26:8000/");
        
        //GET
        public static string Get(string path)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = _baseAddress;
            HttpResponseMessage response = client.GetAsync(path).Result;
            //response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }

        //POST
        public static string Post(string path, string content)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = _baseAddress;
            content="{\"WfRunner\":" + content + "}";
            HttpResponseMessage response = client.PostAsync(path, new StringContent(content, Encoding.Default, "application/json")).Result;
            //response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }
    }

    public class WfRunner
    {
        public WfRunner()
        {
            NextActivityPerformers = new List<Point>();
        }
        public int ID { get; set; }
        public string AppName { get; set; }
        public string AppInstanceID { get; set; }
        public string ProcessGUID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public IList<Point> NextActivityPerformers { get; set; }
        public string JumpbackActivityGUID { get; set; }     //回跳的节点GUID
    }

    public class Point
    {
        public string PathID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
    }

    public class QueryEntity
    {
        public string AppName { get; set; }
        public string AppInstanceID { get; set; }
        public string ProcessGUID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
    }

    public class QueryResult
    {
        public int ID { get; set; }
        public int TaskID { get; set; }
        public string AppName { get; set; }
        public string AppInstanceID { get; set; }
        public string ProcessGUID { get; set; }
        public string Version { get; set; }
        public int ProcessInstanceID { get; set; }
        public string ActivityGUID { get; set; }
        public int ActivityInstanceID { get; set; }
        public string ActivityName { get; set; }
        public short ActivityType { get; set; }
        public short TaskType { get; set; }
        public string AssignedToUserID { get; set; }
        public string AssignedToUserName { get; set; }
        public string EndedByUserID { get; set; }
        public string EndedByUserName { get; set; }
        public short TaskState { get; set; }
        public short ActivityState { get; set; }
        public short ProcessState { get; set; }
    }
}
