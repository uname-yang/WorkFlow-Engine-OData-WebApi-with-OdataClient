using Newtonsoft.Json;
using Slickflow.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using WebAPIwithODataSample.Models;

namespace WebAPIwithODataSample.Extension
{
    public class JsonBinder<T> : HttpParameterBinding
    {
        private struct AsyncVoid { }

        public JsonBinder(HttpParameterDescriptor desc) : base(desc) { }

        public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider,
            HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            string json = actionContext.ControllerContext.Request.Content.ReadAsStringAsync().Result;

            T obj = Activator.CreateInstance<T>();

          //  var aa = JsonConvert.DeserializeObject<T>(json);
            SetValue(actionContext, JsonConvert.DeserializeObject<T>(json));

            //using (System.IO.MemoryStream ms =new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            //{       
            //    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            //    SetValue(actionContext, (T)serializer.ReadObject(ms));
            //}

            TaskCompletionSource<AsyncVoid> tcs = new TaskCompletionSource<AsyncVoid>();
            tcs.SetResult(default(AsyncVoid));
            return tcs.Task;
        }
    }
}