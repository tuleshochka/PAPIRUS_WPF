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
using System.Windows.Shapes;

namespace PAPIRUS_WPF.Dialog
{
    /// <summary>
    /// Логика взаимодействия для Calculations.xaml
    /// </summary>
    public partial class Calculations : Window
    {
        public Calculations()
        {
            InitializeComponent();
            RadioButtonSMatrix.IsChecked = true;
            RadioButtonViaMatrix.IsChecked = true;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if(RadioButtonViaMatrix.IsChecked == true)
            {
                if (RadioButtonSMatrix.IsChecked == true)
                {
                }
            }
            else
            {
                Graphic calc = new Graphic();
                calc.Show();
            }
        }

        
    }
}
