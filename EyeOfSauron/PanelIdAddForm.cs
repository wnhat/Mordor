using Container;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EyeOfSauron
{
    public delegate void PanelIdAddWork(string[] a);
    public partial class PanelIdAddForm : Form
    {
        PanelIdAddWork TheWorkMethod;
        public PanelIdAddForm(PanelIdAddWork work)
        {
            InitializeComponent();
            TheWorkMethod = work;
        }
        private void Addbutton_Click(object sender, EventArgs e)
        {
            TheWorkMethod(this.IdTextBox.Lines);
            this.Close();
        }
    }
}
