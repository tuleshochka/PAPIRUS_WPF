using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAPIRUS_WPF
{
    public interface PowerObject
    {
        /// <summary>
        /// The state of the current power object
        /// </summary>
        bool State { get; set; }
    }
}
