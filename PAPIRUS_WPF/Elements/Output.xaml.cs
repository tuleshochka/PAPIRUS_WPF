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
    /// Логика взаимодействия для Output.xaml
    /// </summary>
    public partial class Output : UserControl
    {
        /// <summary>
        /// Called when the state of the output is changed
        /// </summary>
        public event StateChangeHandler StateChange;
        public delegate void StateChangeHandler();

        // The state of the output
        private bool _state;

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
        public Output()
        {
            _state = false;
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
    }
}
