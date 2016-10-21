using Nbaz1Lab.WindowVisualData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Nbaz1Lab
{
    internal class SearchVisualData : IVisualData
    {
        private TextBox queryTextBox;
        private TextBox userInputTextBox;
        private Dictionary<string, RadioButton> dictionaryofRadioButton = new Dictionary<string, RadioButton>();
        private Dictionary<string, RadioButton> dictionaryofRadioButton2 = new Dictionary<string, RadioButton>();
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

        public void AcceptButtonClicked()
        {
            throw new NotImplementedException();
        }
    }
}
