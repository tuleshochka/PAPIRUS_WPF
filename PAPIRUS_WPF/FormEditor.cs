using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PAPIRUS_WPF
{
    public class FormEditor
    {
      public double Zoom()
        {
            
            MainWindow main = new MainWindow();
           // MessageBox.Show(main.zoom.Value.ToString());
            return main.zoom.Value;
        }
    }
}
