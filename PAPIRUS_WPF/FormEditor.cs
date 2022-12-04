using PAPIRUS_WPF.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PAPIRUS_WPF
{
    public class FormEditor
    {

        private const int ClickProximity = 2 * Symbol.PinRadius;

        public MainWindow Mainframe { get; private set; }
        private Dispatcher Dispatcher { get { return this.Mainframe.Dispatcher; } }
        protected Canvas Diagram { get { return this.Mainframe.CircuitCanvas; } }

        // public CircuitProject CircuitProject { get; private set; }
        // public Project Project { get { return this.CircuitProject.ProjectSet.Project; } }

        private bool refreshPending;
        //private readonly WireValidator wireValidator;

        //public abstract bool InEditMode { get; }

        //private LogicalCircuit currentLogicalCircuit;
        private readonly Dictionary<GridPoint, Ellipse> wireJunctionPoint = new Dictionary<GridPoint, Ellipse>();

        private readonly Dictionary<Symbol, Marker> selection = new Dictionary<Symbol, Marker>();
        private Canvas selectionLayer;

        private Marker movingMarker;
        private Point moveStart;
        private Point panStart;
        private bool panning;
        private TranslateTransform moveVector;
        private Point maxMove;


        public double Zoom()
        {

            MainWindow main = new MainWindow();
            // MessageBox.Show(main.zoom.Value.ToString());
            return main.zoom.Value;
        }

      

        //---------Selection--------------//

        public int SelectionCount { get { return this.selection.Count; } }

        public IEnumerable<Symbol> SelectedSymbols { get { return this.selection.Keys; } }

        public IEnumerable<Symbol> Selection()
        {
            return new List<Symbol>(this.selection.Keys);
        }

        public void ClearSelection()
        {
            this.selection.Clear();
            if (this.selectionLayer != null)
            {
                this.selectionLayer.Children.Clear();
            }
        }

       /* private static Marker CreateMarker(Symbol symbol)
        {
            if (symbol is CircuitSymbol circuitSymbol)
            {
                if (circuitSymbol.Circuit is CircuitButton)
                {
                    return new ButtonMarker(circuitSymbol);
                }
                else
                {
                    return new CircuitSymbolMarker(circuitSymbol);
                }
            }
            if (symbol is Wire wire)
            {
                return new WireMarker(wire);
            }
            if (symbol is TextNote textNote)
            {
                return new TextNoteMarker(textNote);
            }
            throw new InvalidOperationException();
        }*/

        private Marker FindMarker(Symbol symbol)
        {
            if (this.selection.TryGetValue(symbol, out Marker marker))
            {
                return marker;
            }
            return null;
        }

        private void AddMarkerGlyph(Marker marker)
        {
            if (this.selectionLayer == null)
            {
                this.selectionLayer = new Canvas()
                {
                    RenderTransform = this.moveVector = new TranslateTransform()
                };
                Panel.SetZIndex(this.selectionLayer, int.MaxValue);
            }
            if (this.selectionLayer.Parent != this.Diagram)
            {
                this.Diagram.Children.Add(this.selectionLayer);
            }
            this.selectionLayer.Children.Add(marker.Glyph);
        }

        /*
        private Marker SelectSymbol(Symbol symbol)
        {
            // Tracer.Assert(symbol.LogicalCircuit == this.Project.LogicalCircuit);
            Marker marker = this.FindMarker(symbol);
            if (marker == null)
            {
                marker = FormEditor.CreateMarker(symbol);
                this.selection.Add(symbol, marker);
                this.AddMarkerGlyph(marker);
            }
            return marker;
        }*/

        public void MoveSelection(Point point)
        {
            this.moveVector.X = point.X - this.moveStart.X;
            this.moveVector.Y = point.Y - this.moveStart.Y;
        }

    }
}
