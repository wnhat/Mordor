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
            this.Avibutton = new System.Windows.Forms.Button();
            this.ExamManagerButton = new System.Windows.Forms.Button();
            this.EvilButton = new System.Windows.Forms.Button();
            this.userid_box = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.Avibutton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.ExamManagerButton, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.EvilButton, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.userid_box, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(89, 23);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(408, 326);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // Avibutton
            // 
            this.Avibutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.Avibutton.BackColor = System.Drawing.Color.Cornsilk;
            this.Avibutton.Font = new System.Drawing.Font("Microsoft YaHei", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Avibutton.ForeColor = System.Drawing.Color.DarkGreen;
            this.Avibutton.Location = new System.Drawing.Point(149, 139);
            this.Avibutton.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.Avibutton.Name = "Avibutton";
            this.Avibutton.Size = new System.Drawing.Size(109, 47);
            this.Avibutton.TabIndex = 3;
            this.Avibutton.Text = "量产检查";
            this.Avibutton.UseVisualStyleBackColor = false;
            this.Avibutton.Click += new System.EventHandler(this.Inspectbutton_Click);
            // 
            // ExamManagerButton
            // 
            this.ExamManagerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.ExamManagerButton.BackColor = System.Drawing.Color.Cornsilk;
            this.ExamManagerButton.Font = new System.Drawing.Font("Microsoft YaHei", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ExamManagerButton.ForeColor = System.Drawing.Color.DarkGreen;
            this.ExamManagerButton.Location = new System.Drawing.Point(114, 269);
            this.ExamManagerButton.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.ExamManagerButton.Name = "ExamManagerButton";
            this.ExamManagerButton.Size = new System.Drawing.Size(180, 48);
            this.ExamManagerButton.TabIndex = 5;
            this.ExamManagerButton.Text = "ExamManager";
            this.ExamManagerButton.UseVisualStyleBackColor = false;
            this.ExamManagerButton.Click += new System.EventHandler(this.ExamManagerButton_Click);
            // 
            // EvilButton
            // 
            this.EvilButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.EvilButton.BackColor = System.Drawing.Color.Cornsilk;
            this.EvilButton.Font = new System.Drawing.Font("Microsoft YaHei", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EvilButton.ForeColor = System.Drawing.Color.DarkGreen;
            this.EvilButton.Location = new System.Drawing.Point(149, 204);
            this.EvilButton.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.EvilButton.Name = "EvilButton";
            this.EvilButton.Size = new System.Drawing.Size(109, 47);
            this.EvilButton.TabIndex = 4;
            this.EvilButton.Text = "考试";
            this.EvilButton.UseVisualStyleBackColor = false;
            this.EvilButton.Click += new System.EventHandler(this.Evilbutton_Click);
            // 
            // userid_box
            // 
            this.userid_box.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userid_box.Location = new System.Drawing.Point(2, 79);
            this.userid_box.Margin = new System.Windows.Forms.Padding(2, 14, 2, 2);
            this.userid_box.Name = "userid_box";
            this.userid_box.Size = new System.Drawing.Size(404, 21);
            this.userid_box.TabIndex = 0;
            this.userid_box.KeyDown += new System.Windows.Forms.KeyEventHandler(this.userid_box_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(7, 2);
            this.label1.Margin = new System.Windows.Forms.Padding(7, 2, 7, 6);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.label1.Size = new System.Drawing.Size(394, 57);
            this.label1.TabIndex = 2;
            this.label1.Text = "请输入工号";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(588, 381);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "用户登录";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox userid_box;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button EvilButton;
        private System.Windows.Forms.Button Avibutton;
        private System.Windows.Forms.Button ExamManagerButton;
    }
}