using System;
using System.Linq;
using System.Windows;
using Window = System.Windows.Window;
using Matrix = PAPIRUS_WPF.Models.Matrix;
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
            Data.visibleBool = true;

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
                            SMatrix dialog = new SMatrix(matrix);
                            this.Close();
                            dialog.ShowDialog();
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message);
                            this.Close();
                            return;
                        }

                    }
                    catch (Exception b)
                    { MessageBox.Show(b.Message); }

                }
                else if (RadioButtonDMatrix.IsChecked == true)
                {
                    SMatrixCalculation calculation = new SMatrixCalculation();
                    DMatrixCalculation Dcalc = new DMatrixCalculation();
                    Matrix matrix = null;
                    Matrix DMatrix = null;
                    try
                    {
                        matrix = calculation.CalculateTotal(Data.elements);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                        this.Close();
                        return;
                    }
                    DMatrix = Dcalc.Calculate(matrix);
                    SMatrix dialog = new SMatrix(DMatrix);
                    dialog.ShowDialog();
                }
            }
            else
            {
                if(RadioButtonSMatrix.IsChecked == true)
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
                        return;
                    }
                    DrawGraphic(matrix);
                }
                else if (RadioButtonDMatrix.IsChecked == true)
                {
                    SMatrixCalculation calculation = new SMatrixCalculation();
                    DMatrixCalculation Dcalc = new DMatrixCalculation();
                    Matrix matrix = null;
                    Matrix DMatrix = null;
                    try
                    {
                        matrix = calculation.CalculateTotal(Data.elements);
                        DMatrix = Dcalc.Calculate(matrix);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                        this.Close();
                        return;
                    }
                    DrawGraphic(DMatrix);
                }
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void RadioButtonViaMatrix_Checked(object sender, RoutedEventArgs e)
        {
            //foreach(var a in ValueGroup.)
        }

        private void DrawGraphic(Matrix matrix)
        {
            ChartValues<ObservablePoint> frec = new ChartValues<ObservablePoint>();
            ChartValues<ObservablePoint> phase = new ChartValues<ObservablePoint>();
            int dotsNum;
            int.TryParse(DotsNumber.Text, out dotsNum);
            if (dotsNum >= 2)
            {
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
                    try
                    {
                        Complex complex = (Complex)entity.EvalNumerical();
                        y1 = Math.Sqrt(Math.Pow(complex.Real, 2) + Math.Pow(complex.Imaginary, 2));
                        y1 = y1/Data.factorForHertz;
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
                        y2 = y2/Data.factorForHertz;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Произошла ошибка при вычислениях");
                        return;
                    }
                    x = x/Data.factorForHertz;
                    frec.Add(new ObservablePoint(x, y1));

                    //------------------------ФЧХ------------------------------//
                    phase.Add(new ObservablePoint(x, y2));
                }
            }
            Graphic calc = new Graphic(frec, phase);
            this.Close();
            calc.ShowDialog();
        }
    }
}
