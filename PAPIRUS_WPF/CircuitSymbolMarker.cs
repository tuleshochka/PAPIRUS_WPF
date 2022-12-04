using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace PAPIRUS_WPF
{
    public class CircuitSymbolMarker: Marker
    {
        public Symbol Symbol { get; private set; }

        public override FrameworkElement Glyph { get; }

        public CircuitSymbolMarker(Symbol symbol):base(null)
        {
            this.Symbol = symbol;
        }

        public override void Refresh()
        {
            throw new NotImplementedException();
        }

        public override Rect Bounds()
        {
            throw new NotImplementedException();
        }

        public override void Shift(int dx, int dy)
        {
            throw new NotImplementedException();
        }

        public override void PositionGlyph()
        {
            throw new NotImplementedException();
        }
    }
}
