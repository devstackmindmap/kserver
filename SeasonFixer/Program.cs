using AkaConfig;
using AkaData;
using AkaEnum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SeasonFixer
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var loader = new FileLoader(FileType.Table, "Live", 10);
            var fileList = await loader.GetFileList("http://download.akastudio.co.kr/table/Live/10/table.json");
            DataSetter dataSetter = new DataSetter();
            dataSetter.DataSet(fileList);


            var idAndPoint =File.ReadAllLines(".\\season1rank.txt");

            Dictionary<int, int> idWithPoint = new Dictionary<int, int>(); 
            for( int i =0;i<idAndPoint.Length;i+=2)
            {
                idWithPoint.Add(int.Parse(idAndPoint[i]), int.Parse(idAndPoint[i + 1]));
            }

            var rows = Data.GetPrimitiveValues<uint, DataUserRank>(DataType.data_user_rank);

            var start = 0;
            var oldRankpointLevel = rows.Select( (dataUserRank,index) => {
                start += dataUserRank.NeedRankPointForNextLevelUp;
                return (index, start);
            }).ToList();

            var correctRankpointLevel = rows.Select( (dataUserRank,index) => (index,dataUserRank.NeedRankPointForNextLevelUp)).ToList();


            var rewardIds = Data.GetPrimitiveValues<uint, DataSeasonReward>(DataType.data_season_reward).Select(data => data.RewardId);
            var weaponTokens = rewardIds.Select(rewardId => (rewardId, Data.GetReward(rewardId).ItemIdList.Select(itemId => Data.GetItem(itemId).First()).First(item => item.ItemType == ItemType.StarCoin))).ToList();

            List<string> result = new List<string>();
            foreach(var idpoint in idWithPoint)
            {
                var oldRankLevel = oldRankpointLevel.First(point => idpoint.Value < point.start);
                var correctRankLevel = correctRankpointLevel.First(point => idpoint.Value < point.NeedRankPointForNextLevelUp);

                result.Add(string.Join("|", idpoint.Key, idpoint.Value, oldRankLevel.index + 1, oldRankLevel.start, correctRankLevel.index + 1, correctRankLevel.NeedRankPointForNextLevelUp
                                          , weaponTokens[oldRankLevel.index].rewardId, weaponTokens[oldRankLevel.index].Item2.MinNumber, weaponTokens[correctRankLevel.index].rewardId, weaponTokens[correctRankLevel.index].Item2.MinNumber)
                    );
            }

            File.WriteAllLines(".\\season1rankresult.csv", new string[] { "UserId|Point|OldRank|OldRankPoint|CorrectRank|CorrectRankPoint|GivenCoinId|GiveCoin|CorrectCoinId|CorrectCoin" });
            File.AppendAllLines(".\\season1rankresult.csv", result);
           
        }
    }
}
