using ClientSample.WebAPIwithODataSample.Models;
using ClientSample.WebAPIwithODataSample.Models.SampleModel;
using ClientSample.WF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientSample
{
    class Program
    {
        //部分步骤需要需改TaskID，否则报错
        static void Main(string[] args)
        {
            //TestMethod();
            //AndSplit();
            //OrSplit();
            //OrSplitEnd();
            Multipleinstance();
            //SubProcess();
            //SubprocessMultipleinstance();
        }

        private static void SubprocessMultipleinstance()
        {
            string process_guid = "40c4be7f-2c7d-4391-92a3-32e2dc24f58d";
            string application_instance_id = Guid.NewGuid().ToString();
            //var serviceurl = "http://172.16.22.26:8000/odata/";
            var serviceurl = "http://localhost:53433/odata/";
            var data = new Container(new Uri(serviceurl));
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            WfRunner appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr1";
            appRunner.UserName = "usr1";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "fc8c71c5-8786-450e-af27-9f6a9de8560f", UserID = "usr21", UserName = "usr21" });
            appRunner.NextActivityPerformers.Add(new Point { PathID = "fc8c71c5-8786-450e-af27-9f6a9de8560f", UserID = "usr22", UserName = "usr22" });
            var aa = data.StartProcess(appRunner).GetValue();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var app = new QueryEntity();
            app.ProcessGUID = null;
            app.AppInstanceID = application_instance_id;
            app.AppName = "ApiTest";
            app.UserID = "usr21";
            app.UserName = "usr21";
            var cc = data.GetReadyTasks(app).Execute();

            appRunner = new WfRunner();
            appRunner.ProcessGUID = "40c4be7f-2c7d-4391-92a3-32e2dc24f58b";
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr21";
            appRunner.UserName = "usr21";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "76f7ef75-b538-40c8-b529-0849ca777b94", UserID = "", UserName = "" });
            string aaa = data.RunProcess(appRunner).GetValue();

            appRunner = new WfRunner();
            appRunner.ProcessGUID = "40c4be7f-2c7d-4391-92a3-32e2dc24f58b";
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr22";
            appRunner.UserName = "usr22";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "76f7ef75-b538-40c8-b529-0849ca777b94", UserID = "", UserName = "" });
             aaa = data.RunProcess(appRunner).GetValue();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

             appRunner = new WfRunner();
             appRunner.ProcessGUID = process_guid;
             appRunner.AppInstanceID = application_instance_id;
             appRunner.AppName = "ApiTest";
             appRunner.UserID = "usr21";
             appRunner.UserName = "usr21";
             appRunner.NextActivityPerformers.Add(new Point { PathID = "39c71004-d822-4c15-9ff2-94ca1068d745", UserID = "usr3", UserName = "usr3" });
             string ss = data.RunProcess(appRunner).GetValue();

             appRunner = new WfRunner();
             appRunner.ProcessGUID = process_guid;
             appRunner.AppInstanceID = application_instance_id;
             appRunner.AppName = "ApiTest";
             appRunner.UserID = "usr22";
             appRunner.UserName = "usr22";
             appRunner.NextActivityPerformers.Add(new Point { PathID = "39c71004-d822-4c15-9ff2-94ca1068d745", UserID = "usr3", UserName = "usr3" });
              ss = data.RunProcess(appRunner).GetValue();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr3";
            appRunner.UserName = "usr3";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "b70e717a-08da-419f-b2eb-7a3d71f054de", UserID = "", UserName = "" });
            ss = data.RunProcess(appRunner).GetValue();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        private static void SubProcess()
        {
            string process_guid = "40c4be7f-2c7d-4391-92a3-32e2dc24f585";
            string application_instance_id = Guid.NewGuid().ToString();
            //var serviceurl = "http://172.16.22.26:8000/odata/";
            var serviceurl = "http://localhost:53433/odata/";
            var data = new Container(new Uri(serviceurl));
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            WfRunner appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr1";
            appRunner.UserName = "usr1";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "fc8c71c5-8786-450e-af27-9f6a9de8560f", UserID = "usr2", UserName = "usr2" });
           // appRunner.NextActivityPerformers.Add(new Point { PathID = "fc8c71c5-8786-450e-af27-9f6a9de8560f", UserID = "usr22", UserName = "usr22" });
            var aa = data.StartProcess(appRunner).GetValue();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
             appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr2";
            appRunner.UserName = "usr2";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "5fa796f6-2d5d-4ed6-84e2-a7c4e4e6aabc", UserID = "usr3", UserName = "usr3" });
            // appRunner.NextActivityPerformers.Add(new Point { PathID = "fc8c71c5-8786-450e-af27-9f6a9de8560f", UserID = "usr22", UserName = "usr22" });
            string aaa = data.RunProcess(appRunner).GetValue();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var app = new QueryEntity();
            app.ProcessGUID = null;
            app.AppInstanceID = null;
            app.AppName = "ApiTest";
            app.UserID = "usr21";
            app.UserName = "usr21";
            var cc = data.GetReadyTasks(app).Execute();

            appRunner = new WfRunner();
            appRunner.ProcessGUID = "40c4be7f-2c7d-4391-92a3-32e2dc24f58c";
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr2";
            appRunner.UserName = "usr2";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "e60084e4-517a-4892-a290-517159f1b7f4", UserID = "usr21", UserName = "usr21" });
            string ss = data.RunProcess(appRunner).GetValue();

            appRunner = new WfRunner();
            appRunner.ProcessGUID = "40c4be7f-2c7d-4391-92a3-32e2dc24f58c";
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr21";
            appRunner.UserName = "usr21";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "0fdff3c0-be97-43d6-b4ff-90d52efb5d6f", UserID = "usr22", UserName = "usr22" });
            ss = data.RunProcess(appRunner).GetValue();

            appRunner = new WfRunner();
            appRunner.ProcessGUID = "40c4be7f-2c7d-4391-92a3-32e2dc24f58c";
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr22";
            appRunner.UserName = "usr22";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "76f7ef75-b538-40c8-b529-0849ca777b94", UserID = "usr23", UserName = "usr23" });
            ss = data.RunProcess(appRunner).GetValue();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr3";
            appRunner.UserName = "usr3";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "39c71004-d822-4c15-9ff2-94ca1068d745", UserID = "usr4", UserName = "usr4" });
            ss = data.RunProcess(appRunner).GetValue();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr4";
            appRunner.UserName = "usr4";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "b70e717a-08da-419f-b2eb-7a3d71f054de", UserID = "", UserName = "" });
            ss = data.RunProcess(appRunner).GetValue();
        }

        //O-D-|DDD|-D-O
        private static void Multipleinstance()
        {
            string process_guid = "40c4be7f-2c7d-4391-92a3-32e2dc24f58e";
            string application_instance_id = Guid.NewGuid().ToString();
            //var serviceurl = "http://172.16.22.26:8000/odata/";
            var serviceurl = "http://localhost:53433/odata/";
            var data = new Container(new Uri(serviceurl));
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            WfRunner appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr1";
            appRunner.UserName = "usr1";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "fc8c71c5-8786-450e-af27-9f6a9de8560f", UserID = "usr21", UserName = "usr21" });
            appRunner.NextActivityPerformers.Add(new Point { PathID = "fc8c71c5-8786-450e-af27-9f6a9de8560f", UserID = "usr22", UserName = "usr22" });
            var aa = data.StartProcess(appRunner).GetValue();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var app = new QueryEntity();
            app.ProcessGUID = process_guid;
            app.AppInstanceID = null;
            app.AppName = "ApiTest";
            app.UserID = "usr21";
            app.UserName = "usr21";
            var cc = data.GetReadyTasks(app).Execute();

            appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr21";
            appRunner.UserName = "usr21";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "39c71004-d822-4c15-9ff2-94ca1068d745", UserID = "usr31", UserName = "usr31" });
            string ss = data.RunProcess(appRunner).GetValue();

            appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr22";
            appRunner.UserName = "usr22";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "39c71004-d822-4c15-9ff2-94ca1068d745", UserID = "usr3", UserName = "usr3" });
            ss = data.RunProcess(appRunner).GetValue();

            app = new QueryEntity();
            app.ProcessGUID = process_guid;
            app.AppInstanceID = null;
            app.AppName = "ApiTest";
            app.UserID = "usr3";
            app.UserName = "usr3";
            var dd = data.GetReadyTasks(app).Execute();  ///XXX
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr3";
            appRunner.UserName = "usr3";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "b70e717a-08da-419f-b2eb-7a3d71f054de", UserID = "", UserName = "" });
            ss = data.RunProcess(appRunner).GetValue();
        }

        //           D
        //         /   \
        //O-D-R     R-D-O
        //         \   /
        //          D
        private static void OrSplit()
        {
            string process_guid = "40c4be7f-2c7d-4391-92a3-32e2dc24f58c";
            string application_instance_id = Guid.NewGuid().ToString();
            //var serviceurl = "http://172.16.22.26:8000/odata/";
            var serviceurl = "http://localhost:53433/odata/";
            var data = new Container(new Uri(serviceurl));
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            WfRunner appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr1";
            appRunner.UserName = "usr1";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "e60084e4-517a-4892-a290-517159f1b7f4", UserID = "usr21", UserName = "usr21" });
            appRunner.NextActivityPerformers.Add(new Point { PathID = "ce3343b6-930d-4962-a2b9-2c4c4b2dab06", UserID = "usr22", UserName = "usr22" });
            var aa = data.StartProcess(appRunner).GetValue();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //appRunner = new WfRunner();
            //appRunner.ProcessGUID = process_guid;
            //appRunner.AppInstanceID = application_instance_id;
            //appRunner.AppName = "ApiTest";
            //appRunner.UserID = "usr21";
            //appRunner.UserName = "usr21";
            //string sss = data.SendBackProcess(appRunner).GetValue();

            var app = new QueryEntity();
            app.ProcessGUID = process_guid;
            app.AppInstanceID = null;
            app.AppName = "ApiTest";
            app.UserID = "usr21";
            app.UserName = "usr21";
            var cc = data.GetReadyTasks(app).Execute();

            appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr21";
            appRunner.UserName = "usr21";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "0fdff3c0-be97-43d6-b4ff-90d52efb5d6f", UserID = "usr3", UserName = "usr3" });
            string ss = data.RunProcess(appRunner).GetValue();

            appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr22";
            appRunner.UserName = "usr22";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "0fdff3c0-be97-43d6-b4ff-90d52efb5d6f", UserID = "usr3", UserName = "usr3" });
            ss = data.RunProcess(appRunner).GetValue();

             app = new QueryEntity();
            app.ProcessGUID = process_guid;
            app.AppInstanceID = null;
            app.AppName = "ApiTest";
            app.UserID = "usr3";
            app.UserName = "usr3";
            var dd = data.GetReadyTasks(app).Execute();  ///XXX
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr3";
            appRunner.UserName = "usr3";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "76f7ef75-b538-40c8-b529-0849ca777b94", UserID = "", UserName = "" });
            ss = data.RunProcess(appRunner).GetValue();
        }

        //            D                                                 
        //         /      \
        //O-D-R    D-O
        //         \  /
        //          D
        private static void OrSplitEnd()
        {
            string process_guid = "40c4be7f-2c7d-4391-92a3-32e2dc24f58b";
            string application_instance_id = Guid.NewGuid().ToString();
            //var serviceurl = "http://172.16.22.26:8000/odata/";
            var serviceurl = "http://localhost:53433/odata/";
            var data = new Container(new Uri(serviceurl));
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            WfRunner appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr1";
            appRunner.UserName = "usr1";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "e60084e4-517a-4892-a290-517159f1b7f4", UserID = "usr21", UserName = "usr21" });
            appRunner.NextActivityPerformers.Add(new Point { PathID = "76f7ef75-b538-40c8-b529-0849ca777b95", UserID = "", UserName = "" });
            var aa = data.StartProcess(appRunner).GetValue();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            QueryEntity app = new QueryEntity();
            app.ProcessGUID = process_guid;
            app.AppInstanceID = null;
            app.AppName = "ApiTest";
            app.UserID = "usr21";
            app.UserName = "usr21";
            var cc = data.GetReadyTasks(app).Execute();

            //XXX
            appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr21";
            appRunner.UserName = "usr21";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "76f7ef75-b538-40c8-b529-0849ca777b94", UserID = "", UserName = "" });
            string ss = data.RunProcess(appRunner).GetValue();

        }

        //           D
        //         /   \
        //O-D-A     A-D-O
        //         \    /
        //          D
        private static void AndSplit()
        {
            string process_guid = "40c4be7f-2c7d-4391-92a3-32e2dc24f58a";
            string application_instance_id = Guid.NewGuid().ToString();
            //var serviceurl = "http://172.16.22.26:8000/odata/";
            var serviceurl = "http://localhost:53433/odata/";
            var data = new Container(new Uri(serviceurl));
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            WfRunner appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr1";
            appRunner.UserName = "usr1";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "e60084e4-517a-4892-a290-517159f1b7f4", UserID = "usr21", UserName = "usr21" });
            appRunner.NextActivityPerformers.Add(new Point { PathID = "ce3343b6-930d-4962-a2b9-2c4c4b2dab06", UserID = "usr22", UserName = "usr22" });
            var aa = data.StartProcess(appRunner).GetValue();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr21";
            appRunner.UserName = "usr21";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "0fdff3c0-be97-43d6-b4ff-90d52efb5d6f", UserID = "usr3", UserName = "usr3" });
            //appRunner.TaskID = ;
            string  ss = data.RunProcess(appRunner).GetValue();

            QueryEntity app = new QueryEntity();
            app.ProcessGUID = process_guid;
            app.AppInstanceID = null;
            app.AppName = "ApiTest";
            app.UserID = "usr3";
            app.UserName = "usr3";
            var cc = data.GetReadyTasks(app).Execute();

            appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr22";
            appRunner.UserName = "usr22";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "0fdff3c0-be97-43d6-b4ff-90d52efb5d6f", UserID = "usr3", UserName = "usr3" });
            ss = data.RunProcess(appRunner).GetValue();

            app = new QueryEntity();
            app.ProcessGUID = process_guid;
            app.AppInstanceID = null;
            app.AppName = "ApiTest";
            app.UserID = "usr3";
            app.UserName = "usr3";
             cc = data.GetReadyTasks(app).Execute();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
             appRunner = new WfRunner();
             appRunner.ProcessGUID = process_guid;
             appRunner.AppInstanceID = application_instance_id;
             appRunner.AppName = "ApiTest";
             appRunner.UserID = "usr3";
             appRunner.UserName = "usr3";
             appRunner.NextActivityPerformers.Add(new Point { PathID = "76f7ef75-b538-40c8-b529-0849ca777b94", UserID = "", UserName = "" });
             ss = data.RunProcess(appRunner).GetValue();
        }

        private static void TestMethod()
        {
            string process_guid = "40c4be7f-2c7d-4391-92a3-32e2dc24f58f";
            string application_instance_id = "56";

            // var serviceurl = "http://172.16.22.26:8000/odata/";
            var serviceurl = "http://localhost:53433/odata/";
            var data = new Container(new Uri(serviceurl));
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            Window w = new Window();
            w.Id = 1;
            w.Name = "2";
            w.CurrentShape = new Circle() { Radius = 1, HasBorder = true };
            w.OptionalShapes.Add(new Circle() { Radius = 1, HasBorder = true });
            var ls = data.ListAc(w).GetValue();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var eventDataItems = data.Vedios.ToList();
            data.AddObject("Vedios", new Vedio() { ID = 8, Title = "Inferno of Retribution", Year = 2005 });
            data.SaveChanges();
            var eventDataItemsw = data.Vedios.Where(p => p.Title == "2").ToList();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            WfRunner appRunner = new WfRunner();
            appRunner.ProcessGUID = process_guid;
            appRunner.AppInstanceID = application_instance_id;
            appRunner.AppName = "ApiTest";
            appRunner.UserID = "usr1";
            appRunner.UserName = "usr1";
            appRunner.NextActivityPerformers.Add(new Point { PathID = "fc8c71c5-8786-450e-af27-9f6a9de8560f", UserID = "usr20", UserName = "usr20" });
            var aa = data.StartProcess(appRunner).GetValue();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            QueryEntity app = new QueryEntity();
            app.ProcessGUID = process_guid;
            app.AppInstanceID = null;
            app.AppName = "ApiTest";
            app.UserID = "usr2";
            app.UserName = "usr2";
            var bb = data.GetRunningTasks(app).Execute();
            var cc = data.GetReadyTasks(app).Execute();
        }
    }
}
