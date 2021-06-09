using System.Drawing;
using System.IO;
namespace ExamManager
{
    partial class examManageForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ExamDBGridView = new System.Windows.Forms.DataGridView();
            this.AddPanelIdbutton = new System.Windows.Forms.Button();
            this.Cleanbutton = new System.Windows.Forms.Button();
            this.ExamInfocomboBox = new System.Windows.Forms.ComboBox();
            this.NewIdListBox = new System.Windows.Forms.ListBox();
            this.DelButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.DefectcomboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CommitButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ExamDBGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(788, 437);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox2.Location = new System.Drawing.Point(797, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(789, 437);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox3.Location = new System.Drawing.Point(3, 446);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(788, 438);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 7;
            this.pictureBox3.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ExamDBGridView, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(301, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1589, 887);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // ExamDBGridView
            // 
            this.ExamDBGridView.AllowUserToAddRows = false;
            this.ExamDBGridView.AllowUserToDeleteRows = false;
            this.ExamDBGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ExamDBGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ExamDBGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ExamDBGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.ExamDBGridView.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.ExamDBGridView.Location = new System.Drawing.Point(797, 446);
            this.ExamDBGridView.MultiSelect = false;
            this.ExamDBGridView.Name = "ExamDBGridView";
            this.ExamDBGridView.RowTemplate.Height = 23;
            this.ExamDBGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ExamDBGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ExamDBGridView.Size = new System.Drawing.Size(789, 438);
            this.ExamDBGridView.TabIndex = 11;
            this.ExamDBGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.CellContentClick);
            this.ExamDBGridView.CurrentCellChanged += new System.EventHandler(this.dataGridviewSelectChange);
            this.ExamDBGridView.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGridViewRowPrePaint);
            this.ExamDBGridView.SelectionChanged += new System.EventHandler(this.dataGridviewSelectChange);
            this.ExamDBGridView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ExamDBGridView_MouseClick);
            // 
            // AddPanelIdbutton
            // 
            this.AddPanelIdbutton.Font = new System.Drawing.Font("SimSun", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AddPanelIdbutton.Location = new System.Drawing.Point(12, 15);
            this.AddPanelIdbutton.Name = "AddPanelIdbutton";
            this.AddPanelIdbutton.Size = new System.Drawing.Size(140, 59);
            this.AddPanelIdbutton.TabIndex = 9;
            this.AddPanelIdbutton.Text = "添加ID";
            this.AddPanelIdbutton.UseVisualStyleBackColor = true;
            this.AddPanelIdbutton.Click += new System.EventHandler(this.AddPanelIdbutton_Click);
            // 
            // Cleanbutton
            // 
            this.Cleanbutton.Font = new System.Drawing.Font("SimSun", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Cleanbutton.Location = new System.Drawing.Point(158, 15);
            this.Cleanbutton.Name = "Cleanbutton";
            this.Cleanbutton.Size = new System.Drawing.Size(140, 59);
            this.Cleanbutton.TabIndex = 10;
            this.Cleanbutton.Text = "清空ID";
            this.Cleanbutton.UseVisualStyleBackColor = true;
            this.Cleanbutton.Click += new System.EventHandler(this.Cleanbutton_Click);
            // 
            // ExamInfocomboBox
            // 
            this.ExamInfocomboBox.Font = new System.Drawing.Font("SimSun", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ExamInfocomboBox.FormattingEnabled = true;
            this.ExamInfocomboBox.ItemHeight = 20;
            this.ExamInfocomboBox.Location = new System.Drawing.Point(304, 952);
            this.ExamInfocomboBox.Name = "ExamInfocomboBox";
            this.ExamInfocomboBox.Size = new System.Drawing.Size(247, 28);
            this.ExamInfocomboBox.TabIndex = 12;
            this.ExamInfocomboBox.TextUpdate += new System.EventHandler(this.InfoFilterAdd);
            this.ExamInfocomboBox.SelectedValueChanged += new System.EventHandler(this.FilterChanged);
            // 
            // NewIdListBox
            // 
            this.NewIdListBox.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.NewIdListBox.FormattingEnabled = true;
            this.NewIdListBox.HorizontalExtent = 1;
            this.NewIdListBox.HorizontalScrollbar = true;
            this.NewIdListBox.ItemHeight = 19;
            this.NewIdListBox.Location = new System.Drawing.Point(12, 80);
            this.NewIdListBox.Name = "NewIdListBox";
            this.NewIdListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.NewIdListBox.Size = new System.Drawing.Size(286, 821);
            this.NewIdListBox.TabIndex = 13;
            this.NewIdListBox.Click += new System.EventHandler(this.NewIdListBox_Click);
            this.NewIdListBox.SelectedValueChanged += new System.EventHandler(this.NewIdListBox_SelectedValueChanged);
            this.NewIdListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NewIdListBox_KeyDown);
            this.NewIdListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NewIdListBox_MouseDoubleClick);
            // 
            // DelButton
            // 
            this.DelButton.Location = new System.Drawing.Point(1247, 905);
            this.DelButton.Name = "DelButton";
            this.DelButton.Size = new System.Drawing.Size(131, 82);
            this.DelButton.TabIndex = 14;
            this.DelButton.Text = "删除";
            this.DelButton.UseVisualStyleBackColor = true;
            this.DelButton.TextChanged += new System.EventHandler(this.ButtonBackColorChange);
            this.DelButton.Click += new System.EventHandler(this.del_button_Click);
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(1599, 905);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(131, 82);
            this.AddButton.TabIndex = 15;
            this.AddButton.Text = "添加";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.TextChanged += new System.EventHandler(this.ButtonBackColorChange);
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // DefectcomboBox
            // 
            this.DefectcomboBox.Font = new System.Drawing.Font("SimSun", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DefectcomboBox.FormattingEnabled = true;
            this.DefectcomboBox.Location = new System.Drawing.Point(1403, 952);
            this.DefectcomboBox.Name = "DefectcomboBox";
            this.DefectcomboBox.Size = new System.Drawing.Size(167, 35);
            this.DefectcomboBox.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(1397, 905);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 34);
            this.label1.TabIndex = 17;
            this.label1.Text = "选择不良名";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("SimSun", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(298, 915);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(253, 34);
            this.label2.TabIndex = 18;
            this.label2.Text = "样本任务集选择";
            // 
            // CommitButton
            // 
            this.CommitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(227)))), ((int)(((byte)(219)))));
            this.CommitButton.Location = new System.Drawing.Point(1756, 905);
            this.CommitButton.Name = "CommitButton";
            this.CommitButton.Size = new System.Drawing.Size(131, 82);
            this.CommitButton.TabIndex = 19;
            this.CommitButton.Text = "提交添加";
            this.CommitButton.UseVisualStyleBackColor = true;
            this.CommitButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CommitButtonClick);
            // 
            // examManageForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1902, 999);
            this.Controls.Add(this.CommitButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DefectcomboBox);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.DelButton);
            this.Controls.Add(this.NewIdListBox);
            this.Controls.Add(this.ExamInfocomboBox);
            this.Controls.Add(this.Cleanbutton);
            this.Controls.Add(this.AddPanelIdbutton);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximumSize = new System.Drawing.Size(1920, 1080);
            this.MinimumSize = new System.Drawing.Size(1918, 1038);
            this.Name = "examManageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "试题管理系统";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ExamDBGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button AddPanelIdbutton;
        private System.Windows.Forms.Button Cleanbutton;
        private System.Windows.Forms.ComboBox ExamInfocomboBox;
        private System.Windows.Forms.ListBox NewIdListBox;
        private System.Windows.Forms.Button DelButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.ComboBox DefectcomboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView ExamDBGridView;
        private System.Windows.Forms.Button CommitButton;
    }
}

