using Pathfinding_Visulazier.Basic;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pathfinding_Visulazier.Visualizer
{
    class AlgoMenu
    {
        private Vector2D<int> StandardButtonSize            = new Vector2D<int>(60, 60);
        private Vector2D<int> StandardButtonInterspace      = new Vector2D<int>(20, 18);
        private Vector2D<int> ButtonsStartPositionRightTop  = new Vector2D<int>(40, 40);
        private LabyrinthContainer _labyrinthContainer;

        public AlgoMenu(UIElementCollection children, LabyrinthContainer labyrinthContainer)
        {
            _labyrinthContainer = labyrinthContainer;
            Vector2D<int> standardButtonDelta = new Vector2D<int>(StandardButtonSize.X + StandardButtonInterspace.X,
                                                                  StandardButtonSize.Y + StandardButtonInterspace.Y);
            List<Button> buttons = new List<Button>();

            Button bfs = new Button
            {
                Background = Brushes.Wheat,
                Content = "Apply\nBFS"
            };
            bfs.Click += Bfs_Click;
            buttons.Add(bfs);

            Button dijkstra = new Button
            {
                Background = Brushes.Wheat,
                Content = "Apply\nDijkstra"
            };
            dijkstra.Click += Dijkstra_Click;
            buttons.Add(dijkstra);

            Button aStar = new Button
            {
                Background = Brushes.Wheat,
                Content = "Apply\nClassical\nA*"
            };
            aStar.Click += AStar_Click;
            buttons.Add(aStar);

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                buttons[i].VerticalAlignment = System.Windows.VerticalAlignment.Center;
                buttons[i].Width = StandardButtonSize.X;
                buttons[i].Height = StandardButtonSize.Y;
                Canvas.SetRight(buttons[i], ButtonsStartPositionRightTop.X + (i / 2) * standardButtonDelta.X);
                Canvas.SetTop(buttons[i], ButtonsStartPositionRightTop.Y + (i % 2) * standardButtonDelta.Y);
                children.Add(buttons[i]);
            }
        }

        private void AStar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _labyrinthContainer.VisulaizeAStar();
        }

        private void Dijkstra_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _labyrinthContainer.VisulaizeDijkstra();
        }

        private void Bfs_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _labyrinthContainer.VisulaizeBFS();
        }
    }
}
