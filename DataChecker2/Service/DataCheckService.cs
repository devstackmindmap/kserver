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
        private readonly MainVM _dataModel;
        private readonly List<DataCheck> _dataCheckList = new List<DataCheck>();
        private readonly ConcurrentDictionary<string,CommonData> _datas = new ConcurrentDictionary<string, CommonData>();

        private readonly List<string> _report = new List<string>();

        public DataCheckService(MainVM model)
        {
            _dataModel = model;
        }

        public bool CheckPath()
        {
            return Directory.Exists(_dataModel.ReadDataPath) && Directory.Exists(_dataModel.OutputPath);
        }

        public bool LoadCheckerData()
        {
            try
            {
                var targetPath = Path.Combine(_dataModel.ReadDataPath, "data_checklistex.csv");

                var checklist = new CsvFileDescription
                {
                    SeparatorChar = '|',
                    FirstLineHasColumnNames = true,
                    IgnoreUnknownColumns = true,
                    TextEncoding = Encoding.UTF8
                };

                var csvContext = new CsvContext();
                _dataCheckList.Clear();
                _dataCheckList.AddRange(csvContext.Read<DataCheck>(targetPath, checklist));

                return true;
            }
            catch (Exception)
            {

            }

            return false;
        }

        public bool InitChecklist()
        {
            _checkFiles.Clear();

            var categories = _dataCheckList.GroupBy(dataForKey => dataForKey.Category);
            foreach(var category in categories)
            {
                _checkFiles.Add(category.Key, category.Select(datacheck => datacheck.FileName).Distinct().ToArray());
            }


            _checkMeta.Clear();
            var files = _dataCheckList.GroupBy(dataForKey => dataForKey.FileName);
            foreach (var file in files)
            {
                var aggregations = file.Where(data => data.CustomAction == false && data.Property != null && data.AggrFileName != null && data.AggrProperty != null)
                                        .Select(data => CreateAggregationItem(data.Property, (data.AggrFileName, data.AggrProperty)));
                var zeroable = file.Where(data => data.Property != null && data.Zeroable)
                                   .Select(data => data.Property)
                                   .ToList();
                var nullable = file.Where(data => data.Property != null && data.Nullable)
                                                   .Select(data => data.Property)
                                                   .ToList();
                var keys = file.Where(data => data.Property != null && data.IsKey)
                                                   .Select(data => data.Property)
                                                   .ToList();
                _checkMeta.Add(file.Key, new DataCheckMeta
                {
                    Aggregation = CreateAggregation(aggregations.ToArray()),
                    Zeroable = zeroable,
                    Nullable = nullable,
                    Keys = keys
                });
            }
            return true;
        }


        public bool LoadDatas()
        {
            _datas.Clear();
            _report.Clear();

            var files = Directory.GetFiles(_dataModel.ReadDataPath, "*.csv",SearchOption.AllDirectories )
                                         .Where(filename => filename.ToLower().Contains("data_checklist") == false)
                                         ;

            Parallel.ForEach(files, fileName =>
            {
                _datas.TryAdd(Path.GetFileNameWithoutExtension(fileName), CommonData.LoadDataFromFile(fileName));
            });
            return true;
        }

        private void ShowMessage(string message, int tabDepth = 0)
        {
            var tab = "\n";
            for (int i = 0; i < tabDepth; i++)
                tab += "    ";
            _dataModel.ProcessState += tab + message;
        }

        private void AddErrResult(string category, string subject , IDictionary<string,List<string>> result)
        {
            _report.AddRange(
                    result.SelectMany(keyValue =>
                        keyValue.Value.Select(report => string.Join("|", category, subject, keyValue.Key.Replace(".","|"), report)))
                );
        }

        public void WriteReport()
        {
            if (false == Directory.Exists(_dataModel.OutputPath))
                Directory.CreateDirectory(_dataModel.OutputPath);
            var path = Path.Combine(_dataModel.OutputPath, DateTime.Now.ToString("yy-MM-dd_hh-mm") + ".csv");
            _report.Insert(0,string.Join("|","종류","카테고리","검사대상파일","검사대상속성","참조대상파일","참조대상속성","잘못된 값"));
            File.WriteAllLines(path ,_report);


            _dataModel.ProcessState += $"\n검사 완료: 오류 {_report.Count - 1} 건. \n결과:{path}";
        }
         
        private void AddErrResult(string category, string subject, ConcurrentQueue<Dictionary<string, List<string>>> result)
        {
            while (result.TryDequeue(out var errResult))
            {
                AddErrResult(category, subject, errResult);
            }
        }

        public void CheckData()
        {
            ShowMessage("데이터 유효성 검증 시작");

            _dataModel.ProcessState = "검사 시작...";

            CheckData_KeyValidation(1);

            foreach ( var target in _checkFiles.Keys)
            {
                CheckCommonData(1, target);
            }
            /*
            CheckUnitnData(1);
            CheckSkillData(1);
            CheckStageData(1);
            CheckMonsterData(1);
            CheckRogulelikeData(1);
            CheckWeaponData(1);
            CheckRankData(1);
            CheckRewardData(1);
            CheckStoreData(1);
            CheckQuestData(1);
            CheckSquareData(1);
            CheckProfileData(1);
            CheckEmoticonData(1);
            */
        }


        private void CheckCommonData(int tabDepth , string target)
        {
            ShowMessage(target + " 데이터 검사", tabDepth);

            var checkFiles = _checkFiles[target];

            var result = CheckCommonData(checkFiles);

            if (target == "스테이지")
            {
                var stageRoundFileName = "data_stage_round";
                GetDataAndMeta(stageRoundFileName, out var metaData, out var datas);
                result.Enqueue(CheckStageRound(stageRoundFileName, datas));
            }

            AddErrResult("참조 속성 오류", target, result);
        }

        private void CheckData_KeyValidation(int tabDepth)
        {
            ShowMessage("중복 키 오류 검사중...", tabDepth);
            ConcurrentDictionary<string,List<string>> result = new ConcurrentDictionary<string, List<string>>();
            Parallel.ForEach(_AllCheckFiles.ToList(), fileName =>
            {
                if (GetDataAndMeta(fileName,  out var metaData, out var datas))
                {
                    foreach (var key in metaData.Keys)
                    {
                        bool nullableKey = metaData.Nullable.Contains(key);
                        var wrongKeys = datas.GroupBy(data => data[key])
                                                                .Where(group => group.Count() > 1 && (group.Key != null || nullableKey == false))
                                                                .Select(group => group.Key)
                                                                .ToList();
                        if (wrongKeys.Any())
                        {
                            result.TryAdd(fileName + "..." + key, wrongKeys);
                        }
                    }
                }
            });
            AddErrResult("중복 키 오류", "상점", result);
        }

        private Dictionary<string, List<string>> CheckAggregation(string fileName, DataCheckMeta metaData, CommonData datas)
        {
            var errResult = new Dictionary<string, List<string>>();
            foreach (var fileAttr in metaData.Aggregation)
            {
                bool nullableKey = metaData.Nullable.Contains(fileAttr.Key);
                bool zeroableKey = metaData.Zeroable.Contains(fileAttr.Key);
                var checkDatas = datas[fileAttr.Key].Where(data => (data != null || nullableKey) && false == (data == "0" && zeroableKey) );
                foreach (var target in fileAttr.Value)
                {
                    if (_datas.TryGetValue(target.Key, out var targetData) == false)
                    {
                        errResult.TryAdd($"올바르지 않은 참조대상 파일 이름 {fileName}.{fileAttr.Key}.{target.Key}.{target.Value} ", new List<string>() { "" } );
                        break;
                    }
                    else if (targetData.TryGetValue(target.Value, out var values) == false)
                    {
                        errResult.TryAdd($"올바르지 않은 참조대상 속성 이름 {fileName}.{fileAttr.Key}.{target.Key}.{target.Value} ", new List<string>() { "" });
                        break;
                    }
                    else
                    {
                        
                        var wrongProperties = checkDatas.SelectMany(data => data.Split("/")).Except(values).ToList();
                        if (wrongProperties.Any())
                        {
                            errResult.TryAdd( $"{fileName}.{fileAttr.Key}.{target.Key}.{target.Value}", wrongProperties);
                        }
                    }

                }
            }

            return errResult;
        }

        private ConcurrentQueue<Dictionary<string, List<string>>> CheckCommonData(string[] checkFiles)
        {
            ConcurrentQueue<Dictionary<string, List<string>>> result = new ConcurrentQueue<Dictionary<string, List<string>>>();
            Parallel.ForEach(checkFiles, fileName =>
                {
                    if (GetDataAndMeta(fileName, out var metaData, out var datas))
                    {
                        var errResult = CheckAggregation(fileName, metaData, datas);
                        result.Enqueue(errResult);
                    }
                }
            );
            return result;
        }

        private bool GetDataAndMeta(string fileName, out DataCheckMeta metaData, out CommonData datas)
        {
            metaData = null;
            datas = null;
            return _checkMeta.TryGetValue(fileName, out  metaData) && _datas.TryGetValue(fileName, out  datas);
        }
    }
}
 
 