using AkaConfig;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BattleServer.BattleRecord
{
    public class NullWriter : IWriter
    {
        public NullWriter()
        {
        }

        public async Task<bool> WriteAsync(MemoryStream stream, string recordKey)
        {
            return true;
        }
    }

}