using AkaEnum;
using MessagePack;

namespace AkaData
{
    [MessagePackObject]
    public class ProtoFileInfo 
    {
        [Key(0)]
        public string Url { get; set; }

        [Key(1)]
        public string Name { get; set; }

        [Key(2)]
        public ulong Version { get; set; }

        [Key(3)]
        public FileExtensionType FileExtensionType { get; set; }
    }
}
