using AkaConfig;
using AkaData;
using AkaDB;
using AkaEnum;
using AkaThreading;
using KnightUWP.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace KnightUWP.Servicecs
{
    class ProfileManager
    {
        public static ObservableCollection<StorageFolder> Profiles { get; private set; }
        public static RunMode Current { get; private set; }

        public static ServerInfos ServerInfo { get; private set; }

        public static async Task Load()
        {
            StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
            StorageFolder configPath = await assets.GetFolderAsync("Configs");
            StorageFolder server = await configPath.GetFolderAsync("Server");
            var configs = await server.GetFoldersAsync();

            try
            {
                string[] exceptPath = new string[] { "dev12", "dev21", "develop"};

                var loadJsons = configs.Where(config => exceptPath.Contains(config.DisplayName.ToLower()) == false);
                Profiles = new ObservableCollection<StorageFolder>(loadJsons);
            }
            catch (Exception e)
            {
                //TODO print exception page
            }
        }

        internal static void Load(object clickedItem)
        {
            var profile = clickedItem as StorageFolder;
            // Config.GameServerInitConfig(AkaEnum.Server.GameServer, profile.Name);
            Config.InitConfigInAbsolutePath($"{profile.Path}");

            if (Config.BattleServerConfig.GameRedisSetting.Password?.Length > 0)
            {
                AkaRedis.AkaRedis.AddServer(Config.Server, Config.BattleServerConfig.GameRedisSetting.ServerSetting
                    , Config.BattleServerConfig.GameRedisSetting.Password);
            }

            SemaphoreManager.Add(
                SemaphoreType.MatchServer2GameServerRequestBalancer,
                50,50);
            SemaphoreManager.Add(
                SemaphoreType.BattleServer2GameServerBalancer,
                50, 50);

            var isSuccess = Enum.TryParse<RunMode>(profile.Name, out var runmode);
            Current = runmode;
            DBEnv.AllSetUp();



        }

        internal static async Task LoadDesignDatas()
        {
            await Task.Factory.StartNew(async () =>
            {
                using (var webClient = new System.Net.WebClient())
                {
                    var client2ServerInfoPath = "";
                    if (Current == RunMode.Live)
                        client2ServerInfoPath = "http://download.akastudio.co.kr/Client2ServerInfo.json";
                    else
                        client2ServerInfoPath = "http://download-dev.akastudio.co.kr/Client2ServerInfo.json";

                    var jsonData = await webClient.DownloadDataTaskAsync(client2ServerInfoPath);
                    var jsonString = Encoding.UTF8.GetString(jsonData);

                    var json = Newtonsoft.Json.Linq.JObject.Parse(jsonString);
                    ServerInfo = json.ToObject<ServerInfos>();
                }

                /*
                var fileList = Current != RunMode.Live ? await new FileLoader(FileType.Table, Current.ToString(), ServerInfo.Servers["Android"][Current.ToString()].Version)
                    .GetFileList(ServerInfo.Servers["Android"][Current.ToString()].TableData)
                    : await new FileLoader(FileType.Table, Current.ToString(), 7).GetFileLists(); ;
                    */
                var fileList = await new FileLoader(FileType.Table, Current.ToString(), ServerInfo.Servers["Android"][Current.ToString()].Version)
                    .GetFileList(ServerInfo.Servers["Android"][Current.ToString()].TableData) ;
                new DataSetter().DataSet(fileList);
            });
        }
    }
}
