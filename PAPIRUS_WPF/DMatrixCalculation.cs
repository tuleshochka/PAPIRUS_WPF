using PAPIRUS_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAPIRUS_WPF
{
    public class DMatrixCalculation
    {
        public Matrix Calculate(Matrix matrix)
        {
            Matrix DMatrix = new Matrix(matrix.M, matrix.N);
            Matrix one = new Matrix(matrix.M, matrix.N);
            one.ProcessFunctionOverData((i, j) => one[i, j] = i == j ? 1 : 0);
            DMatrix = one - matrix * Data.reflectionCoef;
            DMatrix.ProcessFunctionOverData((i, j) => Console.WriteLine(DMatrix[i, j]));
            return DMatrix;
        }
    }
}
