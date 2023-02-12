using PAPIRUS_WPF.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;

namespace PAPIRUS_WPF.Dialog
{
    /// <summary>
    /// Логика взаимодействия для GeneratorDialog.xaml
    /// </summary>
    /// 



    public partial class GeneratorDialog : Window
    {

        private List<Limits> limits = new List<Limits>();

        private List<Specific> specifics = new List<Specific>();

        CultureInfo culture = new CultureInfo("en");

        public GeneratorDialog(string elementName)
        {
            InitializeComponent();
            generatorName.Text = elementName;
            RadioButtonDopusk.IsChecked = true;
            dataGridLimits.Visibility = Visibility.Hidden;
            if (Data.dataLimits.Count() != 0)
            {
                GetDataLimits();
            }
            else
            {
                limits.Add(new Limits { lowerLimit = 0, upperLimit = 10, frequencyStep = 1 });
            }
            if(Data.dataSpecifics.Count() != 0)
            {
                GetDataSpecific();
            }
            else
            {
                specifics.Add(new Specific { frequency = 5, tolerance = 50 });
            }
            dataGridLimits.ItemsSource = limits;
            dataGridSpecific.ItemsSource = specifics;
        }

        private void RadioButtonLimits_Checked(object sender, RoutedEventArgs e)
        {
            dataGridLimits.Visibility = Visibility.Visible;
        }

        private void RadioButtonDopusk_Checked(object sender, RoutedEventArgs e)
        {
            dataGridSpecific.Visibility = Visibility.Visible;
        }


        private void RadioButtonLimits_Unchecked(object sender, RoutedEventArgs e)
        {
            dataGridLimits.Visibility = Visibility.Hidden;
        }

        private void RadioButtonDopusk_Unchecked(object sender, RoutedEventArgs e)
        {
            dataGridSpecific.Visibility = Visibility.Hidden;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            LimitsSaveData();
            SpesificSaveData();
            if (RadioButtonDopusk.IsChecked == true)
            {
                Data.specificFrequency = specifics[0].frequency - ((specifics[0].frequency * specifics[0].tolerance) / 100);
                Data.upperLimit = specifics[0].frequency+ (specifics[0].frequency * specifics[0].tolerance) / 100;
                Data.lowerLimit= specifics[0].frequency- (specifics[0].frequency * specifics[0].tolerance) / 100;
            }
            else
            {
                Data.specificFrequency = limits[0].lowerLimit;
                Data.lowerLimit = limits[0].lowerLimit;
                Data.upperLimit = limits[0].upperLimit;
            }
        }
        private void LimitsSaveData()
        {
            int i = 0;
            Data.dataLimits.Clear();
            foreach (var column in dataGridLimits.Columns)
            {
                foreach (var item in dataGridLimits.Items)
                {
                    if (i == 0)
                    {
                        var x = column.GetCellContent(item) as TextBlock;
                        if (!(string.IsNullOrEmpty(x.Text)))
                        {
                            limits[0].lowerLimit = double.Parse(x.Text.Replace(" ", ""), culture);
                        }
                    }
                    if (i == 1)
                    {
                        var x = column.GetCellContent(item) as TextBlock;
                        if (!(string.IsNullOrEmpty(x.Text)))
                        {
                            limits[0].upperLimit = double.Parse(x.Text.Replace(" ", ""), culture);
                        }
                    }
                    if(i == 2)
                    {
                        var x = column.GetCellContent(item) as TextBlock;
                        if (!(string.IsNullOrEmpty(x.Text)))
                        {
                            limits[0].frequencyStep = double.Parse(x.Text.Replace(" ", ""), culture);
                        }
                    }
                }
                i++;
            }
            Data.dataLimits.Add(limits[0]);
        }

        private void SpesificSaveData()
        {
            Data.dataSpecifics.Clear();
            int i = 0;
            foreach(var column in dataGridSpecific.Columns)
            {
                foreach (var item in dataGridSpecific.Items)
                {
                    if (i == 0)
                    {
                        var x = column.GetCellContent(item) as TextBlock;
                        if (!(string.IsNullOrEmpty(x.Text)))
                        {
                            
                            specifics[0].frequency = Double.Parse(x.Text.Replace(" ", ""),culture);
                        }
                    }

                    if (i == 1)
                    {
                        var x = column.GetCellContent(item) as TextBlock;
                        if (!(string.IsNullOrEmpty(x.Text)))
                        {
                            specifics[0].tolerance = double.Parse(x.Text.Replace(" ", ""), culture);
                        }
                    }
                }
                i++;
            }

            Data.dataSpecifics = specifics;
        }

        private void GetDataLimits()
        {
            limits.Add(Data.dataLimits[0]);
        }

        private void GetDataSpecific()
        {
            specifics.Add(Data.dataSpecifics[0]);
        }
    }
}
