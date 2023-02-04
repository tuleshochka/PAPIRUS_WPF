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
using System.Reflection;
using System.Numerics;

namespace PAPIRUS_WPF.Dialog
{

    /// <summary>
    /// Логика взаимодействия для GeneratorDialog.xaml
    /// </summary>
    ///

    public class PoleInsideElementsAndParams
    {
        public Element element { get; set; }
        public List<DataGridElements> parameters = new List<DataGridElements>();
    }

     public partial class PoleDialog : Window
    {
        
        private string filePath;  //путь до файла
        string imgPath; //путь до картинки
        string jsonString;  //считанный из файла json текст
        private List<Element> elementsList = new List<Element>();  //лист элементов считанных из файла
        List<string> nameElements = new List<string>(); //лист названий всех элементов из файла
        int poleNum;  //сколько полюсов в полюснике

        private List<PoleInsideElementsAndParams> listOfElements;
        private Element el;  //выбранный пользователем элемент из ListBox
        private List<DataGridElements> datagridelements = new List<DataGridElements>(); //для хранения параметров и их значений с dataGridView
        private List<double> values = new List<double>();
        private Object _object;

        private bool generatorConnected;
        private bool isFirst = true;

        public PoleDialog(string elementName, string fileName, Object _element)
        {
            listOfElements = new List<PoleInsideElementsAndParams>();
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
                    listOfElements.Add( new PoleInsideElementsAndParams { element = element });
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
            }

            listBox.ItemsSource = listOfElements.Select(x => x.element.name);
            if (_object.insideElement != null)
            {
                listOfElements.Clear();
                datagridelements.Clear();
                foreach (PoleInsideElementsAndParams element in _object.insideParams)
                {
                    listOfElements.Add(element);
                }
                el = _object.insideElement;
                int index = listOfElements.FindIndex(x => x.element.name == el.name);
                foreach (DataGridElements element in _object.insideParams[index].parameters)
                {
                    datagridelements.Add(element);
                }
                listBox.SelectedItem = el.name;
                dataGrid.ItemsSource = datagridelements;
            }
            else
            {
                el = elementsList[0];
                listBox.SelectedIndex = 0;
            }
            isFirst = false;

        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isFirst)
            {
                SaveData(el);
                datagridelements.Clear();
                dataGrid.ItemsSource = null;
                el = elementsList.Find(x => x.name == listBox.SelectedItem.ToString());
                int index = listOfElements.FindIndex(x => x.element.name == el.name);

                if (el.parameters.Count() != 0)
                {
                    for (int i = 0; i < el.parameters.Count(); i++)
                    {
                        datagridelements.Add(new DataGridElements { columnParam = el.parameters[i].paramColumn + " (" + el.parameters[i].unitColumn + ")" });
                        if (listOfElements[index].parameters.Count != 0)
                        {
                            datagridelements[i].columnValue = listOfElements[index].parameters[i].columnValue;
                            datagridelements[i].columnDopusk = listOfElements[index].parameters[i].columnDopusk;
                        }
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
            else
            {
                datagridelements.Clear();
                dataGrid.ItemsSource = null;
                el = elementsList.Find(x => x.name == listBox.SelectedItem.ToString());
                int index = listOfElements.FindIndex(x => x.element.name == el.name);
                if (el.parameters.Count() != 0)
                {
                    for (int i = 0; i < el.parameters.Count(); i++)
                    {
                        datagridelements.Add(new DataGridElements { columnParam = el.parameters[i].paramColumn + " (" + el.parameters[i].unitColumn + ")" });
                        if (listOfElements[index].parameters.Count != 0)
                        {
                            datagridelements[i].columnValue = listOfElements[index].parameters[i].columnValue;
                            datagridelements[i].columnDopusk = listOfElements[index].parameters[i].columnDopusk;
                        }
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
                _object.insideParams.Clear();
                foreach(PoleInsideElementsAndParams element in listOfElements)
                {
                    _object.insideParams.Add(element);
                }
                int number = el.group;
                SMatrixCalculation calculation = new SMatrixCalculation();
                Complex[,] matrix = new Complex[number, number];
                try
                {
                    matrix = calculation.Calculate(el, _object.generatorConnected, datagridelements);
                    _object.matrix = matrix;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    this.Close();
                }
                this.Close();
            }
        }

        private bool SaveData()
        {
            bool f = false;
            int i = 0;
            el = elementsList.Find(x => x.name == listBox.SelectedItem.ToString());
            int index = listOfElements.FindIndex(x => x.element.name == el.name);
            listOfElements[index].parameters.Clear();
            if (el.parameters.Count() != 0)
            {
                foreach (DataGridElements element in datagridelements)
                {
                    var x = dataGrid.Columns[1].GetCellContent(dataGrid.Items[i]) as TextBlock;

                    if (string.IsNullOrEmpty(x.Text))
                    {
                        f = false;
                        break;
                    }
                    else f = true;
                    element.columnValue = x.Text;
                    i++;
                    listOfElements[index].parameters.Add(element);
                }
            }
            return f;
        }

        private void SaveData(Element elem)
        {
            int i = 0;
            int index = listOfElements.FindIndex(x => x.element.name == elem.name);
            listOfElements[index].parameters.Clear();
           
            if (elem.parameters.Count() != 0)
            {
                foreach (DataGridElements element in datagridelements)
                {
                    var x = dataGrid.Columns[1].GetCellContent(dataGrid.Items[i]) as TextBlock;
                    var y = dataGrid.Columns[2].GetCellContent(dataGrid.Items[i]) as TextBlock;
                    if (string.IsNullOrEmpty(x.Text))
                    {
                        element.columnValue = "";
                    }
                    else
                    {
                        element.columnValue = x.Text;
                    }
                    if (string.IsNullOrEmpty(y.Text))
                    {
                        element.columnDopusk = "";
                    }
                    else
                    {
                        element.columnDopusk = x.Text;
                    }
                    listOfElements[index].parameters.Add(element);
                    i++;
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
