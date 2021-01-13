using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container;

namespace EyeOfSauron
{
    class MissionPackage
    {
        private PanelMission OnInspectedMission;
        private Queue<PanelMission> UnDownloadedMissionQueue;
        private int DownloadQuantity;
        public string ImageSavingPath { get; set; }

        public MissionPackage(int downloadQuantity, string imageSavingPath)
        {
            UnDownloadedMissionQueue = new Queue<PanelMission>();
            DownloadQuantity = downloadQuantity;
            ImageSavingPath = imageSavingPath;
        }



        public void AddMission(PanelMission newmission)
        {
            UnDownloadedMissionQueue.Enqueue(newmission);
        }

        public void DownloadFile()
        {

        }
    }

    class InspectMission
    {
        private PanelMission MissionInfo;
        private MemoryStream ImageBuffer;
        private string[] ImageNameList;
        private string SavePath;

        public InspectMission(PanelMission missionInfo, string[] imageNameList, string savePath)
        {
            MissionInfo = missionInfo;
            ImageBuffer = new MemoryStream();
            ImageNameList = imageNameList;
            SavePath = savePath;
        }

        public void DownloadFile()
        {
            DirectoryInfo srcdir = new DirectoryInfo(MissionInfo.PanelPath.Result_path);
            if (srcdir.Exists)
            {
                srcdir.
            }
        }

        public bool Readimage()
        {
            // read the image form disk to memory.
            foreach (string filename in ImageNameList)
            {
                Bitmap imgFullSize = new Bitmap(Path.Combine(MissionInfo., filename));
                imgFullSize.Save(ImageBuffer, System.Drawing.Imaging.ImageFormat.Bmp);
                imgFullSize.Dispose();
            }

            return false;
        }

    }

}
