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
    public partial class multi_pole : Object
    {
        
        public override int group { get => base.group; set => base.group = value; }
        public override List<Output> listOfOutput { get; set; } = new List<Output>();
        public multi_pole()
        {
            BorderBrush = Brushes.Transparent;
            BorderThickness = new Thickness(1);
            InitializeComponent();
            LeftInput.outPos = 0;
            LeftInput2.outPos = 0;
            LeftInput3.outPos = 0;
            Buttom3.outPos = 3;
            Buttom.outPos = 3;
            Buttom2.outPos = 3;
            RightInput3.outPos = 1;
            RightInput2.outPos = 1;
            RightInput.outPos = 1;
            Top2.outPos = 2;
            Top.outPos = 2;
            Top3.outPos = 2;

            listOfOutput.Add(LeftInput);
            listOfOutput.Add(LeftInput2);
            listOfOutput.Add(LeftInput3);
            listOfOutput.Add(Buttom3);
            listOfOutput.Add(Buttom);
            listOfOutput.Add(Buttom2);
            listOfOutput.Add(RightInput3);
            listOfOutput.Add(RightInput2);
            listOfOutput.Add(RightInput);
            listOfOutput.Add(Top2);
            listOfOutput.Add(Top);
            listOfOutput.Add(Top3);
            group = 12;
        }

        private void EightPol_Loaded(object sender, RoutedEventArgs e)
        {
            if (DefaultNumberVisible == Visibility.Hidden)
            {
                foreach (TextBlock tb in utils.GetControls<TextBlock>(EightPol))
                {
                    tb.Visibility = Visibility.Hidden;
                }
            }
        }
    }
    static class utils
    {
        public static IEnumerable<T> GetControls<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }
                    foreach (T childOfChild in GetControls<T>(child))
                    {
                        yield return childOfChild;
                    }

                }

            }

        }
    }
}


