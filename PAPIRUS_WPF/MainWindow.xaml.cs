using PAPIRUS_WPF.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Timers;
using WPF_SHF_Element_lib;


namespace PAPIRUS_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //--------данные для dialog--------------//
        public string elementName;
        public string fileName;

        public int num;  //для счета количества элементов на canvas

        //The boolean that signifys when an output is being linked
        private bool _linkingStarted = false;
        //The temporary line that shows when linking an output
        private LineGeometry _tempLink;
        //The output that is being linked to
        private Output _tempOutput;
        bool MiddleClick = false;  //для перемещения по canvas
        public Point point;
        private Point MoveSelStartPoint;
        private Point panStart;
        public bool panning;
        private TranslateTransform moveVector;
        private Point maxMove;
        
        private List<PowerObject> _powerList;
       
        public FrameworkElement singleElement = null;
        
        private Timer ClickTimer;
        private int ClickCounter;

        public bool MovingSelectionIsStarted = false;
        FrameworkElement Source = null;
        Point MousePosition;
        Point startPoint;
        ModifierKeys Keys;
        Object Object;
        public bool inDrag = false;
        //The anchor point of the object when being moved
        //private Point _anchorPoint;
        Object MovingElement;

        //The lines being connected to the input
        private List<LineGeometry> _attachedInputLines = new List<LineGeometry>();

        //The lines being connected to the output
        private List<LineGeometry> _attachedOutputLines = new List<LineGeometry>();


        Point p2;
        Point targetPoints;
        Point center;


        public MainWindow()
        {
            InitializeComponent();
            _powerList = new List<PowerObject>();
            ClickTimer = new Timer(300);
            ClickTimer.Elapsed += new ElapsedEventHandler(EvaluateClicks);
            

            CircuitCanvas.MouseDown += CircuitCanvas_MouseDown;
            CircuitCanvas.MouseMove += CircuitCanvas_MouseMove;
            CircuitCanvas.MouseUp += CircuitCanvas_MouseUp;
            

        }
        public delegate System.Windows.Media.HitTestResultBehavior HitTestResultCallbak(HitTestResult result);


        //--------Selection-------//

        public void ClearSelection()
        {
            foreach (FrameworkElement object_ in CircuitCanvas.Children)
            {
                if (object_ is Object) 
                {
                    Object o = (Object)object_;
                    o.isSelected = false;
                    o.BorderBrush = Brushes.Transparent;
                }
            }
        Data.selection.Clear();
        }

        public void SingleElementSelect(FrameworkElement element)
        {

            {
                Object = element as Object;
                MovingElement = Object;
                Object.BorderBrush = Brushes.Magenta;
                p2 = CircuitCanvas.TranslatePoint(new Point(0, 0), Object);
                Object.anchorPoint.X = p2.X - Object.Width / 2;
                Object.anchorPoint.Y = p2.Y - Object.Height / 2;
                Data.selection.Add(Object);
                startPoint = MousePosition;
                Object.startPoint = p2;
                Object.isSelected = true;
            }

        }

        private HitTestResultBehavior MyHitFilter (HitTestResult result)
        {
            if(result.VisualHit.GetType() == typeof(UserControl))
            {
                return HitTestResultBehavior.Stop;
            }
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
                //Do a hit test under the mouse position
                HitTestResult result = VisualTreeHelper.HitTest(CircuitCanvas, e.GetPosition(CircuitCanvas));
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
                else if (!(result.VisualHit is Border))
                {
                    Source = e.Source as FrameworkElement;
                    MousePosition = e.GetPosition(CircuitCanvas);
                    Keys = Keyboard.Modifiers;
                    ClickTimer.Stop();
                    ClickCounter++;
                    Object = e.Source as Object;
                    if (e.Source as FrameworkElement is Canvas)
                    {
                        ClearSelection();
                    }

                    else if (Object != null && ClickCounter == 2)
                    {
                        if (Data.selection.Count > 1)
                        {
                            ClearSelection();
                            SingleElementSelect(Object);
                        }
                        elementName = Object.name;
                        switch (Object)
                            {
                                case two_pole _:
                                    fileName = "2pole.json";
                                    break;
                                case four_pole _:
                                    fileName = "4pole.json";
                                    break;
                                case six_pole _:
                                    fileName = "6pole.json";
                                    break;
                                case generator _:
                                    ;
                                    break;
                            }
                            GeneratorDialog gd = new GeneratorDialog(elementName, fileName);
                            gd.ShowDialog();
                    }
                    //в другом случае (один тык)
                    else if (ClickCounter == 1)
                    {
                        if (!(Source is Canvas))
                        {
                            if (Keys == ModifierKeys.Control)
                            {
                                if (Data.selection.Contains(Source) == false)
                                {
                                    SingleElementSelect(Source);
                                }
                            }
                            else
                            {
                                if (Data.selection.Contains(Source) == false)
                                {
                                    ClearSelection();
                                    SingleElementSelect(Source);
                                    inDrag = true;
                                }
                                else
                                {
                                    inDrag= true;
                                    startPoint = MousePosition;

                                }
                            }
                        }

                    }
                    ClickTimer.Start();
                }
            }  
        }

        private void EvaluateClicks(object source, ElapsedEventArgs e)
        {
            ClickTimer.Stop();
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
            //if (MovingSelectionIsStarted)
            //{
              //  Point point = e.GetPosition(this.CircuitCanvas);
               // this.movingMarker.Move(this, new Point(Math.Max(this.maxMove.X, point.X), Math.Max(this.maxMove.Y, point.Y)));
            //} else if (this.panning)
            else
            if (inDrag)
            {
                
                var mousePos = e.GetPosition(CircuitCanvas);
                foreach (Object element in Data.selection)
                {
                    p2 = CircuitCanvas.TranslatePoint(new Point(0, 0), element);
                    double left = Math.Abs(element.startPoint.X) + (mousePos.X-startPoint.X);
                    double top = Math.Abs(element.startPoint.Y) + (mousePos.Y - startPoint.Y);
                    Canvas.SetLeft(element, left);
                    Canvas.SetTop(element, top);
                    _attachedInputLines = element.GetInputLine();
                    var mouse = e.GetPosition(element);
                    center.X = mousePos.X - mouse.X;
                    center.Y = mousePos.Y - mouse.Y;
                    foreach (LineGeometry attachedLine in _attachedInputLines)
                    {
                        attachedLine.EndPoint = MoveLine(attachedLine.EndPoint,
                                                        (center.X + element.Width / 2 + element.anchorPoint.X),
                                                      (center.Y + element.Height / 2 + element.anchorPoint.Y));

                    }

                    _attachedOutputLines = element.GetOutputLine();
                    //Transform the attached line if its an output (uses StartPoint)
                    foreach (LineGeometry attachedLine in _attachedOutputLines)
                    {
                        attachedLine.StartPoint = MoveLine(attachedLine.StartPoint,
                                                         (Math.Abs(center.X + element.Width / 2) + element.anchorPoint.X),
                                                        (Math.Abs(center.Y + element.Height / 2) + element.anchorPoint.Y));
                    }
                    element.anchorPoint.X = p2.X - element.Width / 2;
                    element.anchorPoint.Y = p2.Y - element.Height / 2;
                }
                
            }
        }

        private Point MoveLine(Point PointToMove, double AmountToMoveX, double AmountToMoveY)
        {
            Point transformedPoint = new Point();
            transformedPoint.X = PointToMove.X + AmountToMoveX;
            transformedPoint.Y = PointToMove.Y + AmountToMoveY;
            return transformedPoint;
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
                        if (IO is Output)
                        {
                            //Convert to a input element
                            Output IOInput = (Output)IO;

                            //Get the center of the input relative to the canvas
                            Point inputPoint = IOInput.TransformToAncestor(CircuitCanvas).Transform(new Point(IOInput.ActualWidth / 2, IOInput.ActualHeight / 2));

                            //Ends the line in the centre of the input
                            _tempLink.EndPoint = inputPoint;

                            //Links the output to the input
                            IOInput.LinkInputs(_tempOutput);

                            //Adds to the global list
                            _powerList.Add((PowerObject)_tempOutput);
                            _powerList.Add((PowerObject)IOInput);

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
            else if (inDrag)
            {
                inDrag = false;
                
                foreach (Object element in Data.selection)
                {
                    p2 = CircuitCanvas.TranslatePoint(new Point(0, 0), element);
                    element.startPoint = p2;
                }
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
                if (instance is two_pole || instance is four_pole || instance is six_pole)
                {
                    instance.name = "Эл-" + num;
                }
                else if(instance is generator)
                {
                    instance.name = "Г-"+num;
                }
                //instances.Add(instance);
                //If the format doesn't exist do nothing

                if (instance == null)
                    return;

                //Add the element to the canvas
                CircuitCanvas.Children.Add(instance);

                //Get the point of the mouse relative to the canvas
                Point p = e.GetPosition(CircuitCanvas);

                //Take 15 from the mouse position to center the element on the mouse
                Canvas.SetLeft(instance, p.X - instance.Width / 2);
                Canvas.SetTop(instance, p.Y - instance.Height / 2);
                num++;
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

        public double Zoom()
        {
            return zoom.Value;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WPF_SHF_Element_lib.Window1 window1 = new WPF_SHF_Element_lib.Window1();
            window1.ShowDialog();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                foreach (var select in Data.selection)
                {
                    CircuitCanvas.Children.Remove(select);
                }
            }
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                Console.WriteLine("оно будет робить");
            }
        }

        private void Form_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            WPF_SHF_Element_lib.Window4 window4 = new WPF_SHF_Element_lib.Window4();
            window4.ShowDialog();
        }

       
    }
}
