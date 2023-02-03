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
using PAPIRUS_WPF.Elements;
using static AngouriMath.Entity;

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
            //Object totalMatrix = elements.Aggregate((x, y) => Sum(x, y));

            List<int> free = new List<int>();
            List<int> connected = new List<int>();
            int i = 0;
            foreach (Object _object in elements)
            {
                _object.FillME();
                int number = _object.group;
                int j = 0;
                foreach (Output output in _object.GetOutputs())
                {
                    j = _object.GetOutputs().FindIndex(x => x == output);
                    if (!(output.isLinked())) //тут надо вставлять текст блоки
                    {
                        free.Add(i);
                        for (int k = 0; k < _object.matrix.GetLength(1); k++)
                        {
                            MatrixElement mx = _object.matrixElements.First(x => x.unique == k + number * j);
                            mx.rowIndex = i;
                            mx = _object.matrixElements.First(x => x.unique == number * k + j);
                            mx.columnIndex = i;
                        }

                        i++;
                    }
                    j++;
                }
            }
            foreach (Object _object in elements)
            {
                int number = _object.group;
                int j = 0;
                foreach (Output output in _object.GetOutputs())
                {
                    j = _object.GetOutputs().FindIndex(x => x == output);
                    if (output.isLinked()) //тут надо вставлять текст блоки
                    {
                        connected.Add(i);
                        for (int k = 0; k < _object.matrix.GetLength(1); k++)
                        {
                            MatrixElement mx = _object.matrixElements.Find(x => x.unique == k + number * j);
                            mx.rowIndex = i;
                            mx = _object.matrixElements.Find(x => x.unique == number * k + j);
                            mx.columnIndex = i;
                        }

                        i++;
                    }
                    j++;
                }
            }

            Complex[,] A = new Complex[i, i];
            Complex[,] aa = new Complex[free.Count, free.Count];
            Complex[,] ab = new Complex[free.Count, connected.Last()];
            Complex[,] ba = new Complex[connected.Last(), free.Count];
            Complex[,] bb = new Complex[connected.Last(), connected.Last()];
            for (int n = 0; n < i; n++)
            {
                for (int m = 0; m < i; m++)
                {
                    Object right = elements.Find(x => x.matrixElements.Any(y => y.rowIndex == n && y.columnIndex == m) == true);
                    if (right == null)
                    {
                        A[n, m] = 0;
                    }
                    else
                    {
                        MatrixElement mx = right.matrixElements.Find(x => x.rowIndex == n && x.columnIndex == m);
                        A[n, m] = mx.value;
                    }
                }
            }
            for (int n = 0; n < free.Count; n++)
            {
                for (int m = 0; m < free.Count; m++)
                {
                    aa[n, m] = A[n, m];
                    Console.WriteLine(aa[n, m]);
                }
            }
            int q = 0, w = 0;
            for (int n = 0; n < free.Count; n++)
            {
                for (int m = free.Count; m <= connected.Last(); m++)
                {
                    ab[n, q] = A[n, m];
                    Console.WriteLine(ab[n, q]);
                    q++;
                }
            }
            q = 0;
            for (int n = free.Count; n <= connected.Last(); n++)
            {
                for (int m = 0; m < free.Count; m++)
                {
                    ba[q, m] = A[n, m];
                    Console.WriteLine(ba[q, m]);
                }
                q++;
            }
            q = 0;
            for (int n = free.Count; n <= connected.Last(); n++)
            {
                w = 0;
                for (int m = free.Count; m <= connected.Last(); m++)
                {
                    bb[q, w] = A[n, m];
                    Console.WriteLine(bb[q, w]);
                    w++;
                }
                q++;
            }
        }

            //private Object Sum(Object x, Object y)
            //{
            //    Dictionary<int,Output> free = new Dictionary<int, Output>();
            //    Dictionary<int, Output> connected = new Dictionary<int, Output>();
            //    Dictionary<string, Complex> xMatrixElements = new Dictionary<string, Complex>();
            //    Dictionary<string, Complex> yMatrixElements = new Dictionary<string, Complex>();
            //    List<Output> xOutputs = x.GetOutputs();
            //    List<Output> yOutputs = y.GetOutputs();
            //    Complex[,] xMatrix = x.matrix;
            //    Complex[,] yMatrix = y.matrix;
            //    int i = 0;
            //    foreach (Output output in xOutputs)
            //    {
            //        if(!(output.isLinked()))
            //        {
            //            free.Add(i, output);
            //        }
            //        i++;
            //    }
            //    int j = i;
            //    foreach (Output output in yOutputs)
            //    {
            //        if (!(output.isLinked()))
            //        {
            //            free.Add(j, output);
            //        }
            //        j++;
            //    }
            //    foreach (Output output in xOutputs)
            //    {
            //        if (output.isLinked())
            //        {
            //            connected.Add(i, output);
            //        }
            //    }
            //    foreach (Output output in yOutputs)
            //    {
            //        if (output.isLinked())
            //        {
            //            connected.Add(i, output);
            //        }
            //    }
            //    for(int n =0; n < xMatrix.GetLength(0); n++)
            //    {
            //        for (int m = 0; m < xMatrix.GetLength(1); m++)
            //        {
            //            if(free.ContainsKey(i))
            //            {
            //                xMatrixElements.Add(i)
            //            }
            //        }
            //    }
            //    return y;
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


        private Element IntermediateValuesEvalNumerical(Element element, Entity expr, int i)
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
                matrix[i, j] = ((double)expr.EvalNumerical());
            }
            else
            {
                matrix[i, j] = complex;
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


