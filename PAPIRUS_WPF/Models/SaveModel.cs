using PAPIRUS_WPF.Dialog;
using PAPIRUS_WPF.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace PAPIRUS_WPF.Models
{
    public class SaveModel
    {
        public string name { get; set; }
        public int group { get; set; }
        public string model { get; set; }
        public Point coordinates { get; set; }
        public string insideElement { get; set; }
        public bool generatorConnected { get; set; }
        public List<string> matrix { get; set; }


        //------сохраненные параметры-------------//
        public List<PoleInsideElementsAndParams_Save> insideParams { get; set; }

        public List<SaveOutput> listOfOutput { get; set; }

        public SaveModel()
        {
            insideParams = new List<PoleInsideElementsAndParams_Save>();
            listOfOutput = new List<SaveOutput>();
            matrix = new List<string>();
        }
    }

    public class PoleInsideElementsAndParams_Save
    {
        public string element { get; set; }
        public List<DataGridElements> parameters { get; set; }
    }

    public class SaveOutput
    {
        public int stateId { get; set; }
        public int id { get; set; }
        public int outPos { get; set; }  // 0 - Left, 1 - Right, 2 - Top, 3 - Bottom
    }

}
