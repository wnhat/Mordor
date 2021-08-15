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
            db.TrayLot.Add(lot);

            foreach (var item in lot.Panel)
            {
                db.Panel.Add(item);
            }
            db.WaitLot.Add(new WaitLot { TrayLot = lot });

            db.SaveChanges();
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

        public static TrayLot WaitToOninspect(WaitLot waitLot,User op)
        {
            TrayLot lot = waitLot.TrayLot;
            OnInspectLot newOninspectLot = new OnInspectLot { TrayLot = lot, User = op, RequestTime = TimeNow};
            db.OnInspectLot.Add(newOninspectLot);
            db.WaitLot.Remove(waitLot);
            return lot;
        }

        public static InspectResult finishInspect()
        {
            
        }
    }
}
