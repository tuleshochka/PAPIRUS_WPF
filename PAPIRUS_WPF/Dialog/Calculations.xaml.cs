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
using System.Numerics;
using AngouriMath;
using LiveCharts;

namespace PAPIRUS_WPF.Dialog
{
    /// <summary>
    /// Логика взаимодействия для Calculations.xaml
    /// </summary>
    public partial class Calculations : Window
    {
        public Window window = Application.Current.MainWindow;
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
                ChartValues<ObservablePoint> frec = new ChartValues<ObservablePoint>();
                ChartValues<ObservablePoint> phase = new ChartValues<ObservablePoint>();
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
                    //while (Data.frec.Count > 0)
                    //{
                    //    Data.frec.RemoveAt(Data.frec.Count - 1);
                    //}
                    double x = 0;
                    double y1 = 0;
                    double y2 = 0;
                    int a;
                    int.TryParse(MatrixElement1.Text, out a);
                    int b;
                    int.TryParse(MatrixElement2.Text, out b);
                    Entity tempY = matrix[a - 1, b - 1];
                    //------------------------АЧХ------------------------------//
                    for (double i = Data.lowerLimit; i <= Data.upperLimit; i += (Data.upperLimit - Data.lowerLimit) / dotsNum)
                    {
                        x = i;
                        Entity entity = tempY.Substitute("f", x);
                        Complex complex = (Complex)entity.EvalNumerical();
                        y1 = Math.Sqrt(Math.Pow(complex.Real, 2) + Math.Pow(complex.Imaginary, 2));
                        if (complex.Real > 0)
                        {
                            if (complex.Imaginary >= 0) y2 = Math.Atan2(complex.Imaginary, complex.Real);
                            else y2 = 2 * Math.PI - Math.Atan2(complex.Imaginary, complex.Real);
                        }
                        else if (complex.Real < 0)
                        {
                            if (complex.Imaginary >= 0) y2 = Math.PI - Math.Atan2(complex.Imaginary, complex.Real);
                            else y2 = Math.PI + Math.Atan2(complex.Imaginary, complex.Real);
                        }
                        else if (complex.Real == 0)
                        {
                            if (complex.Imaginary > 0) y2 = Math.PI / 2;
                            else if (complex.Imaginary < 0) y2 = (3 * Math.PI) / 2;

                        }
                        frec.Add(new ObservablePoint(x, y1));

                        //------------------------ФЧХ------------------------------//
                        phase.Add(new ObservablePoint(x, y2));
                    }
                   
                }
                Graphic calc = new Graphic(frec, phase);
                calc.ShowDialog();
            }
        }
    

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        foreach (Object el in (window as MainWindow).CircuitCanvas.Children.OfType<Object>())
        {
            if (!(el is generator))
            {
                Object _ = (Object)el.Resources["DataSource"];
                _.DefaultNumberVisible = Visibility.Visible;
            }
        }
    }

    private void RadioButtonViaMatrix_Checked(object sender, RoutedEventArgs e)
    {
        //foreach(var a in ValueGroup.)
    }
}
}
