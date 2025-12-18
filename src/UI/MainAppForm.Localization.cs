using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphSNA.UI
{
    public partial class MainAppForm
    {
        // Dil değiştiğinde veya program açıldığında burayı çağırın
        private void UpdateUITexts()
        {
            this.Text = Properties.Resources.App_Title;
            this.tabPage1.Text = Properties.Resources.UI_Functions;
            this.tabPage2.Text = Properties.Resources.UI_Stats;
            this.button1.Text = Properties.Resources.UI_Import;
            this.button2.Text = Properties.Resources.UI_Export;
        }
    }
}
