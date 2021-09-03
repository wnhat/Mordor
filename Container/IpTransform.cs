using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Container
{
    public static class IpTransform
    {
        public static List<PC> PcListAll;
        static IpTransform()
        {
            StreamReader file = new StreamReader(@"D:\RingWraith\IP.json");
            StringReader file_string = new StringReader(file.ReadToEnd());
            JObject jsonobj = JObject.Parse(file_string.ReadToEnd());
            PcListAll = (List<PC>)jsonobj.GetValue("PcListAll").ToObject(typeof(List<PC>));
        }
        public static List<PC> name_to_ip(List<int> eq_id_list, InspectSection[] pc_name_list, Side[] pc_side_list)
        {
            return PcListAll.Where(x => { return x.IsPcInType(eq_id_list, pc_name_list, pc_side_list); }).ToList();
        }
        public static List<PC> name_to_ip(List<int> eq_id_list, InspectSection[] pc_name_list)
        {
            Side[] pc_side_list = (Side[])Enum.GetValues(typeof(Side));
            return name_to_ip(eq_id_list, pc_name_list, pc_side_list);
        }
        public static List<PC> name_to_ip(List<int> eq_id_list)
        {
            Side[] pc_side_list = (Side[])Enum.GetValues(typeof(Side));
            InspectSection[] pc_name_list = (InspectSection[])Enum.GetValues(typeof(InspectSection));
            return name_to_ip(eq_id_list, pc_name_list, pc_side_list);
        }
        public static List<PC> name_to_ip(InspectSection[] pc_name_list)
        {
            // GROUP BY PC name;
            Side[] pc_side_list = (Side[])Enum.GetValues(typeof(Side));
            List<int> eq_id_list = Enumerable.Range(1, 33).ToList();
            return name_to_ip(eq_id_list, pc_name_list, pc_side_list);
        }
    }

}
