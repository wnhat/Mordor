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
            
            TrayLot newlot = new TrayLot { TrayGroupName = lot.TrayGroupName, AddTime = lot.AddTime, MachineName = lot.MachineName, ProductInfo = lot.ProductInfo };
            db.TrayLot.Add(newlot);
            db.SaveChanges();
            db.WaitLot.Add(new WaitLot { LotId = newlot.Lotid });
            db.SaveChanges();
            foreach (var item in lot.Panel)
            {
                var buffer = (PanelMissionFromMES)item;
                Panel newpanel = buffer.OriginPanel;
                newpanel.LotId = newlot.Lotid;
                newpanel.TrayLot = newlot;
                db.Panel.Add(newpanel);
            }
            db.SaveChanges();
        }
        public static List<ProductInfo> GetProductInfo()
        {
            var infoList = from info in db.OninspectProduct
                           select info.ProductInfo;
            return infoList.ToList();
        }
        public static TrayLot GetWaitedMission(ProductInfo info, User op)
        {
            // 向服务器请求对应型号的检查任务；
            var returnlotlist = from lot in db.WaitLot
                                where lot.TrayLot.ProductInfo == info.IndexId
                                select lot;
            if (returnlotlist.Count()==0)
            {
                return null;
            }
            else
            {
                var waitlot = returnlotlist.First();
                var returnlot = waitlot.TrayLot;

                db.WaitLot.Remove(waitlot);
                db.SaveChanges();
                AddOninspectLot(returnlot, op);

                return returnlot;
            }
        }
        static void AddOninspectLot(TrayLot lot, User op)
        {
            OnInspectLot newOninspectLot = new OnInspectLot { Lotid = lot.Lotid, RequestOp = op.IndexId, RequestTime = TimeNow};
            db.OnInspectLot.Add(newOninspectLot);
            db.SaveChanges();
        }

        public static void finishInspect(MissionLot lot)
        {
            var removelot = from item in db.OnInspectLot
                            where item.Lotid == lot.lot.Lotid
                            select item;
            if (removelot.Count() == 0)
            {
                string errorstring = String.Format("完成任务时出现问题，TrayGroupName {0}",lot.lot.TrayGroupName);
                throw new ArgumentNullException(errorstring);
            }
            else
            {
                db.OnInspectLot.Remove(removelot.First());
                foreach (var item in lot.panelcontainer)
                {
                    var newresult = new InspectResult
                    {
                        operaterId = item.Op.IndexId,
                        Indexid = item.mesPanel.IndexId,
                        LOTGRADE = item.LotGrade.ToString(),
                        LOTDETAILGRADE = item.PanelJudge.ToString(),
                        DefectCode = item.DefectByOp.DefectCode,
                        DefectName = item.DefectByOp.DefectName,
                        EndTime = TimeNow,
                    };
                    db.InspectResult.Add(newresult);
                }
                db.SaveChanges();
            }
        }
        public static void InsertExamResult(ExamMission mission)
        {
            var newresult = new AET_IMAGE_EXAM_RESULT {  };
        }
        public static List<AET_IMAGE_EXAM> GetExam()
        {
            return db.AET_IMAGE_EXAM.ToList();
        }
        public static User GetOp(User op)
        {
            var newop = from operater in db.User
                        where operater.UserId == op.UserId && operater.PassWord == op.PassWord
                        select operater;
            if (newop.Count() == 0)
            {
                return null;
            }
            else
            {
                return newop.First();
            }
            
        }
    }
}
