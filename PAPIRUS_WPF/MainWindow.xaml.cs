using PAPIRUS_WPF.Elements;
using PAPIRUS_WPF.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
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
using System.Xml.Linq;
using System.Timers;
using static System.Net.Mime.MediaTypeNames;

namespace PAPIRUS_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point? myDragStartPoint { get; set; }
        //The boolean that signifys when an output is being linked
        private bool _linkingStarted = false;
        //The temporary line that shows when linking an output
        private LineGeometry _tempLink;
        //The output that is being linked to
        private Output _tempOutput;
        bool MiddleClick = false;
        public Point point;
        private Point MoveSelStartPoint;
        private Point panStart;
        public bool panning;
        private TranslateTransform moveVector;
        private Point maxMove;

        private Canvas selectionLayer;
        private List<UserControl> selection = new List<UserControl>();
        public FrameworkElement singleElement = null;
        private Timer ClickTimer;
        private int ClickCounter;

        public bool MovingSelectionIsStarted = false;
        FrameworkElement Source = null;
        Point MousePosition;
        ModifierKeys Keys;
        Object Object;

        public MainWindow()
        {
            ClickTimer = new Timer(300);
            ClickTimer.Elapsed += new ElapsedEventHandler(EvaluateClicks);
            InitializeComponent();
            WPF_SHF_Element_lib.Window1 window1 = new WPF_SHF_Element_lib.Window1();
            window1.ShowDialog();

            CircuitCanvas.MouseDown += CircuitCanvas_MouseDown;
            CircuitCanvas.MouseMove += CircuitCanvas_MouseMove;
            CircuitCanvas.MouseUp += CircuitCanvas_MouseUp;


        }
        public delegate System.Windows.Media.HitTestResultBehavior HitTestResultCallbak(HitTestResult result);


        //--------Selection-------//



        public void ClearSelection()
        {
            this.selection.Clear();
            if (this.selectionLayer != null)
            {
                foreach (FrameworkElement object_ in CircuitCanvas.Children)
                {
                    Console.WriteLine(object_.ToString());
                    if (object_ is Object) 
                    {
                        Object = (Object)object_;
                        Object.isSelected = false;
                    }
                }
                this.selectionLayer.Children.Clear();
            }
        }

        public void SingleElementSelect(FrameworkElement element)
        {
            //Object = element as Object;
            //Object.


            this.selection.Add((UserControl)element);
            Border item = new Border();

            item = Symbol.Skin<Border>(SymbolShape.SelectionMarker);
            if (this.selectionLayer == null)
            {
                this.selectionLayer = new Canvas()
                {
                    RenderTransform = this.moveVector = new TranslateTransform()
                };
                Panel.SetZIndex(this.selectionLayer, int.MaxValue);
            }
            if (this.selectionLayer.Parent != this.CircuitCanvas)
            {
                this.CircuitCanvas.Children.Add(this.selectionLayer);
            }
            selectionLayer.Children.Add(item);
            Canvas.SetLeft(item, element.TransformToAncestor(CircuitCanvas).Transform(new Point(0, 0)).X);
            Canvas.SetTop(item, element.TransformToAncestor(CircuitCanvas).Transform(new Point(0, 0)).Y);

            item.Width = element.ActualWidth;
            item.Height = element.ActualHeight;
        }

        private HitTestResultBehavior MyHitFilter (HitTestResult result)
        {
            if(result.VisualHit.GetType() == typeof(UserControl))
            {
                Console.WriteLine("result: " + (result.VisualHit));
                return HitTestResultBehavior.Stop;
            }
            Console.WriteLine("result: " + (result.VisualHit));
            return HitTestResultBehavior.Continue;
        }

        private void CircuitCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //для тыка колесика
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
            {
                Cursor = Cursors.SizeAll;
                MiddleClick = true;
                point = Mouse.GetPosition(CircuitCanvas);

            }
            //тык левой кнопкой
            if (e.ChangedButton == MouseButton.Left)
            {
                Source = e.Source as FrameworkElement;
                MousePosition = e.GetPosition(CircuitCanvas);
                Keys = Keyboard.Modifiers;
                ClickTimer.Stop();
                ClickCounter++;
                if (ClickCounter == 2)
                {
                    if (selection.Count > 1)
                    {
                        ClearSelection();
                        SingleElementSelect(Source);
                        Object = Source as Object;
                        Object.isSelected = true;
                    }
                    GeneratorDialog gd = new GeneratorDialog();
                    gd.ShowDialog();

                }
                //в другом случае (один тык)
                else if (ClickCounter == 1)
                {
                    if (Source != this.CircuitCanvas)
                    {
                        if (Keys == ModifierKeys.Control)
                        {
                            if (selection.Contains(Source) == false)
                            { 
                                SingleElementSelect(Source);
                                Object = Source as Object;
                                Object.isSelected = true;
                            }

                        }
                        else
                        {

                            ClearSelection();
                            SingleElementSelect(Source);
                            Object = Source as Object;
                            Object.isSelected = true;
                        }
                    }
                    else if (Source == this.CircuitCanvas)
                    {
                        if (Keys != ModifierKeys.Control)
                        { // Nothing was clicked on the diagram
                            if (ClickCounter < 2)
                            {
                                if (Keys != ModifierKeys.Shift)
                                {
                                    this.ClearSelection();
                                    MovingSelectionIsStarted = true;
                                    MoveSelStartPoint = MousePosition;
                                    //CaptureMouse();
                                }
                                //this.StartAreaSelection(e.GetPosition(this.CircuitCanvas));
                                else
                                {
                                    this.ClearSelection();
                                }
                            }
                        }
                    }
                }
                ClickTimer.Start();

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
        }

        private void EvaluateClicks(object source, ElapsedEventArgs e)
        {
            ClickTimer.Stop();
            //Console.WriteLine(ClickCounter.ToString());
            //если тык два раза
            
                //Evaluate ClickCounter here
                ClickCounter = 0;
        }


        private void CircuitCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if(MiddleClick) CircuitCanvas_MouseWheelCLick();
            //If there is a linking in progress
            if (_linkingStarted)
            {
                //Move the link endpoint to the current location of the mouse
                _tempLink.EndPoint = e.GetPosition(CircuitCanvas);
                e.Handled = true;
            } 
            else
            if (MovingSelectionIsStarted)
            {
                Point point = e.GetPosition(this.CircuitCanvas);
               // this.movingMarker.Move(this, new Point(Math.Max(this.maxMove.X, point.X), Math.Max(this.maxMove.Y, point.Y)));
            } else if (this.panning)
            {

            }
        }

       

        private void CircuitCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
			if(this.panning) {
            Mouse.Capture(null);
            this.panning = false;
        }
            MiddleClick =  false;
            Cursor = Cursors.Arrow;
            //If there is a linking in progress
            if (_linkingStarted)
            {
                //Temporary value to show 
                bool linked = false;

                //Get the type of the element that the mouse went up on
                var BaseType = e.Source.GetType().BaseType;

                if (BaseType == typeof(Object))
                {
                    //Convert to a circuit object
                    Object obj = (Object)e.Source;

                    //Get the position of the mouse relative to the circuit object
                    Point MousePosition = e.GetPosition(obj);

                    //Get the element underneath the mouse
                    HitTestResult result = VisualTreeHelper.HitTest(obj, MousePosition);

                    //Return if there is no element under the cursor
                    if (result == null || result.VisualHit == null)
                    {
                        //Remove the temporary line
                        Connections.Children.Remove(_tempLink);
                        _tempLink = null;
                        _linkingStarted = false;
                        return;
                    }

                    //If the underlying element is a border element
                    if (result.VisualHit is Border)
                    {
                        Border border = (Border)result.VisualHit;
                        var IO = border.Parent;

                        //Check if the border element is a input element in disguise
                        if (IO is Input)
                        {
                            //Convert to a input element
                            Input IOInput = (Input)IO;

                            //Get the center of the input relative to the canvas
                            Point inputPoint = IOInput.TransformToAncestor(CircuitCanvas).Transform(new Point(IOInput.ActualWidth / 2, IOInput.ActualHeight / 2));

                            //Ends the line in the centre of the input
                            _tempLink.EndPoint = inputPoint;

                            //Links the output to the input
    //IOInput.LinkInputs(_tempOutput);

                            //Adds to the global list
                            

                            //Attaches the line to the object
                            obj.AttachInputLine(_tempLink);

                            //Some evil casting (the outputs' parent of the parent is the circuit object that contains the output). Attaches the output side to the object
                            ((Object)((Grid)_tempOutput.Parent).Parent).AttachOutputLine(_tempLink);

                            //Set linked to true
                            linked = true;
                        }
                    }
                }

                //If it isn't linked remove the temporary link
                if (!linked)
                {
                    Connections.Children.Remove(_tempLink);
                    _tempLink = null;
                }

                //Stop handling linking
                _linkingStarted = false;
                e.Handled = true;
            }
        }


        private void ObjectSelector_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show(ObjectSelector.SelectedItem.ToString());
            //Don't do anything if no element clicked
            if (ObjectSelector.SelectedItem == null)
                return;

            //Copy the element to the drag & drop clipboard
            DragDrop.DoDragDrop(ObjectSelector, ObjectSelector.SelectedItem, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void CircuitCanvas_Drop_1(object sender, DragEventArgs e)
        {
            //Get the type of element that is dropped onto the canvas
            String[] allFormats = e.Data.GetFormats();
            //Make sure there is a format there
            
            if (allFormats.Length == 0)
                return;

            string ItemType = allFormats[0];
            
            //Create a new type of the format
            Object instance = (Object)Assembly.GetExecutingAssembly().CreateInstance(ItemType);

            //If the format doesn't exist do nothing
            
            if (instance == null)
                return;

            //Add the element to the canvas
            CircuitCanvas.Children.Add(instance);

            //Get the point of the mouse relative to the canvas
            Point p = e.GetPosition(CircuitCanvas);

            //Take 15 from the mouse position to center the element on the mouse
            Canvas.SetLeft(instance, p.X - 15);
            Canvas.SetTop(instance, p.Y - 15);
        }

        private void CircuitCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                DiagramScroll.ScrollToHorizontalOffset(DiagramScroll.HorizontalOffset - e.Delta);
                e.Handled = true;
            }
              else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                double old = this.zoom.Value;
                double zoom = (old + Math.Sign(e.Delta) * 0.1);
                if (0.0001 < Math.Abs(zoom - old))
                {
                    this.zoom.Value = zoom;
                }
                e.Handled = true;
            }
        }

        private void CircuitCanvas_MouseWheelCLick()
        {
            DiagramScroll.ScrollToHorizontalOffset(DiagramScroll.HorizontalOffset + (point.X - Mouse.GetPosition(CircuitCanvas).X));
            DiagramScroll.ScrollToVerticalOffset(DiagramScroll.VerticalOffset + (point.Y - Mouse.GetPosition(CircuitCanvas).Y));

        }

        private void CircuitCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                Point MousePosition = Mouse.GetPosition(CircuitCanvas);

                //Do a hit test under the mouse position
                HitTestResult result = VisualTreeHelper.HitTest(CircuitCanvas, MousePosition);
                var element = result.VisualHit;
                MessageBox.Show("Hi"+element.ToString());
            }
        }

        private void GroupBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                Point MousePosition = Mouse.GetPosition(CircuitCanvas);

                //Do a hit test under the mouse position
                HitTestResult result = VisualTreeHelper.HitTest(CircuitCanvas, MousePosition);
               
                var element = result.VisualHit;
                
                    CircuitCanvas.Children.Remove((UIElement)element);
                
                MessageBox.Show(element.ToString());
            }
        }
        public double Zoom()
        {
            return zoom.Value;
        }

        private void two_pole_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void CircuitCanvas_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
