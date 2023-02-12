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
           
            for (int i = 0; i < 10; i++)
            {

                Data.frec.Add(new ObservablePoint(i, -i));

            }

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "АЧХ",
                    Stroke= Brushes.Red,
                    Fill = Brushes.Transparent,
                    Values = Data.frec
                  },
                     new LineSeries
                 {
                        Title = "ФЧХ",
                        Stroke= Brushes.Blue,
                    Fill = Brushes.Transparent,
                    Values = Data.phase
                }

            };
            DataContext = this;

        }
    }
}
