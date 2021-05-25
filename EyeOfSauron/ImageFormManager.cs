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
        PictureBox pictureBox1;
        PictureBox pictureBox2;
        PictureBox pictureBox3;
        DirContainer dir;
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
        public void RefreshForm()
        {
            if (ImageArray != null)
            {
                if ((Refreshflag) * 3 < ImageArray.Count())
                {
                    SetImage(ImageArray.Skip((Refreshflag) * 3).Take(3).ToArray());
                    Refreshflag++;
                }
                else
                {
                    Refreshflag = 0;
                    RefreshForm();
                }
            }
        }
        public void SetImageArray(Bitmap[] imagearray)
        {
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
