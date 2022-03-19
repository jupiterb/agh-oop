using Pathfinding_Visulazier.Basic;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace Pathfinding_Visulazier.Visualizer.LabyrinthVisulaziers
{
    public class PositionPolygonDictionary
    {
        private Dictionary<Vector2D<int>, Polygon> PolygonsMap = new Dictionary<Vector2D<int>, Polygon>();
        private Dictionary<Polygon, Vector2D<int>> PositionsMap = new Dictionary<Polygon, Vector2D<int>>();

        public PositionPolygonDictionary() { }

        public void Add(Vector2D<int> position, Polygon polygon)
        {
            PolygonsMap.Add(position, polygon);
            PositionsMap.Add(polygon, position);
        }

        public void Remove(Vector2D<int> position)
        {
            Polygon polygon = PolygonsMap[position];
            PolygonsMap.Remove(position);
            PositionsMap.Remove(polygon);
        }

        public Polygon Get(Vector2D<int> position)
        {
            return PolygonsMap[position];
        }

        public Vector2D<int> Get(Polygon polygon)
        {
            return PositionsMap[polygon];
        }

        public void Clear()
        {
            PositionsMap.Clear();
            PolygonsMap.Clear();
        }

        public Polygon[] Polygons()
        {
            Polygon[] polygons = new Polygon[PolygonsMap.Count];
            PolygonsMap.Values.CopyTo(polygons, 0);
            return polygons;
        }
    }
}
