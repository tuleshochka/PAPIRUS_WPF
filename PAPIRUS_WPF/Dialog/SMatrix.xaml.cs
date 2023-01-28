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

            CalculateIntermediateValues();
            CalculateMatrix();
        }

        private void CalculateIntermediateValues()
        {
            if (ele.other_par.Count() != 0)
            {
                if(!CalculateIntermediateValues_params())
                {
                    this.Close();
                    return;
                }
                CalculateIntermediateValues_cycle();
            }
        }

        private void CalculateMatrix()
        {
            int a = 0;
            for (int i = 0; i < dataGridView.ColumnCount; i++)
            {
                for (int j = 0; j < dataGridView.RowCount; j++)
                {
                    string temp = OptimizeBeginString(ele.matrix[a].element);
                    if (temp == null)
                    {
                        this.Close();
                        return;
                    }
                    if ((ele.parameters.Count()) != 0)
                    {
                        for (int k = 0; k < ele.parameters.Count(); k++)
                        {
                            temp = OptimizeFinalString(temp.Replace(ele.parameters[k].paramColumn, (datagridelements1.Find(x => x.columnParam == ele.parameters[k].paramColumn + " (" + ele.parameters[k].unitColumn + ")").columnValue).ToString()));
                            expr = temp;
                            if (expr.EvaluableNumerical)
                            {
                                if(!MatrixCellEvalNumerical(i, j))
                                {
                                    this.Close();
                                    return;
                                }
                            }
                            else
                            {
                                ele.matrix[a].element = temp;
                            }
                        }
                    }
                    if (ele.other_par.Count() != 0)
                    {
                        for (int k = 0; k < ele.other_par.Count(); k++)
                        {
                            temp = OptimizeFinalString(temp.Replace(ele.other_par[k].headerColumn, ele.other_par[k].formulaColumn));
                            expr = temp;
                            if (expr.EvaluableNumerical)
                            {
                                if (!MatrixCellEvalNumerical(i, j))
                                {
                                    this.Close();
                                    return;
                                }
                            }
                            else
                            {
                                ele.matrix[a].element = temp;
                            }
                        }
                    }
                    a++;
                }
            }
            }

        private bool CalculateIntermediateValues_params()
        {
            for (int i = 0; i < ele.other_par.Count(); i++)
            {
                string temp = OptimizeBeginString(ele.other_par[i].formulaColumn);
                if(temp == null)
                {
                    return false;
                }
                if ((ele.parameters.Count()) != 0)
                {
                    try
                    {
                        for (int j = 0; j < ele.parameters.Count(); j++)
                        {
                            temp = OptimizeFinalString(temp.Replace(ele.parameters[j].paramColumn, (datagridelements1.Find(x => x.columnParam == ele.parameters[j].paramColumn + " (" + ele.parameters[j].unitColumn + ")").columnValue)));
                            expr = temp;
                            if (expr.EvaluableNumerical)
                            {
                                IntermediateValuesEvalNumerical(i);
                            }
                            else
                            {
                                ele.other_par[i].formulaColumn = temp;
                            }
                        }
                        this.WindowState = WindowState.Normal;
                    }
                    catch (Exception e)
                    {                        
                        MessageBox.Show("Произошла ошибка", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        this.Close();
                    }
                }
            }
            return true;
        }

        private void CalculateIntermediateValues_cycle()
        {
            for (int i = 0; i < ele.other_par.Count(); i++)
            {
                if (!(((Entity)ele.other_par[i].formulaColumn).EvaluableNumerical))
                {
                    foreach(var value in ele.other_par.Where(el => el != ele.other_par[i]))
                    {
                        expr = ele.other_par[i].formulaColumn.Replace(value.headerColumn, value.formulaColumn);
                        if (expr.EvaluableNumerical)
                        {
                            IntermediateValuesEvalNumerical(i);
                        }
                        else
                        {
                            ele.other_par[i].formulaColumn = ele.other_par[i].formulaColumn.Replace(value.headerColumn, value.formulaColumn);
                        }
                    }
                }
            }
            if(ele.other_par.Exists(x => !(((Entity)x.formulaColumn).EvaluableNumerical)))
            {
                CalculateIntermediateValues_cycle();
            }
        }

        private bool IntermediateValuesEvalNumerical(int i)
        {
            Complex complex = (Complex)expr.EvalNumerical();
            if (complex.Real is double.NaN || complex.Imaginary is double.NaN)
            {
                MessageBox.Show("Произошла ошибка в вычислениях, проверьте введённые данные");
                return false;
            }

            if (complex.Imaginary == 0)
            {
                ele.other_par[i].formulaColumn = ((double)expr.EvalNumerical()).ToString().Replace(",", ".");
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
                ele.other_par[i].formulaColumn = (complex.Real + "" + sigh + complex.Imaginary + "i").Replace(",", ".");
            }
                return true;
        }

        private bool MatrixCellEvalNumerical(int i, int j)  //для правильного отображения расчетов
        {
            Complex complex = (Complex)expr.EvalNumerical();
            if(complex.Real is double.NaN || complex.Imaginary is double.NaN)
            {
                MessageBox.Show("Произошла ошибка в вычислениях, проверьте введённые данные");
                return false;
            }
            if (complex.Imaginary == 0)
            {
                if ((Math.Abs(Math.Round((double)expr.EvalNumerical(), 3)) <= 0.01 && Math.Abs(Math.Round((double)expr.EvalNumerical(), 3)) >= 0 && (double)expr.EvalNumerical() != 0) || Math.Abs(Math.Round((double)expr.EvalNumerical(), 3)) >= 1000)
                {
                    dataGridView.Rows[j].Cells[i].Value = String.Format("{0:0.###E+0}", ((double)expr.EvalNumerical()).ToString());
                }
                else
                {
                    dataGridView.Rows[j].Cells[i].Value = (Math.Round((double)expr.EvalNumerical(), 3)).ToString();
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
                dataGridView.Rows[j].Cells[i].Value = real + "" + sigh + imaginary + "i";
            }
            return true;
        }

        private string OptimizeBeginString(string originalString)
        {
            string temp = originalString.Replace(" ", "");
            if (generatorCon)
            {
                temp = temp.Replace("w", "2*pi*f");
                temp = temp.Replace("f", (Data.specificFrequency.ToString()).Replace(",","."));
            }
            else if (!generatorCon && (temp.Contains("w") || temp.Contains("f")))
            {
                MessageBox.Show("Элемент не подключен к генератору");
                return null;
            }
            return temp;
        }

        private string OptimizeFinalString(string originalString)
        {
            string temp = originalString;
            foreach (var _operator in operators)
            {
                temp = temp.Replace(_operator.Key, _operator.Value);
            }
            return temp;
        }
    }
}

