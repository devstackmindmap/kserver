using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AkaUtility;
using AkaEnum;
using Newtonsoft.Json;

namespace AkaData
{
    public class FileLoader
    {
        private FileType _fileType;
        private string _strRunMode;
        private int _dataVersion;

        public FileLoader(FileType fileType, string strRunMode, int dataVersion)
        {
            _fileType = fileType;
            _strRunMode = strRunMode;

            var isSuccess = Enum.TryParse<RunMode>(strRunMode, out var runMode);
            _dataVersion = isSuccess ?  GetDataVersion(runMode, dataVersion) : 0;
        }

        private int GetDataVersion(RunMode runMode, int dataVersion)
        {
            return runMode == RunMode.Live || runMode == RunMode.Review || runMode == RunMode.Staging ? dataVersion : 0;
        }

        public async Task<List<ProtoFileInfo>> GetFileLists()
        {
            if (_fileType.ToString() == "Table")
            {
                //string url = $"{AkaConfig.Config.CommonServerConfig.DownloadUrl}/static/table/{_strRunMode}/{_dataVersion}/table.json";

                // 정식코드
                //string url = $"{AkaConfig.Config.CommonServerConfig.DownloadUrl}/table/{_strRunMode}/{_dataVersion}/table.json"; 

                // 임시코드
                string url = $"{AkaConfig.Config.CommonServerConfig.DownloadUrl}/data/table.json";
                downObj json = JsonConvert.DeserializeObject<downObj>(DownloadJson(url));
                return ParseFileInfoList(json, false);
            }
            else
            {
                string url = $"{AkaConfig.Config.CommonServerConfig.DownloadUrl}/assetBundle/{_strRunMode}/{_dataVersion}/assetBundle.json";
                downObj json = JsonConvert.DeserializeObject<downObj>(DownloadJson(url));
                return ParseFileInfoList(json, false);
            }
        }

        public async Task<List<ProtoFileInfo>> GetFileList(string url)
        {
            downObj json = JsonConvert.DeserializeObject<downObj>(DownloadJson(url));
            return ParseFileInfoList(json, false);
        }

        private List<ProtoFileInfo> ParseFileInfoList(downObj json, bool isCheckingTableSelection)
        {
            List<ProtoFileInfo> fileInfos = new List<ProtoFileInfo>();

            for (int i = 0; i < json.CDN.Count; i++)
            {
                var protoFileInfo = ParseFileInfo(json.CDN[i]);
                fileInfos.Add(protoFileInfo);
            }

            return fileInfos;
        }

        private ProtoFileInfo ParseFileInfo(CDN list)
        {
            var name = list.Name.ToString();
            ulong version = ulong.Parse(list.Version.ToString());
            var extensionName = list.FileExtensionType.ToString();
            var fileExtensionType = (FileExtensionType)Enum.Parse(typeof(FileExtensionType), extensionName);
            string url = list.Url;

            return new ProtoFileInfo
            {
                Name = name,
                Version = version,
                Url = url,
                FileExtensionType = fileExtensionType
            };
        }

        public string DownloadJson(string url)
        {
            using (var webClient = new System.Net.WebClient())
            {
                var jsonData = webClient.DownloadString(url);

                return jsonData;
            }
        }
    }
}


