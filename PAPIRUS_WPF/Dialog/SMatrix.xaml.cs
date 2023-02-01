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

namespace PAPIRUS_WPF.Dialog
{

    /// <summary>
    /// Логика взаимодействия для SMatrix.xaml
    /// </summary>
    public partial class SMatrix : Window
    {
        CultureInfo culture = new CultureInfo("en");
        System.Windows.Forms.Integration.WindowsFormsHost host =
        new System.Windows.Forms.Integration.WindowsFormsHost();
        DataGridView dataGridView = new DataGridView();
        //для DataGridView
        private Entity expr;
        Element ele;
        List<DataGridElements> datagridelements1;
        int poleNum;
        private bool generatorCon;
        Dictionary<string, string> operators = new Dictionary<string, string>()
        {
            {"+-","-"},
            {"--","+"},
        };

        public SMatrix(int polenum, List<DataGridElements> datagridelements, Element el, bool generatorConnected)
        {
            InitializeComponent();
            this.WindowState = WindowState.Minimized;
            generatorCon = generatorConnected;
            host.Child = dataGridView;
            Grid.SetColumn(host, 1);
            Grid.SetRow(host, 1);
            poleNum = polenum;
            ele = el;
            datagridelements1 = datagridelements;
            // Add the interop host control to the Grid
            // control's collection of child controls.
            this.grid.Children.Add(host);
            dataGridView.RowCount = poleNum + 1;
            dataGridView.ColumnCount = poleNum;
            dataGridView.BackgroundColor = System.Drawing.Color.White;
            dataGridView.RowHeadersVisible = false;
            dataGridView.ColumnHeadersVisible = false;

        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < poleNum; i++)
            {
                dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            dataGridView.AllowUserToAddRows = false;
            int number = ele.group;
            SMatrixCalculation calculation = new SMatrixCalculation();
            Complex[,] matrix = new Complex[number, number];
            try
            {
                matrix = calculation.Calculate(ele, generatorCon, datagridelements1);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                this.Close();
            }
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j].Imaginary == 0)
                    {
                        Entity expr = matrix[i, j].Real;
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
                        switch (Math.Sign(matrix[i, j].Imaginary))
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

                        if ((Math.Abs(Math.Round(matrix[i, j].Real, 3)) <= 0.01 && Math.Abs(Math.Round(matrix[i, j].Real, 3)) >= 0 && matrix[i, j].Real != 0) || Math.Abs(Math.Round(matrix[i, j].Real, 3)) >= 1000)
                        {

                            real = String.Format("{0:0.###E+0}", matrix[i, j].Real);
                        }
                        else
                        {
                            real = (Math.Round(matrix[i, j].Real, 3)).ToString();
                        }
                        if ((Math.Abs(Math.Round(matrix[i, j].Imaginary, 3)) <= 0.01 && Math.Abs(Math.Round(matrix[i, j].Imaginary, 3)) >= 0 && matrix[i, j].Imaginary != 0) || Math.Abs(Math.Round(matrix[i, j].Imaginary, 3)) >= 1000)
                        {
                            imaginary = String.Format("{0:0.###E+0}", matrix[i, j].Imaginary);
                        }
                        else
                        {
                            imaginary = (Math.Round(matrix[i, j].Imaginary, 3)).ToString();
                        }
                        dataGridView.Rows[i].Cells[j].Value = real + "" + sigh + imaginary + "i";
                    }
                }
            }
            this.WindowState = WindowState.Normal;

        }
    }
}

