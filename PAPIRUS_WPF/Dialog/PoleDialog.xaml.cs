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
using PAPIRUS_WPF;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;
using PAPIRUS_WPF.Models;
using Element = PAPIRUS_WPF.Models.Element;
using System.Xml.Linq;

namespace PAPIRUS_WPF.Dialog
{

    /// <summary>
    /// Логика взаимодействия для GeneratorDialog.xaml
    /// </summary>
    ///


     public partial class PoleDialog : Window
    {
        
        private string filePath;  //путь до файла
        string imgPath; //путь до картинки
        string jsonString;  //считанный из файла json текст
        private List<Element> elementsList = new List<Element>();  //лист элементов считанных из файла
        List<string> nameElements = new List<string>(); //лист названий всех элементов из файла
        int poleNum;  //сколько полюсов в полюснике

        private Element el;  //выбранный пользователем элемент из ListBox
        private List<DataGridElements> datagridelements = new List<DataGridElements>(); //для хранения параметров и их значений с dataGridView
        private List<double> values = new List<double>();
        private Object _object;

        private bool generatorConnected;

        public PoleDialog(string elementName, string fileName, Object _element)
        {
            InitializeComponent();
            _object = _element;
            generatorConnected = _element.generatorConnected;
            listBox.SelectedIndex= 0;
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
            if (el.parameters.Count()!=0)
            {
                for (int i = 0; i < el.parameters.Count(); i++)
                {
                    datagridelements.Add(new DataGridElements { columnParam = el.parameters[i].paramColumn +" ("+ el.parameters[i].unitColumn + ")" });
                }
                dataGrid.ItemsSource = datagridelements;
            }
            try
            {
                imageElement.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + el.imagePath));
            }
            catch (Exception)
            {
                imageElement.Source = null;
            }
            
        }

        private void sMatrix_Click(object sender, RoutedEventArgs e)
        {
            if (!SaveData()) { MessageBox.Show("Введены не все параметры"); }
            else
            {
                PAPIRUS_WPF.Dialog.SMatrix window1 = new PAPIRUS_WPF.Dialog.SMatrix(poleNum, datagridelements, el, generatorConnected);
                window1.ShowDialog();
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SaveData()) { MessageBox.Show("Введены не все параметры"); }
            else
            {
                _object.insideElement = el;
                _object.insideParams = datagridelements;
            }
        }

        private bool SaveData()
        {
            jsonString = File.ReadAllText(filePath);
            elementsList = JsonSerializer.Deserialize<List<Element>>(jsonString);
            bool f = true;
            int i = 0;
            el = elementsList.Find(x => x.name == listBox.SelectedItem.ToString());
            if (el.parameters.Count() != 0)
            {
                foreach (DataGridElements element in datagridelements)
                {
                    var x = dataGrid.Columns[1].GetCellContent(dataGrid.Items[i]) as TextBlock;

                    if (string.IsNullOrEmpty(x.Text))
                    {
                        f = true;
                        break;
                    }
                    else f = false;
                    element.columnValue = x.Text;
                    i++;
                }
            }
            return f;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
