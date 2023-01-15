﻿using PAPIRUS_WPF.Dialog;
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
        public static List<Object> selection = new List<Object>();
        public static List<Line> selectedWires = new List<Line>();
        public static Stack<FrameworkElement> undo = new Stack<FrameworkElement>();

        //---------для GeneratorDialog-------------//
        public static List<Limits> dataLimits = new List<Limits>();  //
        public static List<Specific> dataSpecifics = new List<Specific>();

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
    }
}
