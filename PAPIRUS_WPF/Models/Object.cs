using PAPIRUS_WPF.Dialog;
using PAPIRUS_WPF.Elements;
using PAPIRUS_WPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Element = PAPIRUS_WPF.Models.Element;
using Matrix = PAPIRUS_WPF.Models.Matrix;

namespace PAPIRUS_WPF
{
    public class Object : UserControl, INotifyPropertyChanged
    {

        public static readonly DependencyProperty CanMoveProperty = DependencyProperty.Register("CanMove", typeof(bool), typeof(Object), new PropertyMetadata(true));
        //public static readonly DependencyProperty DefaultNumberVisibleProperty = DependencyProperty.Register("DefaultNumberVisible", typeof(Visibility), typeof(TextBlock), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnVisibilityChanged)));
        /// <summary>
        /// Allows the circuit objects to be able to be frozen.
        /// </summary>

        private Visibility visibility = Visibility.Visible;

        public bool CanMove
        {   
            get { return (bool)GetValue(CanMoveProperty); }
            set { SetValue(CanMoveProperty, value); }
        }
        //public Visibility DefaultNumberVisible
        //{
        //    get { return (Visibility)GetValue(DefaultNumberVisibleProperty); }
        //    set { SetValue(DefaultNumberVisibleProperty, value); }
        //}

        

        public Visibility DefaultNumberVisible
        {
            get { return visibility; }
            set { visibility = value;
                OnPropertyChanged("DefaultNumberVisible");

            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        public virtual List<Output> listOfOutput { get; set; }

        public List<Output> GetOutputs()
        {
            return listOfOutput;
        }

        public Point coordinates { get; set; }

        public virtual int group { get; set; }

        public Matrix matrix;
        public List<MatrixElement> matrixElements;

        public string name { get; set; }
        public bool isSelected = false;
        public Point anchorPoint;
        public Point startPoint = new Point(0,0);

        public Dictionary<Output,Line> _attachedInputLines;
        public Dictionary<Output, Line> _attachedOutputLines;

        //--------подключен ли к генератору----------//
        public bool generatorConnected = false;


        //---------сохраненный элемент-----------//
        public Element insideElement { get; set; }

        //------сохраненные параметры-------------//
        public List<PoleInsideElementsAndParams> insideParams = new List<PoleInsideElementsAndParams>();

        /// <summary>
        /// Creates a new Circuit Object to be manipulated
        /// </summary>
        public Object()
        {
            //Initialize the lists
            _attachedInputLines = new Dictionary<Output, Line>();
            _attachedOutputLines = new Dictionary<Output, Line>();
            connectedElements = new List<Object>();
            matrixElements = new List<MatrixElement>();
        }

        public List<Object> connectedElements;

        public void FillME()
        {
            int k = 0;
            if (matrix is null)
            {
                throw new Exception("Введите параметры для элемента");
            }
            else
            {
                matrixElements.Clear();
                for (int i = 0; i < matrix.M; i++)
                {
                    for (int j = 0; j < matrix.N; j++)
                    {
                        if (matrix[i,j].EvaluableNumerical)
                        {
                            matrixElements.Add(new MatrixElement { rowIndex = i, columnIndex = j, value = (Complex)matrix[i, j].EvalNumerical(), unique = k });
                            k++;
                        }
                        else
                        {
                            matrixElements.Add(new MatrixElement { rowIndex = i, columnIndex = j, value = matrix[i, j].Evaled, unique = k });
                            k++;
                        }
                    }
                }
            }
        }

        public virtual void AttachInputLine(Output output,Line line)
        {
            if (!_attachedInputLines.ContainsKey(output))
            {
                _attachedInputLines.Add(output, line);
            }
        }

        public virtual void AttachOutputLine(Output output, Line line)
        {
            if (!_attachedOutputLines.ContainsKey(output))
            {
                _attachedOutputLines.Add(output, line);
            }
            
        }

        public Dictionary<Output, Line> GetOutputLine()
        {
            return _attachedOutputLines;
        }
        public Dictionary<Output, Line> GetInputLine()
        {
            return _attachedInputLines;
        }

        public UserControl GetChild(string name)
        {
             return (UserControl)this.GetTemplateChild(name);
        }
    }
}
