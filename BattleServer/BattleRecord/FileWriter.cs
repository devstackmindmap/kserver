using AkaConfig;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BattleServer.BattleRecord
{
    public class FileWriter : IWriter
    {
        private string _dirPath;
        public FileWriter()
        {
            _dirPath = Environment.ExpandEnvironmentVariables(Config.BattleServerConfig.RecordSetting.LocalStoragePath);
        }

        public async Task<bool> WriteAsync(MemoryStream stream, string recordKey)
        {
            bool result = true;
            try
            {
                if (Directory.Exists(_dirPath) == false)
                    Directory.CreateDirectory(_dirPath);

                var filePath = Path.Combine(_dirPath, recordKey);

                using (FileStream fs = File.Create(filePath))
                {
                    var recordData = stream.ToArray();

                    await fs.WriteAsync(recordData, 0, recordData.Length);
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                AkaLogger.Log.Debug.Exception("Record.Local", e);
                AkaLogger.Logger.Instance().Error("[Record.Local] " + e.ToString());
                result = false;
            }
            return result;
        }
    }

}