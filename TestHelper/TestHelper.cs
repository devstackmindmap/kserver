using AkaConfig;
using AkaData;
using AkaDB;
using AkaEnum;
using AkaUtility;
using System.Collections.Generic;

namespace TestHelper
{
    public class TestHelper
    {
        public static void SetUp(string runMode)
        {
            Config.AllServerInitConfig(runMode);

            DBEnv.AllSetUp();
            AkaRedis.AkaRedis.AllSetUp();
            var loader = new FileLoader(FileType.Table, Config.RunMode, 0);
            var taskResult = loader.GetFileLists();
            taskResult.Wait();

            DataSetter dataSetter = new DataSetter();
            dataSetter.DataSet(taskResult.Result);

            var slangs = Data.GetSlang();
            List<string> strSlangs = new List<string>();
            foreach (var slang in slangs)
                strSlangs.Add(slang.Word);
            SlangFilter.SetSlang(strSlangs);
        }
    }
}
