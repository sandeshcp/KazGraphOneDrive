using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ByteSizeLib;
using System.Threading.Tasks;

namespace KazGraph.DataServices
{
    public class FileItem
    {
        public FileItem()
        {
            Children = new List<FileItem>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public long? Size { get; set; }
        public string SizeName
        {
            get
            {
                return ByteSize.FromBytes((double)(Size ?? 0.0)).ToString();
            }
        }
        public DateTimeOffset? Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public string Path { get; set; }
        public List<FileItem> Children { get; set; }

        public void Add(FileItem child)
        {
            Children.Add(child);
        }
    }
}