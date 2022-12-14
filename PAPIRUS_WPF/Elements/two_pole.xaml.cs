using PAPIRUS_WPF.Models;
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
    /// Логика взаимодействия для two_pole.xaml
    /// </summary>
    public partial class two_pole : Object
    {
        public two_pole()
        {
            BorderBrush = Brushes.Transparent;
            BorderThickness = new Thickness(2);
            InitializeComponent();
            LeftInput.StateChanged += Two_PoleStateChanged;
        }

        private void Two_PoleStateChanged()
        {
            //Reset the internal state
            bool StateSet = false;
            LeftInput.State = false;

            //Check if any Input is high. If so, set state to high
            foreach (UIElement e in TwoPol.Children)
            {
                if (e is Output)
                {
                    Output IO = (Output)e;
                    if (IO.State)
                    {
                        StateSet = true;
                        LeftInput.State = true;
                        break;
                    }
                }
            }

            //Color the LED based on the internal state
            if (StateSet)
            {
                //LEDRect.Fill = new SolidColorBrush(Colors.Red); 
            }
            else
            {
                // LEDRect.Fill = new SolidColorBrush(Colors.Black); 
            }
        }

      
    }
}
