using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Container
{
    public class FileContainer
    {
        FileInfo fileInformation;
        MemoryStream FileMemory = new MemoryStream();
        public bool ReadComplete
        {
            get
            {
                return FileMemory.CanRead;
            }
        }
        public FileContainer(FileInfo fileInformation)
        {
            this.fileInformation = fileInformation;
        }
        public FileContainer(string filepath)
        {
            this.fileInformation = new FileInfo(filepath);
        }
        public void ReadFileInMemory()
        {
            try
            {
                fileInformation.OpenRead().CopyTo(FileMemory);
            }
            catch
            {
                string errorstring = String.Format("file Read Error,path:{0}", fileInformation.FullName);
                throw new FileContainerException(errorstring);
            }
            
        }
        public void SaveFileInDisk(string savePath)
        {
            // TODO：Async Process;
            if (FileMemory == null)
            {
                ReadFileInMemory();
            }
            FileInfo newsavefile = new FileInfo(Path.Combine(savePath, fileInformation.Name));
            var writestream = newsavefile.OpenWrite();
            FileMemory.CopyTo(writestream);
        }
        public MemoryStream FileFromMemory
        {
            get
            {
                if (FileMemory == null)
                {
                    ReadFileInMemory();
                }
                return FileMemory;
            }
        }
        public string Name
        {
            get
            {
                return fileInformation.Name;
            }
        }
        public FileInfo FileInformation { get { return fileInformation; } }
    }
    /// <summary>
    /// copy the giving path dir(and it`s subdir) to local memory;
    /// </summary>
    public class DirContainer
    {
        DirectoryInfo DirInfo;
        FileContainer[] FileContainerArray = null;
        DirContainer[] DirContainerArray = null;
        public string[] FileNameList
        {
            get
            {
                List<string> filenamelist = new List<string>();
                if (FileContainerArray != null)
                {
                    foreach (var item in FileContainerArray)
                    {
                        filenamelist.Add(item.Name);
                    }
                }
                if (DirContainerArray != null)
                {
                    foreach (var dir in DirContainerArray)
                    {
                        foreach (var item in dir.FileNameList)
                        {
                            filenamelist.Add(item);
                        }
                    }
                }
                return filenamelist.ToArray();
            }
        }
        public string Name { get { return DirInfo.Name; } }
        public DateTime CreationTime{get{return DirInfo.CreationTime;}}
        public DirContainer(string dirPath)
        {
            Initial(dirPath,true);
        }
        public DirContainer(string dirPath,bool downloadflag)
        {
            Initial(dirPath,downloadflag);
        }
        public void Initial(string dirPath, bool downloadflag)
        {
            DirInfo = new DirectoryInfo(dirPath);
            if (!DirInfo.Exists)
            {
                string errorstring = String.Format("Directory not exist, path:{0}", dirPath);
                throw new FileContainerException(errorstring);
            }
            InitialFile();
            InitialDir(downloadflag);
            if (downloadflag)
            {
                Read();
            }
        }
        void InitialFile()
        {
            FileInfo[] filearray = DirInfo.GetFiles();
            if (filearray.Count() > 0)
            {
                FileContainerArray = new FileContainer[filearray.Count()];
                for (int i = 0; i < FileContainerArray.Count(); i++)
                {
                    FileContainerArray[i] = new FileContainer(filearray[i]);
                }
            }
        }
        void InitialDir(bool downloadflag)
        {
            DirectoryInfo[] dirarray = DirInfo.GetDirectories();
            if (dirarray.Count() > 0)
            {
                DirContainerArray = new DirContainer[dirarray.Count()];
                for (int i = 0; i < dirarray.Count(); i++)
                {
                    DirContainerArray[i] = new DirContainer(dirarray[i].FullName, downloadflag);
                }
            }
        }
        public bool ReadComplete
        {
            get
            {
                if (FileContainerArray != null)
                {
                    foreach (var item in FileContainerArray)
                    {
                        if (!item.ReadComplete)
                        {
                            return false;
                        }
                    }
                }
                if (DirContainerArray != null)
                {
                    foreach (var item in DirContainerArray)
                    {
                        if (!item.ReadComplete)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
        public bool CheckFileReaded(string[] fileNameList)
        {
            foreach (var item in fileNameList)
            {
                var file = GetFileContainer(item);
                if (!file.ReadComplete)
                {
                    return false;
                }
            }
            return true;
        }
        public void Read()
        {
            if (FileContainerArray != null)
            {
                foreach (var item in FileContainerArray)
                {
                    item.ReadFileInMemory();
                }
            }
            if (DirContainerArray != null)
            {
                foreach (var item in DirContainerArray)
                {
                    item.Read();
                }
            }
        }
        public void Read(string[] filenamelist)
        {
            foreach (var item in filenamelist)
            {
                GetFileFromMemory(item);
            }
        }
        public void SaveReadedDirInDisk(string savePath)
        {
            DirectoryInfo savetarget = new DirectoryInfo(savePath);
            DirectoryInfo subDir = savetarget.CreateSubdirectory(DirInfo.Name);
            foreach (var file in FileContainerArray)
            {
                if (file.ReadComplete)
                {
                    file.SaveFileInDisk(subDir.FullName);
                }
            }
            foreach (var Dir in DirContainerArray)
            {
                Dir.SaveDirInDisk(subDir.FullName);
            }
        }
        public void SaveDirInDisk(string savePath)
        {
            DirectoryInfo savetarget = new DirectoryInfo(savePath);
            DirectoryInfo subDir = savetarget.CreateSubdirectory(DirInfo.Name);
            foreach (var file in FileContainerArray)
            {
                file.SaveFileInDisk(subDir.FullName);
            }
            foreach (var Dir in DirContainerArray)
            {
                Dir.SaveDirInDisk(subDir.FullName);
            }
        }
        public MemoryStream GetFileFromMemory(string fileName)
        {
            if (FileContainerArray != null)
            {
                foreach (var file in FileContainerArray)
                {
                    if (file.Name == fileName)
                    {
                        return file.FileFromMemory;
                    }
                }
            }
            if (DirContainerArray != null)
            {
                foreach (var Dir in DirContainerArray)
                {
                    var returnvalue = Dir.GetFileFromMemory(fileName);
                    if (returnvalue != null)
                    {
                        return returnvalue;
                    }
                }
            }
            return null;
        }
        public FileContainer GetFileContainer(string fileName)
        {
            if (FileContainerArray != null)
            {
                foreach (var file in FileContainerArray)
                {
                    if (file.Name == fileName)
                    {
                        return file;
                    }
                }
            }
            if (DirContainerArray != null)
            {
                foreach (var Dir in DirContainerArray)
                {
                    var returnvalue = Dir.GetFileContainer(fileName);
                    if (returnvalue != null)
                    {
                        return returnvalue;
                    }
                }
            }
            return null;
        }
        public bool Contains(string filename)
        {
            if (FileNameList.Contains(filename))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Contains(string[] filename)
        {
            string[] filenamelist = this.FileNameList;
            foreach (var item in filename)
            {
                if (!filenamelist.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }
    }
    public class FileContainerException : ApplicationException
    {
        public FileContainerException(string message) : base(message)
        {
            
        }
    }

}
