namespace EyeOfSauron
{
    partial class AviInspectForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.blot_defect_button = new System.Windows.Forms.Button();
            this.vertical_defect_button = new System.Windows.Forms.Button();
            this.horizontal_defect_button = new System.Windows.Forms.Button();
            this.ETC_defect_button = new System.Windows.Forms.Button();
            this.dark_defect_button = new System.Windows.Forms.Button();
            this.bright_defect_button = new System.Windows.Forms.Button();
            this.s_grade_button = new System.Windows.Forms.Button();
            this.e_grade_button = new System.Windows.Forms.Button();
            this.virtual_image_Box = new System.Windows.Forms.PictureBox();
            this.defect_Listview = new System.Windows.Forms.ListView();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.login_button = new System.Windows.Forms.Button();
            this.cell_id_label = new System.Windows.Forms.Label();
            this.remain_label = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.virtual_image_Box)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.blot_defect_button, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.vertical_defect_button, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.horizontal_defect_button, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.ETC_defect_button, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.dark_defect_button, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.bright_defect_button, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.s_grade_button, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.e_grade_button, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1333, 786);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(521, 227);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // blot_defect_button
            // 
            this.blot_defect_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blot_defect_button.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.blot_defect_button.Location = new System.Drawing.Point(2, 2);
            this.blot_defect_button.Margin = new System.Windows.Forms.Padding(2);
            this.blot_defect_button.Name = "blot_defect_button";
            this.blot_defect_button.Size = new System.Drawing.Size(169, 71);
            this.blot_defect_button.TabIndex = 0;
            this.blot_defect_button.Text = "污渍";
            this.blot_defect_button.UseVisualStyleBackColor = true;
            this.blot_defect_button.Click += new System.EventHandler(this.judge_function);
            // 
            // vertical_defect_button
            // 
            this.vertical_defect_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vertical_defect_button.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.vertical_defect_button.Location = new System.Drawing.Point(175, 2);
            this.vertical_defect_button.Margin = new System.Windows.Forms.Padding(2);
            this.vertical_defect_button.Name = "vertical_defect_button";
            this.vertical_defect_button.Size = new System.Drawing.Size(169, 71);
            this.vertical_defect_button.TabIndex = 1;
            this.vertical_defect_button.Text = "竖-条纹";
            this.vertical_defect_button.UseVisualStyleBackColor = true;
            this.vertical_defect_button.Click += new System.EventHandler(this.judge_function);
            // 
            // horizontal_defect_button
            // 
            this.horizontal_defect_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.horizontal_defect_button.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.horizontal_defect_button.Location = new System.Drawing.Point(348, 2);
            this.horizontal_defect_button.Margin = new System.Windows.Forms.Padding(2);
            this.horizontal_defect_button.Name = "horizontal_defect_button";
            this.horizontal_defect_button.Size = new System.Drawing.Size(171, 71);
            this.horizontal_defect_button.TabIndex = 2;
            this.horizontal_defect_button.Text = "横-条纹";
            this.horizontal_defect_button.UseVisualStyleBackColor = true;
            this.horizontal_defect_button.Click += new System.EventHandler(this.judge_function);
            // 
            // ETC_defect_button
            // 
            this.ETC_defect_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ETC_defect_button.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.ETC_defect_button.Location = new System.Drawing.Point(2, 77);
            this.ETC_defect_button.Margin = new System.Windows.Forms.Padding(2);
            this.ETC_defect_button.Name = "ETC_defect_button";
            this.ETC_defect_button.Size = new System.Drawing.Size(169, 71);
            this.ETC_defect_button.TabIndex = 3;
            this.ETC_defect_button.Text = "ETC";
            this.ETC_defect_button.UseVisualStyleBackColor = true;
            this.ETC_defect_button.Click += new System.EventHandler(this.judge_function);
            // 
            // dark_defect_button
            // 
            this.dark_defect_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dark_defect_button.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.dark_defect_button.Location = new System.Drawing.Point(175, 77);
            this.dark_defect_button.Margin = new System.Windows.Forms.Padding(2);
            this.dark_defect_button.Name = "dark_defect_button";
            this.dark_defect_button.Size = new System.Drawing.Size(169, 71);
            this.dark_defect_button.TabIndex = 4;
            this.dark_defect_button.Text = "暗团";
            this.dark_defect_button.UseVisualStyleBackColor = true;
            this.dark_defect_button.Click += new System.EventHandler(this.judge_function);
            // 
            // bright_defect_button
            // 
            this.bright_defect_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bright_defect_button.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.bright_defect_button.Location = new System.Drawing.Point(348, 77);
            this.bright_defect_button.Margin = new System.Windows.Forms.Padding(2);
            this.bright_defect_button.Name = "bright_defect_button";
            this.bright_defect_button.Size = new System.Drawing.Size(171, 71);
            this.bright_defect_button.TabIndex = 5;
            this.bright_defect_button.Text = "亮团";
            this.bright_defect_button.UseVisualStyleBackColor = true;
            this.bright_defect_button.Click += new System.EventHandler(this.judge_function);
            // 
            // s_grade_button
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.s_grade_button, 2);
            this.s_grade_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.s_grade_button.Font = new System.Drawing.Font("微软雅黑", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.s_grade_button.ForeColor = System.Drawing.Color.ForestGreen;
            this.s_grade_button.Location = new System.Drawing.Point(175, 152);
            this.s_grade_button.Margin = new System.Windows.Forms.Padding(2);
            this.s_grade_button.Name = "s_grade_button";
            this.s_grade_button.Size = new System.Drawing.Size(344, 73);
            this.s_grade_button.TabIndex = 6;
            this.s_grade_button.Text = "S";
            this.s_grade_button.UseVisualStyleBackColor = true;
            this.s_grade_button.Click += new System.EventHandler(this.judge_function);
            // 
            // e_grade_button
            // 
            this.e_grade_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.e_grade_button.Font = new System.Drawing.Font("微软雅黑", 26F, System.Drawing.FontStyle.Bold);
            this.e_grade_button.ForeColor = System.Drawing.SystemColors.Highlight;
            this.e_grade_button.Location = new System.Drawing.Point(2, 152);
            this.e_grade_button.Margin = new System.Windows.Forms.Padding(2);
            this.e_grade_button.Name = "e_grade_button";
            this.e_grade_button.Size = new System.Drawing.Size(169, 73);
            this.e_grade_button.TabIndex = 7;
            this.e_grade_button.Text = "E";
            this.e_grade_button.UseVisualStyleBackColor = true;
            this.e_grade_button.Click += new System.EventHandler(this.judge_function);
            // 
            // virtual_image_Box
            // 
            this.virtual_image_Box.BackColor = System.Drawing.Color.DarkGreen;
            this.virtual_image_Box.Location = new System.Drawing.Point(936, 786);
            this.virtual_image_Box.Margin = new System.Windows.Forms.Padding(2);
            this.virtual_image_Box.Name = "virtual_image_Box";
            this.virtual_image_Box.Size = new System.Drawing.Size(393, 230);
            this.virtual_image_Box.TabIndex = 1;
            this.virtual_image_Box.TabStop = false;
            // 
            // defect_Listview
            // 
            this.defect_Listview.HideSelection = false;
            this.defect_Listview.Location = new System.Drawing.Point(936, 516);
            this.defect_Listview.Margin = new System.Windows.Forms.Padding(2);
            this.defect_Listview.Name = "defect_Listview";
            this.defect_Listview.Size = new System.Drawing.Size(710, 266);
            this.defect_Listview.TabIndex = 3;
            this.defect_Listview.UseCompatibleStateImageBehavior = false;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.login_button, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.cell_id_label, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.remain_label, 0, 1);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(1650, 513);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(206, 269);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // login_button
            // 
            this.login_button.BackColor = System.Drawing.Color.SandyBrown;
            this.login_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.login_button.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.login_button.Location = new System.Drawing.Point(2, 180);
            this.login_button.Margin = new System.Windows.Forms.Padding(2);
            this.login_button.Name = "login_button";
            this.login_button.Size = new System.Drawing.Size(202, 87);
            this.login_button.TabIndex = 1;
            this.login_button.Text = "用户登录";
            this.login_button.UseVisualStyleBackColor = false;
            this.login_button.Click += new System.EventHandler(this.logout);
            // 
            // cell_id_label
            // 
            this.cell_id_label.AutoSize = true;
            this.cell_id_label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cell_id_label.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.cell_id_label.Location = new System.Drawing.Point(3, 0);
            this.cell_id_label.Name = "cell_id_label";
            this.cell_id_label.Size = new System.Drawing.Size(200, 89);
            this.cell_id_label.TabIndex = 2;
            this.cell_id_label.Text = "CELL ID";
            this.cell_id_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // remain_label
            // 
            this.remain_label.AutoSize = true;
            this.remain_label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.remain_label.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.remain_label.Location = new System.Drawing.Point(3, 89);
            this.remain_label.Name = "remain_label";
            this.remain_label.Size = new System.Drawing.Size(200, 89);
            this.remain_label.TabIndex = 3;
            this.remain_label.Text = "剩余任务数：";
            this.remain_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox1.Location = new System.Drawing.Point(10, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(920, 500);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox2.Location = new System.Drawing.Point(936, 8);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(920, 500);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox3.Location = new System.Drawing.Point(10, 516);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(920, 500);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 7;
            this.pictureBox3.TabStop = false;
            // 
            // AviInspectForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1898, 1024);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.virtual_image_Box);
            this.Controls.Add(this.defect_Listview);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximumSize = new System.Drawing.Size(1920, 1080);
            this.MinimumSize = new System.Drawing.Size(1920, 1080);
            this.Name = "AviInspectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.virtual_image_Box)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button blot_defect_button;
        private System.Windows.Forms.Button vertical_defect_button;
        private System.Windows.Forms.Button horizontal_defect_button;
        private System.Windows.Forms.Button ETC_defect_button;
        private System.Windows.Forms.Button dark_defect_button;
        private System.Windows.Forms.Button bright_defect_button;
        private System.Windows.Forms.Button s_grade_button;
        private System.Windows.Forms.Button e_grade_button;
        private System.Windows.Forms.PictureBox virtual_image_Box;
        private System.Windows.Forms.ListView defect_Listview;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button login_button;
        private System.Windows.Forms.Label cell_id_label;
        private System.Windows.Forms.Label remain_label;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
    }
}

