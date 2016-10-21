using Nbaz1Lab.WindowVisualData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Nbaz1Lab
{
    internal class AddVisualData : IVisualData
    {
        internal List<string> typeOfData = new List<string>() { "Title", "Keywords", "Summary", "Body"};
        public AddVisualData(ref Grid mainGrid)
        {
            for (int i = 0; i < typeOfData.Count(); i++)
            {
                TextBlock textBlockTemp = new TextBlock();
                textBlockTemp.Text = typeOfData[i];
                Grid.SetRow(textBlockTemp, i);
                Grid.SetColumn(textBlockTemp, 1);

                mainGrid.Children.Add(textBlockTemp);
            }
            for (int i = 0; i < typeOfData.Count(); i++)
            {
                var txtBox = new TextBox();
                txtBox.Margin = new Thickness(20, 20, 20, 40);
                Grid.SetRow(txtBox, i);
                Grid.SetColumn(txtBox, 2);

                txtBox.AcceptsReturn = true;
                txtBox.IsReadOnly = false;
                txtBox.Tag = typeOfData[i];

                mainGrid.Children.Add(txtBox);
            }

        }

        public void Dispose(ref Grid mainGrid)
        {
            mainGrid.Children.RemoveRange(2, mainGrid.Children.Count - 2);
        }
    }
}
