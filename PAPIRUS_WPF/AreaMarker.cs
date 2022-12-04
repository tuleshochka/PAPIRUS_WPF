using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;

namespace PAPIRUS_WPF
{
    public class AreaMarker:Marker
    {
        private readonly Point point0;
        private readonly Rectangle markerGlyph;

        public override FrameworkElement Glyph { get { return this.markerGlyph; } }

        public AreaMarker(Point point) : base(null)
        {
            this.point0 = point;
            this.markerGlyph = Symbol.Skin<Rectangle>(SymbolShape.SelectionMarker);
            this.markerGlyph.DataContext = this;
            this.PositionGlyph(point);
        }

        public override Rect Bounds()
        {
            return new Rect(Canvas.GetLeft(this.markerGlyph), Canvas.GetTop(this.markerGlyph), this.markerGlyph.Width, this.markerGlyph.Height);
        }

        public override void Move(MainWindow editor, Point point)
        {
            this.PositionGlyph(point);
        }

        public override void Commit(MainWindow editor, Point point, bool withWires)
        {
            //editor.Select(new Rect(this.point0, point));
        }

        private void PositionGlyph(Point point)
        {
            Rect rect = new Rect(this.point0, point);
            Canvas.SetLeft(this.markerGlyph, rect.X);
            Canvas.SetTop(this.markerGlyph, rect.Y);
            this.markerGlyph.Width = rect.Width;
            this.markerGlyph.Height = rect.Height;
        }

        public override void CancelMove(Panel selectionLayer)
        {
            selectionLayer.Children.Remove(this.Glyph);
        }

        public override void Refresh()
        {
            throw new InvalidOperationException();
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
