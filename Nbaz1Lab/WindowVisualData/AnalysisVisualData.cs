using Nbaz1Lab.WindowVisualData;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Nbaz1Lab
{
    internal class AnalysisVisualData : IVisualData
    {
        private RadioButton granulateByDay;
        private RadioButton granulateByHour;
        private TextBox datePickerTo;
        private TextBox datePickerFrom;

        public AnalysisVisualData(ref Grid mainGrid)
        {
            var textLabel = new TextBlock();
            Grid.SetRow(textLabel, 0);
            textLabel.Text = "Granulate by:";
            Grid.SetColumn(textLabel, 1);
            textLabel.Margin = new Thickness(20, 25, 0, 0);
            mainGrid.Children.Add(textLabel);

            var dateLabel = new TextBlock();
            dateLabel.Margin = new Thickness(190, 0, 0, 0);
            Grid.SetRow(dateLabel, 1);
            Grid.SetColumn(dateLabel, 1);
            dateLabel.HorizontalAlignment = HorizontalAlignment.Center;
            dateLabel.Text = "Day from - Day to           DD.MM.YYYY hh:mm:ss";
            mainGrid.Children.Add(dateLabel);

            var radio = new RadioButton();
            radio.Content = "day";
            radio.GroupName = "FirstGroup";
            Grid.SetRow(radio, 0);
            Grid.SetColumn(radio, 1);
            radio.Margin = new Thickness(20, 50, 20, 0);
            granulateByDay = radio;
            mainGrid.Children.Add(radio);

            radio = new RadioButton();
            radio.Content = "hour";
            radio.GroupName = "FirstGroup";
            Grid.SetRow(radio, 0);
            Grid.SetColumn(radio, 1);
            radio.Margin = new Thickness(70, 50, 0, 0);
            granulateByHour = radio;
            mainGrid.Children.Add(radio);

            var txtBox = new TextBox();
            txtBox.Width = 150;
            txtBox.Height = 20;
            txtBox.VerticalAlignment = VerticalAlignment.Top;
            txtBox.Margin = new Thickness(20, 40, 0, 0);
            Grid.SetRow(txtBox, 1);
            Grid.SetColumn(txtBox, 1);
            txtBox.AcceptsReturn = true;
            txtBox.IsReadOnly = false;
            datePickerFrom = txtBox;
            mainGrid.Children.Add(txtBox);

            txtBox = new TextBox();
            txtBox.Width = 150;
            txtBox.Height = 20;
            txtBox.VerticalAlignment = VerticalAlignment.Top;
            txtBox.Margin = new Thickness(400, 40, 0, 0);
            Grid.SetRow(txtBox, 1);
            Grid.SetColumn(txtBox, 1);
            txtBox.AcceptsReturn = true;
            txtBox.IsReadOnly = false;
            datePickerTo = txtBox;
            mainGrid.Children.Add(txtBox);
        }

        public void AcceptButtonClicked()
        {
            try
            {
                string dateFrom = datePickerFrom.Text;
                string dateTo = datePickerTo.Text;
                string query;
                DateTime dt = Convert.ToDateTime(dateFrom);
                DateTime dt2 = Convert.ToDateTime(dateTo);

                double differenceInDays = ((dt2 - dt).TotalDays);
                string tempTablename;

                if (granulateByDay.IsChecked == true)
                {
                    query = DatabaseHelper.TimeAnalysisQueryBuilder("day", dt, dt2);
                    DatabaseHelper.CreateTempTableDays(differenceInDays, dt);
                    var result = DatabaseHelper.QueryAnalysis(query);
                    tempTablename = "days";
                    //DisplayResultsOnScreenMorphologic(itemsRetrieved);
                }
                else
                {
                    query = DatabaseHelper.TimeAnalysisQueryBuilder("hour", dt, dt2);
                    DatabaseHelper.CreateTempTableHours();
                    tempTablename = "hours";

                }
                DatabaseHelper.DeleteTempTable(tempTablename);
                //DisplayQueryOnScreen(query);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Dispose(ref Grid mainGrid)
        {
            mainGrid.Children.RemoveRange(2, mainGrid.Children.Count - 2);
        }
    }
}
