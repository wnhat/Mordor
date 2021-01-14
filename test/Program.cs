using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> imagenamelist = new List<string> { "04_WHITE_Pre_Input.jpg" , "06_G64_Pre_Input.jpg ","08_G64-2_Pre_Input.jpg"};
            string FilePath = @"\\172.16.180.51\NetworkDrive\H_Drive\Defect Info\Result\761L0Z0002A3BAP05";
            DirectoryInfo imagefile = new DirectoryInfo(FilePath);
            FileInfo[] filearray = imagefile.GetFiles();
            foreach (var file in filearray)
            {
                if (imagenamelist.Contains(file.Name))
                {
                    MemoryStream memo = new MemoryStream();
                    file.OpenRead().CopyTo(memo);
                }
            }

        }
    }
}
