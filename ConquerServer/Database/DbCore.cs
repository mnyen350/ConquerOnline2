using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConquerServer.Database
{
    using DirectoryIO = System.IO.Directory;

    public class DbCore
    {
        public string Directory { get; set; }

        public DbCore(string databaseFolder)
        {
            var info = new DirectoryInfo(databaseFolder);

            Directory = info.FullName;
        }

        public string SelectTable(string name)
        {
            return Path.Combine(Directory, name);
        }

        public string SelectRawFile(string table, string name, bool create = false)
        {
            var result = string.IsNullOrEmpty(table)
                             ? Path.Combine(Directory, name)
                             : Path.Combine(Directory, table, name);

            if (create && !File.Exists(result))
            {
                File.WriteAllText(result, string.Empty);
            }

            return result;
        }

        public void DeleteFile(string table, string name)
        {
            var fname = SelectRawFile(table, name);
            if (File.Exists(fname))
                File.Delete(fname);
        }

        public string[] SelectAllFiles(string table)
        {
            return DirectoryIO.GetFiles(this.SelectTable(table));
        }

        public DbFile SelectFile(string table, string file)
        {
            string raw = SelectRawFile(table, file);
            return new DbFile(raw);
        }

        public DbNode SelectNode(string table, string file, string section)
        {
            return SelectFile(table, file)[section];
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public bool FileExists(string table, string name)
        {
            return File.Exists(SelectRawFile(table, name));
        }

        public bool TableExists(string table)
        {
            return DirectoryIO.Exists(SelectTable(table));
        }

        public void CreateTable(string name)
        {
            string dir = SelectTable(name);
            if (!DirectoryIO.Exists(dir))
                DirectoryIO.CreateDirectory(dir);
        }

        public void DeleteTable(string name)
        {
            string dir = SelectTable(name);
            if (DirectoryIO.Exists(dir))
                DirectoryIO.Delete(dir, true);
        }
    }
}
