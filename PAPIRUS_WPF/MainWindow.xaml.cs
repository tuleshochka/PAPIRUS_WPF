using PAPIRUS_WPF.Dialog;
using PAPIRUS_WPF.Elements;
using PAPIRUS_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Controls.TextBox;
using Window = System.Windows.Window;

namespace PAPIRUS_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //--------данные для dialog--------------//
        public string elementName;
        private string fileName;

        private int num = 1;  //для счета количества элементов на canvas

        //The boolean that signifys when an output is being linked
        private bool _linkingStarted = false;
        //The temporary line that shows when linking an output
        private Line _tempLink;
        //The output that is being linked to
        private Output _tempOutput;
        private Input _tempInput;
        private bool MiddleClick = false;  //для перемещения по canvas

        private List<PowerObject> _powerList;   //хз

        //---------для подсчета нажатий--------//
        private Timer ClickTimer;
        private int ClickCounter;

        //The lines being connected to the input
        private List<Line> _attachedInputLines = new List<Line>();

        //The lines being connected to the output
        private List<Line> _attachedOutputLines = new List<Line>();

        //------для выделения-------//
        private Point point;
        private Point p2;
        private Point center;
        private FrameworkElement Source = null;
        private Point MousePosition;
        private Point startPoint;
        private ModifierKeys Keys;
        private Object startObject;
        private bool inDrag = false;

        //------для общего выделения-------//
        private bool selectionIsStarted = false;
        private Point selectionStartPoint;
        private Rectangle markerGlyph;
        private TranslateTransform moveVector;
        private Canvas selectionLayer;
        private bool selectionMoving = false;
        private Point moveStart;
        private bool panning;
        private Point maxMove;

        //------Для копирования и вставки-------//
        private List<Object> copyData = new List<Object>();
        private List<Point> copyDataPos = new List<Point>();

        //-------подключение к генератору----------//
        private bool generatorConnect = false;

        public MainWindow()
        {

            InitializeComponent();
            _powerList = new List<PowerObject>();
            Data.elements = new List<Object>();
            ClickTimer = new Timer(300);
            ClickTimer.Elapsed += new ElapsedEventHandler(EvaluateClicks);
            CircuitCanvas.MouseDown += CircuitCanvas_MouseDown;
            CircuitCanvas.MouseMove += CircuitCanvas_MouseMove;
            CircuitCanvas.MouseUp += CircuitCanvas_MouseUp;

        }
        public delegate System.Windows.Media.HitTestResultBehavior HitTestResultCallbak(HitTestResult result);


        private void SetElementName(Object instance)
        {
            if (instance is generator)
            {
                instance.name = "Г-" + num;
            }
            else
            {
                instance.name = "Эл-" + num;
            }
            num++;
        }

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
                    Line line = (Line)object_;
                    line.Stroke = System.Windows.Media.Brushes.Black;
                }
            }
            Data.selection.Clear();
            Data.selectedWires.Clear();
        }

        private void SingleElementSelect(FrameworkElement element)
        {
            if (element is Object)
            {
                Object obj = element as Object;
                obj.BorderBrush = Brushes.Magenta;
                p2 = CircuitCanvas.TranslatePoint(new Point(0, 0), obj);
                obj.anchorPoint.X = p2.X - obj.Width / 2;
                obj.anchorPoint.Y = p2.Y - obj.Height / 2;
                Data.selection.Add(obj);
                startPoint = MousePosition;
                obj.startPoint = p2;
                obj.isSelected = true;
            }
            else if (element is Line)
            {
                Line line = (Line)element;
                line.Stroke = Brushes.Magenta;
                Data.selectedWires.Add(line);
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
        }

        private Point MoveLine(Point PointToMove, double AmountToMoveX, double AmountToMoveY)
        {
            Point transformedPoint = new Point();
            transformedPoint.X = PointToMove.X + AmountToMoveX;
            transformedPoint.Y = PointToMove.Y + AmountToMoveY;
            return transformedPoint;
        }

        private void SetGeneratorConnection(Object startObject)
        {
            foreach (Object obj in startObject.connectedElements)
            {
                if (obj.generatorConnected == false)
                {
                    obj.generatorConnected = true;
                    if (obj.connectedElements.Count() > 1)
                    {
                        SetGeneratorConnection(obj);
                    }
                }
            }
        }

        private void RemoveGeneratorConnection(Object startObject)
        {
            foreach (Object obj in startObject.connectedElements)
            {
                if (obj.generatorConnected == true)
                {
                    if (!(IsGeneratorConnected(obj, startObject)) || startObject is generator)
                    {
                        obj.generatorConnected = false;
                        if (obj.connectedElements.Count() > 1)
                        {
                            RemoveGeneratorConnection(obj);
                        }
                    }
                }
            }
        }

        private bool IsGeneratorConnected(Object _object, Object startObject)
        {
            var generator = _object.connectedElements.Where(o => o is generator);
            if (generator.Count() > 0)
            {
                return true;
            }
            else
            {
                var connected = _object.connectedElements.Where(o => o != startObject);
                foreach (Object obj in connected)
                {
                    if (IsGeneratorConnected(obj, _object))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private void RemoveElement(Object element)
        {
            if (element.generatorConnected == true)
            {
                RemoveGeneratorConnection(element);
            }
            _attachedInputLines = element.GetInputLine();
            foreach (var wire in _attachedInputLines)
            {
                RemoveWire(wire);
            }
            _attachedOutputLines = element.GetOutputLine();
            foreach (var wire in _attachedOutputLines)
            {
                RemoveWire(wire);
            }
            for (int i = 0; i < element.connectedElements.Count(); i++)
            {
                Object obj = element.connectedElements[i];
                obj.connectedElements.Remove(element);
            }
            CircuitCanvas.Children.Remove(element);
            Data.elements.Remove(element);
        }

        private void RemoveWire(Line wire)
        {
            Point startpoint = new Point(wire.X1, wire.Y1);
            Point endpoint = new Point(wire.X2, wire.Y2);

            //Move the link endpoint to the current location of the mouse
            if ((wire.X2 - wire.X1) > 0 && (wire.Y2 - wire.Y1) > 0)
            {
                startpoint = new Point(wire.X1 - 1, wire.Y1 - 1);
                endpoint = new Point(wire.X2 + 1, wire.Y2 + 1);
            }
            if ((wire.X2 - wire.X1) < 0 && (wire.Y2 - wire.Y1) < 0)
            {
                startpoint = new Point(wire.X1 + 1, wire.Y1 + 1);
                endpoint = new Point(wire.X2 - 1, wire.Y2 - 1);
            }
            if ((wire.X2 - wire.X1) < 0 && (wire.Y2 - wire.Y1) > 0)
            {
                startpoint = new Point(wire.X1 + 1, wire.Y1 - 1);
                endpoint = new Point(wire.X2 - 1, wire.Y2 + 1);
            }
            if ((wire.X2 - wire.X1) > 0 && (wire.Y2 - wire.Y1) < 0)
            {
                startpoint = new Point(wire.X1 - 1, wire.Y1 + 1);
                endpoint = new Point(wire.X2 + 1, wire.Y2 - 1);
            }

            HitTestResult startResult = VisualTreeHelper.HitTest(CircuitCanvas, startpoint);
            HitTestResult endResult = VisualTreeHelper.HitTest(CircuitCanvas, endpoint);
            //If the underlying element is a border element
            if (startResult.VisualHit is Border)
            {
                Border border = (Border)startResult.VisualHit;
                var IO = border.Parent;
                if (IO is Output)
                {
                    Output IOOutput = IO as Output;
                    IOOutput.DeleteLink();
                }
            }
            if (endResult.VisualHit is Border)
            {
                Border border = (Border)endResult.VisualHit;
                var IO = border.Parent;
                if (IO is Output)
                {
                    Output IOOutput = IO as Output;
                    IOOutput.DeleteLink();
                }
            }
            CircuitCanvas.Children.Remove(wire);
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


                if (Data.visibleBool == true)
                {
                    foreach (var box in Data.outputNumber)
                    {
                        if (box is TextBox)
                        {
                            CircuitCanvas.Children.Remove((TextBox)box);
                        }

                    }
                    foreach (Object el in CircuitCanvas.Children.OfType<Object>())
                    {
                        if (!(el is generator))
                        {
                            Object _ = (Object)el.Resources["DataSource"];
                            _.DefaultNumberVisible = Visibility.Visible;
                        }
                    } 
                    Data.visibleBool = false;
                }
               

                Source = e.Source as FrameworkElement;
                startObject = e.Source as Object;
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
                        if (startObject is generator || startObject.generatorConnected == true)
                        {
                            generatorConnect = true;
                        }
                        //Cast to output
                        Output IOOutput = (Output)IO;
                        startPoint = e.GetPosition(CircuitCanvas);
                        //Get the center of the output relative to the canvas
                        Point position = IOOutput.TransformToAncestor(CircuitCanvas).Transform(new Point(IOOutput.ActualWidth / 2, IOOutput.ActualHeight / 2));

                        //Creates a new line
                        _linkingStarted = true;
                        _tempLink = new Line();
                        _tempLink.X1 = position.X;
                        _tempLink.Y1 = position.Y;
                        _tempLink.X2 = position.X;
                        _tempLink.Y2 = position.Y;
                        _tempLink.Stroke = Brushes.Black;
                        _tempLink.StrokeThickness = 1;

                        CircuitCanvas.Children.Add(_tempLink);

                        //Assign the temporary output to the current output
                        _tempOutput = (Output)IO;
                        if (_tempOutput.isLinked())
                        {
                            CircuitCanvas.Children.Remove(_tempLink);
                            return;
                        }

                        e.Handled = true;
                    }
                }
                else if (!(result.VisualHit is Border))
                {
                    MousePosition = e.GetPosition(CircuitCanvas);
                    Keys = Keyboard.Modifiers;
                    ClickTimer.Stop();
                    ClickCounter++;
                    if (e.Source as FrameworkElement is Canvas)
                    {
                        ClearSelection();
                        StartAreaSelection(MousePosition);
                    }

                    else if (startObject != null && ClickCounter == 2)
                    {
                        if (Data.selection.Count > 1)
                        {
                            ClearSelection();
                            SingleElementSelect(Source);
                        }
                        elementName = startObject.name;
                        if (startObject is generator)
                        {
                            GeneratorDialog gd = new GeneratorDialog(elementName);
                            gd.ShowDialog();
                        }
                        else
                        {
                            fileName = startObject.GetType().Name + ".json";
                            PoleDialog gd = new PoleDialog(elementName, fileName, startObject);
                            gd.ShowDialog();
                        }
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
                                if (Data.selection.Contains(Source) == false && Data.selectedWires.Contains(Source) == false)
                                {
                                    ClearSelection();
                                    SingleElementSelect(Source);
                                    inDrag = true;
                                }
                                else
                                {
                                    inDrag = true;
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
            if (MiddleClick) CircuitCanvas_MouseWheelCLick();
            //If there is a linking in progress
            if (_linkingStarted)
            {
                ClearSelection();
                Point currentPoint = e.GetPosition(CircuitCanvas);
                //Move the link endpoint to the current location of the mouse
                if ((currentPoint.X - startPoint.X) > 0 && (currentPoint.Y - startPoint.Y) > 0)
                {
                    _tempLink.X2 = (e.GetPosition(CircuitCanvas).X) - 1;
                    _tempLink.Y2 = (e.GetPosition(CircuitCanvas).Y) - 1;
                }
                if ((currentPoint.X - startPoint.X) < 0 && (currentPoint.Y - startPoint.Y) < 0)
                {
                    _tempLink.X2 = (e.GetPosition(CircuitCanvas).X) + 1;
                    _tempLink.Y2 = (e.GetPosition(CircuitCanvas).Y) + 1;
                }
                if ((currentPoint.X - startPoint.X) < 0 && (currentPoint.Y - startPoint.Y) > 0)
                {
                    _tempLink.X2 = (e.GetPosition(CircuitCanvas).X) + 1;
                    _tempLink.Y2 = (e.GetPosition(CircuitCanvas).Y) - 1;
                }
                if ((currentPoint.X - startPoint.X) > 0 && (currentPoint.Y - startPoint.Y) < 0)
                {
                    _tempLink.X2 = (e.GetPosition(CircuitCanvas).X) - 1;
                    _tempLink.Y2 = (e.GetPosition(CircuitCanvas).Y) + 1;
                }
                e.Handled = true;
            }
            else
            if (inDrag)
            {

                var mousePos = e.GetPosition(CircuitCanvas);
                foreach (Object element in Data.selection)
                {
                    p2 = CircuitCanvas.TranslatePoint(new Point(0, 0), element);
                    double left = Math.Abs(element.startPoint.X) + (mousePos.X - startPoint.X);
                    double top = Math.Abs(element.startPoint.Y) + (mousePos.Y - startPoint.Y);
                    Canvas.SetLeft(element, left);
                    Canvas.SetTop(element, top);
                    _attachedInputLines = element.GetInputLine();
                    var mouse = e.GetPosition(element);
                    center.X = mousePos.X - mouse.X;
                    center.Y = mousePos.Y - mouse.Y;
                    foreach (Line attachedLine in _attachedInputLines)
                    {
                        Point endPoint = new Point(attachedLine.X2, attachedLine.Y2);
                        attachedLine.X2 = (MoveLine(endPoint,
                                                        (center.X + element.Width / 2 + element.anchorPoint.X),
                                                      (center.Y + element.Height / 2 + element.anchorPoint.Y))).X;
                        attachedLine.Y2 = (MoveLine(endPoint,
                                                        (center.X + element.Width / 2 + element.anchorPoint.X),
                                                      (center.Y + element.Height / 2 + element.anchorPoint.Y))).Y;

                    }

                    _attachedOutputLines = element.GetOutputLine();
                    //Transform the attached line if its an output (uses StartPoint)
                    foreach (Line attachedLine in _attachedOutputLines)
                    {
                        Point startPoint = new Point(attachedLine.X1, attachedLine.Y1);
                        attachedLine.X1 = (MoveLine(startPoint,
                                                         (Math.Abs(center.X + element.Width / 2) + element.anchorPoint.X),
                                                        (Math.Abs(center.Y + element.Height / 2) + element.anchorPoint.Y))).X;
                        attachedLine.Y1 = (MoveLine(startPoint,
                                                         (Math.Abs(center.X + element.Width / 2) + element.anchorPoint.X),
                                                        (Math.Abs(center.Y + element.Height / 2) + element.anchorPoint.Y))).Y;
                    }
                    element.anchorPoint.X = p2.X - element.Width / 2;
                    element.anchorPoint.Y = p2.Y - element.Height / 2;
                }
            }
            if (selectionMoving)
            {
                Point point = e.GetPosition(CircuitCanvas);
                if ((point.X - selectionStartPoint.X) < 0 && (point.Y - selectionStartPoint.Y) < 0)
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

        private void CircuitCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {

            MiddleClick = false;
            Cursor = Cursors.Arrow;

            if (_linkingStarted)
            {
                //Temporary value to show 
                bool linked = false;

                //Get the type of the element that the mouse went up on
                var BaseType = e.Source.GetType().BaseType;

                if (BaseType == typeof(Object))
                {
                    Object obj = (Object)e.Source;
                    Point MousePosition = e.GetPosition(obj);
                    HitTestResult result = VisualTreeHelper.HitTest(obj, MousePosition);

                    if (result == null || result.VisualHit == null)
                    {
                        //Remove the temporary line
                        CircuitCanvas.Children.Remove(_tempLink);
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
                            obj.connectedElements.Add(startObject);
                            startObject.connectedElements.Add(obj);
                            if (generatorConnect == true || obj is generator || obj.generatorConnected == true)
                            {
                                obj.generatorConnected = true;
                                startObject.generatorConnected = true;
                                SetGeneratorConnection(startObject);
                                SetGeneratorConnection(obj);
                            }

                            Output IOInput = (Output)IO;

                            //Get the center of the input relative to the canvas
                            Point inputPoint = IOInput.TransformToAncestor(CircuitCanvas).Transform(new Point(IOInput.ActualWidth / 2, IOInput.ActualHeight / 2));

                            //Ends the line in the centre of the input
                            _tempLink.X2 = inputPoint.X;
                            _tempLink.Y2 = inputPoint.Y;

                            //Links the output to the input
                            if (!(startObject is generator) && !(obj is generator))
                            {
                                try
                                {
                                    IOInput.LinkInputs(_tempOutput);
                                }
                                catch (Exception)
                                {
                                    CircuitCanvas.Children.Remove(_tempLink);
                                    _tempLink = null;
                                    _linkingStarted = false;
                                    return;
                                }
                                _tempOutput.LinkInputs(IOInput);
                            }

                            //Adds to the global list
                            _powerList.Add((PowerObject)_tempOutput);
                            _powerList.Add((PowerObject)IOInput);

                            //Attaches the line to the object
                            obj.AttachInputLine(_tempLink);

                            //Some evil casting (the outputs' parent of the parent is the circuit object that contains the output). Attaches the output side to the object
                            ((Object)((Grid)_tempOutput.Parent).Parent).AttachOutputLine(_tempLink);

                            //Set linked to true
                            linked = true;
                            generatorConnect = false;
                        }

                    }
                }

                //If it isn't linked remove the temporary link
                if (!linked)
                {
                    CircuitCanvas.Children.Remove(_tempLink);
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
                    else if (object_ is Line)
                    {
                        Line line = (Line)object_;
                        Point point1 = new Point(line.X1, line.Y1);
                        Point point2 = new Point(line.X2, line.Y2);
                        if (rect.Contains(point1) && (rect.Contains(point2)))
                        {
                            SingleElementSelect(line);
                        }
                    }
                }

                CircuitCanvas.Children.Remove(markerGlyph);
                HitTestResult result = VisualTreeHelper.HitTest(CircuitCanvas, Mouse.GetPosition(CircuitCanvas));
                if (result != null && Data.FindParent<Object>(result.VisualHit) != null)
                {
                    SingleElementSelect(Data.FindParent<Object>(result.VisualHit));
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

            if (instance == null)
                return;

            if (instance is twentyfour_pole)
            {
                multi_pole_select multi_Pole_Select = new multi_pole_select();
                multi_Pole_Select.ShowDialog();
                instance = Data.multiPole;
                if (instance != null)
                {
                    SetElementName(instance);
                    CircuitCanvas.Children.Add(instance);
                    Point p = e.GetPosition(CircuitCanvas);
                    Canvas.SetLeft(instance, p.X - instance.Width / 2);
                    Canvas.SetTop(instance, p.Y - instance.Height / 2);
                }

            }
            else
            {
                SetElementName(instance);
                CircuitCanvas.Children.Add(instance);
                Point p = e.GetPosition(CircuitCanvas);
                Canvas.SetLeft(instance, p.X - instance.Width / 2);
                Canvas.SetTop(instance, p.Y - instance.Height / 2);
            }
            if (!(instance is generator))
            {

                Data.elements.Add(instance);

            }
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
                foreach (var element in Data.selection)
                {
                    RemoveElement(element);
                }
                foreach (var wire in Data.selectedWires)
                {
                    RemoveWire(wire);
                }
            }
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                copyData.Clear();
                foreach (Object data in Data.selection)
                {
                    data.startPoint = CircuitCanvas.TranslatePoint(new Point(0, 0), data);
                    copyData.Add(data);

                }
            }

            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.V)
            {
                var mousePos = Mouse.GetPosition(CircuitCanvas);
                Point startPoint = copyData[0].startPoint;

                foreach (Object data in copyData)
                {
                    Object instance = (Object)Assembly.GetExecutingAssembly().CreateInstance(data.GetType().ToString());
                    if (instance == null)
                        return;
                    SetElementName(instance);
                    //instances.Add(instance);
                    //If the format doesn't exist do nothing

                    //Add the element to the canvas
                    CircuitCanvas.Children.Add(instance);

                    double left = mousePos.X + (startPoint.X - data.startPoint.X);
                    double top = mousePos.Y + (startPoint.Y - data.startPoint.Y);
                    Canvas.SetLeft(instance, left);
                    Canvas.SetTop(instance, top);
                }
            }

            if (e.Key == Key.Escape)
            {
                if (_linkingStarted)
                {
                    CircuitCanvas.Children.Remove(_tempLink);
                    _tempLink = null;
                    _linkingStarted = false;
                    e.Handled = true;
                }
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Calculations calc = new Calculations();
            calc.ShowDialog();

        }
    }
}

