
using PAPIRUS_WPF.Elements;
using PAPIRUS_WPF.Wrappers;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Windows.Forms.LinkLabel;

namespace PAPIRUS_WPF.Models
{
    public class Wire
    {
        private Line line;
        public Line WireGlyph
        {
            get { return line ?? (line = this.CreateGlyph()); }
        }

        public Line CreateGlyph()
        {
            Line line = new Line
            {
                Stroke = Symbol.WireStroke,
                StrokeThickness = 1,
                DataContext = this
            };
            return line;
        }


        public FrameworkElement Glyph { get { return this.WireGlyph; } }
        public bool HasCreatedGlyph { get { return this.line != null; } }


        public Wire()
        {
            line = new Line();
        }

        public GridPoint Point1
        {
            get { return new GridPoint(this.X1, this.Y1); }
            set { this.X1 = value.X; this.Y1 = value.Y; }
        }

        public GridPoint Point2
        {
            get { return new GridPoint(this.X2, this.Y2); }
            set { this.X2 = value.X; this.Y2 = value.Y; }
        }

        public int X1
        {
            get
            {
                return (int)line.X1;
            }
            set
            {
                line.X1 = value;
            }
        }
        public int Y1
        {
            get
            {
                return (int)line.Y1;
            }
            set
            {
                line.Y1 = value;
            }
        }

        public int X2
        {
            get
            {
                return (int)line.X2;
            }
            set
            {
                line.X2 = value;
            }
        }

        public int Y2
        {
            get
            {
                return (int)line.Y2;
            }
            set
            {
                line.Y2 = value;
            }
        }
    }
}
