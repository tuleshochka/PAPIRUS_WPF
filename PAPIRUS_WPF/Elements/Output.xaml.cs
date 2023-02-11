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
using static PAPIRUS_WPF.Elements.Output;

namespace PAPIRUS_WPF.Elements
{
    /// <summary>
    /// Логика взаимодействия для Output.xaml
    /// </summary>
    public partial class Output : UserControl, PowerObject
    {
        /// <summary>
        /// Called when the state of the output is changed
        /// </summary>
        public event StateChangeHandler StateChange;
        public event StateChangedHandler StateChanged;
        public delegate void StateChangeHandler();
        public delegate void StateChangedHandler();
        public Output _state_;
        public int index;
        public int outPos;  // 0 - Left, 1 - Right, 2 - Top, 3 - Bottom

      // The state of the output
        public bool _state;
        /// <summary>
        /// The delayed state of the input. Doesn't get updated internally until one tick has passed.
        /// </summary>
        public bool _delayedState;

        /// <summary>
        /// The current state of the Output
        /// </summary>
        public bool State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        /// <summary>
        /// Creates a new output class
        /// </summary>
        public Output() : this(false)
        {
        }
        //public Output(bool state)
        //    {
        //        _state = false;
        //        _state_ = null;
        //        _delayedState = state;
        //        InitializeComponent();
        //    }

        public Output(bool state)
        {
            _state = false;
            _state_ = null;
            InitializeComponent();
        }

        ///// <summary>
        ///// Updates any connected inputs. Called once per tick.
        ///// </summary>
        //public void CallChange()
        //{
        //    if (StateChange != null)
        //    {
        //        StateChange();
        //    }
        //}
        public void LinkInputs(Output output)
        {
            if (output == this) return;
            //Makes sure that it only listens to one output event at a time
            if (_state_ != null)
                throw new Exception("Нельзя подключить больше одного элемента");
            //Sets the state to the output
            _state_ = output;
        }

        public void DeleteLink()
        {
            _state_ = null;
        }

        public bool isLinked()
        {
            if (_state_ != null)
                return true;
            return false;
        }

        ///// <summary>
        ///// Called when the state of the input is changed
        ///// </summary>
        //public void _state_StateChange()
        //{
        //    //Set the delayed state AFTER a tick
        //    _delayedState = _state_.State;

        //    //Make sure there is a subscriber to the event
        //    if (StateChanged != null)
        //    {
        //        //Passes the value of the output to the circuit
        //        StateChanged();
        //    }
        //}
    }
}
