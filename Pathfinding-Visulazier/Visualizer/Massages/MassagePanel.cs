using Pathfinding_Visulazier.Basic;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pathfinding_Visulazier.Visualizer.Massages
{
    public class MassagePanel : IObserver<LabyrinthContainerStatus>
    {
        private Vector2D<int> StandardBoxSize               = new Vector2D<int>(100, 250);
        private int StandardBoxInterspace                   = 20;
        private Vector2D<int> BoxsesStartPositionLeftTop    = new Vector2D<int>(20, 40);

        private int DistanceWalked = 0;

        TextBox lower = new TextBox
        {
            Background = Brushes.Wheat
        };
        TextBox upper = new TextBox
        {
            Background = Brushes.Violet
        };

        public MassagePanel(UIElementCollection children)
        {
            int standardBoxesDelta = StandardBoxInterspace + StandardBoxSize.Y;
            List<TextBox> boxes = new List<TextBox>
            {
                upper,
                lower
            };

            for (int i=0; i<boxes.Count; i++)
            {
                boxes[i].Width = StandardBoxSize.X;
                boxes[i].Height = StandardBoxSize.Y;
                Canvas.SetLeft(boxes[i], BoxsesStartPositionLeftTop.X);
                Canvas.SetTop(boxes[i], BoxsesStartPositionLeftTop.Y + i * standardBoxesDelta);
                children.Add(boxes[i]);
            }
        }

        public void OnCompleted()
        {
            upper.Text = null;
            lower.Text = null;
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(LabyrinthContainerStatus value)
        {
            if (value.PathfinderWorking == true)
            {
                upper.Text = "Pathfinder is\nlooking for the\nshortest path";
            }
            else if (value.PathfinderWorking == null)
            {
                if (value.Last_DisatnceWalked == null)
                    upper.Text = "Pathfinder mode\n";
            }
            else
            {
                DistanceWalked = 0;
                upper.Text = "Labyrinth mode\n\nTo Walke use\nW A S D\n(square borad)\nor\nQ W E A S D\n(trinagle board)";
            }
            if (value.VisitedVetexes != null)
            {
                lower.Text = "Visited vertexes:\n" + value.VisitedVetexes.ToString() + "\nShortest\npath length:\n" 
                  + ((value.ShortedDistance_Start == null) ? "Unknown path" : value.ShortedDistance_Start.ToString());
            }
            if (value.Last_DisatnceWalked != null)
            {
                DistanceWalked += (int)value.Last_DisatnceWalked;
                lower.Text = "Distance walked:\n" + DistanceWalked.ToString() + "\nDistance from\nstart to finish:\n"
                    + ((value.ShortedDistance_Start == null) ? "Unknown path" : value.ShortedDistance_Start.ToString()) 
                    + "\nDistance from\nWalker to finish:\n" 
                    + ((value.ShortedDistance_Walker == null) ? "Unknown path" : value.ShortedDistance_Walker.ToString());
            }
            if (value.WalkerOnFinish == true)
            {
                if (value.ShortedDistance_Start == DistanceWalked)
                {
                    lower.Text += "\nCongratulations!\nYou go throw\nthis labyrinth\nby the shortest path!";
                }
                else
                {
                    lower.Text += "\nYou went to\nfinish, bu this\nwasn't the\nshortest way.";
                }
            }
        }
    }
}
