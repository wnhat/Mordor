using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container;

namespace EyeOfSauron
{
    class Defectcode
    {
        Dictionary<string, string> translate_table;

        public Defectcode()
        {
            translate_table = new Dictionary<string, string>
            {
                {"污渍"       , "DE" },
                {"竖-条纹"    , "DE" },
                {"横-条纹"    , "DE" },
                {"ETC"        , "DE" },
                {"暗团"       , "DE" },
                {"亮团"       , "DE" },
                {"E"          , "" },
                {"S"          , "" },
            };
        }

        public string name2code(string defect_name)
        {
            string return_code = translate_table[defect_name];
            return return_code;
        }

        public JudgeGrade name2judge(string defect_name)
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
