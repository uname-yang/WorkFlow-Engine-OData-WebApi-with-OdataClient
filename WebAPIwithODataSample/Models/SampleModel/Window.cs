using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIwithODataSample.Models.SampleModel
{
    public class Window
    {
        public Window()
        {
            OptionalShapes = new List<Shape>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public Shape CurrentShape { get; set; }
        public IList<Shape> OptionalShapes { get; set; }
    }

    public abstract class Shape
    {
        public bool HasBorder { get; set; }
    }

    public class Circle : Shape
    {
        public int Radius { get; set; }
    }
}