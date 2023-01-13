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

namespace PAPIRUS_WPF.Elements
{
    /// <summary>
    /// Логика взаимодействия для Input.xaml
    /// </summary>
    public partial class Input : UserControl
    {

        /// <summary>
        /// Called when the state of the output is changed
        /// </summary>
        public event StateChangeHandler StateChange;
        public event StateChangedHandler StateChanged;
        public delegate void StateChangeHandler();
        public delegate void StateChangedHandler();
        public Output _state_;

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
        public Input() : this(false)
        {
        }
        public Input(bool state)
        {
            _state = false;
            _state_ = null;
            _delayedState = state;
            InitializeComponent();
        }

        /// <summary>
        /// Updates any connected inputs. Called once per tick.
        /// </summary>
        public void CallChange()
        {
            if (StateChange != null)
            {
                StateChange();
            }
        }
        public void LinkInputs(Output output)
        {
            //Makes sure that it only listens to one output event at a time
            if (_state_ != null)
                _state_.StateChange -= _state_StateChange;

            //Sets the state to the output
            _state_ = output;
        }

        /// <summary>
        /// Called when the state of the input is changed
        /// </summary>
        public void _state_StateChange()
        {
            //Set the delayed state AFTER a tick
            _delayedState = _state_.State;

            //Make sure there is a subscriber to the event
            if (StateChanged != null)
            {
                //Passes the value of the output to the circuit
                StateChanged();
            }
        }
    }
}
