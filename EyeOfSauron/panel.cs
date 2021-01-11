using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Panel
    {
        DirectoryInfo origin_directory;
        DirectoryInfo local_directory;
        public string panel_id;

        //Dictionary<string, byte[]> image_dict = new Dictionary<string, byte[]>();
        Dictionary<string, MemoryStream> image_dict = new Dictionary<string, MemoryStream>();

        bool copyflag;

        private string[] image_list = new string[] {"08_G64_CAM00.bmp", "01_WHITE_CAM00.bmp" };
        
        public Panel(string panel_id_,string origin_path_, string local_path_)
        {
            // origin_path_ like : "\\172.16.170.11\NetworkDrive\G_Drive\Defect Info\Origin\780A070001A9AAH04"
            // local_path_ like : "D:\eye of sauron\runtime\1218180"
            panel_id = panel_id_;
            origin_directory = new DirectoryInfo(origin_path_);
            local_directory = new DirectoryInfo(Path.Combine(local_path_, origin_directory.Name));

            // check if local directory is exist,in case of the operator had inspect one panel for twise;
            Delete_filedirectory();
            // copy file and directory to local.
            copyflag = Copy_filetolocal();
            // read the image form disk to memory.
            Readimage();
        }

        // TODO: read file through net -->memory -->save image
        public bool Copy_filetolocal()
        {
            // copy origin_directory 中的图像到 local_path ，copy 的图像序号遵循image_list
            // copy 成功返回 TRUE 失败返回FALSE。
            if (origin_directory.Exists)
            {
                FileInfo[] files = origin_directory.GetFiles();
                foreach (FileInfo sourcefile in files)
                {
                    // 判断该image是否在需求拷贝的imagelist中；
                    if (image_list.Contains(sourcefile.Name))
                    sourcefile.CopyTo(Path.Combine(local_directory.FullName, sourcefile.Name));
                }
                
                return true;
            }
            else
            {
                Console.WriteLine("Source or local path does not exist!");
                return false;
            }
        }

        public bool Readimage()
        {
            // read the image form disk to memory.
            foreach (string filename in image_list)
            {
                Bitmap imgFullSize = new Bitmap(Path.Combine(local_directory.FullName, filename));
                MemoryStream stmimage = new MemoryStream();
                imgFullSize.Save(stmimage, System.Drawing.Imaging.ImageFormat.Bmp);
                imgFullSize.Dispose();
                image_dict.Add(filename, stmimage);
                imgFullSize.Dispose();
            }

            return false;
        }

        private void Delete_filedirectory()
        {
            // check if local directory is exist,in case of the operator had inspect one panel for twise;
            local_directory.Refresh();
            if (local_directory.Exists)
            {
                // delete the directory even if there is files in directory.
                FileInfo[] filelist = local_directory.GetFiles();
                if (filelist.Length == 0)
                {
                    local_directory.Delete();
                }
                else
                {
                    foreach (FileInfo file in filelist)
                    {
                        file.Delete();
                    }
                    local_directory.Delete();
                }
            }

            local_directory.Refresh();
            if (!local_directory.Exists)
            {
                local_directory.Create();
            }
        }

        public MemoryStream get_image()
        {
            return image_dict["08_G64_CAM00.bmp"];
        }

        
    }
}
