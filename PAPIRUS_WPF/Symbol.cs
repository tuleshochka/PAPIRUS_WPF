using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;

namespace PAPIRUS_WPF
{
    public abstract class Symbol : INotifyPropertyChanged
    {

        public const int PinRadius = 3;
        public const int GridSize = Symbol.PinRadius * 6;

        public static Rect LogicalCircuitBackgroundTile { get { return new Rect(0, 0, Symbol.GridSize, Symbol.GridSize); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected bool HasListener { get { return this.PropertyChanged != null; } }

    }
}
