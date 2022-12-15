using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PAPIRUS_WPF
{
    /// <summary>
    /// Логика взаимодействия для GeneratorDialog.xaml
    /// </summary>
    public partial class GeneratorDialog : Window
    {
        public GeneratorDialog()
        {
            InitializeComponent();
            GeneratorList.ItemsSource= Customer.GetSampleCustomerList();
        }

        
    }



    public class Customer
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Address { get; set; }
      

        // A null value for IsSubscribed can indicate 
        // "no preference" or "no response".
        public Boolean? IsSubscribed { get; set; }

        public Customer(String firstName, String lastName,
            String address,  Boolean? isSubscribed)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Address = address;
            this.IsSubscribed = isSubscribed;
        }

        public static List<Customer> GetSampleCustomerList()
        {
            return new List<Customer>(new Customer[4] {
            new Customer("A.", "Zero",
                "12 North Third Street, Apartment 45",
                 true),
            new Customer("B.", "One",
                "34 West Fifth Street, Apartment 67",
                 false),
            new Customer("C.", "Two",
                "56 East Seventh Street, Apartment 89",
                 null),
            new Customer("D.", "Three",
                "78 South Ninth Street, Apartment 10",
                 true)
        });
        }
    }
}
