using System;
using System.Collections.Generic;
using System.Windows;
using Nbaz1Lab.WindowVisualData;
using Npgsql;

namespace Nbaz1Lab
{
    public partial class MainWindow : Window
    {
        private IVisualData visualData;
        private string connString = String.Format("Server=192.168.56.12;Port=5432;" +
                "User Id=postgres;Password=reverse;Database=postgres;");
        NpgsqlConnection conn;
        private List<string> menuChoices = new List<string>() { "Search", "Add","Analysis" };

        public MainWindow()
        {
            InitializeComponent();
            menuListBox.ItemsSource = menuChoices;
            OpenDatabaseConnection();
        }

        private void OpenDatabaseConnection()
        {
            conn = new NpgsqlConnection(connString);
            conn.Open();

            //string sql = "SELECT * FROM simple_table";
            //NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
        }

        private void menuListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            menuListBox = sender as System.Windows.Controls.ListBox;
            string selectedItem = menuListBox.SelectedItem.ToString(); //TODO : može li vde doći do pogreške?

            if (visualData != null)
            {
                visualData.Dispose(ref MainGrid);
            }

            switch (selectedItem)
            {
                case "Add":
                    visualData = new AddVisualData(ref MainGrid);
                    break;
                case "Search":
                    visualData = new SearchVisualData(ref MainGrid);
                    break;
                case "Analysis":
                    visualData = new AnalysisVisualData(ref MainGrid);
                    break;
            }
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (visualData != null)
            {
                visualData.AcceptButtonClicked();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            conn.Close();
        }
    }
}