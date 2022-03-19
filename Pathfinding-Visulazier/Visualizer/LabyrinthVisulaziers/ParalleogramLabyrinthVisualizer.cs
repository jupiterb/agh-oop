using Pathfinding_Visulazier.Basic;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Pathfinding_Visulazier.Visualizer.LabyrinthVisulaziers
{
    public class ParalleogramLabyrinthVisualizer : AbstarctLabyrinthVisualizer
    {
        private int _maxY;

        public ParalleogramLabyrinthVisualizer(Vector2D<double> upperLeft, Vector2D<double> size, UIElementCollection children) :
            base(upperLeft, size, children)
        { }

        private double FieldWidth(double fieldSize)
        {
            return fieldSize * Math.Sqrt(3) / 2;
        }

        protected override double CountFieldSize(Vector2D<int> labyrinthSzie)
        {
            _maxY = labyrinthSzie.Y - 2;
            Vector2D<int> elements = new Vector2D<int>
            (
                labyrinthSzie.X + labyrinthSzie.Y / 2,
                labyrinthSzie.X / 2 + labyrinthSzie.Y / 4 + 1
            );
            return Math.Min(Size.X / elements.X * 2 / Math.Sqrt(3), Size.Y / elements.Y);
        }

        protected override Polygon MakeField(Vector2D<int> position)
        {
            Polygon trinagle = new Polygon();
            int yInt = (_maxY / 2 - position.Y / 2);
            double yDouble = (double)yInt / 2;;
            Point upper = new Point
            {
                X = UpperLeft.X + (1 + position.X + position.Y / 2) * FieldWidth(FieldSize),
                Y = UpperLeft.Y + ( (double)yInt / 2 + (double)position.X / 2) * FieldSize
            };
            Point lower = new Point
            {
                X = upper.X,
                Y = upper.Y + FieldSize
            };
            Point side = new Point
            {
                X = (position.Y % 2 == 0)? upper.X - FieldWidth(FieldSize) : upper.X + FieldWidth(FieldSize),
                Y = upper.Y + FieldSize / 2
            };
            PointCollection pointCollection = new PointCollection()
            {
                upper,
                lower,
                side
            };
            trinagle.Points = pointCollection;
            trinagle.Fill = null;
            trinagle.Stroke = Brushes.Black;
            trinagle.StrokeThickness = 1;
            return trinagle;
        }
    }
}
