using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Microsoft.OData.Edm;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using WebAPIwithODataSample.Models.SampleModel;
using WebAPIwithODataSample.Models;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Common;
using WebAPIwithODataSample.Extension;

namespace WebAPIwithODataSample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //config.ParameterBindingRules.Insert(0, param =>
            //{
            //    if (param.ParameterType == typeof(WfRunner))
            //        return new JsonBinder<WfRunner>(param);
            //    //if (param.ParameterType == typeof(QueryEntity))
            //    //    return new JsonBinder<QueryEntity>(param);
            //    return null;
            //});

            config.MapODataServiceRoute("OData", "odata", GetEdmModel());
           // config.enab
        }
        // Builds the EDM model for the OData service, including the OData action definitions.
        private static IEdmModel GetEdmModel()
        {
            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<Vedio>("Vedios");

            ActionConfiguration returnAcAction = modelBuilder.Action("ReturnAc");
            returnAcAction.Parameter<int>("key");
            returnAcAction.ReturnsFromEntitySet<Vedio>("Vedios");

            FunctionConfiguration checkOutAction = modelBuilder.Function("CheckOut");
            checkOutAction.Parameter<int>("key");
            checkOutAction.ReturnsFromEntitySet<Vedio>("Vedios");

            ActionConfiguration createMovieAction = modelBuilder.Action("CreateVedio");
            createMovieAction.Parameter<Vedio>("vedio");
            createMovieAction.ReturnsFromEntitySet<Vedio>("Vedios");

            ActionConfiguration ListAcAction = modelBuilder.Action("ListAc");
            ListAcAction.Parameter<Window>("windows");
            ListAcAction.Returns<int>();

            ActionConfiguration checkOutManyAction = modelBuilder.Action("CheckOutMany");
            checkOutManyAction.CollectionParameter<int>("MovieIDs");
            checkOutManyAction.ReturnsCollectionFromEntitySet<Vedio>("Vedios");

            //######################################################################################################//
            modelBuilder.EntitySet<QueryResult>("QueryResults");

            FunctionConfiguration GetRunningTasks = modelBuilder.Function("GetRunningTasks");
            GetRunningTasks.Parameter<QueryEntity>("query");
            GetRunningTasks.ReturnsCollectionFromEntitySet<QueryResult>("QueryResults");

            FunctionConfiguration GetReadyTasks = modelBuilder.Function("GetReadyTasks");
            GetReadyTasks.Parameter<QueryEntity>("query");
            GetReadyTasks.ReturnsCollectionFromEntitySet<QueryResult>("QueryResults");

            //######################################################################################################//
           
            ActionConfiguration StartProcess = modelBuilder.Action("StartProcess");
            StartProcess.Parameter<WfRunner>("WfRunner");
            StartProcess.Returns<int>();

            ActionConfiguration RunProcess = modelBuilder.Action("RunProcess");
            RunProcess.Parameter<WfRunner>("WfRunner");
            RunProcess.Returns<string>();

            ActionConfiguration WithdrawProcess = modelBuilder.Action("WithdrawProcess");
            WithdrawProcess.Parameter<WfRunner>("WfRunner");
            WithdrawProcess.Returns<string>();

            ActionConfiguration SendBackProcess = modelBuilder.Action("SendBackProcess");
            SendBackProcess.Parameter<WfRunner>("WfRunner");
            SendBackProcess.Returns<string>();

            ActionConfiguration JumpProcess = modelBuilder.Action("JumpProcess");
            JumpProcess.Parameter<WfRunner>("WfRunner");
            JumpProcess.Returns<string>();

            ActionConfiguration ReverseProcess = modelBuilder.Action("ReverseProcess");
            ReverseProcess.Parameter<WfRunner>("WfRunner");
            ReverseProcess.Returns<string>();

            ActionConfiguration DiscardProcess = modelBuilder.Action("DiscardProcess");
            DiscardProcess.Parameter<WfRunner>("WfRunner");
            DiscardProcess.Returns<string>();

            modelBuilder.Namespace = "WF"; 
            return modelBuilder.GetEdmModel();
        }
    }
}
