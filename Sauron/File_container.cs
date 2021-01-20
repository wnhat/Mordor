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
        ILogger Log;
        IP_TR ip_tr;
        List<INS_pc_manage> Ins_pc_list;

        public FileManager(IP_TR ip_tr_)
        {
            Log = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            this.Ins_pc_list = new List<INS_pc_manage>();
            this.PathManager = new PanelPathManager();
            ip_tr = ip_tr_;

            List<PC> refresh_pc_list = ip_tr.name_to_ip(new List<string> { "AVI"});
            foreach (var pc in refresh_pc_list)
            {
                Ins_pc_list.Add(new INS_pc_manage(pc, Log));
            }
            Refresh_file_list();
        }

        public async void Refresh_file_list()
        {
            Console.WriteLine("start to refresh the file dict, time is {0}", DateTime.Now);
            PanelPathManager newPanelPathManager = new PanelPathManager();
            List<Task> task_list = new List<Task>();
            foreach (var pc in Ins_pc_list)
            {
                var refresh_task = Task.Run(() => pc.Serch_file(newPanelPathManager));
                task_list.Add(refresh_task);
            }
            await Task.WhenAll(task_list);
            Console.WriteLine("finished Refresh, time is {0}", DateTime.Now);
            PathManager = newPanelPathManager;
        }

        public List<PanelPathContainer> GetPanelList(string panel_id)
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
            var result = GetPanelList(panel_id);
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

    class INS_pc_manage : PC
    {
        ILogger Log;
        public INS_pc_manage(PC input_pc, ILogger log)
        {
            this.EqId = input_pc.EqId;
            this.PcIp = input_pc.PcIp;
            this.PcName = input_pc.PcName;
            this.PcSide = input_pc.PcSide;
            Log = log;
        }

        public void Serch_file(PanelPathManager new_panel_list)
        {
            PanelPathManager panel_list = new PanelPathManager();
            List<string> search_list = new Disk_part().getall;  // get serch disk name;
            foreach (var search_disk in search_list)
            {
                try
                {
                    string diskpath1 = Path.Combine(PcIp, "NetworkDrive", search_disk, "Defect Info", "Origin");
                    string diskpath2 = Path.Combine(PcIp, "NetworkDrive", search_disk, "Defect Info", "Result");
                    List<string> image_directory_list = Directory.GetDirectories(diskpath1).ToList();
                    List<string> result_directory_list = Directory.GetDirectories(diskpath2).ToList();
                    image_directory_list.Sort();
                    result_directory_list.Sort();

                    foreach (var item in image_directory_list)
                    {
                        string this_panel_result_dir = result_directory_list.FirstOrDefault(x => { return x.Substring(x.Length-17) == item.Substring(item.Length-17); });
                        if (this_panel_result_dir != null)
                        {
                            PanelPathContainer this_panel = new PanelPathContainer(item.Substring(item.Length - 17), item, this_panel_result_dir, EqId, PcName, search_disk);
                            panel_list.PanelPathAdd(this_panel);
                            result_directory_list.Remove(this_panel_result_dir);
                        }
                        else
                        {
                            Log.Information("result file not exist; panel id : {0}; path: {1}", item.Substring(item.Length - 17), item);
                        }
                    }

                    foreach (var item in result_directory_list)
                    {
                        string this_panel_image_dir = image_directory_list.FirstOrDefault(x => { return x.Substring(x.Length - 17) == item.Substring(item.Length - 17); });
                        if (this_panel_image_dir != null)
                        {
                            PanelPathContainer this_panel = new PanelPathContainer(item.Substring(item.Length - 17), this_panel_image_dir, item, EqId, PcName, search_disk);
                            if (!panel_list.Contains(this_panel.Panel_id))
                            {
                                panel_list.PanelPathAdd(this_panel);
                            }
                            image_directory_list.Remove(this_panel_image_dir);
                        }
                        else
                        {
                            Log.Information("image file not exist; panel id : {0}; path: {0}", item.Substring(item.Length - 17), item);
                        }
                    }
                }

                catch (Exception e)
                {
                    Log.Information("查询文件失败：{0}", e.Message);
                }
            }

            Log.Information("pc: {0} finishied;", PcIp);
            new_panel_list.AddRange(panel_list);
        }
    }
}
