using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace PAPIRUS_WPF
{

        public  class Marker:Symbol
        {
            public Symbol Symbol { get; private set; }
            public FrameworkElement Glyph { get; }

           public Marker(Symbol symbol)
            {
                this.Symbol = symbol;
            }

            //public virtual Rect Bounds()
           // {
                //return this.Symbol.Bounds();
            //}

            public void Move(FormEditor editor, Point point)
            {
                editor.MoveSelection(point);
            }

            public void CancelMove(Panel selectionLayer)
            {
            }

            //public virtual void Refresh();
        }
    }

