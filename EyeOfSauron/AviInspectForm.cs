using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOfSauron
{
    class AviInspectForm : InspectForm
    {
        public AviInspectForm(LoginForm parentForm, Manager theManager) : base(parentForm, theManager)
        {

            //imagelabel1.Text = TheManager.SystemParameter.ImageNameList[0];
            //imagelabel2.Text = TheManager.SystemParameter.ImageNameList[1];
            //imagelabel3.Text = TheManager.SystemParameter.ImageNameList[2];
        }
    }
}
