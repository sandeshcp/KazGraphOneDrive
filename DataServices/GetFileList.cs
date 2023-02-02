using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KazGraph.DataServices
{
    public class GetFileList
    {
        public Guid FileListID { get; set; }
        public Guid FileImportID { get; set; }
        public string FileName { get; set; }
        public long FileSizeByte { get; set; }
        public string FilePath { get; set; }
        public DateTime? FileCreatedDate { get; set; }
        public DateTime? FileLastModifiedDate { get; set; }
        public Guid? FileListIDParent { get; set; }
        public bool IsFolder { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}