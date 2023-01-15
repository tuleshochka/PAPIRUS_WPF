using PAPIRUS_WPF.Elements;
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
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class multi_pole_select : Window
    {
        bool f=false;
        public multi_pole_select()
        {
            InitializeComponent();
            btn_12.IsChecked= true;
        }

        private void btn_choice_Click(object sender, RoutedEventArgs e)
        {
            f= true;
            if (btn_12.IsChecked == true)
            {
                Data.multiPole = new twelve_pole();
            }
            else if (btn_14.IsChecked == true)
            {
                Data.multiPole = new fourteen_pole();
            }
            else if (btn_16.IsChecked == true)
            {
                Data.multiPole = new sixteen_pole();
            }
            else if (btn_18.IsChecked == true)
            {
                Data.multiPole = new eighteen_pole();
            }
            else if (btn_20.IsChecked == true)
            {
                Data.multiPole = new twentee_pole();
            }
            else if (btn_22.IsChecked == true)
            {
                Data.multiPole = new twenteetwo_pole();
            }
            else if (btn_24.IsChecked == true)
            {
                Data.multiPole = new multi_pole();
            }
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!f)
            {
                Data.multiPole = null;
            }
            else { }
        }
    }
}
