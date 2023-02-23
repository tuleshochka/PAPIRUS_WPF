using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PAPIRUS_WPF.Elements
{
    /// <summary>
    /// Логика взаимодействия для six_pole.xaml
    /// </summary>
    public partial class six_pole : Object
    {
        
        public override List<Output> listOfOutput { get; set; } = new List<Output>();
        public override int group { get => base.group; set => base.group = value; }
        public six_pole()
        {
            InitializeComponent();
            BorderBrush = Brushes.Transparent;
            BorderThickness = new Thickness(1);
            BorderBrush = Brushes.Transparent;
            BorderThickness = new Thickness(1);
            InitializeComponent();
            LeftInput.outPos = 0;
            RightInput.outPos = 1;
            Top.outPos = 2;
            listOfOutput.Add(LeftInput);
            listOfOutput.Add(RightInput);
            listOfOutput.Add(Top);
            group = 3;
        }

        private void SixPole_Loaded(object sender, RoutedEventArgs e)
        {
            if (DefaultNumberVisible == Visibility.Hidden)
            {
                foreach (TextBlock tb in Data.GetControls<TextBlock>(EightPol))
                {
                    tb.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
