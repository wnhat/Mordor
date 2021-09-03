using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container;
using Serilog;
using System.Threading;
using System.Diagnostics;

namespace RingWraith
{
    class FileManager
    {
        List<PC> InsPCList = IpTransform.name_to_ip(new InspectSection[] { InspectSection.AVI, InspectSection.SVI, InspectSection.APP });
        List<HardDisk> DiskCollection = new List<HardDisk>();

        public FileManager()
        {
            foreach (var pc in InsPCList)
            {
                foreach (var disk in (DiskPart[])Enum.GetValues(typeof(DiskPart)))
                {
                    DiskCollection.Add(new HardDisk(pc, disk));
                };
            }
        }
        public void RefreshFileList()
        {
            FilePathLogClass.Logger.Information("start to refresh the file dict, time ： {0}", DateTime.Now);

            CancellationTokenSource cancel = new CancellationTokenSource();
            Task looptask = Task.Run(() => { Parallel.For(0, DiskCollection.Count, i => { DiskCollection[i].GetDiskPathCollection(); }); }, cancel.Token);
            if (looptask.Wait(200000))
            {
                FilePathLogClass.Logger.Information("路径搜寻正常完成；");
            }
            else
            { 
                FilePathLogClass.Logger.Error("路径搜寻超过设定时间已被取消，请调查问题原因；");
            }
            FilePathLogClass.Logger.Information("finished Refresh, time is {0}", DateTime.Now);
            GC.Collect();
        }
        public Dictionary<string,List<PanelPathContainer>> GetPanelPathList(string[] panelIdList)
        {
            PanelPathManager newManager = new PanelPathManager();
            foreach (var item in DiskCollection)
            {
                item.GetPath(panelIdList, newManager);
            }
            return newManager.PathDict;
        }
    }
    class HardDisk
    {
        PC ParentPc;
        DiskPart DiskName;
        public DiskStatus Status = DiskStatus.Unchecked;
        string lastErrorMessage = null;
        DateTime LastSearchTime = DateTime.UtcNow;
        string[] PathCollection;
        public HardDisk(PC parentPc, DiskPart diskName)
        {
            ParentPc = parentPc;
            DiskName = diskName;
        }
        public void GetDiskPathCollection()
        {
            string originpath = Path.Combine("\\\\", ParentPc.PcIp, "NetworkDrive", DiskName.ToString(), "Defect Info", "Origin");
            string resultpath = Path.Combine("\\\\", ParentPc.PcIp, "NetworkDrive", DiskName.ToString(), "Defect Info", "Result");
            Status = DiskStatus.Unchecked;
            try
            {
                var newTime = Directory.GetLastWriteTimeUtc(originpath);
                if (!(LastSearchTime == newTime))
                {
                    string[] image_directory_list = Directory.GetDirectories(originpath);
                    string[] result_directory_list = Directory.GetDirectories(resultpath);
                    Status = DiskStatus.OK;
                    var intersectlist = Enumerable.Intersect(image_directory_list, result_directory_list, new StringPathCompare());
                    List<string> panelIdList = new List<string>();
                    foreach (var item in intersectlist)
                    {
                        panelIdList.Add(Path.GetFileName(item));
                    }
                    PathCollection = panelIdList.ToArray();
                    LastSearchTime = newTime;
                }
                
            }
            catch (UnauthorizedAccessException e)
            {
                // 硬盘损坏，该硬盘路径无法通过远程访问的方式打开
                FilePathLogClass.Logger.Debug(e.Message);
                Status = DiskStatus.ConnectError;
                lastErrorMessage = e.Message;
                PathCollection = null;
            }
            catch (DirectoryNotFoundException e)
            {
                // 硬盘损坏，或该路径下硬盘无实物存在，该硬盘路径无法通过远程访问的方式打开
                FilePathLogClass.Logger.Debug(e.Message);
                Status = DiskStatus.NotExist;
                lastErrorMessage = e.Message;
                PathCollection = null;
            }
            catch (IOException e)
            {
                // 网络通信问题，网络无法链接到该计算机，可能是因为该计算连接交换机的网线出现了故障，或网卡断开，需要检查设备的硬件原因；
                FilePathLogClass.Logger.Debug(e.Message);
                Status = DiskStatus.ConnectError;
                lastErrorMessage = e.Message;
                PathCollection = null;
            }
            catch (Exception e)
            {
                FilePathLogClass.Logger.Error(e.Message);
                Status = DiskStatus.ConnectError;
                lastErrorMessage = e.Message;
                PathCollection = null;
            }
        }
        public void GetPath(string[] panelIdList, PanelPathManager Manager)
        {
            foreach (var item in panelIdList)
            {
                if (PathCollection.Contains(item))
                {
                    var path = new PanelPathContainer(item,ParentPc,DiskName);
                    Manager.AddPanelPath(path);
                }
            }
        }
    }
    public enum DiskStatus
    {
        Unchecked,
        NotExist,
        ConnectError,
        ConnectOverTime,
        OK,
    }
    class StringPathCompare : IEqualityComparer<string>
    {
        //用于图像及result文件夹路径是否位于同一硬盘的对比；
        public bool Equals(string x, string y)
        {
            if (Path.GetFileName(x) == Path.GetFileName(y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetHashCode(string obj)
        {
            return Path.GetFileName(obj).GetHashCode();
        }
    }
}
