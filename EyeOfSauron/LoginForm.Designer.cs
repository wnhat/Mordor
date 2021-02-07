namespace EyeOfSauron
{
    partial class LoginForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.userid_box = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.Svibutton = new System.Windows.Forms.Button();
            this.Avibutton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.userid_box, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(78, 34);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(606, 318);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // userid_box
            // 
            this.userid_box.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userid_box.Location = new System.Drawing.Point(3, 117);
            this.userid_box.Margin = new System.Windows.Forms.Padding(3, 21, 3, 3);
            this.userid_box.Name = "userid_box";
            this.userid_box.Size = new System.Drawing.Size(600, 28);
            this.userid_box.TabIndex = 0;
            this.userid_box.KeyDown += new System.Windows.Forms.KeyEventHandler(this.userid_box_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(10, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(10, 3, 10, 9);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.label1.Size = new System.Drawing.Size(586, 84);
            this.label1.TabIndex = 2;
            this.label1.Text = "请输入工号";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.Svibutton, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.Avibutton, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 196);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(598, 88);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // Svibutton
            // 
            this.Svibutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.Svibutton.BackColor = System.Drawing.Color.Cornsilk;
            this.Svibutton.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Svibutton.ForeColor = System.Drawing.Color.DarkGreen;
            this.Svibutton.Location = new System.Drawing.Point(366, 14);
            this.Svibutton.Margin = new System.Windows.Forms.Padding(15, 14, 15, 14);
            this.Svibutton.Name = "Svibutton";
            this.Svibutton.Size = new System.Drawing.Size(164, 60);
            this.Svibutton.TabIndex = 1;
            this.Svibutton.Text = "SVI";
            this.Svibutton.UseVisualStyleBackColor = false;
            this.Svibutton.Click += new System.EventHandler(this.Svibutton_Click);
            // 
            // Avibutton
            // 
            this.Avibutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.Avibutton.BackColor = System.Drawing.Color.Cornsilk;
            this.Avibutton.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Avibutton.ForeColor = System.Drawing.Color.DarkGreen;
            this.Avibutton.Location = new System.Drawing.Point(67, 14);
            this.Avibutton.Margin = new System.Windows.Forms.Padding(15, 14, 15, 14);
            this.Avibutton.Name = "Avibutton";
            this.Avibutton.Size = new System.Drawing.Size(164, 60);
            this.Avibutton.TabIndex = 3;
            this.Avibutton.Text = "AVI";
            this.Avibutton.UseVisualStyleBackColor = false;
            this.Avibutton.Click += new System.EventHandler(this.Avibutton_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 418);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "用户登录";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button Svibutton;
        private System.Windows.Forms.TextBox userid_box;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button Avibutton;
    }
}