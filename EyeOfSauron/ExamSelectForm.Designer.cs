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
            this.ExamIfoncomboBox.Font = new System.Drawing.Font("SimSun", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ExamIfoncomboBox.FormattingEnabled = true;
            this.ExamIfoncomboBox.Location = new System.Drawing.Point(49, 60);
            this.ExamIfoncomboBox.Name = "ExamIfoncomboBox";
            this.ExamIfoncomboBox.Size = new System.Drawing.Size(197, 37);
            this.ExamIfoncomboBox.TabIndex = 0;
            // 
            // Startbutton
            // 
            this.Startbutton.Font = new System.Drawing.Font("Microsoft YaHei", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Startbutton.Location = new System.Drawing.Point(49, 171);
            this.Startbutton.Name = "Startbutton";
            this.Startbutton.Size = new System.Drawing.Size(197, 86);
            this.Startbutton.TabIndex = 1;
            this.Startbutton.Text = "开始考试";
            this.Startbutton.UseVisualStyleBackColor = true;
            // 
            // ExamSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 305);
            this.Controls.Add(this.Startbutton);
            this.Controls.Add(this.ExamIfoncomboBox);
            this.Name = "ExamSelectForm";
            this.Text = "ExamSelectForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox ExamIfoncomboBox;
        private System.Windows.Forms.Button Startbutton;
    }
}