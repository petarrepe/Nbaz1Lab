using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Data.SqlClient;


namespace Nbaz1Lab
{
    public partial class MainWindow : Window
    {
        private List<string> menuChoices = new List<string>() { "Search", "Add","Analysis" };
        public MainWindow()
        {
            InitializeComponent();
            menuListBox.ItemsSource = menuChoices;
            OpenDatabaseConnection();
        }

        private void OpenDatabaseConnection()
        {
            string ConnectionString = "Network Library=DBMSSOCN;" + "Data Source= 192.168.56.12,5432; Initial Catalog=datadasename;" + "User Id=root; Password=reverse;";
            SqlConnection connection;
            connection = new SqlConnection(ConnectionString);
            //connection.Open();
        }

        private void menuListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            menuListBox = sender as System.Windows.Controls.ListBox;
            string selectedItem = menuListBox.SelectedItem.ToString();
        }
    }
}