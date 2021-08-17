using MessagePack;
using System;
using System.IO;

namespace AkaSerializer
{
    public static class AkaSerializer<Data>
    {
        public static byte[] Serialize(Data obj)
        {
            try
            {
                return MessagePackSerializer.Serialize<Data>(obj);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        public static Data Deserialize(Stream stream)
        {
            try
            {
                return MessagePackSerializer.Deserialize<Data>(stream);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public static Data Deserialize(byte[] bytes)
        {
            try
            {
                return MessagePackSerializer.Deserialize<Data>(bytes);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
