using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container;

namespace EyeOfSauron
{
    public static class Defectcode
    {
        static Dictionary<string, string> translate_table;
        static Defectcode()
        {
            Defect[] defectlist = Parameter.CodeNameList;
            translate_table = new Dictionary<string, string>();
            foreach (var item in defectlist)
            {
                translate_table.Add(item.DefectName, item.DefectCode);
            }
            translate_table.Add("E", " ");
            translate_table.Add("S", " ");
        }

        public static string name2code(string defect_name)
        {
            string return_code = translate_table[defect_name];
            return return_code;
        }

        public static JudgeGrade name2judge(string defect_name)
        {
            if (defect_name == "S")
            {
                return JudgeGrade.S;
            }
            else if(defect_name == "E")
            {
                return JudgeGrade.E;
            }
            else
            {
                return JudgeGrade.F;
            }
        }
    }
}
