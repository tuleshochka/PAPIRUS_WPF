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
using static AngouriMath.Entity;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Window = System.Windows.Window;
using Matrix = PAPIRUS_WPF.Models.Matrix;
using System.Drawing.Drawing2D;
using LiveCharts.Defaults;
using PAPIRUS_WPF.Elements;

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
                if (el is generator)
                { }
                else
                {
                    Object _ = (Object)el.Resources["DataSource"];
                    _.DefaultNumberVisible = Visibility.Hidden;
                }
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
                        SMatrixCalculation calculation = new SMatrixCalculation();
                        Matrix matrix = null;
                        try
                        {
                            matrix = calculation.CalculateTotal(Data.elements);
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message);
                            this.Close();
                        }
                        SMatrix dialog = new SMatrix(matrix);
                        dialog.ShowDialog();
                    }
                    catch (Exception b)
                    { MessageBox.Show(b.Message); }
                   
                }
            }
            else
            {
                SMatrixCalculation calculation = new SMatrixCalculation();
                Matrix matrix = null;
                try
                {
                    matrix = calculation.CalculateTotal(Data.elements);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    this.Close();
                }
                
                int dotsNum;
                int.TryParse(DotsNumber.Text, out dotsNum);
                if (dotsNum < 2)
                {

                }
                else
                { 
                    double x;
                    double y;
                    //------------------------АЧХ------------------------------//
                    for (int i = 0; i < dotsNum; i++)
                    {

                        x = i;
                        
                        //Data.frec.Add(new ObservablePoint(x, y));


                    }
                    //------------------------ФЧХ------------------------------//
                    for (int i = 0; i < dotsNum; i++)
                    {
                        x = i;

                        //Data.phase.Add(new ObservablePoint(x, y));

                    }
                }
                Graphic calc = new Graphic();
                calc.Show();
                
              
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
