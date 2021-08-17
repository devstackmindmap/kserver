using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataChecker2.Dao.Datas;
using DataChecker2.Dao.Enums;
using DataChecker2.Dao.ViewModel;
using LINQtoCSV;
using DataRow = DataChecker2.Dao.Datas.DataRow;

namespace DataChecker2.Service
{
    public partial class DataCheckService
    {

        private void CheckStageData(int tabDepth)
        {
            string target = "stage";
            string target_kor = "스테이지";
            ShowMessage(target_kor + " 데이터 검사", tabDepth);

            var checkFiles = _checkFiles[target];
            var result = CheckCommonData(checkFiles);

            var stageRoundFileName = "data_stage_round";
            GetDataAndMeta(stageRoundFileName, out var metaData, out var datas);
            result.Enqueue(CheckStageRound(stageRoundFileName, datas));

            AddErrResult("참조 속성 오류", target_kor, result);
        }

        private Dictionary<string, List<string>> CheckStageRound(string fileName, CommonData datas)
        {
            var errResult = new Dictionary<string, List<string>>();
            var checkDatas = datas["BackgroundImageId"];

            var backgroundData = _datas["data_background"];
            var backgroundIds = backgroundData["BackgroundId"];
            var backgroundImageIds = backgroundData["BackgroundImageId"];

            var wrongProperties = checkDatas.Except(backgroundImageIds).Except(backgroundIds).ToList();
            if (wrongProperties.Any())
            {
                errResult.TryAdd($"{fileName}.BackgroundImageId.data_background.BackgroundId_OR_BackgroundImageId", wrongProperties);
            }


            return errResult;
        }
    }
}