using PAPIRUS_WPF.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PAPIRUS_WPF
{
    public class FormEditor
    {

        private const int ClickProximity = 2 * Symbol.PinRadius;

        public MainWindow Mainframe { get; private set; }
        private Dispatcher Dispatcher { get { return this.Mainframe.Dispatcher; } }
        protected Canvas Diagram { get { return this.Mainframe.CircuitCanvas; } }


        public double Zoom()
        {

            MainWindow main = new MainWindow();
            // MessageBox.Show(main.zoom.Value.ToString());
            return main.zoom.Value;
        }

      

        //---------Selection--------------//

    

    }
}
