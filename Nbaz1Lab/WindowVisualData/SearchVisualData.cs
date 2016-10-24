using Nbaz1Lab.WindowVisualData;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Nbaz1Lab
{
    internal class SearchVisualData : IVisualData
    {
        private TextBox queryTextBox;
        private TextBox userInputTextBox;
        private Dictionary<string, RadioButton> dictionaryofRadioButton = new Dictionary<string, RadioButton>();
        private Dictionary<string, RadioButton> dictionaryofRadioButton2 = new Dictionary<string, RadioButton>();
        private TextBlock resultsTextBlock;
        public SearchVisualData(ref Grid mainGrid)
        {
            RadioButton radio = new RadioButton();
            radio.Content = "AND";
            radio.GroupName = "First";
            radio.Checked += new RoutedEventHandler(rb_Checked);
            Grid.SetRow(radio, 0);
            Grid.SetColumn(radio, 1);
            radio.Margin = new Thickness(20, 50, 20, 0);
            mainGrid.Children.Add(radio);
            dictionaryofRadioButton.Add("AND", radio);

            radio = new RadioButton();
            radio.Content = "OR";
            radio.GroupName = "First";
            radio.Checked += new RoutedEventHandler(rb_Checked);
            Grid.SetRow(radio, 0);
            Grid.SetColumn(radio, 1);
            radio.Margin = new Thickness(100, 50, 20, 0);
            mainGrid.Children.Add(radio);
            dictionaryofRadioButton.Add("OR", radio);

            radio = new RadioButton();
            radio.Content = "Morphology&Semantic";
            radio.GroupName = "Second";
            radio.Checked += new RoutedEventHandler(rb_Checked);
            Grid.SetRow(radio, 0);
            Grid.SetColumn(radio, 2);
            radio.Margin = new Thickness(500, 50, 20, 0);
            mainGrid.Children.Add(radio);
            dictionaryofRadioButton2.Add("Morphology&Semantic", radio);

            radio = new RadioButton();
            radio.Content = "Fuzzy";
            radio.GroupName = "Second";
            radio.Checked += new RoutedEventHandler(rb_Checked);
            Grid.SetRow(radio, 0);
            Grid.SetColumn(radio, 2);
            radio.Margin = new Thickness(650, 50, 20, 0);
            mainGrid.Children.Add(radio);
            dictionaryofRadioButton2.Add("Fuzzy", radio);

            var label = new TextBlock();
            Grid.SetRow(label, 1);
            label.Text = "Search:";
            Grid.SetColumn(label, 1);
            mainGrid.Children.Add(label);

            userInputTextBox = new TextBox();
            userInputTextBox.IsEnabled = true;
            userInputTextBox.Margin= new Thickness(50, 0, 50, 0);
            Grid.SetRow(userInputTextBox, 1);
            Grid.SetColumn(userInputTextBox, 1);
            mainGrid.Children.Add(userInputTextBox);

            queryTextBox = new TextBox();
            userInputTextBox.Margin = new Thickness(0, 20, 50, 0);
            userInputTextBox.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetRow(queryTextBox, 2);
            Grid.SetColumn(queryTextBox, 1);
            mainGrid.Children.Add(queryTextBox);

            resultsTextBlock = new TextBlock();
            Grid.SetRow(resultsTextBlock, 3);
            Grid.SetColumn(resultsTextBlock, 1);
            resultsTextBlock.Margin = new Thickness(0, 50, 0, 0);
            mainGrid.Children.Add(resultsTextBlock);
        }

        private void rb_Checked(object sender, RoutedEventArgs e)
        {
            var radio = sender as RadioButton;

            if (dictionaryofRadioButton.ContainsValue(radio))
            {
                if (dictionaryofRadioButton.Values.ElementAt(0) == radio)
                {
                    dictionaryofRadioButton.Values.ElementAt(1).IsChecked = false;
                }
                else
                {
                    dictionaryofRadioButton.Values.ElementAt(0).IsChecked = false;
                }
            }
            else
            {
                if (dictionaryofRadioButton2.Values.ElementAt(0) == radio)
                {
                    dictionaryofRadioButton2.Values.ElementAt(1).IsChecked = false;
                }
                else
                {
                    dictionaryofRadioButton2.Values.ElementAt(0).IsChecked = false;
                }
            }
        }

        public void Dispose(ref Grid mainGrid)
        {
            mainGrid.Children.RemoveRange(2, mainGrid.Children.Count - 2);
        }

        public void AcceptButtonClicked() //FIXME : code duplication
        {
            string logicalOperator = dictionaryofRadioButton.Values.First().IsChecked == true ? "&" : "|";

            resultsTextBlock.Text = "";
            queryTextBox.Text = "";

            try
            {
                string query;
                if (dictionaryofRadioButton2.Values.First().IsChecked == true)
                {
                    query = DatabaseHelper.MorphologySearchQueryBuilder(logicalOperator, userInputTextBox.Text);

                    Tuple<List<string>, List<float>> itemsRetrieved = DatabaseHelper.QueryMorphologic(query);
                    DisplayResultsOnScreenMorphologic(itemsRetrieved);
                }
                else
                {
                    query = DatabaseHelper.FuzzySearchQueryBuilder(logicalOperator, userInputTextBox.Text);
                    //NpgsqlDataReader itemsRetrieved = DatabaseHelper.QueryFuzzy(query);
                    //DisplayResultsOnScreen(itemsRetrieved);
                }

                DisplayQueryOnScreen(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DisplayResultsOnScreenMorphologic(Tuple<List<string>, List<float>> itemsRetrieved)
        {
            if (dictionaryofRadioButton2.Values.First().IsChecked == true)//morphology search
            {
                List<string> listOfDocTitlesBolded = new List<string>(itemsRetrieved.Item1);
                List<float> listOfDocRanks = new List<float>(itemsRetrieved.Item2);

                resultsTextBlock.Text += "Number of documents retrieved: " + listOfDocRanks.Count + "\n";

                for (int i = 0; i < listOfDocTitlesBolded.Count; i++)
                {

                    var parts = listOfDocTitlesBolded[i].Split(new[] { "<b>", "</b>", " " }, StringSplitOptions.RemoveEmptyEntries);

                    for (int j = 0; j < parts.Count(); j++)
                    {
                        bool isbold = userInputTextBox.Text.Contains(parts[j]) ? true : false;

                        if (isbold) resultsTextBlock.Inlines.Add(new Run(parts[j]) { FontWeight = FontWeights.Bold });
                        else resultsTextBlock.Inlines.Add(new Run(parts[j]));
                        resultsTextBlock.Inlines.Add(new Run(" "));
                    }
                    //resultsTextBlock.Text += " " + listOfDocRanks[i] + "\n";
                    resultsTextBlock.Inlines.Add(new Run(listOfDocRanks[i] + "\n"));
                    resultsTextBlock.TextWrapping = TextWrapping.Wrap;

                }

            }
        }

        private void DisplayQueryOnScreen(string query)
        {
            queryTextBox.Text = query.Replace("FROM","\nFROM").Replace("WHERE", "\nWHERE").Replace("ORDER", "\nORDER").Replace("AND", "AND\n");
            queryTextBox.FontSize = 18;
            queryTextBox.AcceptsReturn = true;
        }
    }
}
