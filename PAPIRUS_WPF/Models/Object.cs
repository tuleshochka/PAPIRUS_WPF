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
        public List<DataGridElements> insideParams;

        /// <summary>
        /// Creates a new Circuit Object to be manipulated
        /// </summary>
        public Object()
        {
            //Initialize the lists
            _attachedInputLines = new List<Line>();
            _attachedOutputLines = new List<Line>();
            connectedElements = new List<Object>();
            insideParams = new List<DataGridElements>();
        }

        public List<Object> connectedElements;

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
