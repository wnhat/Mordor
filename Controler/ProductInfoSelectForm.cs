using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Container;
using Container.SeverConnection;

namespace Controler
{
    public partial class ProductInfoSelectForm : Form
    {
        List<ProductInfo> InfoList;
        User inputUser;
        public ProductInfoSelectForm()
        {
            InitializeComponent();
            //inputUser = op;
            InfoList = SeverConnecter.GetProductInfo();
            InitialCombox();
        }

        private void InitialCombox()
        {
            var nameList = from item in InfoList
                           select item.Name;
            this.comboBox1.DataSource = nameList.ToArray();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var FGcodeList = from item in InfoList
                             where item.Name == this.comboBox1.Text
                             select item.FGcode;
            this.comboBox2.DataSource = FGcodeList.ToArray();
            this.comboBox3.DataSource = null;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var productTypeList = from item in InfoList
                              where item.Name == this.comboBox1.Text && item.FGcode == this.comboBox2.Text
                              select item.ProductType;
            this.comboBox3.DataSource = productTypeList.ToArray();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            var selectProduct = from item in InfoList
                                where item.Name == this.comboBox1.Text && item.FGcode == this.comboBox2.Text && item.ProductType == this.comboBox3.Text
                                select item;
            if (selectProduct.Count() == 0)
            {
                MessageBox.Show("请选择正确的Product；");
            }
            else
            {
                SeverConnecter.AddPanelMission(selectProduct.First());
            }
        }
    }
}
