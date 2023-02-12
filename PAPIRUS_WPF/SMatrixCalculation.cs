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

        public void CalculateTotal(List<Object> elements) // расчет общей S-матрицы
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
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                    return;
                }
                int number = _object.group;
                int j = 0;
                foreach (Output output in _object.GetOutputs())
                {
                    j = _object.GetOutputs().FindIndex(x => x == output);
                    if (!(output.isLinked())) //тут надо вставлять текст блоки  
                    {
                        
                        Point p = (window as MainWindow).CircuitCanvas.TranslatePoint(new Point(0, 0), output);
                        p.Y = p.Y - output.Height / 2;
                        TextBox text = new TextBox();
                        text.BorderThickness = new Thickness(0);
                        text.IsReadOnly = true;
                        text.Cursor = Cursors.Arrow;
                        text.Focusable = false;
                        text.Background = Brushes.Transparent;
                        text.Text = i + 1.ToString();
                        (window as MainWindow).CircuitCanvas.Children.Add(text);
                        Canvas.SetLeft(text, Math.Abs(p.X));
                        Canvas.SetTop(text, Math.Abs(p.Y));
                        text.Margin = new Thickness(-20, 0, 0, 0);
                        
                        free.Add(i);
                        output.index = i;
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
                        (window as MainWindow).CircuitCanvas.TranslatePoint(new Point(0, 0),output);
                        output.index = i;
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
            Matrix AA = new Matrix(free.Count, free.Count);
            Matrix AB = new Matrix(free.Count, connected.Count);
            Matrix BA = new Matrix(connected.Count, free.Count);
            Matrix BB = new Matrix(connected.Count, connected.Count);
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
                    Console.Write(AA[n, m]);
                }
                Console.WriteLine();
            }
            int q = 0, w = 0;
            Console.WriteLine("AB matrix:");
            if (connected.Count >0)
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
            Matrix totalMatrix = new Matrix(free.Count, free.Count);
            //totalMatrix = AA + AB * ((EMatrix-BB).CreateInvertibleMatrix()) * BA;
            Matrix _0 = EMatrix - BB;
            Console.WriteLine("0");
            for (int x = 0; x < _0.M; x++)
            {
                for (int y = 0; y < _0.N; y++)
                {
                    Console.Write(_0[x,y]);
                }
                Console.WriteLine();
            }
            Matrix _1 = _0.CreateInvertibleMatrix();
            Console.WriteLine("1");
            for (int x = 0; x < _1.M; x++)
            {

                for (int y = 0; y < _1.N; y++)
                {
                    Console.Write(_1[x, y]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("2");
            Matrix _2 = AB * ((EMatrix - BB).CreateInvertibleMatrix());
            for (int x = 0; x < _2.M; x++)
            {

                for (int y = 0; y < _2.N; y++)
                {
                    Console.Write(_2[x, y]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("3");
            Matrix _3 = AB * ((EMatrix - BB).CreateInvertibleMatrix()) * BA;
            for (int x = 0; x < _3.M; x++)
            {

                for (int y = 0; y < _3.N; y++)
                {
                    Console.Write(_3[x, y]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("4");
            totalMatrix = AA + AB * ((EMatrix - BB).CreateInvertibleMatrix()) * BA;
            for (int x = 0; x < free.Count; x++)
            {

                for (int y = 0; y < free.Count; y++)
                {
                    Console.Write(totalMatrix[x, y]);
                }
                Console.WriteLine();
            }
        }

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


