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
        List<Task<DiskPathCollection>> TaskList = new List<Task<DiskPathCollection>>();
        int runcount = 0;
        Random rd = new Random();
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
            return rd.Next().CompareTo(rd.Next());
        }
        public void RefreshFileList()
        {
            runcount = 0;
            FilePathLogClass.Logger.Information("start to refresh the file dict, time is {0}", DateTime.Now);
            PanelPathManager newPanelPathManager = new PanelPathManager();
            TaskList = new List<Task<DiskPathCollection>>();
            foreach (var pc in InsPCList)
            {
                foreach (var item in pc.DiskCollectin)
                {
                    var refresh_task = new Task<DiskPathCollection>(item.RunTask);
                    TaskList.Add(refresh_task);
                }
            }
            TaskList.Sort(SortTaskList);
            while (true)
            {
                while (runcount < 40)
                {
                    TaskList[runcount].Start();
                    runcount++;
                }
                var finishedtask = Task.WhenAny(TaskList);
                finishedtask.Wait();
                finishedtask.Dispose();
                TaskList[runcount].Start();
                runcount++;
                if (runcount == TaskList.Count)
                {
                    break;
                }
            }
            var waittask = Task.WhenAll(TaskList);
            waittask.Wait();
            waittask.Dispose();
            FilePathLogClass.Logger.Information("finished Refresh, time is {0}", DateTime.Now);
            PathManager = newPanelPathManager;
            foreach (var item in TaskList)
            {
                item.Dispose();
            }
            TaskList = null;
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
    public class DiskPathCollection
    {
        string[] originalPath;
        string[] resultPath;

        public DiskPathCollection(string[] originalPath, string[] resultPath)
        {
            this.OriginalPath = originalPath;
            this.ResultPath = resultPath;
        }

        public string[] OriginalPath { get { return originalPath; } set{ originalPath = value; } }
        public string[] ResultPath { get { return resultPath; } set { resultPath = value; } }
    }
    class HardDisk
    {
        InspectPC ParentPc;
        DiskPart DiskName;
        DiskStatus Status = DiskStatus.Unchecked;
        string lastErrorMessage = null;
        public HardDisk(InspectPC parentPc, DiskPart diskName)
        {
            ParentPc = parentPc;
            DiskName = diskName;
        }
        public DiskPathCollection RunTask()
        {
            
            CancellationTokenSource tokensource = new CancellationTokenSource();
            CancellationToken token = tokensource.Token;
            Task newtask = new Task(GetDiskPathCollection,token);
            DateTime starttime = DateTime.Now;
            newtask.Start();
            //newtask.RunSynchronously();
            DateTime endtime = DateTime.Now;
            newtask.Wait(1000);
            DateTime afttime = DateTime.Now;
            //var a = newtask.Result;
            //while (true)
            //{
            //    DateTime endtime = DateTime.Now;
            //    if (endtime - starttime > TimeSpan.FromSeconds(60))
            //    {
            //        break;
            //    }
            //    else
            //    {
            //        Thread.Sleep(100);
            //    }
            //}

            if (newtask.IsCompleted)
            {
                return null;
                //return newtask.Result;
            }
            else
            {
                tokensource.Cancel();
                Status = DiskStatus.ConnectOverTime;
                return null;
            }
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
                //return new DiskPathCollection(image_directory_list, result_directory_list);
            }
            catch (UnauthorizedAccessException e)
            {
                // 硬盘损坏，该硬盘路径无法通过远程访问的方式打开
                FilePathLogClass.Logger.Error(e.Message);
                Status = DiskStatus.ConnectError;
                lastErrorMessage = e.Message;
                //return null;
            }
            catch (DirectoryNotFoundException e)
            {
                // 硬盘损坏，或该路径下硬盘无实物存在，该硬盘路径无法通过远程访问的方式打开
                FilePathLogClass.Logger.Error(e.Message);
                Status = DiskStatus.NotExist;
                lastErrorMessage = e.Message;
                //return null;
            }
            catch (IOException e)
            {
                // 网络通信问题，网络无法链接到该计算机，可能是因为该计算连接交换机的网线出现了故障，或网卡断开，需要检查设备的硬件原因；
                FilePathLogClass.Logger.Error(e.Message);
                Status = DiskStatus.ConnectError;
                lastErrorMessage = e.Message;
                //return null;
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
        public List<HardDisk> DiskCollectin = new List<HardDisk>();
        public InspectPC(PC input_pc)
        {
            PcInfo = input_pc;
            foreach (var disk in (DiskPart[])Enum.GetValues(typeof(DiskPart)))
            {
                DiskCollectin.Add(new HardDisk(this, disk));
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
                        panel_list.PanelPathAdd(this_panel);
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
