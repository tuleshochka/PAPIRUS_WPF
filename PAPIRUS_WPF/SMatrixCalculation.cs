using AngouriMath;
using PAPIRUS_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using WPF_SHF_Element_lib;
using Element = PAPIRUS_WPF.Models.Element;
using WPF_SHF_Element_lib;
using AngouriMath;
using System.Runtime.CompilerServices;
using System.Numerics;
using System.Xml.Linq;
using System.Windows.Documents;
using AngouriMath.Extensions;

namespace PAPIRUS_WPF
{
    public class SMatrixCalculation
    {

        //размерность общей матрицы зависит от количества свободных входов, 2 - 2x2, 5 - 5х5
        // матрица соединений составляется из соединенных плеч
        // матрица делится на блоки aa (свободные плечи) и bb (соединенные плечи)
        public void CalculateTotal(List<Object> elements) // расчет общей S-матрицы
        {
            List<Complex[,]> allMatrix = elements.Select(x => x.matrix).ToList();
            //Complex[,] totalMatrix = allMatrix.Aggregate((x, y) => Sum(x,y));
        }

        //private Complex[,] Sum(Complex[,] x, Complex[,] y)
       // {
            
        //}

        Dictionary<string, string> operators = new Dictionary<string, string>()
        {
            {"+-","-"},
            {"--","+"},
        };

        public Complex[,] Calculate(Element element, bool isGeneretorConnected, List<DataGridElements> dataGridElements)
        {
            Element tempElement = element;
            int number = element.group;
            Complex[,] matrix = new Complex[number, number];
            if (tempElement.other_par.Count() != 0)
            {
                try
                {
                    CalculateIntermediateValues_params(tempElement, isGeneretorConnected, dataGridElements);
                    CalculateIntermediateValues_cycle(tempElement);
                    matrix = CalculateMatrix(tempElement, isGeneretorConnected, dataGridElements);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            return matrix;
        }

        private void CalculateIntermediateValues_params(Element element, bool isGeneretorConnected, List<DataGridElements> dataGridElements)
        {
            for (int i = 0; i < element.other_par.Count(); i++)
            {
                string temp = null;
                try
                {
                    temp = OptimizeBeginString(element.other_par[i].formulaColumn, isGeneretorConnected);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                if ((element.parameters.Count()) != 0)
                {

                    for (int j = 0; j < element.parameters.Count(); j++)
                    {
                        temp = OptimizeFinalString(temp.Replace(element.parameters[j].paramColumn, (dataGridElements.Find(x => x.columnParam == element.parameters[j].paramColumn + " (" + element.parameters[j].unitColumn + ")").columnValue)));
                        Entity expr = temp;
                        if (expr.EvaluableNumerical)
                        {
                            try
                            {
                                element = IntermediateValuesEvalNumerical(element, expr, i);
                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.Message);
                            }
                        }
                        else
                        {
                            element.other_par[i].formulaColumn = temp;
                        }
                    }
                }
            }
        }

        private void CalculateIntermediateValues_cycle(Element element)
        {
            for (int i = 0; i < element.other_par.Count(); i++)
            {
                if (!(((Entity)element.other_par[i].formulaColumn).EvaluableNumerical))
                {
                    foreach (var value in element.other_par.Where(el => el != element.other_par[i]))
                    {
                        Entity expr = element.other_par[i].formulaColumn.Replace(value.headerColumn, value.formulaColumn);
                        if (expr.EvaluableNumerical)
                        {
                            try
                            {
                                element = IntermediateValuesEvalNumerical(element, expr, i);
                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.Message);
                            }
                        }
                        else
                        {
                            element.other_par[i].formulaColumn = element.other_par[i].formulaColumn.Replace(value.headerColumn, value.formulaColumn);
                        }
                    }
                }
            }
            if (element.other_par.Exists(x => !(((Entity)x.formulaColumn).EvaluableNumerical)))
            {
                CalculateIntermediateValues_cycle(element);
            }
        }

        private Complex[,] CalculateMatrix(Element element, bool isGeneretorConnected, List<DataGridElements> dataGridElements)
        {
            int number = element.group;
            Complex[,] matrix = new Complex[number, number];
            int a = 0;
            for (int i = 0; i < number; i++)
            {
                for (int j = 0; j < number; j++)
                {
                    string temp = null;
                    try
                    {
                        temp = OptimizeBeginString(element.matrix[a].element, isGeneretorConnected);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    if ((element.parameters.Count()) != 0)
                    {
                        for (int k = 0; k < element.parameters.Count(); k++)
                        {
                            temp = OptimizeFinalString(temp.Replace(element.parameters[k].paramColumn, (dataGridElements.Find(x => x.columnParam == element.parameters[k].paramColumn + " (" + element.parameters[k].unitColumn + ")").columnValue).ToString()));
                            Entity expr = temp;
                            if (expr.EvaluableNumerical)
                            {
                                try
                                {
                                    matrix = MatrixCellEvalNumerical(matrix, expr, i, j);
                                }
                                catch(Exception e)
                                {
                                    throw new Exception(e.Message);
                                }
                            }
                            else
                            {
                                element.matrix[a].element = temp;
                            }
                        }
                    }
                    if (element.other_par.Count() != 0)
                    {
                        for (int k = 0; k < element.other_par.Count(); k++)
                        {
                            temp = OptimizeFinalString(temp.Replace(element.other_par[k].headerColumn, element.other_par[k].formulaColumn));
                            Entity expr = temp;
                            if (expr.EvaluableNumerical)
                            {
                                try
                                {
                                    matrix = MatrixCellEvalNumerical(matrix, expr, i, j);
                                }
                                catch (Exception e)
                                {
                                    throw new Exception(e.Message);
                                }
                            }
                            else
                            {
                                element.matrix[a].element = temp;
                            }
                        }
                    }
                    a++;
                }
            }
            return matrix;
        }


        private Element IntermediateValuesEvalNumerical(Element element, Entity expr,int i)
        {
            Complex complex = (Complex)expr.EvalNumerical();
            if (complex.Real is double.NaN || complex.Imaginary is double.NaN)
            {
                throw new Exception("Произошла ошибка в вычислениях, проверьте введённые данные");
            }

            if (complex.Imaginary == 0)
            {
                element.other_par[i].formulaColumn = ((double)expr.EvalNumerical()).ToString().Replace(",", ".");
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
                element.other_par[i].formulaColumn = (complex.Real + "" + sigh + complex.Imaginary + "i").Replace(",", ".");
            }
            return element;
        }

        private Complex[,] MatrixCellEvalNumerical(Complex[,] matrix, Entity expr, int i, int j)  //для правильного отображения расчетов
        {
            Complex complex = (Complex)expr.EvalNumerical();
            if (complex.Real is double.NaN || complex.Imaginary is double.NaN)
            {
                throw new Exception("Произошла ошибка в вычислениях, проверьте введённые данные");
            }
            if (complex.Imaginary == 0)
            {
                matrix[i,j] = ((double)expr.EvalNumerical());
            }
            else
            {
                matrix[i,j] = complex;
            }
            return matrix;
        }

        private string OptimizeBeginString(string originalString, bool isGeneretorConnected)
        {
            string temp = originalString.Replace(" ", "");
            if (isGeneretorConnected)
            {
                temp = temp.Replace("w", "2*pi*f");
                temp = temp.Replace("f", (Data.specificFrequency.ToString()).Replace(",", "."));
            }
            else if (!isGeneretorConnected && (temp.Contains("w") || temp.Contains("f")))
            {
                throw new Exception("Элемент не подключен к генератору");
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
    

