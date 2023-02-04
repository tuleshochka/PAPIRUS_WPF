using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_SHF_Element_lib;

namespace PAPIRUS_WPF.Models
{
    public class Element
    {
        public string imagePath { get; set; }
        public int group { get; set; }
        public string name { get; set; }
        public List<DataGrid1_Parameters> parameters { get; set; }
        public List<DataGrid1_Elements> other_par { get; set; }
        public List<MatrixElements> matrix { get; set; }

    }
}
