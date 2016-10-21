using Nbaz1Lab.WindowVisualData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Nbaz1Lab
{
    internal class AnalysisVisualData : IVisualData
    {
        public AnalysisVisualData(ref Grid mainGrid)
        {

        }

        public void Dispose(ref Grid mainGrid)
        {
            mainGrid.Children.RemoveRange(2, mainGrid.Children.Count - 2);
        }
    }
}
