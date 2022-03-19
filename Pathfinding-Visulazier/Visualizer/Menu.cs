using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Pathfinding_Visulazier.Basic;

namespace Pathfinding_Visulazier.Visualizer
{
    class Menu
    {
        private LabyrinthContainer  _labyrinthContainer;
        private Settings            _settings;
        private Window              _window;

        private Vector2D<int> StandardButtonSize             = new Vector2D<int>(140, 40);
        private Vector2D<int> StandardButtonInterspace       = new Vector2D<int>(20, 18);
        private Vector2D<int> ButtonsStartPositionLeftBottom = new Vector2D<int>(40, 40);

        public Menu(UIElementCollection children, LabyrinthContainer labyrinthContainer, Settings settings, Window window)
        {
            _labyrinthContainer = labyrinthContainer;
            _settings = settings;
            _window = window;

            Vector2D<int> standardButtonDelta = new Vector2D<int>(StandardButtonSize.X + StandardButtonInterspace.X,
                                                                  StandardButtonSize.Y + StandardButtonInterspace.Y);
            List<Button> buttons = new List<Button>();

            Button labyrinthMode = new Button
            {
                Background = Brushes.BlueViolet,
                Content = "Labyrinth Mode"
            };
            labyrinthMode.Click += LabyrinthMode_Click;
            buttons.Add(labyrinthMode);

            Button pathfinderMode = new Button
            {
                Background = Brushes.BlueViolet,
                Content = "Pathfinder Mode"
            };
            pathfinderMode.Click += PathfinderMode_Click;
            buttons.Add(pathfinderMode);

            Button squareBoard = new Button
            {
                Background = Brushes.Violet,
                Content = "Square Board"
            };
            squareBoard.Click += SquareBoard_Click;
            buttons.Add(squareBoard);

            Button trinagleBoard = new Button
            {
                Background = Brushes.Violet,
                Content = "Trinagle Board"
            };
            trinagleBoard.Click += TrinagleBoard_Click;
            buttons.Add(trinagleBoard);

            Button manualWall = new Button
            {
                Background = Brushes.SlateBlue,
                Content = "Adding Walls Manualy"
            };
            manualWall.Click += ManualWall_Click;
            buttons.Add(manualWall);

            Button weightedManual = new Button
            {
                Background = Brushes.SlateBlue,
                Content = "Adding Weighted Nodes"
            };
            weightedManual.Click += WeightedManual_Click;
            buttons.Add(weightedManual);

            Button clearAll = new Button
            {
                Background = Brushes.Wheat,
                Content = "Clear All"
            };
            clearAll.Click += ClearAll_Click;
            buttons.Add(clearAll);

            Button clearManual = new Button
            {
                Background = Brushes.Wheat,
                Content = "Clear Manualy"
            };
            clearManual.Click += ClearManual_Click;
            buttons.Add(clearManual);

            Button clearVisited = new Button
            {
                Background = Brushes.Wheat,
                Content = "Clear Visited"
            };
            clearVisited.Click += ClearVisited_Click;
            buttons.Add(clearVisited);

            Button clearMaze = new Button
            {
                Background = Brushes.Wheat,
                Content = "Clear Maze"
            };
            clearMaze.Click += ClearMaze_Click;
            buttons.Add(clearMaze);

            Button generateMaze2 = new Button
            {
                Background = Brushes.PaleVioletRed,
                Content = "Genearte Maze - Type 2"
            };
            buttons.Add(generateMaze2);

            Button generateMaze1 = new Button
            {
                Background = Brushes.PaleVioletRed,
                Content = "Genearte Maze - Type 1"
            };
            generateMaze1.Click += GenerateMaze1_Click;
            buttons.Add(generateMaze1);

            Button escape = new Button
            {
                Background = Brushes.SlateGray,
                Content = "Escape"
            };
            escape.Click += Escape_Click;
            buttons.Add(escape);

            Button applyResize = new Button
            {
                Background = Brushes.SlateGray,
                Content = "Apply Resize"
            };
            applyResize.Click += ApplyResize_Click;
            buttons.Add(applyResize);

            Button resetWalker = new Button
            {
                Background = Brushes.SlateGray,
                Content = "Reset Labyrinth Mode"
            };
            resetWalker.Click += ResetWalker_Click;
            buttons.Add(resetWalker);

            for (int i=0; i<buttons.Count; i++)
            {
                buttons[i].Width = StandardButtonSize.X;
                buttons[i].Height = StandardButtonSize.Y;
                Canvas.SetLeft(buttons[i], ButtonsStartPositionLeftBottom.X + (i / 2) * standardButtonDelta.X);
                Canvas.SetBottom(buttons[i], ButtonsStartPositionLeftBottom.Y + (i % 2) * standardButtonDelta.Y);
                children.Add(buttons[i]);
            }
        }

        private void ResetWalker_Click(object sender, RoutedEventArgs e)
        {
            _labyrinthContainer.SetLabyrinthMode();
        }

        private void LabyrinthMode_Click(object sender, RoutedEventArgs e)
        {
            _labyrinthContainer.SetLabyrinthMode();
        }

        private void PathfinderMode_Click(object sender, RoutedEventArgs e)
        {
            _labyrinthContainer.SetPathfinderMode();
        }

        private void ManualWall_Click(object sender, RoutedEventArgs e)
        {
            _labyrinthContainer.LabyrinthVisualizer.SetManualWalling();
        }

        private void WeightedManual_Click(object sender, RoutedEventArgs e)
        {
            _labyrinthContainer.LabyrinthVisualizer.SetManualWeighting(_settings.GetMazeSettings().Item1);
        }

        private void ClearManual_Click(object sender, RoutedEventArgs e)
        {
            _labyrinthContainer.LabyrinthVisualizer.SetManualCleaning();
        }

        private void Escape_Click(object sender, RoutedEventArgs e)
        {
            _window.Close();
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            _labyrinthContainer.Labyrinth.ClearAll();
        }

        private void ClearVisited_Click(object sender, RoutedEventArgs e)
        {
            _labyrinthContainer.Labyrinth.ClearVisited();
            _labyrinthContainer.Labyrinth.ClearPath();
        }

        private void ClearMaze_Click(object sender, RoutedEventArgs e)
        {
            _labyrinthContainer.Labyrinth.ClearWeighted();
            _labyrinthContainer.Labyrinth.ClearWalls();
        }

        private void TrinagleBoard_Click(object sender, RoutedEventArgs e)
        {
            _labyrinthContainer.SetParalleogram();
        }

        private void SquareBoard_Click(object sender, RoutedEventArgs e)
        {
            _labyrinthContainer.SetRectangle();
        }

        private void ApplyResize_Click(object sender, RoutedEventArgs e)
        {
            _labyrinthContainer.Resize(_settings.GetLabyrinthSize());
        }

        private void GenerateMaze1_Click(object sender, RoutedEventArgs e)
        {
            Tuple<int, int> mazeSettings = _settings.GetMazeSettings();
            _labyrinthContainer.GenearteSpaghettiMaze(10, mazeSettings.Item2, mazeSettings.Item1);
        }
    }
}
