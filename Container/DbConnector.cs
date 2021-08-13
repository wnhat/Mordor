using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container
{
    public static class DbConnector
    {
        static DICS_DBEntities db;
        static DbConnector()
        {
            db = new DICS_DBEntities();
        }
        static string TimeNow
        {
            get
            {
                return DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            }
        }

        public static void AddNewLotFromMes(TrayLot lot)
        {

        }

        public static TrayLot GetWaitedMission(ProductInfo info)
        {
            // 向服务器请求对应型号的检查任务；
            var returnlotlist = db.WaitLot.Where(x => x.TrayLot.ProductInfo1 == info);
            if (returnlotlist.Count()==0)
            {
                return null;
            }
            else
            {
                return returnlotlist.First().TrayLot;
            }
        }

        public static void WaitToOninspect(WaitLot waitLot,User op)
        {
            TrayLot lot = waitLot.TrayLot;
            OnInspectLot newOninspectLot = new OnInspectLot { TrayLot = lot, User = op, RequestTime = TimeNow};
            db.OnInspectLot.Add(newOninspectLot);
            db.WaitLot.Remove(waitLot);
        }
    }
}
