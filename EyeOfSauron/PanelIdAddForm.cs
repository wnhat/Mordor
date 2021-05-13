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
    public partial class PanelIdAddForm : Form
    {
        public PanelIdAddForm()
        {
            InitializeComponent();
        }
        List<PanelInfo> idarray;
        private void AVIbutton_Click(object sender, EventArgs e)
        {
            foreach (var item in this.IdTextBox.Lines)
            {
                idarray.Add(new PanelInfo(item,InspectSection.AVI));
            }
            this.Close();
        }
        private void SviButton_Click(object sender, EventArgs e)
        {
            foreach (var item in this.IdTextBox.Lines)
            {
                idarray.Add(new PanelInfo(item, InspectSection.SVI));
            }
            this.Close();
        }
        public void BindIdArray(List<PanelInfo> id)
        {
            this.idarray = id;
        }
        
    }
}
