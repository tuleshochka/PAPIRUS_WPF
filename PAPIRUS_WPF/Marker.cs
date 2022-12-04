using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace PAPIRUS_WPF
{

        public  abstract class Marker:Symbol
        {
            public Symbol Symbol { get; private set; }
            public abstract FrameworkElement Glyph { get; }

           public Marker(Symbol symbol)
            {
                this.Symbol = symbol;
            }

            public override Rect Bounds()
            {
                return this.Symbol.Bounds();
            }

            public virtual void Move(MainWindow editor, Point point)
            {
                editor.MoveSelection(point);
            }

            public virtual void CancelMove(Panel selectionLayer)
            {
            }

        public virtual void Commit(MainWindow editor, Point point, bool withWires)
        {
            editor.CommitMove(point, withWires);
        }
        public abstract void Refresh();

        //public virtual void Refresh();
    }
    }

