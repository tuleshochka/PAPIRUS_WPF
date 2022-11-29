using PAPIRUS_WPF.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace PAPIRUS_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        //The boolean that signifys when an output is being linked
        private bool _linkingStarted = false;
        //The temporary line that shows when linking an output
        private LineGeometry _tempLink;
        //The output that is being linked to
        private Output _tempOutput;

        public MainWindow()
        {
            InitializeComponent();

            CircuitCanvas.MouseDown += CircuitCanvas_MouseDown;
          // CircuitCanvas.MouseMove += CircuitCanvas_MouseMove;
           // CircuitCanvas.MouseUp += CircuitCanvas_MouseUp;
          //  CircuitCanvas.Drop += CircuitCanvas_Drop;
        }
        private void CircuitCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Get the position of the mouse relative to the circuit canvas
            Point MousePosition = e.GetPosition(CircuitCanvas);

            //Do a hit test under the mouse position
            HitTestResult result = VisualTreeHelper.HitTest(CircuitCanvas, MousePosition);

            //Make sure that there is something under the mouse
            if (result == null || result.VisualHit == null)
                return;

            //If the mouse has hit a border
            if (result.VisualHit is Border)
            {
                //Get the parent class of the border
                Border border = (Border)result.VisualHit;
                var IO = border.Parent;

                //If the parent class is an Output
                if (IO is Output)
                {
                    //Cast to output
                    Output IOOutput = (Output)IO;

                    //Get the center of the output relative to the canvas
                    Point position = IOOutput.TransformToAncestor(CircuitCanvas).Transform(new Point(IOOutput.ActualWidth / 2, IOOutput.ActualHeight / 2));

                    //Creates a new line
                    _linkingStarted = true;
                    _tempLink = new LineGeometry(position, position);

                    //Assign it to the list of connections to be displayed
                    Connections.Children.Add(_tempLink);

                    //Assign the temporary output to the current output
                    _tempOutput = (Output)IO;

                    e.Handled = true;
                }
            }
        }
        private void CircuitCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            //If there is a linking in progress
            if (_linkingStarted)
            {
                //Move the link endpoint to the current location of the mouse
                _tempLink.EndPoint = e.GetPosition(CircuitCanvas);
                e.Handled = true;
            }
        }

    }
}
