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
    /// Логика взаимодействия для eight_pole.xaml
    /// </summary>
    public partial class sixteen_pole : Object
    {
        public override int group { get => base.group; set => base.group = value; }
        public override List<Output> listOfOutput { get; set; } = new List<Output>();

        public sixteen_pole()
        {
            BorderBrush = Brushes.Transparent;
            BorderThickness = new Thickness(1);
            InitializeComponent();
            listOfOutput.Add(LeftInput);
            listOfOutput.Add(LeftInput3);
            listOfOutput.Add(Buttom3);
            listOfOutput.Add(Buttom2);
            listOfOutput.Add(RightInput3);
            listOfOutput.Add(RightInput);
            listOfOutput.Add(Top2);
            listOfOutput.Add(Top3);
            group = 8;
        }

        private void EightPol_Loaded(object sender, RoutedEventArgs e)
        {
            if (DefaultNumberVisible == false)
            {
                foreach (TextBlock tb in utils.GetControls<TextBlock>(EightPol))
                {
                    tb.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
