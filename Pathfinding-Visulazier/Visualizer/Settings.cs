using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Pathfinding_Visulazier.Basic;

namespace Pathfinding_Visulazier.Visualizer
{
    class Settings
    {
        private Vector2D<int> StandardSliderSize               = new Vector2D<int>(140, 30);
        private int StandardSliderInterspace                   = 20;
        private Vector2D<int> SliderssStartPositionRightBottom = new Vector2D<int>(40, 160);

        private Slider _width = new Slider
        {
            Background = Brushes.SlateGray,
            Maximum = 100,
            Minimum = 20,
            Value = 60,
            TickPlacement = TickPlacement.Both,
            TickFrequency = 10
        };
        private Slider _height = new Slider
        {
            Background = Brushes.SlateGray,
            Maximum = 50,
            Minimum = 10,
            Value = 30,
            TickPlacement = TickPlacement.Both,
            TickFrequency = 5
        };
        private Slider _density = new Slider
        {
            Background = Brushes.SlateBlue,
            Maximum = 50,
            Minimum = 1,
            Value = 10,
            TickPlacement = TickPlacement.Both,
            TickFrequency = 4
        };
        private Slider _weight = new Slider
        {
            Background = Brushes.SlateBlue,
            Maximum = 20,
            Minimum = 2,
            Value = 10,
            TickPlacement = TickPlacement.Both,
            TickFrequency = 2
        };

        public Settings(UIElementCollection children)
        {
            int standardSliderDelta = StandardSliderInterspace + StandardSliderSize.Y;
            List<Slider> sliders = new List<Slider>();
            sliders.Add(_width);
            sliders.Add(_height);
            sliders.Add(_weight);
            sliders.Add(_density);
            List<TextBox> textBoxes = new List<TextBox>();

            TextBox widthText = new TextBox
            {
                Background = Brushes.LightGray,
                Text = "Widh of the Labyrinth"
            };
            textBoxes.Add(widthText);

            TextBox heightText = new TextBox
            {
                Background = Brushes.LightGray,
                Text = "Height of the Labyrinth"
            };
            textBoxes.Add(heightText);

            TextBox weightText = new TextBox
            {
                Background = Brushes.Violet,
                Text = "New weighted weight"
            };
            textBoxes.Add(weightText);

            TextBox densityText = new TextBox
            {
                Background = Brushes.Violet,
                Text = "New weighted density"
            };
            textBoxes.Add(densityText);

            for (int i=0; i<sliders.Count; i++)
            {
                sliders[i].Width = StandardSliderSize.X;
                sliders[i].Height = StandardSliderSize.Y;
                textBoxes[i].Width = StandardSliderSize.X;
                textBoxes[i].Height = StandardSliderSize.Y;
                Canvas.SetRight(sliders[i], SliderssStartPositionRightBottom.X);
                Canvas.SetBottom(sliders[i], SliderssStartPositionRightBottom.Y + i * 2 * standardSliderDelta);
                Canvas.SetRight(textBoxes[i], SliderssStartPositionRightBottom.X);
                Canvas.SetBottom(textBoxes[i], SliderssStartPositionRightBottom.Y + (i * 2 + 1) * standardSliderDelta);
                children.Add(sliders[i]);
                children.Add(textBoxes[i]);
            }
        }

        public Vector2D<int> GetLabyrinthSize()
        {
            return new Vector2D<int>
            (
                (int)_width.Value,
                (int)_height.Value
            ); 
        }

        public Tuple<int, int> GetMazeSettings()
        {
            return new Tuple<int, int>
            (
                (int)_weight.Value,
                (int)_density.Value
            );
        }
    }
}
