using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WPF_SHF_Element_lib.Models;

namespace PAPIRUS_WPF.Models
{
    public class Element /*: ICloneable*/
    {
        public string imagePath { get; set; }
        public int group { get; set; }
        public string name { get; set; }
        public List<DataGrid1_Parameters> parameters { get; set; }
        public List<DataGrid1_Elements> other_par { get; set; }
        public List<MatrixElements> matrix { get; set; }

        public Element(string imagePath, int group, string name, List<DataGrid1_Parameters> parameters, List<DataGrid1_Elements> other_par, List<MatrixElements> matrix)
        {
            this.imagePath = imagePath;
            this.group = group;
            this.name = name;
            this.parameters = parameters;
            this.other_par = other_par;
            this.matrix = matrix;
        }

        public Element()
        {
            parameters = new CloneableList<DataGrid1_Parameters>();
            other_par = new CloneableList<DataGrid1_Elements>();
            matrix = new CloneableList<MatrixElements>();
        }

        //public object Clone()
        //{
        //    DataGrid1_Parameters[] par = new DataGrid1_Parameters[parameters.Count];
        //    parameters.CopyTo(par);
        //    DataGrid1_Elements[] el = new DataGrid1_Elements[other_par.Count];
        //    other_par.CopyTo(el);
        //    MatrixElements[] m = new MatrixElements[matrix.Count];
        //    matrix.CopyTo(m);
        //    return new Element(imagePath, group,name, parameters.Clone(), other_par.Clone(), matrix.Clone());
        //}
    }
}
