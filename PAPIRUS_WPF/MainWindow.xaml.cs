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
using System.Windows.Shapes;
using System.Xml.Linq;


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
        private Input _tempInput;
        bool MiddleClick = false;  //для перемещения по canvas
       
        private List<PowerObject> _powerList;   //хз
      
        //---------для подсчета нажатий--------//
        private Timer ClickTimer;
        private int ClickCounter;

        //The lines being connected to the input
        private List<LineGeometry> _attachedInputLines = new List<LineGeometry>();

        //The lines being connected to the output
        private List<LineGeometry> _attachedOutputLines = new List<LineGeometry>();

        //------для выделения-------//
        private Point point;
        private Point p2;
        private Point center;
        private FrameworkElement Source = null;
        private Point MousePosition;
        private Point startPoint;
        private ModifierKeys Keys;
        private Object Object;
        private bool inDrag = false;

        //------для общего выделения-------//
        private bool selectionIsStarted = false;
        private Point selectionStartPoint;
        private Rectangle markerGlyph;
        private TranslateTransform moveVector;
        private Canvas selectionLayer;
        public bool selectionMoving = false;
        private Point moveStart;
        private bool panning;
        private Object clipBoardRef;
        private Point maxMove;

        


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


        private void ClearSelection()
        {
            foreach (FrameworkElement object_ in CircuitCanvas.Children)
            {
                if (object_ is Object) 
                {
                    Object o = (Object)object_;
                    o.isSelected = false;
                    o.BorderBrush = Brushes.Transparent;
                }
                else if (object_ is Line)
                {
                    Console.WriteLine("yes");
                    Line line = (Line)object_;
                    line.Stroke = System.Windows.Media.Brushes.Black;
                }
            }
        Data.selection.Clear();
        }

        private void SingleElementSelect(FrameworkElement element)
        {
            if(element is Object)
            {
                Object = element as Object;
                Object.BorderBrush = Brushes.Magenta;
                p2 = CircuitCanvas.TranslatePoint(new Point(0, 0), Object);
                Object.anchorPoint.X = p2.X - Object.Width / 2;
                Object.anchorPoint.Y = p2.Y - Object.Height / 2;
                Data.selection.Add(Object);
                startPoint = MousePosition;
                Object.startPoint = p2;
                Object.isSelected = true;
            }
            else if (element is Line)
            {
                Console.WriteLine("yes");
                Line line = (Line)element;
                line.Stroke = System.Windows.Media.Brushes.Red;
            }
        }

        private void StartAreaSelection(Point point)
        {
            selectionStartPoint = point;
            markerGlyph = (Rectangle)Application.LoadComponent(new Uri(SymbolShape.SelectionMarker, UriKind.Relative));
            PositionGlyph(point);
            if (selectionLayer == null)
            {
                selectionLayer = new Canvas()
                {
                    RenderTransform = moveVector = new TranslateTransform()
                };
                Panel.SetZIndex(selectionLayer, int.MaxValue);
            }
            if (this.selectionLayer.Parent != CircuitCanvas)
            {
                CircuitCanvas.Children.Add(this.selectionLayer);
            }
            CircuitCanvas.Children.Add(markerGlyph);
            StartMove(point);
        }

        private void StartMove(Point startpoint)
        {
            selectionMoving = true;
            Mouse.Capture(CircuitCanvas, CaptureMode.Element);
            moveStart = startpoint;
            maxMove = new Point(0, 0);
            Rect bound = new Rect(Canvas.GetLeft(markerGlyph), Canvas.GetTop(markerGlyph), markerGlyph.Width, markerGlyph.Height);
        }

        private void PositionGlyph(Point point)
        {
            Rect rect = new Rect(selectionStartPoint, point);
            Canvas.SetLeft(markerGlyph, rect.X);
            Canvas.SetTop(markerGlyph, rect.Y);
            markerGlyph.Width = rect.Width;
            markerGlyph.Height = rect.Height;
            Console.WriteLine(markerGlyph.Width);
            Console.WriteLine(markerGlyph.Height);
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
                    Console.WriteLine(result.VisualHit);
                    MousePosition = e.GetPosition(CircuitCanvas);
                    Keys = Keyboard.Modifiers;
                    ClickTimer.Stop();
                    ClickCounter++;
                    Object = e.Source as Object;
                    if (e.Source as FrameworkElement is Canvas)
                    {
                        ClearSelection();
                        StartAreaSelection(MousePosition);
                    }

                    else if (Object != null && ClickCounter == 2)
                    {
                        if (Data.selection.Count > 1)
                        {
                            ClearSelection();
                            SingleElementSelect(Source);
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
            if(selectionMoving)
            {
                Point point = e.GetPosition(CircuitCanvas);
                if((point.X - selectionStartPoint.X) < 0 && (point.Y - selectionStartPoint.Y) < 0)
                {
                    Canvas.SetLeft(markerGlyph, point.X);
                    Canvas.SetTop(markerGlyph, point.Y);
                    markerGlyph.Width = Math.Abs(point.X - selectionStartPoint.X);
                    markerGlyph.Height = Math.Abs(point.Y - selectionStartPoint.Y);
                }
                else if ((point.X - selectionStartPoint.X) < 0 && (point.Y - selectionStartPoint.Y) > 0)
                {
                    Canvas.SetLeft(markerGlyph, point.X);
                    markerGlyph.Width = Math.Abs(point.X - selectionStartPoint.X);
                    markerGlyph.Height = point.Y - selectionStartPoint.Y;
                }
                else if ((point.X - selectionStartPoint.X) > 0 && (point.Y - selectionStartPoint.Y) < 0)
                {
                    Canvas.SetTop(markerGlyph, point.Y);
                    markerGlyph.Width = point.X - selectionStartPoint.X;
                    markerGlyph.Height = Math.Abs(point.Y - selectionStartPoint.Y);
                }
                else if ((point.X - selectionStartPoint.X) > 0 && (point.Y - selectionStartPoint.Y) > 0)
                {
                    markerGlyph.Width = point.X - selectionStartPoint.X;
                    markerGlyph.Height = point.Y - selectionStartPoint.Y;
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
            if (selectionMoving)
            {
                selectionMoving = false;
                Rect rect = new Rect();
                rect.X = Canvas.GetLeft(markerGlyph);
                rect.Y = Canvas.GetTop(markerGlyph);
                rect.Width = markerGlyph.Width;
                rect.Height = markerGlyph.Height;
                Mouse.Capture(null);
                foreach (FrameworkElement object_ in CircuitCanvas.Children)
                {
                    if (object_ is Object)
                    {
                        Object o = (Object)object_;
                        Rect rect1 = new Rect();
                        rect1.X = Canvas.GetLeft(o);
                        rect1.Y = Canvas.GetTop(o);
                        rect1.Width = o.Width;
                        rect1.Height = o.Height;
                        if (rect.Contains(rect1))
                        {
                            SingleElementSelect(o);
                        }
                    }
                }
                CircuitCanvas.Children.Remove(markerGlyph);
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
                clipBoardRef = Data.selection[0];
                Clipboard.SetData("myControl", "it doesn't matter");
                Clipboard.SetDataObject(Data.selection[0]);
                Console.WriteLine("оно будет робить");
            }
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.V)
            {
                var a = Clipboard.GetDataObject();
                Point position = Mouse.GetPosition(CircuitCanvas);
                Canvas.SetLeft(a, position.X);
                Canvas.SetTop(a, position.Y);
                CircuitCanvas.Children.Add(a);
                Console.WriteLine(a.ToString());
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
