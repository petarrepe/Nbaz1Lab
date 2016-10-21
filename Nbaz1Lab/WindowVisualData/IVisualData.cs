using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nbaz1Lab.WindowVisualData
{
    //ovo je zapravo trebalo kao apstrakna klasa sa defaultnim ponašanjem na dispose
    public interface IVisualData
    {
        void Dispose(ref System.Windows.Controls.Grid mainGrid);

    }
    
}
