using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAPIRUS_WPF
{
        public enum Rotation
        {
            Up,
            Right,
            Down,
            Left
        }

        public interface IRotatable
        {
            Rotation Rotation { get; set; }
        }
}
