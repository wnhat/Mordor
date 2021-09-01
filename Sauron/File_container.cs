using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container;
using Serilog;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace Sauron
{
    class FileManager
    {
        PanelPathManager PathManager;
        List<InspectPC> InsPCList;
        public FileManager()
        {
            this.InsPCList = new List<InspectPC>();
            this.PathManager = new PanelPathManager();

            List<PC> refresh_pc_list = IpTransform.name_to_ip(new InspectSection[] { InspectSection.AVI, InspectSection.SVI, InspectSection.APP });
            foreach (var pc in refresh_pc_list)
            {
                InsPCList.Add(new InspectPC(pc));
            }
        }
        int SortTaskList(Task a, Task b)
        {
            return a.GetHashCode().CompareTo(b.GetHashCode());
        }
        public void RefreshFileList()
        {
            FilePathLogClass.Logger.Information("start to refresh the file dict, time ： {0}", DateTime.Now);
            PanelPathManager newPanelPathManager = new PanelPathManager();

            var disklist = new List<HardDisk>();
            // 将Disk与新的PanelpathManager绑定；
            foreach (var pc in InsPCList)
            {
                foreach (var item in pc.DiskCollection)
                {
                    item.BindNewManager(newPanelPathManager);
                    disklist.Add(item);
                }
            }
            CancellationTokenSource cancel = new CancellationTokenSource();
            Task looptask = Task.Run(() => { Parallel.For(0, disklist.Count, i => { disklist[i].GetDiskPathCollection(); }); }, cancel.Token);
            looptask.Wait(180000);
            if (!looptask.IsCompleted)
            {
                cancel.Cancel();
            }
            if (looptask.IsCanceled)
            {
                ConsoleLogClass.Logger.Information("路径搜寻超过设定时间已被取消，请调查问题原因；");
            }
            FilePathLogClass.Logger.Information("finished Refresh, time is {0}", DateTime.Now);
            PathManager = newPanelPathManager;
            ConsoleLogClass.Logger.Information("开始垃圾收集；");
            GC.Collect();
            ConsoleLogClass.Logger.Information("等待垃圾收集；");
            GC.WaitForPendingFinalizers();
            ConsoleLogClass.Logger.Information("垃圾收集完成；");
        }
        public List<PanelPathContainer> GetPanelPathList(string panel_id)
        {
            if (PathManager.Contains(panel_id))
            {
                return PathManager.PanelPathGet(panel_id);
            }
            else
            {
                return null;
            }
        }
        public PanelPathContainer GetPanelPathList(string panel_id, InspectSection pcsection)
        {
            if (PathManager.Contains(panel_id))
            {
                return PathManager.PanelPathGet(panel_id, pcsection);
            }
            else
            {
                return null;
            }
        }
        public PanelPathContainer GetPanel(string panel_id)
        {
            // return the first matching result;
            var result = GetPanelPathList(panel_id);
            if (result != null)
            {
                return result[0];
            }
            else
            {
                return null;
            }
        }
    }
    //public class DiskPathCollection
    //{
    //    string[] originalPath;
    //    string[] resultPath;

    //    public DiskPathCollection(string[] originalPath, string[] resultPath)
    //    {
    //        this.OriginalPath = originalPath;
    //        this.ResultPath = resultPath;
    //    }

    //    public string[] OriginalPath { get { return originalPath; } set { originalPath = value; } }
    //    public string[] ResultPath { get { return resultPath; } set { resultPath = value; } }
    //}
    class NewHardDisk
    {
        PC PcInfo;
        DiskPart DiskName;
        public DiskStatus Status = DiskStatus.Unchecked;
        string lastErrorMessage = null;
        List<string> PanelList = new List<string>();
        public NewHardDisk(PC parentPc, DiskPart diskName)
        {
            this.PcInfo = parentPc;
            this.DiskName = diskName;
        }
        public void GetDiskPathCollection()
        {
            List<string> newPanelList = new List<string>();
            string originpath = Path.Combine("\\\\", PcInfo.PcIp, "NetworkDrive", DiskName.ToString(), "Defect Info", "Origin");
            string resultpath = Path.Combine("\\\\", PcInfo.PcIp, "NetworkDrive", DiskName.ToString(), "Defect Info", "Result");
            Status = DiskStatus.Unchecked;
            try
            {
                string[] image_directory_list = Directory.GetDirectories(originpath);
                string[] result_directory_list = Directory.GetDirectories(resultpath);
                Status = DiskStatus.OK;
                var intersectlist = Enumerable.Intersect(image_directory_list, result_directory_list, new StringPathCompare());
                foreach (var item in intersectlist)
                {
                    var panelId = Path.GetFileName(item);
                    newPanelList.Add(panelId);
                }
                this.PanelList = newPanelList;
            }
            catch (UnauthorizedAccessException e)
            {
                // 硬盘损坏，该硬盘路径无法通过远程访问的方式打开
                FilePathLogClass.Logger.Error(e.Message);
                Status = DiskStatus.ConnectError;
                lastErrorMessage = e.Message;
            }
            catch (DirectoryNotFoundException e)
            {
                // 硬盘损坏，或该路径下硬盘无实物存在，该硬盘路径无法通过远程访问的方式打开
                FilePathLogClass.Logger.Error(e.Message);
                Status = DiskStatus.NotExist;
                lastErrorMessage = e.Message;
            }
            catch (IOException e)
            {
                // 网络通信问题，网络无法链接到该计算机，可能是因为该计算连接交换机的网线出现了故障，或网卡断开，需要检查设备的硬件原因；
                FilePathLogClass.Logger.Error(e.Message);
                Status = DiskStatus.ConnectError;
                lastErrorMessage = e.Message;
            }
            catch (Exception e)
            {
                FilePathLogClass.Logger.Error(e.Message);
                Status = DiskStatus.ConnectError;
                lastErrorMessage = e.Message;
            }
        }
    }
    class HardDisk
    {
        InspectPC ParentPc;
        DiskPart DiskName;
        public DiskStatus Status = DiskStatus.Unchecked;
        string lastErrorMessage = null;
        PanelPathManager Manager;
        public HardDisk(InspectPC parentPc, DiskPart diskName)
        {
            ParentPc = parentPc;
            DiskName = diskName;
        }
        public void BindNewManager(PanelPathManager newmanager){
            Manager = newmanager;
        }
        public void GetDiskPathCollection()
        {
            string originpath = Path.Combine("\\\\", ParentPc.PcInfo.PcIp, "NetworkDrive", DiskName.ToString(), "Defect Info", "Origin");
            string resultpath = Path.Combine("\\\\", ParentPc.PcInfo.PcIp, "NetworkDrive", DiskName.ToString(), "Defect Info", "Result");
            Status = DiskStatus.Unchecked;
            try
            {
                string[] image_directory_list = Directory.GetDirectories(originpath);
                string[] result_directory_list = Directory.GetDirectories(resultpath);
                Status = DiskStatus.OK;
                var intersectlist = Enumerable.Intersect(image_directory_list, result_directory_list, new StringPathCompare());
                foreach (var item in intersectlist)
                {
                    PanelPathContainer this_panel = new PanelPathContainer(Path.GetFileName(item), ParentPc.PcInfo, this.DiskName);
                    Manager.AddPanelPath(this_panel);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                // 硬盘损坏，该硬盘路径无法通过远程访问的方式打开
                FilePathLogClass.Logger.Error(e.Message);
                Status = DiskStatus.ConnectError;
                lastErrorMessage = e.Message;
            }
            catch (DirectoryNotFoundException e)
            {
                // 硬盘损坏，或该路径下硬盘无实物存在，该硬盘路径无法通过远程访问的方式打开
                FilePathLogClass.Logger.Error(e.Message);
                Status = DiskStatus.NotExist;
                lastErrorMessage = e.Message;
            }
            catch (IOException e)
            {
                // 网络通信问题，网络无法链接到该计算机，可能是因为该计算连接交换机的网线出现了故障，或网卡断开，需要检查设备的硬件原因；
                FilePathLogClass.Logger.Error(e.Message);
                Status = DiskStatus.ConnectError;
                lastErrorMessage = e.Message;
            }
            catch (Exception e)
            {
                FilePathLogClass.Logger.Error(e.Message);
                Status = DiskStatus.ConnectError;
                lastErrorMessage = e.Message;
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

    class InspectPC
    {
        public PC PcInfo;
        public List<HardDisk> DiskCollection = new List<HardDisk>();
        public InspectPC(PC input_pc)
        {
            PcInfo = input_pc;
            foreach (var disk in (DiskPart[])Enum.GetValues(typeof(DiskPart)))
            {
                DiskCollection.Add(new HardDisk(this, disk));
            }
        }
        List<PanelPathContainer> SearchDisk(DiskPart search_disk)
        {
            List<PanelPathContainer> panelList = new List<PanelPathContainer>();
            string diskpath1 = Path.Combine("\\\\", PcInfo.PcIp, "NetworkDrive", search_disk.ToString(), "Defect Info", "Origin");
            string diskpath2 = Path.Combine("\\\\", PcInfo.PcIp, "NetworkDrive", search_disk.ToString(), "Defect Info", "Result");
            try
            {
                string[] image_directory_list = Directory.GetDirectories(diskpath1);
                string[] result_directory_list = Directory.GetDirectories(diskpath2);
                var intersectlist = Enumerable.Intersect(image_directory_list, result_directory_list, new StringPathCompare());
                foreach (var item in intersectlist)
                {
                    PanelPathContainer this_panel = new PanelPathContainer(Path.GetFileName(item), PcInfo, search_disk);
                    panelList.Add(this_panel);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                // 硬盘损坏，该硬盘路径无法通过远程访问的方式打开
                FilePathLogClass.Logger.Error(e.Message);
            }
            catch (DirectoryNotFoundException e)
            {
                // 硬盘损坏，或该路径下硬盘无实物存在，该硬盘路径无法通过远程访问的方式打开
                FilePathLogClass.Logger.Error(e.Message);
            }
            catch (IOException e)
            {
                // 网络通信问题，网络无法链接到该计算机，可能是因为该计算连接交换机的网线出现了故障，或网卡断开，需要检查设备的硬件原因；
                FilePathLogClass.Logger.Error(e.Message);

            }
            return panelList;
        }
        public void Serch_file(PanelPathManager new_panel_list)
        {
            PanelPathManager panel_list = new PanelPathManager();
            foreach (var search_disk in (DiskPart[])Enum.GetValues(typeof(DiskPart)))
            {
                try
                {
                    string diskpath1 = Path.Combine("\\\\", PcInfo.PcIp, "NetworkDrive", search_disk.ToString(), "Defect Info", "Origin");
                    string diskpath2 = Path.Combine("\\\\", PcInfo.PcIp, "NetworkDrive", search_disk.ToString(), "Defect Info", "Result");
                    string[] image_directory_list = Directory.GetDirectories(diskpath1);
                    for (int i = 0; i < image_directory_list.Length; i++)
                    {
                        image_directory_list[i] = image_directory_list[i].Substring(image_directory_list[i].Length - 17);
                    }
                    string[] result_directory_list = Directory.GetDirectories(diskpath2);
                    for (int i = 0; i < result_directory_list.Length; i++)
                    {
                        result_directory_list[i] = result_directory_list[i].Substring(result_directory_list[i].Length - 17);
                    }
                    foreach (var item in image_directory_list.Intersect(result_directory_list))
                    {
                        PanelPathContainer this_panel = new PanelPathContainer(item.Substring(item.Length - 17), PcInfo, search_disk);
                        panel_list.AddPanelPath(this_panel);
                    }
                    foreach (var item in image_directory_list.Except(result_directory_list))
                    {
                        FilePathLogClass.Logger.Information("result or image file not exist; panel id : {0}; path: {1}", item.Substring(item.Length - 17), item);
                    }
                }
                catch (Exception e)
                {
                    FilePathLogClass.Logger.Information("查询文件失败：{0}", e.Message);
                }
            }
            FilePathLogClass.Logger.Information("pc: {0} finishied;", PcInfo.PcIp);
            new_panel_list.AddRange(panel_list);
        }
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
