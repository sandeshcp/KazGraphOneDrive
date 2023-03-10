using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KazGraph.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class CreatedBy
    {
        public User user { get; set; }
    }

    public class File
    {
        public string mimeType { get; set; }
        public Hashes hashes { get; set; }
    }

    public class FileSystemInfo
    {
        public DateTime createdDateTime { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
    }

    public class Folder
    {
        public int childCount { get; set; }
    }

    public class Hashes
    {
        public string quickXorHash { get; set; }
    }

    public class LastModifiedBy
    {
        public User user { get; set; }
    }

    public class ParentReference
    {
        public string driveType { get; set; }
        public string driveId { get; set; }
        public string id { get; set; }
        public string path { get; set; }
    }

    public class clsOneDriveRoot
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }

        [JsonProperty("@odata.deltaLink")]
        public string odatadeltaLink { get; set; }
        public List<clsOneDriveRootValue> value { get; set; }
    }



    public class User
    {
        public string email { get; set; }
        public string id { get; set; }
        public string displayName { get; set; }
    }

    public class clsOneDriveRootValue
    {
        //[JsonProperty("@odata.type")]
        //public string odatatype { get; set; }
        public DateTime createdDateTime { get; set; }
        public string id { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public string webUrl { get; set; }
        public int size { get; set; }
        public ParentReference parentReference { get; set; }
        public FileSystemInfo fileSystemInfo { get; set; }
        public Folder folder { get; set; }
        //public clsOneDriveRoot root { get; set; }
        public string eTag { get; set; }
        public string cTag { get; set; }
        public CreatedBy createdBy { get; set; }
        public LastModifiedBy lastModifiedBy { get; set; }
        public File file { get; set; }

        public string ChildNode => parentReference.path + "/" + name;
    }


}