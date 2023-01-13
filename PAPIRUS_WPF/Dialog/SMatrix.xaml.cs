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

        Entity expr;
        

        public SMatrix(int poleNum, List<dataGridElements> datagridelements, Element el)
        {
            InitializeComponent();
            host.Child = dataGridView;
            Grid.SetColumn(host, 1);
            Grid.SetRow(host, 1);
            
            // Add the interop host control to the Grid
            // control's collection of child controls.
            this.grid.Children.Add(host);
            dataGridView.RowCount = poleNum + 1;
            dataGridView.ColumnCount = poleNum;
            dataGridView.BackgroundColor = System.Drawing.Color.White;
            dataGridView.RowHeadersVisible = false;
            dataGridView.ColumnHeadersVisible = false;
            for (int i = 0; i < poleNum; i++)
            {
                dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            dataGridView.AllowUserToAddRows = false;
            if (el.other_par.Count()!=0)
            {
                Console.WriteLine("есть");
                for (int i = 0; i < el.other_par.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(el.parameters[0]))
                    {
                        for (int j = 0; j < el.parameters.Count(); j++)
                        {
                            expr = el.other_par[i].formulaColumn.ToString().Replace(el.parameters[j], (datagridelements.Find(x => x.columnParam == el.parameters[j]).columnValue).ToString());
                            if(expr.EvaluableNumerical)
                            {
                                el.other_par[i].formulaColumn = expr.EvalNumerical().ToString();
                            }
                            else
                            {
                                el.other_par[i].formulaColumn = el.other_par[i].formulaColumn.ToString().Replace(el.parameters[j], (datagridelements.Find(x => x.columnParam == el.parameters[j]).columnValue).ToString());
                            }
                        }
                    }
                }
            }

            if (el.other_par.Count() != 0)
            {
                for (int i = 0; i < el.other_par.Count(); i++)
                {
                    for (int j = 0; j < el.other_par.Count(); j++)
                    {
                        expr = el.other_par[i].formulaColumn.ToString().Replace(el.other_par[j].headerColumn, el.other_par[j].formulaColumn);
                        if (expr.EvaluableNumerical)
                        {
                            el.other_par[i].formulaColumn = expr.EvalNumerical().ToString();
                        }
                        else
                        {
                            el.other_par[i].formulaColumn = el.other_par[i].formulaColumn.ToString().Replace(el.other_par[j].headerColumn, el.other_par[j].formulaColumn);
                        }
                    }
                    Console.WriteLine("H= "+el.other_par[i].formulaColumn);
                }
            }

            int a = 0;
            for (int i = 0; i < dataGridView.ColumnCount; i++)
            {
                for (int j = 0; j < dataGridView.RowCount; j++)
                {
                    if (!string.IsNullOrEmpty(el.parameters[0]))
                    {
                        for (int k = 0; k < el.parameters.Count(); k++)
                        {
                            expr = el.matrix[a].element.Replace(el.parameters[k], (datagridelements.Find(x => x.columnParam == el.parameters[k]).columnValue).ToString());
                            if (expr.EvaluableNumerical)
                            {
                                dataGridView.Rows[j].Cells[i].Value = expr.EvalNumerical().ToString();
                            }
                            else
                            {
                                el.matrix[a].element = el.matrix[a].element.Replace(el.parameters[k], (datagridelements.Find(x => x.columnParam == el.parameters[k]).columnValue).ToString());
                            }
                            Console.WriteLine(el.matrix[a].element);
                        }
                    }
                    if(el.other_par.Count() != 0)
                    {
                        for (int k = 0; k < el.other_par.Count(); k++)
                        {
                            expr = el.matrix[a].element.Replace(el.other_par[k].headerColumn, el.other_par[k].formulaColumn);
                            if (expr.EvaluableNumerical)
                            {
                                dataGridView.Rows[j].Cells[i].Value = expr.EvalNumerical().ToString();
                            }
                            else
                            {
                                el.matrix[a].element = el.matrix[a].element.Replace(el.other_par[k].headerColumn, el.other_par[k].formulaColumn);
                            }
                            Console.WriteLine(el.matrix[a].element);
                        }  
                    }
                    a++;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
