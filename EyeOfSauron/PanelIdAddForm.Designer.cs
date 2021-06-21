namespace EyeOfSauron
{
    partial class PanelIdAddForm
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
            this.AviButton = new System.Windows.Forms.Button();
            this.IdTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // AviButton
            // 
            this.AviButton.Location = new System.Drawing.Point(12, 363);
            this.AviButton.Name = "AviButton";
            this.AviButton.Size = new System.Drawing.Size(260, 37);
            this.AviButton.TabIndex = 0;
            this.AviButton.Text = "添加 Panel ID";
            this.AviButton.UseVisualStyleBackColor = true;
            this.AviButton.Click += new System.EventHandler(this.Addbutton_Click);
            // 
            // IdTextBox
            // 
            this.IdTextBox.Location = new System.Drawing.Point(12, 12);
            this.IdTextBox.Multiline = true;
            this.IdTextBox.Name = "IdTextBox";
            this.IdTextBox.Size = new System.Drawing.Size(260, 345);
            this.IdTextBox.TabIndex = 1;
            // 
            // PanelIdAddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 412);
            this.Controls.Add(this.IdTextBox);
            this.Controls.Add(this.AviButton);
            this.Name = "PanelIdAddForm";
            this.Text = "PnaelIdAddForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AviButton;
        private System.Windows.Forms.TextBox IdTextBox;
    }
}