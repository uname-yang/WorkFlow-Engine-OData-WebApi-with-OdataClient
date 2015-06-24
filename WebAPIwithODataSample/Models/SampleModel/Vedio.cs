using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIwithODataSample.Models.SampleModel
{
    public class Vedio
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public Picture Pic { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        public bool IsCheckedOut
        {
            get { return DueDate.HasValue; }
        }
    }
}