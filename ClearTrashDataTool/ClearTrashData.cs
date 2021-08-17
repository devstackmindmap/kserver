using AkaEnum;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace ClearTrashDataTool
{
    public class ClearTrashData
    {
        private IDatabase _gameRedis;
        private IDatabase _matchingRedis;

        public ClearTrashData()
        {
            var defaultDatabase = 0;
            _gameRedis = AkaRedis.AkaRedis.Connectors[Server.GameServer].GetDatabase(defaultDatabase);
            _matchingRedis = AkaRedis.AkaRedis.Connectors[Server.MatchingServer].GetDatabase(defaultDatabase);
        }

        public async Task Run(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("You need Enter ClearType");
                return;
            }
            //PastRanking, Ranking, ClearMatch
            if (args[2] == "PastRanking")
            {
                var clearPastRankingBoards = new ClearPastRankingBoards(_gameRedis);
                await clearPastRankingBoards.ClearRankingBoardAll();
            }

            if (args[2] == "PastRanking")
            {
                if (args.Length < 4)
                {
                    Console.WriteLine("You need Enter Season");
                    return;
                }
                var clearPastRankingBoards = new ClearPastRankingBoards(_gameRedis);
                await clearPastRankingBoards.ClearRankingBoardSeason(uint.Parse(args[3]));
            }

            if (args[2] == "ClearMatch")
            {
                var clearMatchingGroup = new ClearDanglingMatchingGroup(_matchingRedis);
                await clearMatchingGroup.ClearMatchingGroupAll();
            }
        }
    }
}
