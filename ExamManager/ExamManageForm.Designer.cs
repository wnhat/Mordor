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
            this.commitButton = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.fileSelectButton = new System.Windows.Forms.Button();
            this.fileSelectTextBox = new System.Windows.Forms.TextBox();
            this.idTextBox = new System.Windows.Forms.TextBox();
            this.imageViewButton = new System.Windows.Forms.Button();
            this.imageIDTextBox = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.upToTopButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // commitButton
            // 
            this.commitButton.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.commitButton.Location = new System.Drawing.Point(479, 32);
            this.commitButton.Name = "commitButton";
            this.commitButton.Size = new System.Drawing.Size(77, 28);
            this.commitButton.TabIndex = 0;
            this.commitButton.Text = "提交";
            this.commitButton.UseVisualStyleBackColor = true;
            this.commitButton.Click += new System.EventHandler(this.commitButton_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(253, 32);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(71, 27);
            this.comboBox1.TabIndex = 1;
            // 
            // fileSelectButton
            // 
            this.fileSelectButton.Location = new System.Drawing.Point(195, 31);
            this.fileSelectButton.Name = "fileSelectButton";
            this.fileSelectButton.Size = new System.Drawing.Size(40, 28);
            this.fileSelectButton.TabIndex = 5;
            this.fileSelectButton.Text = "...";
            this.fileSelectButton.UseVisualStyleBackColor = true;
            this.fileSelectButton.Click += new System.EventHandler(this.fileSelectButton_Click);
            // 
            // fileSelectTextBox
            // 
            this.fileSelectTextBox.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fileSelectTextBox.Location = new System.Drawing.Point(12, 32);
            this.fileSelectTextBox.Multiline = true;
            this.fileSelectTextBox.Name = "fileSelectTextBox";
            this.fileSelectTextBox.Size = new System.Drawing.Size(177, 27);
            this.fileSelectTextBox.TabIndex = 0;
            this.fileSelectTextBox.Text = "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\Common7\\IDE";
            this.fileSelectTextBox.WordWrap = false;
            // 
            // idTextBox
            // 
            this.idTextBox.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.idTextBox.Location = new System.Drawing.Point(12, 76);
            this.idTextBox.Multiline = true;
            this.idTextBox.Name = "idTextBox";
            this.idTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.idTextBox.Size = new System.Drawing.Size(177, 475);
            this.idTextBox.TabIndex = 6;
            this.idTextBox.Text = "ID Input";
            this.idTextBox.WordWrap = false;
            // 
            // imageViewButton
            // 
            this.imageViewButton.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.imageViewButton.Location = new System.Drawing.Point(479, 76);
            this.imageViewButton.Name = "imageViewButton";
            this.imageViewButton.Size = new System.Drawing.Size(93, 28);
            this.imageViewButton.TabIndex = 7;
            this.imageViewButton.Text = "ViewNext";
            this.imageViewButton.UseVisualStyleBackColor = true;
            this.imageViewButton.Click += new System.EventHandler(this.imageViewButton_Click);
            // 
            // imageIDTextBox
            // 
            this.imageIDTextBox.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.imageIDTextBox.Location = new System.Drawing.Point(253, 76);
            this.imageIDTextBox.Multiline = true;
            this.imageIDTextBox.Name = "imageIDTextBox";
            this.imageIDTextBox.Size = new System.Drawing.Size(187, 28);
            this.imageIDTextBox.TabIndex = 8;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(405, 123);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(529, 266);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(940, 123);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(484, 266);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 10;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(405, 395);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(529, 242);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 10;
            this.pictureBox3.TabStop = false;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft YaHei", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(12, 557);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(40, 28);
            this.button1.TabIndex = 5;
            this.button1.Text = "Del";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.fileSelectButton_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(369, 32);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(71, 27);
            this.comboBox2.TabIndex = 1;
            // 
            // upToTopButton
            // 
            this.upToTopButton.Font = new System.Drawing.Font("Microsoft YaHei", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.upToTopButton.Location = new System.Drawing.Point(446, 76);
            this.upToTopButton.Name = "upToTopButton";
            this.upToTopButton.Size = new System.Drawing.Size(22, 28);
            this.upToTopButton.TabIndex = 11;
            this.upToTopButton.Text = "^";
            this.upToTopButton.UseVisualStyleBackColor = true;
            this.upToTopButton.Click += new System.EventHandler(this.upToTopButton_Click);
            // 
            // examManageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.Controls.Add(this.upToTopButton);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.imageIDTextBox);
            this.Controls.Add(this.imageViewButton);
            this.Controls.Add(this.idTextBox);
            this.Controls.Add(this.fileSelectTextBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.fileSelectButton);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.commitButton);
            this.MaximizeBox = false;
            this.Name = "examManageForm";
            this.Text = "Cell ID Register";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button commitButton;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button fileSelectButton;
        private System.Windows.Forms.TextBox fileSelectTextBox;
        private System.Windows.Forms.TextBox idTextBox;
        private System.Windows.Forms.Button imageViewButton;
        private System.Windows.Forms.TextBox imageIDTextBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button upToTopButton;
    }
}

