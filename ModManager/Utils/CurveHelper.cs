using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Imya.UI.Utils
{
    public static class CurveHelper
    {
        public static PathGeometry ConstructPolyBezier(Point start, PointCollection points)
        {
            PathFigure myPathFigure = new PathFigure();
            myPathFigure.StartPoint = start;

            PolyBezierSegment myBezierSegment = new PolyBezierSegment();
            myBezierSegment.Points = points;
            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
            myPathSegmentCollection.Add(myBezierSegment);
            myPathFigure.Segments = myPathSegmentCollection;
            PathFigureCollection myPathFigureCollection = new PathFigureCollection();
            myPathFigureCollection.Add(myPathFigure);
            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures = myPathFigureCollection;

            return myPathGeometry;
        }

        public static double InterpolateX(double rel_max, double abs_max, double x_raw)
        {
            return abs_max - x_raw * (abs_max / rel_max);
        }
    }
}
