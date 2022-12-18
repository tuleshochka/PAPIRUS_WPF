using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF_SHF_Element_lib;

namespace PAPIRUS_WPF
{

    public class Element
    {
        public string group { get; set; }
        public string name { get; set; }
        public string[] parameters { get; set; }
        public List<DataGrid1_Elements> other_par { get; set; }
        public List<MatrixElements> matrix { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для GeneratorDialog.xaml
    /// </summary>
    public partial class GeneratorDialog : Window
    {
        private string filePath;
        string jsonString;
        List<Element> elementsList;
        List<string> nameElements = new List<string>();
        public GeneratorDialog(string group, string fileName)
        {
            InitializeComponent();
            groupTextBox.Text = group;
            filePath = AppDomain.CurrentDomain.BaseDirectory + fileName;
            jsonString = File.ReadAllText(filePath);
            elementsList = JsonSerializer.Deserialize<List<Element>>(jsonString) ?? new List<Element>();
            foreach(Element element in elementsList)
            {
                nameElements.Add(element.name);
            }
            listBox.ItemsSource = nameElements;
            //GeneratorList.ItemsSource= Customer.GetSampleCustomerList();
        }

        
    }
}
