using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts.Defaults;
using Microsoft.SqlServer.Server;

namespace PAPIRUS_WPF.Dialog
{
    /// <summary>
    /// Логика взаимодействия для Calculation.xaml
    /// </summary>
    public partial class Graphic : Window
    {
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public Graphic()
        {
            InitializeComponent();

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "АЧХ",
                    Stroke= Brushes.Red,
                    Fill = Brushes.Transparent,
                    Values = new ChartValues<ObservablePoint>
                           {
                       new ObservablePoint(x:0,y:10),
                       new ObservablePoint(x:2,y:11),
                       new ObservablePoint(x:3,y:12),
                       new ObservablePoint(x:4,y:13),
                       new ObservablePoint(x:6,y:14)
                    }
                  },
                     new LineSeries
                 {
                        Title = "ФЧХ",
                        Stroke= Brushes.Blue,
                    Fill = Brushes.Transparent,
                    Values = new ChartValues<ObservablePoint>
                    {
                       new ObservablePoint(x:10,y:10),
                       new ObservablePoint(x:8,y:11),
                       new ObservablePoint(x:6,y:12),
                       new ObservablePoint(x:4,y:13),
                       new ObservablePoint(x:2,y:14)
                    }
                }

            };
            DataContext = this;

        }
    }
}
