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
using static AngouriMath.Entity;
using Matrix = PAPIRUS_WPF.Models.Matrix;

namespace PAPIRUS_WPF.Dialog
{

    /// <summary>
    /// Логика взаимодействия для GeneratorDialog.xaml
    /// </summary>
    ///

    public class PoleInsideElementsAndParams
    {
        public Element element { get; set; }
        public List<DataGridElements> parameters { get; set; }
    }

    public partial class PoleDialog : Window
    {

        private string filePath;  //путь до файла
        string jsonString;  //считанный из файла json текст
        private List<Element> elementsList = new List<Element>();  //лист элементов считанных из файла
        int poleNum;  //сколько полюсов в полюснике
        private bool generatorConnected;
        private List<PoleInsideElementsAndParams> listOfElements;
        private Element el;  //выбранный пользователем элемент из ListBox
        private List<DataGridElements> datagridelements = new List<DataGridElements>(); //для хранения параметров и их значений с dataGridView
        private Object _object;

        private bool isFirst = true;

        public PoleDialog(string elementName, string fileName, Object _element)
        {
            listOfElements = new List<PoleInsideElementsAndParams>();
            InitializeComponent();
            dataGrid.EnableColumnVirtualization = false;
            dataGrid.EnableRowVirtualization = false;
            _object = _element;
            generatorConnected = _element.generatorConnected;
            listBox.SelectedIndex = 0;
            groupTextBox.Text = elementName;
            filePath = AppDomain.CurrentDomain.BaseDirectory +"elementsLib/" + fileName;
            if (File.Exists(filePath))
            {
                jsonString = File.ReadAllText(filePath);
                elementsList = JsonSerializer.Deserialize<List<Element>>(jsonString);
                foreach (Element element in elementsList)
                {
                    listOfElements.Add(new PoleInsideElementsAndParams { element = element, parameters = new List<DataGridElements>() });
                }
                poleNum = _element.group;
            }
            else
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "elementsLib/");
                File.WriteAllText(filePath, "[]");
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
                if (elementsList.Count>1)
                { 
                    el = elementsList[0];
                    listBox.SelectedIndex = 0;
                }
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

                if (!_object.generatorConnected)
                {
                    if (el.other_par.Any(x => x.formulaColumn.Contains("w") || x.formulaColumn.Contains("ω") || x.formulaColumn.Contains("f"))
                || el.matrix.Any(x => x.element.Contains("w") || x.element.Contains("ω") || x.element.Contains("f")))
                    {
                        MessageBox.Show("Элемент не подключен к генератору");
                    }
                    else
                    {
                        SMatrixCalculation calculation = new SMatrixCalculation();
                        Matrix matrix = new Matrix(poleNum, poleNum);
                        try
                        {
                            matrix = calculation.Calculate(el, datagridelements);
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message);
                            this.Close();
                        }
                        SMatrix dialog = new SMatrix(matrix);
                        dialog.ShowDialog();
                    }
                }

                else
                {

                    SMatrixCalculation calculation = new SMatrixCalculation();
                    Matrix matrix = new Matrix(poleNum, poleNum);
                    try
                    {
                        matrix = calculation.Calculate(el, datagridelements);
                        Console.WriteLine(matrix[0, 0].ToString());
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                        this.Close();
                    }
                    try
                    {
                        SMatrix dialog = new SMatrix(matrix);
                        dialog.ShowDialog();
                    }
                    catch(Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                    }
                }
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
           
            if (!SaveData()) { MessageBox.Show("Введены не все параметры"); }
            else
            {
                _object.insideElement = el;
                _object.insideParams.Clear();
                foreach (PoleInsideElementsAndParams element in listOfElements)
                {
                    _object.insideParams.Add(element);
                }
                int number = el.group;
                SMatrixCalculation calculation = new SMatrixCalculation();
                Matrix matrix = new Matrix(number, number);
                try
                {
                    matrix = calculation.Calculate(el, datagridelements);
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

            if (listOfElements.Count == 0)
            {
                return false;
            }
            else
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
                        Console.WriteLine(i);
                        listOfElements[index].parameters.Add(element);
                    }
                }
                else
                {
                    return true;
                }
                return f;
            }
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
