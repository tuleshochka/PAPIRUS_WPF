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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Window = System.Windows.Window;

namespace PAPIRUS_WPF.Dialog
{
    /// <summary>
    /// Логика взаимодействия для Calculations.xaml
    /// </summary>
    public partial class Calculations : Window
    { public Window window = Application.Current.MainWindow;
        public Calculations()
        {
            InitializeComponent();
            RadioButtonSMatrix.IsChecked = true;
            RadioButtonViaMatrix.IsChecked = true; 
            
           
            foreach (Object el in (window as MainWindow).CircuitCanvas.Children.OfType<Object>())
            {
                Object _ = (Object)el.Resources["DataSource"];
                _.DefaultNumberVisible = Visibility.Hidden;
            }

        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
          
           
            if (RadioButtonViaMatrix.IsChecked == true)
            {
                if (RadioButtonSMatrix.IsChecked == true)
                {
                    try
                    {
                        SMatrixCalculation calc = new SMatrixCalculation();
                        calc.CalculateTotal(Data.elements);
                    }
                    catch (Exception b)
                    { MessageBox.Show(b.Message); }
                    
                    
                }
            }
            else
            {
                Graphic calc = new Graphic();
                calc.Show();
                
                (window as MainWindow).CircuitCanvas.Children.Add(new TextBlock { Text = "hello"});
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Object el in (window as MainWindow).CircuitCanvas.Children.OfType<Object>())
            {
                Object _ = (Object)el.Resources["DataSource"];
                _.DefaultNumberVisible = Visibility.Visible;
            }
        }
    }
}
