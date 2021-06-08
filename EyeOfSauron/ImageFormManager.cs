using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Container;

namespace EyeOfSauron
{
    /// <summary>
    /// using for manage sourceimage and image refreshing method；
    /// </summary>
    class ImageFormManager
    {
        int Refreshflag = 0;
        Bitmap[] ImageArray;
        string[] ImageNameArray;
        PictureBox pictureBox1;
        PictureBox pictureBox2;
        PictureBox pictureBox3;
        Label label1;
        Label label2;
        Label label3;
        public ImageFormManager(params PictureBox[] boxlist)
        {
            pictureBox1 = boxlist[0];
            pictureBox2 = boxlist[1];
            pictureBox3 = boxlist[2];
        }
        private void SetImage(Bitmap[] imagearray)
        {
            pictureBox1.Image = imagearray[0];
            pictureBox2.Image = imagearray[1];
            pictureBox3.Image = imagearray[2];
        }
        public void BindLabel(params Label[] labellist)
        {
            this.label1 = labellist[0];
            this.label2 = labellist[1];
            this.label3 = labellist[2];
        }
        public void RefreshForm()
        {
            if (ImageArray != null)
            {
                if ((Refreshflag) * 3 < ImageArray.Count())
                {
                    SetImage(ImageArray.Skip((Refreshflag) * 3).Take(3).ToArray());
                    if (label1!=null)
                    {
                        SetLabel(ImageNameArray.Skip((Refreshflag) * 3).Take(3).ToArray());
                    }
                    Refreshflag++;
                }
                else
                {
                    Refreshflag = 0;
                    RefreshForm();
                }
            }
        }
        public void SetLabel(string[] stringarray)
        {
            label1.Text = stringarray[0];
            label2.Text = stringarray[1];
            label3.Text = stringarray[2];
        }
        public void SetArray(Bitmap[] imagearray, string[] imagenamearray)
        {
            Refreshflag = 0;
            ImageArray = imagearray;
            ImageNameArray = imagenamearray;
            RefreshForm();
        }
        public void SetImageArray(Bitmap[] imagearray)
        {
            Refreshflag = 0;
            ImageArray = imagearray;
            RefreshForm();
        }
        public void SetImageArray(MemoryStream[] streamarray)
        {
            Bitmap[] imagearray = new Bitmap[streamarray.Length];
            for (int i = 0; i < streamarray.Length; i++)
            {
                imagearray[i] = new Bitmap(streamarray[i]);
            }
            SetImageArray(imagearray);
        }
    }
}