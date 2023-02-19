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
using System.Runtime.CompilerServices;
using System.Numerics;
using System.Xml.Linq;
using System.Windows.Documents;
using AngouriMath.Extensions;
using PAPIRUS_WPF.Elements;
using static AngouriMath.Entity;
using System.Windows.Controls;
using Matrix = PAPIRUS_WPF.Models.Matrix;
using TextBox = System.Windows.Controls.TextBox;
using System.Windows.Input;
using Cursors = System.Windows.Input.Cursors;
using System.Windows.Media;
using System.Drawing;
using Point = System.Windows.Point;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace PAPIRUS_WPF
{
    public class SMatrixCalculation
    {

        //размерность общей матрицы зависит от количества свободных входов, 2 - 2x2, 5 - 5х5
        // матрица соединений составляется из соединенных плеч
        // матрица делится на блоки aa (свободные плечи) и bb (соединенные плечи)

        public Matrix CalculateTotal(List<Object> elements) // расчет общей S-матрицы
        {
            var window = System.Windows.Application.Current.MainWindow;
            List<int> free = new List<int>();
            List<int> connected = new List<int>();
            int i = 0;

            if (elements.Count == 0)
            {
                throw new Exception("Нет элементов на схеме");
            }

            foreach (Object _object in elements)
            {
                try
                {
                    _object.FillME();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                int number = _object.group;
                int j = 0;
                foreach (Output output in _object.GetOutputs())
                {
                    j = _object.GetOutputs().FindIndex(x => x == output);
                    if (!(output.isLinked()))
                    {

                        Point p = (window as MainWindow).CircuitCanvas.TranslatePoint(new Point(0, 0), output);
                        p.Y = p.Y - output.Height / 2;
                        TextBox text = new TextBox();

                        text.BorderThickness = new Thickness(0);
                        text.IsReadOnly = true;
                        text.Cursor = Cursors.Arrow;
                        text.Focusable = false;
                        text.Background = Brushes.Transparent;
                        text.Foreground = Brushes.Blue;
                        text.Text = i + 1.ToString();
                        (window as MainWindow).CircuitCanvas.Children.Add(text);
                        Canvas.SetLeft(text, Math.Abs(p.X));
                        Canvas.SetTop(text, Math.Abs(p.Y));
                        text.Margin = new Thickness(-20, 0, 0, 0);
                        Data.outputNumber.Add(text);

                        free.Add(i);
                        output.index = i;
                        for (int k = 0; k < _object.matrix.N; k++)
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
                        (window as MainWindow).CircuitCanvas.TranslatePoint(new Point(0, 0), output);
                        output.index = i;
                        for (int k = 0; k < _object.matrix.N; k++)
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

            Entity[,] A = new Entity[i, i];
            Matrix AA = new Matrix(free.Count, free.Count);
            Matrix AB = new Matrix(free.Count, connected.Count);
            Matrix BA = new Matrix(connected.Count, free.Count);
            Matrix BB = new Matrix(connected.Count, connected.Count);
            Matrix totalMatrix = new Matrix(free.Count, free.Count);
            Console.WriteLine("matrix A: ");
            for (int n = 0; n < i; n++)
            {
                for (int m = 0; m < i; m++)
                {
                    Object right = elements.Find(x => x.matrixElements.Any(y => y.rowIndex == n && y.columnIndex == m) == true);
                    if (right == null)
                    {
                        A[n, m] = 0;
                        Console.Write(A[n, m]);
                    }
                    else
                    {
                        MatrixElement mx = right.matrixElements.Find(x => x.rowIndex == n && x.columnIndex == m);
                        A[n, m] = mx.value;
                        Console.Write(A[n, m]);
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("AA matrix:");
            for (int n = 0; n < free.Count; n++)
            {
                for (int m = 0; m < free.Count; m++)
                {
                    AA[n, m] = A[n, m];
                    Console.WriteLine(AA[n, m]);
                }
            }
            int q = 0, w = 0;
            Console.WriteLine("AB matrix:");
            if (connected.Count > 0)
            {
                for (int n = 0; n < free.Count; n++)
                {
                    for (int m = free.Count; m <= connected.Last(); m++)
                    {
                        AB[n, q] = A[n, m];
                        Console.Write(AB[n, q]);
                        q++;
                    }
                    Console.WriteLine();
                    q = 0;
                }
                Console.WriteLine("BA matrix:");
                for (int n = free.Count; n <= connected.Last(); n++)
                {
                    for (int m = 0; m < free.Count; m++)
                    {
                        BA[q, m] = A[n, m];
                        Console.Write(BA[q, m]);
                    }
                    Console.WriteLine();
                    q++;
                }
                q = 0;
                Console.WriteLine("BB matrix:");
                for (int n = free.Count; n <= connected.Last(); n++)
                {
                    w = 0;
                    for (int m = free.Count; m <= connected.Last(); m++)
                    {
                        BB[q, w] = A[n, m];
                        Console.Write(BB[q, w]);
                        w++;
                    }
                    Console.WriteLine();
                    q++;
                }
                Matrix EMatrix = new Matrix(connected.Count, connected.Count);
                q = connected.First();
                w = connected.First();
                Console.WriteLine("E matrix:");
                for (int n = 0; n < connected.Count; n++)
                {
                    w = connected.First();
                    for (int m = 0; m < connected.Count; m++)
                    {
                        Object _ = elements.Find(x => x.GetOutputs().Any(y => y.index == q && y._state_.index == w));
                        if (_ == null)
                        {
                            EMatrix[n, m] = 0;
                            Console.Write(EMatrix[n, m]);
                        }
                        else
                        {
                            EMatrix[n, m] = 1;
                            Console.Write(EMatrix[n, m]);
                        }
                        w++;
                    }
                    Console.WriteLine();
                    q++;
                }
                Console.WriteLine("a:");

                Matrix a = (EMatrix - BB);
                a.ProcessFunctionOverData((i, j) => Console.WriteLine(a[i, j]));

                Console.WriteLine("b:");

                Matrix b = (EMatrix - BB).CreateInvertibleMatrix();
                b.ProcessFunctionOverData((i, j) => Console.WriteLine(b[i, j]));

                Console.WriteLine("c:");
                Matrix c = AB * ((EMatrix - BB).CreateInvertibleMatrix());
                c.ProcessFunctionOverData((i, j) => Console.WriteLine(c[i, j]));

                Console.WriteLine("d:");
                Matrix d = AB * ((EMatrix - BB).CreateInvertibleMatrix()) * BA;
                d.ProcessFunctionOverData((i, j) => Console.WriteLine(d[i, j]));
                Console.WriteLine("total:");
                totalMatrix = new Matrix(free.Count, free.Count);
                totalMatrix = AA + AB * ((EMatrix - BB).CreateInvertibleMatrix()) * BA;
            }
            else
            {
                totalMatrix = AA;
            }

            for (int x = 0; x < free.Count; x++)
            {

                for (int y = 0; y < free.Count; y++)
                {
                    Console.Write(totalMatrix[x, y]);
                }
                Console.WriteLine();
            }
            return totalMatrix;
        }

        Dictionary<string, string> operators = new Dictionary<string, string>()
        {
            {"+-","-"},
            {"--","+"},
            {"+ -","-"},
            {"- -","+"},
        };

        public Matrix Calculate(Element element, List<DataGridElements> dataGridElements)
        {

            Element tempElement = (Element)element.Clone();
            int number = element.group;
            Matrix matrix = new Matrix(number, number);
            if (tempElement.other_par.Count() != 0)
            {
                try
                {
                    tempElement = CalculateIntermediateValues_params(tempElement, dataGridElements);
                    tempElement = CalculateIntermediateValues_cycle(tempElement);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            try
            {
                matrix = CalculateMatrix(tempElement, dataGridElements);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            for (int i = 0; i < matrix.M; i++)
            {
                for (int j = 0; j < matrix.N; j++)
                {
                    Console.WriteLine("matrix " + i + "," + j + " = " + matrix[i, j].ToString());
                }
            }
            return matrix;
        }

        private Element CalculateIntermediateValues_params(Element element, List<DataGridElements> dataGridElements)
        {
            for (int i = 0; i < element.other_par.Count(); i++)
            {
                string temp = null;
                try
                {
                    temp = OptimizeBeginString(element.other_par[i].formulaColumn);
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
                        if (temp.Contains("f"))
                        {
                            if (!(element.parameters.Any(x => temp.Contains(x.paramColumn))) && !(element.other_par.Any(x => temp.Contains(x.headerColumn))))
                            {
                                try
                                {
                                    element = IntermediateValuesEvalNumerical(element, expr, i);
                                }
                                catch (Exception e)
                                {
                                    throw new Exception(e.Message);
                                }
                                break;
                            }
                        }
                        else if (expr.EvaluableNumerical)
                        {
                            try
                            {
                                element = IntermediateValuesEvalNumerical(element, expr, i);
                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.Message);
                            }
                            break;
                        }

                        element.other_par[i].formulaColumn = temp;
                    }
                }
            }
            return element;
        }

        private Element CalculateIntermediateValues_cycle(Element element)
        {
            for (int i = 0; i < element.other_par.Count(); i++)
            {
                if (element.parameters.Any(y => element.other_par[i].formulaColumn.Contains(y.paramColumn)) || (element.other_par.Any(y => element.other_par[i].formulaColumn.Contains(y.headerColumn))))
                {
                    foreach (var value in element.other_par.Where(el => el != element.other_par[i]))
                    {
                        string temp = OptimizeFinalString(element.other_par[i].formulaColumn.Replace(value.headerColumn, value.formulaColumn));
                        Entity expr = temp;
                        if (temp.Contains("f"))
                        {
                            if (!(element.parameters.Any(x => temp.Contains(x.paramColumn))) && !(element.other_par.Any(x => temp.Contains(x.headerColumn))))
                            {
                                try
                                {
                                    element = IntermediateValuesEvalNumerical(element, expr, i);
                                }
                                catch (Exception e)
                                {
                                    throw new Exception(e.Message);
                                }
                                break;
                            }
                        }
                        else if (expr.EvaluableNumerical)
                        {
                            try
                            {
                                element = IntermediateValuesEvalNumerical(element, expr, i);
                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.Message);
                            }
                            break;
                        }

                        element.other_par[i].formulaColumn = temp;

                    }
                }
            }

            if (element.other_par.Exists(x => element.parameters.Any(y => x.formulaColumn.Contains(y.paramColumn))))
            {
                CalculateIntermediateValues_cycle(element);
            }
            return element;
        }

        private Matrix CalculateMatrix(Element element, List<DataGridElements> dataGridElements)
        {
            int number = element.group;
            Matrix matrix = new Matrix(number, number);
            int a = 0;
            for (int i = 0; i < number; i++)
            {
                for (int j = 0; j < number; j++)
                {
                    string temp = null;
                    try
                    {
                        temp = OptimizeBeginString(element.matrix[a].element);
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
                            if (temp.Contains("f"))
                            {
                                if (!(element.parameters.Any(x => temp.Contains(x.paramColumn))) && !(element.other_par.Any(x => temp.Contains(x.headerColumn))))
                                {
                                    try
                                    {
                                        matrix = MatrixCellEvalNumerical(matrix, expr, i, j);
                                    }
                                    catch (Exception e)
                                    {
                                        throw new Exception(e.Message);
                                    }
                                    break;
                                }
                            }

                            else if (expr.EvaluableNumerical)
                            {
                                try
                                {
                                    matrix = MatrixCellEvalNumerical(matrix, expr, i, j);
                                }
                                catch (Exception e)
                                {
                                    throw new Exception(e.Message);
                                }
                                break;
                            }
                            element.matrix[a].element = temp;
                        }
                    }
                    if (element.other_par.Count() != 0)
                    {
                        for (int k = 0; k < element.other_par.Count(); k++)
                        {
                            temp = OptimizeFinalString(temp.Replace(element.other_par[k].headerColumn, element.other_par[k].formulaColumn));
                            Entity expr = temp;
                            if (temp.Contains("f"))
                            {
                                if (!(element.parameters.Any(x => temp.Contains(x.paramColumn))) && !(element.other_par.Any(x => temp.Contains(x.headerColumn))))
                                {
                                    try
                                    {
                                        matrix = MatrixCellEvalNumerical(matrix, expr, i, j);
                                    }
                                    catch (Exception e)
                                    {
                                        throw new Exception(e.Message);
                                    }
                                    break;
                                }
                            }

                            else if (expr.EvaluableNumerical)
                            {
                                try
                                {
                                    matrix = MatrixCellEvalNumerical(matrix, expr, i, j);
                                }
                                catch (Exception e)
                                {
                                    throw new Exception(e.Message);
                                }
                                break;
                            }
                            element.matrix[a].element = temp;

                        }
                    }
                    a++;
                }
            }
            return matrix;
        }




        private Element IntermediateValuesEvalNumerical(Element element, Entity expr, int i)
        {
            if (expr.EvaluableNumerical)
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
            }
            else
            {
                Entity entity = expr.Evaled;
                element.other_par[i].formulaColumn = entity.ToString();
            }
            return element;
        }

        private Matrix MatrixCellEvalNumerical(Matrix matrix, Entity expr, int i, int j)  //для правильного отображения расчетов
        {
            if (expr.EvaluableNumerical)
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
            }
            else
            {
                Entity entity = expr.Evaled;
                matrix[i, j] = entity;
            }
            Console.WriteLine("element i = " + matrix[i, j].ToString());
            return matrix;
        }

        private string OptimizeBeginString(string originalString)
        {

            string temp = originalString.Replace(" ", "");
            temp = temp.Replace("w", "2*pi*f");
            temp = temp.Replace("ω", "2*pi*f");
            return temp;

            //string temp = originalString.Replace(" ", "");
            //if (isGeneretorConnected)
            //{
            //    temp = temp.Replace("w", "2*pi*f");
            //    temp = temp.Replace("f", (Data.specificFrequency.ToString()).Replace(",", "."));
            //}
            //else if (!isGeneretorConnected && (temp.Contains("w") || temp.Contains("f")))
            //{
            //    throw new Exception("Элемент не подключен к генератору");
            //}
            //return temp;

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


