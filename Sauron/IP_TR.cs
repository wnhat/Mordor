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
        public List<PC> pc_list_all;
        public IP_TR()
        {
            string ip_path = @"D:\1218180\program2\c#\server\IP.json";
            StreamReader file = new StreamReader(ip_path);
            StringReader file_string = new StringReader(file.ReadToEnd());
            JsonSerializer file_serial = new JsonSerializer();
            pc_list_all = ((PcContainer)file_serial.Deserialize(new JsonTextReader(file_string), typeof(PcContainer))).pc_list_all;
        }

        public List<PC> name_to_ip(List<int> eq_id_list, List<string> pc_name_list, List<string> pc_side_list)
        {
            return pc_list_all.Where(x => { return x.IsPcInType(eq_id_list, pc_name_list, pc_side_list); }).ToList();
        }

        public List<PC> name_to_ip(List<int> eq_id_list, List<string> pc_name_list)
        {
            List<string> pc_side_list = new List<string> { "LEFT", "RIGHT", null};
            return name_to_ip(eq_id_list, pc_name_list, pc_side_list);
        }

        public List<PC> name_to_ip(List<int> eq_id_list)
        {
            List<string> pc_side_list = new List<string> { "LEFT", "RIGHT", null };
            List<string> pc_name_list = new List<string> { "AVI", "SVI", "APP", "MAIN", "MTP", "PRE", "POST"};
            return name_to_ip(eq_id_list, pc_name_list, pc_side_list);
        }

        public List<PC> name_to_ip(List<string> pc_name_list)
        {
            // GROUP BY PC name;
            List<string> pc_side_list = new List<string> { "LEFT", "RIGHT", null };
            List<int> eq_id_list = Enumerable.Range(1, 33).ToList();
            return name_to_ip(eq_id_list, pc_name_list, pc_side_list);
        }
    }

}
