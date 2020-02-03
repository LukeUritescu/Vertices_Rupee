using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameLibrary.GameFiles
{
    public class FileSystem
    {
        protected static FileSystem _filesystem;

        private string path; //Private instance data mamber
        public string Path {
            get { return path; }
            set { this.path = value; }
        }
        public static FileSystem Instance
        {
            get
            {
                if (_filesystem == null)
                    _filesystem = new FileSystem();
                return _filesystem;
            }
        }

        protected FileSystem()
        {
            
        }

        public virtual void CreateFolder(string name)
        {
            //TODO
        }

        public virtual void DeleteFolder(string name)
        {
            //TODO
        }

        public virtual void DeleteFile(string fileName)
        {
            bool fileExists = System.IO.File.Exists(System.IO.Path.Combine(Path, fileName));
            if(fileExists)
            {
                FileInfo fi = new FileInfo(System.IO.Path.Combine(Path, fileName));
                fi.Delete();
            }
        }

        public virtual void CreateTextFile(string fileName, string fileContents)
        {
            
           
            using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine(Path, fileName), true))
            {
                writer.Write(fileContents);
            };
            
        }

        public virtual System.IO.FileStream GetOpenORCreateFileStream(string Name)
        {
            return new System.IO.FileStream(System.IO.Path.Combine(Path, Name), System.IO.FileMode.OpenOrCreate);
        }

        public string ReadTextFile(string Name)
        {
            string fileContents;
            FileStream fileStream = GetOpenORCreateFileStream(Name);
            using (StreamReader reader = new StreamReader(fileStream))
            {
                fileContents = reader.ReadToEnd();
            }
            return fileContents;

        }

    }
}
