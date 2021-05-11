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
    public partial class PanelIdAddForm : Form
    {
        public PanelIdAddForm()
        {
            InitializeComponent();
        }
        List<string> idarray;
        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var item in this.IdTextBox.Lines)
            {
                idarray.Add(item);
            }
            this.Close();
        }
        public void BindIdArray(List<string> id)
        {
            this.idarray = id;
        }
    }
}
