using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF_SHF_Element_lib;
using AngouriMath;
using System.Runtime.CompilerServices;
using System.Numerics;
using System.Xml.Linq;
using System.Globalization;
using MessageBox = System.Windows.MessageBox;
using System.Text.RegularExpressions;
using PAPIRUS_WPF.Models;
using Element = PAPIRUS_WPF.Models.Element;
using static AngouriMath.Entity;
using Matrix = PAPIRUS_WPF.Models.Matrix;
using PAPIRUS_WPF.Elements;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PAPIRUS_WPF.Dialog
{

    /// <summary>
    /// Логика взаимодействия для SMatrix.xaml
    /// </summary>
    public partial class SMatrix : System.Windows.Window
    {

        CultureInfo culture = new CultureInfo("en");
        System.Windows.Forms.Integration.WindowsFormsHost host =
        new System.Windows.Forms.Integration.WindowsFormsHost();
        DataGridView dataGridView = new DataGridView();

        Dictionary<string, string> operators = new Dictionary<string, string>()
        {
            {"+-","-"},
            {"--","+"},
        };

        public SMatrix(Matrix matrix)
        {
            InitializeComponent();
            this.WindowState = WindowState.Minimized;
            host.Child = dataGridView;
            Grid.SetColumn(host, 1);
            Grid.SetRow(host, 1);
            // Add the interop host control to the Grid
            // control's collection of child controls.
            this.grid.Children.Add(host);
            dataGridView.RowCount = matrix.M + 1;
            dataGridView.ColumnCount = matrix.N;
            dataGridView.BackgroundColor = System.Drawing.Color.White;
            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                dataGridView.Rows[i].HeaderCell.Value = (i + 1).ToString();
                for (int j = 0; j < dataGridView.ColumnCount; j++)
                {
                    dataGridView.Columns[j].HeaderText = (j + 1).ToString();
                }
            }
            for (int i = 0; i < matrix.N; i++)
            {
                dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            dataGridView.AllowUserToAddRows = false;
            
            //-----------------ОТОБРАЖЕНИЕ МАТРИЦЫ-----------------//
            for (int i = 0; i < matrix.M; i++)
            {
                for (int j = 0; j < matrix.N; j++)
                {
                    if (matrix[i, j] == null)
                    {
                        throw new Exception("Произошла ошибка в вычислениях");
                    }
                    else
                    {
                        Entity temp = matrix[i, j].Substitute("f", Data.specificFrequency);
                        Complex complex = Complex.Zero;
                        if (temp.EvaluableNumerical)
                        {
                            complex = (Complex)temp.EvalNumerical();
                        }
                        else
                        {
                            throw new Exception("Произошла ошибка в вычислениях");
                        }
                        if (complex.Imaginary == 0)
                        {
                            Entity expr = complex.Real;
                            if ((Math.Abs(Math.Round((double)expr.EvalNumerical(), 3)) <= 0.01 && Math.Abs(Math.Round((double)expr.EvalNumerical(), 3)) >= 0 && (double)expr.EvalNumerical() != 0) || Math.Abs(Math.Round((double)expr.EvalNumerical(), 3)) >= 1000)
                            {
                                dataGridView.Rows[i].Cells[j].Value = String.Format("{0:0.###E+0}", ((double)expr.EvalNumerical()).ToString());
                            }
                            else
                            {
                                dataGridView.Rows[i].Cells[j].Value = (Math.Round((double)expr.EvalNumerical(), 3)).ToString();
                            }
                        }
                        else
                        {
                            string sigh = "+";
                            switch (Math.Sign(complex.Imaginary))
                            {
                                case -1:
                                    sigh = "";
                                    break;
                                case 1:
                                    sigh = "+";
                                    break;
                                case 0:
                                    sigh = "+";
                                    break;
                            }
                            string real = null, imaginary = null;

                            if ((Math.Abs(Math.Round(complex.Real, 3)) <= 0.01 && Math.Abs(Math.Round(complex.Real, 3)) >= 0 && complex.Real != 0) || Math.Abs(Math.Round(complex.Real, 3)) >= 1000)
                            {

                                real = String.Format("{0:0.###E+0}", complex.Real);
                            }
                            else
                            {
                                real = (Math.Round(complex.Real, 3)).ToString();
                            }
                            if ((Math.Abs(Math.Round(complex.Imaginary, 3)) <= 0.01 && Math.Abs(Math.Round(complex.Imaginary, 3)) >= 0 && complex.Imaginary != 0) || Math.Abs(Math.Round(complex.Imaginary, 3)) >= 1000)
                            {
                                imaginary = String.Format("{0:0.###E+0}", complex.Imaginary);
                            }
                            else
                            {
                                imaginary = (Math.Round(complex.Imaginary, 3)).ToString();
                            }
                            dataGridView.Rows[i].Cells[j].Value = real + "" + sigh + imaginary + "i";
                        }
                    }

                }

            }

            this.WindowState = WindowState.Normal;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}

