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
        ILogger Logger;
        IP_TR ip_tr;
        List<INS_pc_manage> InsPCList;

        public FileManager(IP_TR ip_tr_)
        {
            Logger = new LoggerConfiguration()
                .WriteTo.File(@"D:\eye of sauron\log\filemanager\log-.txt", rollingInterval: RollingInterval.Hour)
                .CreateLogger();
            this.InsPCList = new List<INS_pc_manage>();
            this.PathManager = new PanelPathManager();
            ip_tr = ip_tr_;

            List<PC> refresh_pc_list = ip_tr.name_to_ip(new InspectSection[] { InspectSection.AVI,InspectSection.SVI,InspectSection.APP});
            foreach (var pc in refresh_pc_list)
            {
                InsPCList.Add(new INS_pc_manage(pc, Logger));
            }
            RefreshFileList();

        }
        public async void RefreshFileList()
        {
            Logger.Information("start to refresh the file dict, time is {0}", DateTime.Now);
            PanelPathManager newPanelPathManager = new PanelPathManager();
            List<Task> task_list = new List<Task>();
            foreach (var pc in InsPCList)
            {
                var refresh_task = Task.Run(() => pc.Serch_file(newPanelPathManager));
                task_list.Add(refresh_task);
            }
            await Task.WhenAll(task_list);
            Logger.Information("finished Refresh, time is {0}", DateTime.Now);
            PathManager = newPanelPathManager;
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
    class ExamManager
    {
        
    }
    class INS_pc_manage
    {
        ILogger Log;
        PC PcInfo;
        public INS_pc_manage(PC input_pc, ILogger log)
        {
            PcInfo = input_pc;
            Log = log;
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
                        Log.Information("result or image file not exist; panel id : {0}; path: {1}", item.Substring(item.Length - 17), item);
                    }
                }

                catch (Exception e)
                {
                    Log.Information("查询文件失败：{0}", e.Message);
                }
            }

            Log.Information("pc: {0} finishied;", PcInfo.PcIp);
            new_panel_list.AddRange(panel_list);
        }
    }
}
