using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container;
using Serilog;

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

            List<PC> refresh_pc_list = IpTransform.name_to_ip(new InspectSection[] { InspectSection.AVI,InspectSection.SVI,InspectSection.APP});
            foreach (var pc in refresh_pc_list)
            {
                InsPCList.Add(new InspectPC(pc));
            }
        }
        public async Task RefreshFileList()
        {
            FilePathLogClass.Logger.Information("start to refresh the file dict, time is {0}", DateTime.Now);
            PanelPathManager newPanelPathManager = new PanelPathManager();
            List<Task> task_list = new List<Task>();
            foreach (var pc in InsPCList)
            {
                var refresh_task = Task.Run(() => pc.Serch_file(newPanelPathManager));
                task_list.Add(refresh_task);
            }
            await Task.WhenAll(task_list);
            FilePathLogClass.Logger.Information("finished Refresh, time is {0}", DateTime.Now);
            PathManager = newPanelPathManager;
            GC.Collect();
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
    class InspectPC
    {
        PC PcInfo;
        public InspectPC(PC input_pc)
        {
            PcInfo = input_pc;
        }
        public void SearchFile()
        {
            PanelPathManager panel_list = new PanelPathManager();
            List<Task> searchTaskList = new List<Task>();
            foreach (var search_disk in (DiskPart[])Enum.GetValues(typeof(DiskPart)))
            {
                Task newdisksearch = new Task()
                searchTaskList.Add(new Task());
                panel_list.PanelPathAdd(SearchDisk(search_disk));
            }
        }
        List<PanelPathContainer> SearchDisk(DiskPart search_disk)
        {
            List<PanelPathContainer> panelList = new List<PanelPathContainer>();
            string diskpath1 = Path.Combine("\\\\", PcInfo.PcIp, "NetworkDrive", search_disk.ToString(), "Defect Info", "Origin");
            string diskpath2 = Path.Combine("\\\\", PcInfo.PcIp, "NetworkDrive", search_disk.ToString(), "Defect Info", "Result");
            string[] image_directory_list = Directory.GetDirectories(diskpath1);
            string[] result_directory_list = Directory.GetDirectories(diskpath2);
            foreach (var item in Enumerable.Intersect(image_directory_list, result_directory_list, new StringPathCompare()))
            {
                PanelPathContainer this_panel = new PanelPathContainer(Path.GetFileName(item), PcInfo, search_disk);
                panelList.Add(this_panel);
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
                    string diskpath1 = Path.Combine("\\\\",PcInfo.PcIp, "NetworkDrive", search_disk.ToString(), "Defect Info", "Origin");
                    string diskpath2 = Path.Combine("\\\\",PcInfo.PcIp, "NetworkDrive", search_disk.ToString(), "Defect Info", "Result");
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
            return obj.GetHashCode();
        }
    }
}
