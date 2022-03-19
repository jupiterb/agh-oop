using System.Windows;
using System.Windows.Controls;
using Pathfinding_Visulazier.Basic;
using Pathfinding_Visulazier.Visualizer.Massages;

namespace Pathfinding_Visulazier.Visualizer
{
    public class Visualizer
    {
        private LabyrinthContainer _labyrinthContainer;
        private Menu _menu;
        private Settings _settings;
        private AlgoMenu _algoMenu;
        private MassagePanel _massagePanel;
        private Vector2D<double> labyrinthUpperLeft = new Vector2D<double>(160, 40);
        private Vector2D<double> labyrinthPanelSize = new Vector2D<double>(1000, 550);

        public void SetUp(UIElementCollection children, Window window)
        {
            _labyrinthContainer = new LabyrinthContainer(labyrinthUpperLeft, labyrinthPanelSize, new Vector2D<int>(80, 40), children);
            _massagePanel = new MassagePanel(children);
            _labyrinthContainer.Subscribe(_massagePanel);
            _settings = new Settings(children);
            _menu = new Menu(children, _labyrinthContainer, _settings, window);
            _algoMenu = new AlgoMenu(children, _labyrinthContainer);
        }

        public void Action()
        {
            _labyrinthContainer.SimualtePathfinding();
        }

        public void Walk(char direction)
        {
            _labyrinthContainer.Walk(direction);
        }
    }
}
