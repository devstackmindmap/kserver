using System.Collections.Generic;

namespace AkaData
{
    public class CDN
    {
        public int Version { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int Bytes { get; set; }
        public string FileExtensionType { get; set; }
    }

    public class downObj
    {
        public int TotalVersion { get; set; }
        public List<CDN> CDN { get; set; }
    }
}
