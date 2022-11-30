using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PAPIRUS_WPF
{
    internal class FormEditor
    {
        public double Zoom
        {
            get { return this.Table.GetField(this.ProjectRowId, ProjectData.ZoomField.Field); }
            set { this.Table.SetField(this.ProjectRowId, ProjectData.ZoomField.Field, value); }
        }

    }
}
