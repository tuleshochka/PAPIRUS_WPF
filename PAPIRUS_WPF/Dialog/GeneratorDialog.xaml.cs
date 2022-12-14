using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WPF_SHF_Element_lib;
using PAPIRUS_WPF.Dialog;

namespace PAPIRUS_WPF
{

    public class Element
    {   
        public string imagePath { get; set; }
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
        private string filePath;  //путь до файла
        string imgPath; //путь до картинки
        string jsonString;  //считанный из файла json текст
        public List<Element> elementsList = new List<Element>();  //лист элементов считанных из файла
        List<string> nameElements = new List<string>(); //лист названий всех элементов из файла
        int poleNum;  //сколько полюсов в полюснике

        private Element el;  //выбранный пользователем элемент из ListBox
        private List<dataGridElements> datagridelements = new List<dataGridElements>(); //для хранения параметров и их значений с dataGridView
        private List<double> values = new List<double>();


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
                switch (fileName)
                {
                    case "2pole.json":
                        poleNum = 1;
                        break;
                    case "4pole.json":
                        poleNum = 2;
                        break;
                    case "6pole.json":
                        poleNum = 3;
                        break;
                    case "8pole.json":
                        poleNum = 4;
                        break;
                }
                listBox.ItemsSource = nameElements;
                
            }

        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            datagridelements.Clear();
            dataGrid.ItemsSource = null;
            el = elementsList.Find(x => x.name == listBox.SelectedItem.ToString());
            if (!string.IsNullOrEmpty(el.parameters[0]))
            {
                for (int i = 0; i < el.parameters.Count(); i++)
                {
                    datagridelements.Add(new dataGridElements { columnParam = el.parameters[i] });
                }
                dataGrid.ItemsSource = datagridelements;
            }
            try
            {
                imgPath = el.imagePath;
                imageElement.Source = new BitmapImage(new Uri(imgPath));
            }
            catch (Exception)
            {
                imageElement.Source = null;
            }
            
        }

        private void sMatrix_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            bool f = false;
            if (!string.IsNullOrEmpty(el.parameters[0]))
            {
                foreach(dataGridElements element in datagridelements)
                {
                    var x = dataGrid.Columns[1].GetCellContent(dataGrid.Items[i]) as TextBlock;
                    if (string.IsNullOrEmpty(x.Text))
                    {
                        f = true;
                        break;
                    }
                    element.columnValue = x.Text;
                    //values.Add(x.Text);
                    i++;
                }
            }
           SMatrix window1 = new SMatrix(poleNum, datagridelements, el);     
           window1.Show();
        }
    }

    public class dataGridElements
    {
        public string columnParam { get; set; }
        public string columnValue { get; set; }
        public string columnDopusk { get; set; }
    }
}
