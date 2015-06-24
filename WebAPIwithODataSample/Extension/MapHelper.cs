using AutoMapper;
using Slickflow.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPIwithODataSample.Models;

namespace WebAPIwithODataSample.Extension
{
    public  static class MapHelper
    {
        public static WfAppRunner ToAppRunner(this WfRunner WfRunner)
        {
            Mapper.CreateMap<WfRunner, WfAppRunner>()
                    .ForMember(dest => dest.NextActivityPerformers, opt => opt.ResolveUsing<wfResolver>());
            var result = Mapper.Map<WfRunner, WfAppRunner>(WfRunner);
            return result;
        }
    }

    public class wfResolver : ValueResolver<WfRunner, IDictionary<string, PerformerList>>
    {
        protected override IDictionary<string, PerformerList> ResolveCore(WfRunner source)
        {
            var data = source.NextActivityPerformers.GroupBy(p=>p.PathID);
            IDictionary<string, PerformerList> result = new Dictionary<string, PerformerList>();
            foreach (var item in data)
            {
                PerformerList pl = new PerformerList();
                foreach (var per in item)
                {
                    pl.Add(new Performer(per.UserID, per.UserName));
                }
                result.Add(item.Key, pl);
            }
            return result;
        }
    }
}