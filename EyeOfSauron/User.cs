using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOfSauron
{
    public class User
    {
        public string UserId;
        public string PassWord;
        int MissionFinished;
        public User(string userid,string password)
        {
            UserId = userid;
            PassWord = password;
            MissionFinished = 0;
        }
    }
}
