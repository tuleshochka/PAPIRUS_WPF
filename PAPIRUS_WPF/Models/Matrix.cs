﻿using AngouriMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PAPIRUS_WPF.Models
{
    public class Matrix
    {
        public Entity[,] data;

        private int m;
        public int M { get => this.m; }

        private int n;
        public int N { get => this.n; }

        public bool IsSquare { get => this.M == this.N; }

        private Entity precalculatedDeterminant = null;

        public Matrix(int m, int n)
        {
            this.m = m;
            this.n = n;
            this.data = new Entity[m, n];
        }

        public void ProcessFunctionOverData(Action<int, int> func)
        {
            for (var i = 0; i < this.M; i++)
            {
                for (var j = 0; j < this.N; j++)
                {
                    func(i, j);
                }
            }
        }

        public Matrix CreateTransposeMatrix()
        {
            var result = new Matrix(this.N, this.M);
            result.ProcessFunctionOverData((i, j) => result[i, j] = this[j, i]);
            return result;
        }

        public static Matrix CreateIdentityMatrix(int n)
        {
            var result = new Matrix(n, n);
            for (var i = 0; i < n; i++)
            {
                result[i, i] = 1;
            }
            return result;
        }

        public Entity this[int x, int y]
        {
            get
            {
                return this.data[x, y];
            }
            set
            {
                this.data[x, y] = value;
                this.precalculatedDeterminant = null;
            }
        }

        private Matrix CreateMatrixWithoutColumn(int column)
        {
            if (column < 0 || column >= this.N)
            {
                throw new ArgumentException("invalid column index");
            }
            var result = new Matrix(this.M, this.N - 1);
            result.ProcessFunctionOverData((i, j) =>
                result[i, j] = j < column ? this[i, j] : this[i, j + 1]);
            return result;
        }

        private Matrix CreateMatrixWithoutRow(int row)
        {
            if (row < 0 || row >= this.M)
            {
                throw new ArgumentException("invalid row index");
            }
            var result = new Matrix(this.M - 1, this.N);
            result.ProcessFunctionOverData((i, j) =>
                result[i, j] = i < row ? this[i, j] : this[i + 1, j]);
            return result;
        }

        public Entity CalculateDeterminant()
        {
            if (precalculatedDeterminant != null)
            {
                return this.precalculatedDeterminant;
            }
            if (!this.IsSquare)
            {
                throw new InvalidOperationException(
                    "determinant can be calculated only for square matrix");
            }
            if(this.N == 1)
            {
                return this[0, 0];
            }
            if (this.N == 2)
            {
                Entity entity = this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];
                if(entity.EvaluableNumerical)
                {
                    return (Complex)entity.EvalNumerical();
                }
                else
                {
                    return entity.Evaled;
                }
            }
            Entity result = 0;
            for (var j = 0; j < this.N; j++)
            {
                result += (j % 2 == 1 ? 1 : -1) * this[0, j] *
                    this.CreateMatrixWithoutColumn(j).
                    CreateMatrixWithoutRow(0).CalculateDeterminant();
            }
            if (result.EvaluableNumerical)
            {
                result =  (Complex)result.EvalNumerical();
            }
            else
            {
                result = result.Evaled;
            }
            this.precalculatedDeterminant = result;
            return result;
        }

        public Matrix CreateInvertibleMatrix()
        {
            if (this.M != this.N)
                return null;
            var determinant = CalculateDeterminant();
            //if (Math.Abs(determinant.Real) < Constants.DoubleComparisonDelta && Math.Abs(determinant.Imaginary) < Constants.DoubleComparisonDelta)
            //    return null;

            var result = new Matrix(M, M);
            ProcessFunctionOverData((i, j) =>
            {
                result[i, j] = ((i + j) % 2 == 1 ? -1 : 1) * CalculateMinor(i, j) / determinant;
            });
            for (int i = 0; i < result.M; i++)
            {
                for (int j = 0; j < result.N; j++)
                {
                    if (result[i, j].EvaluableNumerical)
                    {
                        result[i, j] = (Complex)result[i, j].EvalNumerical();
                    }
                    else
                    {
                        result[i, j] = result[i, j].Evaled;
                    }
                }
            }
            result = result.CreateTransposeMatrix();
            return result;
        }

        private Entity CalculateMinor(int i, int j)
        {
            return CreateMatrixWithoutColumn(j).CreateMatrixWithoutRow(i).CalculateDeterminant();
        }


        public static Matrix operator *(Matrix matrix, double value)
        {
            var result = new Matrix(matrix.M, matrix.N);
            result.ProcessFunctionOverData((i, j) =>
                result[i, j] = matrix[i, j] * value);
            for (int i = 0; i < result.M; i++)
            {
                for (int j = 0; j < result.N; j++)
                {
                    if (result[i, j].EvaluableNumerical)
                    {
                        result[i, j] = (Complex)result[i, j].EvalNumerical();
                    }
                    else
                    {
                        result[i, j] = result[i, j].Evaled;
                    }
                }
            }
            return result;
        }

        public static Matrix operator *(Matrix matrix, Matrix matrix2)
        {
            if (matrix.N != matrix2.M)
            {
                throw new ArgumentException("matrixes can not be multiplied");
            }
            var result = new Matrix(matrix.M, matrix2.N);
            result.ProcessFunctionOverData((i, j) => {
                for (var k = 0; k < matrix.N; k++)
                {
                    result[i, j] = (matrix[i, k] * matrix2[k, j]);
                    Console.WriteLine(result[i, j]);
                }
            });
            for (int i = 0; i < result.M; i++)
            {
                for (int j = 0; j < result.N; j++)
                {
                    if (result[i, j].EvaluableNumerical)
                    {
                        result[i, j] = (Complex)result[i, j].EvalNumerical();
                    }
                    else
                    {
                        result[i, j] = result[i, j].Evaled;
                    }
                }
            }
            return result;
        }

        public static Matrix operator +(Matrix matrix, Matrix matrix2)
        {
            if (matrix.M != matrix2.M || matrix.N != matrix2.N)
            {
                throw new ArgumentException("matrixes dimensions should be equal");
            }
            Matrix result = new Matrix(matrix.M, matrix.N);
            result.ProcessFunctionOverData((i, j) => result[i, j] = matrix[i, j] + matrix2[i, j]);
            for(int i = 0; i < result.M; i++)
            {
                for (int j = 0; j < result.N; j++)
                {
                    if (result[i,j].EvaluableNumerical)
                    {
                        result[i, j] = (Complex)result[i, j].EvalNumerical();
                    }
                    else
                    {
                        result[i, j] = result[i, j].Evaled;
                    }
                }
            }
            return result;
        }

        public static Matrix operator -(Matrix matrix, Matrix matrix2)
        {
            if (matrix.M != matrix2.M || matrix.N != matrix2.N)
            {
                throw new ArgumentException("matrixes dimensions should be equal");
            }
            Matrix result = new Matrix(matrix.M, matrix.N);
            result.ProcessFunctionOverData((i, j) => result[i, j] = matrix[i, j] - matrix2[i, j]);
            for (int i = 0; i < result.M; i++)
            {
                for (int j = 0; j < result.N; j++)
                {
                    if (result[i, j].EvaluableNumerical)
                    {
                        result[i, j] = (Complex)result[i, j].EvalNumerical();
                    }
                    else
                    {
                        result[i, j] = result[i, j].Evaled;
                    }
                }
            }
            return result;
        }
    }
}
