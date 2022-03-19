using Pathfinding_Visulazier.Basic;
using System;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Pathfinding_Visulazier.Visualizer.LabyrinthVisulaziers
{
    public class RectangleLabyrinthVisualizer : AbstarctLabyrinthVisualizer
    {
        public RectangleLabyrinthVisualizer(Vector2D<double> upperLeft, Vector2D<double> size, UIElementCollection children) : 
            base(upperLeft, size, children) { }

        protected override double CountFieldSize(Vector2D<int> labyrinthSzie)
        {
            return Math.Min(Size.X / labyrinthSzie.X, Size.Y / labyrinthSzie.Y);
        }

        protected override Polygon MakeField(Vector2D<int> position)
        {
            Polygon square = new Polygon();
            Point upperLeft = new Point(position.X * FieldSize + UpperLeft.X, position.Y * FieldSize + UpperLeft.Y);
            Point upperRight = new Point(upperLeft.X + FieldSize, upperLeft.Y);
            Point lowerRight = new Point(upperRight.X, upperRight.Y + FieldSize);
            Point lowerLeft = new Point(upperLeft.X, lowerRight.Y);
            PointCollection pointCollection = new PointCollection()
            {
                upperLeft,
                upperRight,
                lowerRight,
                lowerLeft
            };
            square.Points = pointCollection;
            square.Fill = null;
            square.Stroke = Brushes.Black;
            square.StrokeThickness = 1;
            return square;
        }
    }
}
