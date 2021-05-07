using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Container
{
    public class FileContainer
    {
        FileInfo FileInformation;
        MemoryStream FileMemory;
        public FileContainer(FileInfo fileInformation)
        {
            FileInformation = fileInformation;
            FileMemory = new MemoryStream();
            ReadFileInMemory();
        }
        public void ReadFileInMemory()
        {
            // TODO: ADD TRY, if read file error,log it;
            FileInformation.OpenRead().CopyTo(FileMemory);
        }
        public void SaveFileInDisk(string savePath)
        {
            // TODO：Async Process;
            FileInfo newsavefile = new FileInfo(Path.Combine(savePath, FileInformation.Name));
            var writestream = newsavefile.OpenWrite();
            FileMemory.CopyTo(writestream);
        }
        public MemoryStream FileFromMemory
        {
            get
            {
                return FileMemory;
            }
        }
        public string Name
        {
            get
            {
                return FileInformation.Name;
            }
        }
    }
    /// <summary>
    /// copy the giving path dir(and it`s subdir) to local memory;
    /// </summary>
    public class DirContainer
    {
        DirectoryInfo DirInfo;
        FileContainer[] FileContainerArray;
        DirContainer[] DirContainerArray;
        public DirContainer(string dirPath)
        {
            DirInfo = new DirectoryInfo(dirPath);
            if (!DirInfo.Exists)
            {
                string errorstring = String.Format("Directory not exist, path:{0}",dirPath);
                throw new FileContainerException(errorstring);
            }
            Initial();
        }
        public void Initial()
        {
            InitialFile();
            InitialDir();
        }
        public void InitialFile()
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
        public void InitialDir()
        {
            DirectoryInfo[] dirarray = DirInfo.GetDirectories();
            if (dirarray.Count() > 0)
            {
                DirContainerArray = new DirContainer[dirarray.Count()];
                for (int i = 0; i < dirarray.Count(); i++)
                {
                    DirContainerArray[i] = new DirContainer(dirarray[i].FullName);
                }
            }
            else
            {
                DirContainerArray = null;
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
    }
    public class FileContainerException : ApplicationException
    {
        public FileContainerException(string message) : base(message)
        {
            
        }
    }
}
