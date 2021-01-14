using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOfSauron
{
    class Manager
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
            TheMissionPackage = new MissionPackage(
                SystemParameter.PreLoadQuantity,
                SystemParameter.SavePath,
                SystemParameter.ImageNameList);

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
            string ip_path = @"D:\1218180\program2\c#\Mordor\Sauron\IP.json";
            StreamReader file = new StreamReader(ip_path);
            StringReader file_string = new StringReader(file.ReadToEnd());
            JsonSerializer file_serial = new JsonSerializer();
            Parameter newparameter = (Parameter)file_serial.Deserialize(new JsonTextReader(file_string), this.GetType());
            return newparameter;
        }

        public void SaveParameter()
        {

        }
    }
}
