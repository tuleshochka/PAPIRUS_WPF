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

namespace PAPIRUS_WPF
{

    public class Element
    {   
        public string imagePath { get; set; }
        public string group { get; set; }
        public string name { get; set; }
        public List<DataGrid1_Parameters> parameters { get; set; }
        public List<DataGrid1_Elements> other_par { get; set; }
        public List<MatrixElements> matrix { get; set; }

    }


    /// <summary>
    /// Логика взаимодействия для GeneratorDialog.xaml
    /// </summary>
    ///
    public class DataGridNumericColumn : DataGridTextColumn
    {
    protected override object PrepareCellForEdit(System.Windows.FrameworkElement editingElement, System.Windows.RoutedEventArgs editingEventArgs)
    {
        TextBox edit = editingElement as TextBox;
        edit.PreviewTextInput += OnPreviewTextInput;

        return base.PrepareCellForEdit(editingElement, editingEventArgs);
    }

    void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {

            if (int.TryParse(e.Text,out int temp)||e.Text=="."||e.Text=="-")
            {

            }
            else e.Handled = true;
    }
    }
     public partial class PoleDialog : Window
    {
        
        private string filePath;  //путь до файла
        string imgPath; //путь до картинки
        string jsonString;  //считанный из файла json текст
        private List<Element> elementsList = new List<Element>();  //лист элементов считанных из файла
        List<string> nameElements = new List<string>(); //лист названий всех элементов из файла
        int poleNum;  //сколько полюсов в полюснике

        private Element el;  //выбранный пользователем элемент из ListBox
        private List<dataGridElements> datagridelements = new List<dataGridElements>(); //для хранения параметров и их значений с dataGridView
        private List<double> values = new List<double>();

        private bool generatorConnected;

        public PoleDialog(string elementName, string fileName, Object _element)
        {
            InitializeComponent();
            Console.WriteLine(_element.generatorConnected);
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

                    datagridelements.Add(new dataGridElements { columnParam = el.parameters[i].paramColumn +" ("+ el.parameters[i].unitColumn + ")" });
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
            jsonString = File.ReadAllText(filePath);
            elementsList = JsonSerializer.Deserialize<List<Element>>(jsonString);
            bool f = true;
            int i = 0;
            el = elementsList.Find(x => x.name == listBox.SelectedItem.ToString());
            if (el.parameters.Count() != 0)
            {
                foreach(dataGridElements element in datagridelements)
                {
                    var x = dataGrid.Columns[1].GetCellContent(dataGrid.Items[i]) as TextBlock;

                    if (string.IsNullOrEmpty(x.Text))
                    {
                        f = true;
                        break;
                    }
                    else f = false ;

                    element.columnValue = x.Text;
                    i++;
                }
            }
            if (f) { MessageBox.Show("Введены не все параметры"); }
            else
            {
                PAPIRUS_WPF.Dialog.SMatrix window1 = new PAPIRUS_WPF.Dialog.SMatrix(poleNum, datagridelements, el, generatorConnected);
                window1.ShowDialog();
            }
        }
    }

    public class dataGridElements
    {
        public string columnParam { get; set; }
        public string columnValue { get; set; }
        public string columnDopusk { get; set; }
    }
}
