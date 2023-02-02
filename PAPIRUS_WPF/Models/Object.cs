using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;
using System.Windows.Shapes;
using WPF_SHF_Element_lib;
using PAPIRUS_WPF.Models;
using Element = PAPIRUS_WPF.Models.Element;
using PAPIRUS_WPF.Dialog;
using System.Numerics;
using PAPIRUS_WPF.Elements;

namespace PAPIRUS_WPF
{
    public class Object : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty CanMoveProperty = DependencyProperty.Register("CanMove", typeof(bool), typeof(Object), new PropertyMetadata(true));
        /// <summary>
        /// Allows the circuit objects to be able to be frozen.
        /// </summary>
        public bool CanMove
        {   
            get { return (bool)GetValue(CanMoveProperty); }
            set { SetValue(CanMoveProperty, value); }
        }

        public virtual List<Output> listOfOutput { get; set; }

        public List<Output> GetOutputs()
        {
            return listOfOutput;
        }

        public virtual int group { get; set; }

        public Complex[,] matrix;
        public List<MatrixElement> matrixElements;

        public string name { get; set; }
        public bool isSelected = false;
        public Point anchorPoint;
        public Point startPoint = new Point(0,0);

        public List<Line> _attachedInputLines;
        public List<Line> _attachedOutputLines;

        //--------подключен ли к генератору----------//
        public bool generatorConnected = false;

        public event PropertyChangedEventHandler PropertyChanged;

        //---------сохраненный элемент-----------//
        public Element insideElement;

        //------сохраненные параметры-------------//
        public List<PoleInsideElementsAndParams> insideParams = new List<PoleInsideElementsAndParams>();

        /// <summary>
        /// Creates a new Circuit Object to be manipulated
        /// </summary>
        public Object()
        {
            //Initialize the lists
            _attachedInputLines = new List<Line>();
            _attachedOutputLines = new List<Line>();
            connectedElements = new List<Object>();
            matrixElements = new List<MatrixElement>();
        }

        public List<Object> connectedElements;

        public void FillME()
        {
            int k = 0;
            for (int i =0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrixElements.Add(new MatrixElement {rowIndex = i,columnIndex =j, value = matrix[i, j], unique =  k});
                    k++;
                }
            }
        }

        public virtual void AttachInputLine(Line line)
        {
            _attachedInputLines.Add(line);
        }

        public virtual void AttachOutputLine(Line line)
        {
            _attachedOutputLines.Add(line);
        }

        public List<Line> GetOutputLine()
        {
            return _attachedOutputLines;
        }
        public List<Line> GetInputLine()
        {
            return _attachedInputLines;
        }

        public UserControl GetChild(string name)
        {
             return (UserControl)this.GetTemplateChild(name);
        }
    }
}
