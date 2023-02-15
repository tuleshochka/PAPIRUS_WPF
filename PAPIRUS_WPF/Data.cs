using LiveCharts.Defaults;
using LiveCharts;
using PAPIRUS_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PAPIRUS_WPF
{
    public static class Data
    {
        //---------для MainForm-------------//
        public static List<FrameworkElement> outputNumber = new List<FrameworkElement>();
        public static List<Object> elements;
        public static List<Object> selection = new List<Object>();
        public static List<Line> selectedWires = new List<Line>();
        public static Stack<FrameworkElement> undo = new Stack<FrameworkElement>();
        public static Object multiPole;


        //---------для GeneratorDialog-------------//
        public static List<Limits> dataLimits = new List<Limits>();  //
        public static List<Specific> dataSpecifics = new List<Specific>();
        public static double lowerLimit = 0;
        public static double upperLimit = 10;
        public static double specificFrequency = 1;



        //----------найти родительский элемент-------------//
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;
            Console.WriteLine(parentObject);
            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }


        //----------АЧХ и ФЧХ-------------//
        //Тут храним точки
        public static ChartValues<ObservablePoint> frec = new ChartValues<ObservablePoint>();
        public static ChartValues<ObservablePoint> phase = new ChartValues<ObservablePoint>();

        /*вариант заполнения 
            for (int i = 0; i< 10; i++)
            {
                Data.frec.Add(new ObservablePoint(i, -i));
            }
        */
}
}
