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
using System.Xml.Linq;
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
        public List<dataGridElements> datagridelements = new List<dataGridElements>();
        private string elementName;
        private string filePath;
        string jsonString;
        public List<Element> elementsList = new List<Element>();
        List<string> nameElements = new List<string>();
        public GeneratorDialog(string elementName, string fileName)
        {
            InitializeComponent();
            groupTextBox.Text = elementName;
            filePath = AppDomain.CurrentDomain.BaseDirectory + fileName;
            if (File.Exists(filePath))
            {
                jsonString = File.ReadAllText(filePath);
                elementsList = JsonSerializer.Deserialize<List<Element>>(jsonString);
                foreach (Element element in elementsList)
                {
                    nameElements.Add(element.name);
                }
                listBox.ItemsSource = nameElements;
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            elementName = listBox.SelectedItem.ToString();
            Element el = elementsList.Find(x => x.name == elementName);
            Console.WriteLine(el.parameters.Count());
            if (!string.IsNullOrEmpty(el.parameters[0]))
            {
                for (int i = 0; i < el.parameters.Count(); i++)
                {
                    datagridelements.Add(new dataGridElements { columnParam = el.parameters[i] });
                }
            }
            dataGrid.ItemsSource = datagridelements;
        }
    }

    public class dataGridElements
    {
        public string columnParam { get; set; }
        public string columnValue { get; set; }
        public string columnDopusk { get; set; }
    }
}
