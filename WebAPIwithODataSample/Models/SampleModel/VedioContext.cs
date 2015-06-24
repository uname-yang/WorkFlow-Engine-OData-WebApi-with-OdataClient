using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIwithODataSample.Models.SampleModel
{
    public class VedioContext
    {
        static List<Vedio> _vedio;
        static VedioContext()
        {
            _vedio = new List<Vedio>()
            {
                new Vedio() { ID=1, Title = "Maximum Payback", Year = 1990 ,Pic=new Picture{ID=1,Name="1",Path="1"}},
                new Vedio() { ID=2, Title = "Inferno of Retribution", Year = 2005 ,Pic=new Picture{ID=2,Name="2",Path="2"} },
                new Vedio() { ID=3, Title = "Fatal Vengeance 2", Year = 2012 ,Pic=new Picture{ID=3,Name="3",Path="3"} },
                new Vedio() { ID=4, Title = "Sudden Danger", Year = 2012  ,Pic=new Picture{ID=4,Name="4",Path="4"}},
                new Vedio() { ID=5, Title = "Beyond Outrage", Year = 2014  ,Pic=new Picture{ID=5,Name="5",Path="5"}},
                new Vedio() { ID=6, Title = "The Nut Job", Year = 2014  ,Pic=new Picture{ID=6,Name="6",Path="6"}}
            };
        }

        public List<Vedio> Vedio { get { return _vedio; } }
    }
}