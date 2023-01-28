using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PAPIRUS_WPF.Models
{
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

            if (int.TryParse(e.Text, out int temp) || e.Text == "." || e.Text == "-")
            {

            }
            else e.Handled = true;
        }
    }
}
