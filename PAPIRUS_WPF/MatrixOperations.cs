using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PAPIRUS_WPF
{
    public static class MatrixOperations
    {
        public static bool IsSquare (Complex[,] matrix) 
        {
            int N = matrix.GetLength(0);
            int M = matrix.GetLength(1);
            return N == M;
        }
        public static Complex CreateInvertibleMatrix(Complex[,] matrix)
        {
            int M = matrix.GetLength(0);
            int N = matrix.GetLength(1);
            if (M != N)
                throw new Exception("Матрица не квадратная");
            var determinant = CalculateDeterminant(matrix);
            if (Math.Abs(determinant) < Constants.DoubleComparisonDelta)
                return null;

            var result = new Matrix(M, M);
            ProcessFunctionOverData((i, j) =>
            {
                result[i, j] = ((i + j) % 2 == 1 ? -1 : 1) *
                    CalculateMinor(i, j) / determinant;
            });

            result = result.CreateTransposeMatrix();
            return result;
        }

        public static double CalculateDeterminant(Complex[,] matrix)
        {
            if (!IsSquare(matrix))
            {
                throw new InvalidOperationException(
                    "determinant can be calculated only for square matrix");
            }
            if (this.N == 2)
            {
                return this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];
            }
            double result = 0;
            for (var j = 0; j < this.N; j++)
            {
                result += (j % 2 == 1 ? 1 : -1) * this[1, j] *
                    this.CreateMatrixWithoutColumn(j).
                    CreateMatrixWithoutRow(1).CalculateDeterminant();
            }
            return result;
        }
    }
}
