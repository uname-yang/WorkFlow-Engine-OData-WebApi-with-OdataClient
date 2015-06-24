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
        static void Main(string[] args)
        {
            string process_guid = "40c4be7f-2c7d-4391-92a3-32e2dc24f58f";
            string application_instance_id = "45603";

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
            appRunner.NextActivityPerformers.Add(new Point { PathID = "4324234", UserID = "332", UserName = "34423" });
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
