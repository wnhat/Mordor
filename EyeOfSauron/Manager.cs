using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOfSauron
{
    public class Manager
    {
        Serverconnecter TheServerConnecter;
        User Operater;
        MissionPackage TheMissionPackage;
        Parameter SystemParameter;
        public Manager()
        {
            SystemParameter = GetParameter();
            TheServerConnecter = new Serverconnecter();
            Operater = null;
            //TheMissionPackage = new MissionPackage(SystemParameter);
        }

        public bool CheckUser(User newUser)
        {
            return TheServerConnecter.check_user_password(newUser.UserId, newUser.PassWord);
        }
        public void OperaterCheckIn(User newUser)
        {
            Operater = newUser;
        }
        public void OperaterCheckOut()
        {
            Operater = null;
        }

        public Parameter GetParameter()
        {
            //string ip_path = @"D:\1218180\program2\c#\Mordor\EyeOfSauron\sysconfig.json";
            //StreamReader file = new StreamReader(ip_path);
            //StringReader file_string = new StringReader(file.ReadToEnd());
            //JsonSerializer file_serial = new JsonSerializer();
            //Parameter newparameter = (Parameter)file_serial.Deserialize(new JsonTextReader(file_string), this.GetType());
            //return newparameter;
            return null;
        }
        public void SaveParameter()
        {

        }

        public void InspectFinished(string defectName)
        {
            var finishedMission = TheMissionPackage.OnInspectedMission.Finish(defectName);
            TheServerConnecter.FinishMission(finishedMission);
            TheMissionPackage.NewMission();
        }
        public Bitmap[] GetOnInspectPanelImage()
        {
            var imageInMemory = TheMissionPackage.OnInspectedMission.GetImageArray();
            int imageCount = imageInMemory.Count();
            Bitmap[] returnArray = new Bitmap[imageCount];
            for (int i = 0; i < imageCount; i++)
            {
                returnArray[i] = new Bitmap(imageInMemory[i]);
            }
            return returnArray;
        }

    }
}
