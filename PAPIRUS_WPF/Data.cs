using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PAPIRUS_WPF
{
    public class Data
    {
        public static List<Object> selection = new List<Object>();
        public static Stack<FrameworkElement> undo = new Stack<FrameworkElement>();

    }
}
