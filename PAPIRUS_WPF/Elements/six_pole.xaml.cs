﻿using System;
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
        public six_pole()
        {
            InitializeComponent();
            BorderBrush = Brushes.Transparent;
            BorderThickness = new Thickness(1);

            
        }

        private void SixPole_Loaded(object sender, RoutedEventArgs e)
        {
            if (CanMove == false)
            {
                foreach (TextBlock tb in utils.GetControls<TextBlock>(SixPole))
                {
                    tb.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
