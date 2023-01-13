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

namespace PAPIRUS_WPF.Dialog
{

    /// <summary>
    /// Логика взаимодействия для SMatrix.xaml
    /// </summary>
    public partial class SMatrix : Window
    {

        System.Windows.Forms.Integration.WindowsFormsHost host =
        new System.Windows.Forms.Integration.WindowsFormsHost();
        DataGridView dataGridView = new DataGridView();
        //для DataGridView
        private Entity expr;
        Element ele;
        List<string> formulaColumn = new List<string>();
        List<dataGridElements> datagridelements1;
        int poleNum;
        public SMatrix(int polenum, List<dataGridElements> datagridelements, Element el)
        {
            InitializeComponent();
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
            if (ele.other_par.Count() != 0)
            {

                for (int i = 0; i < ele.other_par.Count(); i++)
                {

                    formulaColumn.Add(ele.other_par[i].formulaColumn.ToString());
                    if (!string.IsNullOrEmpty(ele.parameters[0]))
                    {
                        try
                        {
                            for (int j = 0; j < ele.parameters.Count(); j++)
                            {
                                expr = ele.other_par[i].formulaColumn.ToString().Replace(" ", "");

                                expr = expr.ToString().Replace(ele.parameters[j], (datagridelements1.Find(x => x.columnParam == ele.parameters[j]).columnValue).ToString());
                                if (expr.EvaluableNumerical)
                                {
                                    ele.other_par[i].formulaColumn = expr.EvalNumerical().ToString();
                                }
                                else
                                {
                                    ele.other_par[i].formulaColumn = ele.other_par[i].formulaColumn.ToString().Replace(ele.parameters[j], (datagridelements1.Find(x => x.columnParam == ele.parameters[j]).columnValue).ToString());
                                }
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Произошла ошибка", "", MessageBoxButton.OK, MessageBoxImage.Error);
                            this.Close();
                        }

                    }
                }
            }

            if (ele.other_par.Count() != 0)
            {
                for (int i = 0; i < ele.other_par.Count(); i++)
                {

                    for (int j = 0; j < ele.other_par.Count(); j++)
                    {
                        expr = ele.other_par[i].formulaColumn.ToString().Replace(" ", "");
                        expr = expr.ToString().Replace(ele.other_par[j].headerColumn, ele.other_par[j].formulaColumn);
                        if (expr.EvaluableNumerical)
                        {
                            ele.other_par[i].formulaColumn = expr.EvalNumerical().ToString();
                        }
                        else
                        {
                            ele.other_par[i].formulaColumn = ele.other_par[i].formulaColumn.ToString().Replace(ele.other_par[j].headerColumn, ele.other_par[j].formulaColumn);
                        }
                    }

                }
            }

            int a = 0;
            for (int i = 0; i < dataGridView.ColumnCount; i++)
            {
                for (int j = 0; j < dataGridView.RowCount; j++)
                {
                    if (!string.IsNullOrEmpty(ele.parameters[0]))
                    {
                        for (int k = 0; k < ele.parameters.Count(); k++)
                        {
                            expr = ele.matrix[a].element.Replace(ele.parameters[k], (datagridelements1.Find(x => x.columnParam == ele.parameters[k]).columnValue).ToString());
                            if (expr.EvaluableNumerical)
                            {
                                Complex complex = (Complex)expr.EvalNumerical();
                                if (complex.Imaginary == 0)
                                {
                                    if ((Math.Abs(Math.Round((double)expr.EvalNumerical(), 3)) <= 0.01 && Math.Abs(Math.Round((double)expr.EvalNumerical(), 3)) >= 0 && (double)expr.EvalNumerical() != 0) || Math.Abs(Math.Round((double)expr.EvalNumerical(), 3)) >= 1000)
                                    {
                                        dataGridView.Rows[j].Cells[i].Value = String.Format("{0:0.###E+0}", (double)expr.EvalNumerical());
                                    }
                                    else
                                    {
                                        dataGridView.Rows[j].Cells[i].Value = (Math.Round((double)expr.EvalNumerical(), 3)).ToString();
                                    }
                                }
                                else
                                {
                                    string sigh;
                                    switch (Math.Sign(complex.Imaginary))
                                    {
                                        case -1:
                                            sigh = "";
                                            break;
                                        default:
                                            sigh = "+";
                                            break;
                                    }

                                    dataGridView.Rows[j].Cells[i].Value = (Math.Round(complex.Real, 3) + "" + sigh + "" + Math.Round(complex.Imaginary, 3) + "i").ToString();
                                }

                            }
                            else
                            {
                                ele.matrix[a].element = ele.matrix[a].element.Replace(ele.parameters[k], (datagridelements1.Find(x => x.columnParam == ele.parameters[k]).columnValue).ToString());
                            }
                        }
                    }
                    if (ele.other_par.Count() != 0)
                    {
                        for (int k = 0; k < ele.other_par.Count(); k++)
                        {
                            expr = ele.matrix[a].element.Replace(ele.other_par[k].headerColumn, ele.other_par[k].formulaColumn);
                            if (expr.EvaluableNumerical)
                            {
                                Complex complex = (Complex)expr.EvalNumerical();
                                if (complex.Imaginary == 0)
                                {
                                    if ((Math.Abs(Math.Round((double)expr.EvalNumerical(), 3)) <= 0.01 && Math.Abs(Math.Round((double)expr.EvalNumerical(), 3)) >= 0 && (double)expr.EvalNumerical() != 0) || Math.Abs(Math.Round((double)expr.EvalNumerical(), 3)) >= 1000)
                                    {
                                        dataGridView.Rows[j].Cells[i].Value = String.Format("{0:0.###E+0}", (double)expr.EvalNumerical());
                                    }
                                    else
                                    {
                                        dataGridView.Rows[j].Cells[i].Value = (Math.Round((double)expr.EvalNumerical(), 3)).ToString();
                                    }
                                }
                                else
                                {
                                    string sigh;
                                    switch (Math.Sign(complex.Imaginary))
                                    {
                                        case -1:
                                            sigh = "";
                                            break;
                                        default:
                                            sigh = "+";
                                            break;
                                    }

                                    dataGridView.Rows[j].Cells[i].Value = (Math.Round(complex.Real, 3) + "" + sigh + "" + Math.Round(complex.Imaginary, 3) + "i").ToString();
                                }
                            }
                            else
                            {
                                ele.matrix[a].element = ele.matrix[a].element.Replace(ele.other_par[k].headerColumn, ele.other_par[k].formulaColumn);
                            }
                        }
                    }
                    a++;
                }
            }
        }
    }
}

