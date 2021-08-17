using AkaConfig;
using AkaData;
using AkaDB;
using AkaEnum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using CommonProtocol;
using AkaLogger;
using System.Linq;
using AkaUtility;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace WebServer
{
    public class StartUp
    {
        public StartUp(IConfiguration configuration)
        {
            //AssemblyLoadContext.Default.Unloading += Default_Unloading;

            Config.GameServerInitConfig(Server.GameServer, configuration);
            DBEnv.AllSetUp();

            Network.PubSubConnector.Instance.Connect(Config.GameServerConfig.WebPubServer.ip, Config.GameServerConfig.WebPubServer.port, Config.GameServerConfig.WebPubServer.tryReconnectTime);
           
            AkaRedis.AkaRedis.AddServer(Config.Server, Config.GameServerConfig.GameRedisSetting.ServerSetting,
                Config.GameServerConfig.GameRedisSetting.Password);

            //Json 파일 읽어서 세팅
            string version = configuration.GetValue<string>("DataVersion");
            if (version == null)
            {
                version = configuration.GetValue<string>("AkaEnvironment:" + configuration.GetValue<string>("ENVIRONMENT") + ":DataVersion");
            }

            if (version == null)
                version = "0";

            var loader = new FileLoader(FileType.Table, Config.RunMode, Int32.Parse(version));
            var taskResult = loader.GetFileLists();

            taskResult.Wait();

            DataSetter dataSetter = new DataSetter();
            dataSetter.DataSet(taskResult.Result);
            SetSlang();


            //var appVersion = typeof(Program).Assembly.GetCustomAttributes(false).SingleOrDefault(attribute => attribute.GetType() == typeof(System.Reflection.AssemblyFileVersionAttribute));
            //if (appVersion != null)
            //{
            //    var fileVersionAttribute = appVersion as System.Reflection.AssemblyFileVersionAttribute;

            //    var buildVersion = Versioning.Version;
            //    Console.WriteLine("WebServer Version:" + buildVersion);
            //    Console.WriteLine("Profile:" + configuration.GetValue<string>("ENVIRONMENT"));
            //    Console.WriteLine("Data Version:" + version);

            //}

        }


        private void SetSlang()
        {
            var slangs = Data.GetSlang();
            List<string> strSlangs = new List<string>();
            foreach (var slang in slangs)
                strSlangs.Add(slang.Word);
            SlangFilter.SetSlang(strSlangs);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IApplicationLifetime lifeTime)
        {
            //DomainExceptionHandler.Initialize();
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            lifeTime.ApplicationStopping.Register(OnShutdown);

            app.Run(async context =>
            {
                var featureCollection = context.Features[typeof(IHttpRequestFeature)] as IHttpRequestFeature;
                var url = featureCollection.Path.TrimStart('/');
                
                if (Enum.TryParse<MessageType>(url, out var messageType))
                {
                    var requestInfo = ProtocolFactory.DeserializeProtocol(messageType, featureCollection.Body);
                    var controller = ControllerFactory.CreateController(messageType, context);

                    var responseInfo = await controller.DoPipeline(requestInfo);
                    await context.Response.Body.WriteAsync(ProtocolFactory.SerializeProtocol(messageType, responseInfo));
                }

            });
        }

        private bool _processExited = false;
        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            _processExited = true;
        }

        public void OnShutdown()
        {
            ApplicationQuit();
        }

        private void Default_Unloading(AssemblyLoadContext obj)
        {
            ApplicationQuit();
        }

        private void ApplicationQuit()
        {
            Console.WriteLine("Application Quit");
            AkaLogger.Logger.Instance().Dispose();
            Network.PubSubConnector.Instance.Close();
            Console.WriteLine("PubsubConnection Disposed");
            DomainExceptionHandler.Unintialize();
            Console.WriteLine("Resource Disposed");

            int exitTime = 5;
            var now = DateTime.Now;

            for (int i = 0; (DateTime.Now - now).TotalSeconds < exitTime && _processExited == false; i++)
            {
                Task.Delay(1).Wait();
            }

            if (_processExited == false)
            {
                Console.WriteLine("Application Force quit");
                Process.GetCurrentProcess().Kill();
            }
            Console.WriteLine("Application  exit ");
        }
    }
}