namespace EyeOfSauron
{
    partial class ExamSelectForm
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
            this.ExamIfoncomboBox = new System.Windows.Forms.ComboBox();
            this.Startbutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ExamIfoncomboBox
            // 
            this.ExamIfoncomboBox.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ExamIfoncomboBox.FormattingEnabled = true;
            this.ExamIfoncomboBox.Location = new System.Drawing.Point(74, 90);
            this.ExamIfoncomboBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ExamIfoncomboBox.Name = "ExamIfoncomboBox";
            this.ExamIfoncomboBox.Size = new System.Drawing.Size(294, 52);
            this.ExamIfoncomboBox.TabIndex = 0;
            // 
            // Startbutton
            // 
            this.Startbutton.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Startbutton.Location = new System.Drawing.Point(74, 256);
            this.Startbutton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Startbutton.Name = "Startbutton";
            this.Startbutton.Size = new System.Drawing.Size(296, 129);
            this.Startbutton.TabIndex = 1;
            this.Startbutton.Text = "开始考试";
            this.Startbutton.UseVisualStyleBackColor = true;
            this.Startbutton.Click += new System.EventHandler(this.Startbutton_Click);
            // 
            // ExamSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 458);
            this.Controls.Add(this.Startbutton);
            this.Controls.Add(this.ExamIfoncomboBox);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ExamSelectForm";
            this.Text = "ExamSelectForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox ExamIfoncomboBox;
        private System.Windows.Forms.Button Startbutton;
    }
}