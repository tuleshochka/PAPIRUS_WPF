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
    /// Логика взаимодействия для Generator.xaml
    /// </summary>
    public partial class generator : Object
    {
        public override List<Output> listOfOutput { get; set; } = new List<Output>();

        public generator()
        {
            BorderBrush = Brushes.Transparent;
            BorderThickness = new Thickness(1);
            InitializeComponent();
            listOfOutput.Add(RightInput);
        }

        private void Object_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
