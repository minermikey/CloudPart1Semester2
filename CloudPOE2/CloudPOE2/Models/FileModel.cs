namespace CloudPOE2.Models
{
    public class FileModel
    {

        public string Name { get; set; } // what is the name fo the file 
        public long Size { get; set; } // how large is the file
        public DateTimeOffset? LastModified { get; set; }

        public string DisplaySize
        {
            get
            {
                if (Size >= 1024 * 1024)
                    return $"{Size / 1024 / 1024} MB";
                if (Size >= 1024)
                    return $"{Size / 1024} KB";
                return $"{Size} Bytes";
            }
        }

    }
}
