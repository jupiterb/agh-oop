using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Pathfinding_Visulazier
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Visualizer.Visualizer _visualizer = new Visualizer.Visualizer();

        public MainWindow()
        {
            InitializeComponent();
            var timer = new DispatcherTimer(); 
            timer.Interval = TimeSpan.FromMilliseconds(5); 
            timer.Start(); 
            timer.Tick += Timer_Tick;
            KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            _visualizer.Walk(e.Key.ToString()[0]);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _visualizer.Action();
        }

        private void WindowContentRendered(object sender, EventArgs e)
        {
            _visualizer.SetUp(VisualizeArea.Children, this);
        }
    }
}
