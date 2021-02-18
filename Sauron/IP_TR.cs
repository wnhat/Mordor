using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Container;

namespace Sauron
{
    class IP_TR
    {
        public List<PC> PcListAll;
        public IP_TR(){}
        public IP_TR(string ippath)
        {
            StreamReader file = new StreamReader(ippath);
            StringReader file_string = new StringReader(file.ReadToEnd());
            JsonSerializer file_serial = new JsonSerializer();
            PcListAll = ((IP_TR)file_serial.Deserialize(new JsonTextReader(file_string), typeof(IP_TR))).PcListAll;
        }
        public List<PC> name_to_ip(List<int> eq_id_list, InspectSection[] pc_name_list, Side[] pc_side_list)
        {
            return PcListAll.Where(x => { return x.IsPcInType(eq_id_list, pc_name_list, pc_side_list); }).ToList();
        }
        public List<PC> name_to_ip(List<int> eq_id_list, InspectSection[] pc_name_list)
        {
            Side[] pc_side_list = (Side[])Enum.GetValues(typeof(Side));
            return name_to_ip(eq_id_list, pc_name_list, pc_side_list);
        }
        public List<PC> name_to_ip(List<int> eq_id_list)
        {
            Side[] pc_side_list = (Side[])Enum.GetValues(typeof(Side));
            InspectSection[] pc_name_list = (InspectSection[])Enum.GetValues(typeof(InspectSection));
            return name_to_ip(eq_id_list, pc_name_list, pc_side_list);
        }
        public List<PC> name_to_ip(InspectSection[] pc_name_list)
        {
            // GROUP BY PC name;
            Side[] pc_side_list = (Side[])Enum.GetValues(typeof(Side));
            List<int> eq_id_list = Enumerable.Range(1, 33).ToList();
            return name_to_ip(eq_id_list, pc_name_list, pc_side_list);
        }
    }

}
