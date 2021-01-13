using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOfSauron
{
    class Manager
    {
        Serverconnecter TheServerConnecter;
        User Operater;

        public Manager()
        {
            TheServerConnecter = new Serverconnecter();
            Operater = null;

        }

        public bool CheckUser(User newUser)
        {
            return TheServerConnecter.check_user_password(newUser.UserId,newUser.PassWord);
        }

        public void OperaterCheckIn(User newUser)
        {
            Operater = newUser;
        }
        public void OperaterCheckOut()
        {
            Operater = null;
        }
    }
}
