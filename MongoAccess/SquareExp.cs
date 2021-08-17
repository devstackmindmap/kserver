using AkaConfig;
using AkaData;
using AkaDB;
using AkaDB.MySql;
using AkaEnum;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoAccess
{
    class SquareExp
    {
        public static async Task DoProcess(IMongoDatabase db)
        {
            dbSquareEntitiy[] userList;
            using (var sqldb = new DBContext(11))
            {
                var query = "SELECT userId, objectExp, objectLevel, coreExp, coreLevel, agencyExp,agencyLevel FROM square_object_schedule WHERE enableContents = 1"
                    
            //        +" AND userId = 11"
                    +";";



                var cursor = await sqldb.ExecuteReaderAsync(query);
                userList = cursor.Cast<System.Data.IDataRecord>().Select(record => new dbSquareEntitiy{
                    userId = ((uint)record["userId"]).ToString(),
                    objectExp = (int)record["objectExp"],
                    objectLevel = (uint)record["objectLevel"],
                    coreExp = (int)record["coreExp"],
                    coreLevel = (uint)record["coreLevel"],
                    agencyExp = (int)record["agencyExp"],
                    agencyLevel = (uint)record["agencyLevel"],
                }).ToArray();
            }

            var userIdList = userList.Select(user => user.userId).Where(user => user == "4451").ToArray();



            var squareElements = await Program.GetCollection<Square>(db, "Square", "Stop", userIdList);
            var invadedElements = await Program.GetCollection<Square>(db, "Square", "Invaded", userIdList);

            squareElements = squareElements.GetRange(0, 12);

            var groupSquareElements = squareElements.GroupBy(element => (string)element.fields["fields"]["UserId"]);
            var groupInvadedElements = invadedElements.GroupBy(element => (string)element.fields["fields"]["UserId"]);

            Console.WriteLine(squareElements);

            //2020 05 20
            foreach(var user in groupSquareElements)
            {
                var totalExp = user.Sum(square => {
                    var objectLevel = uint.Parse(square.fields["fields"]["ObjectLevel"].ToString());
                    var boxLevel = uint.Parse(square.fields["fields"]["BoxLevel"].ToString());
                    var destroyed = square.fields["fields"]["Destoryed"].ToString() == "True";

                    var planetBox = Data.GetSquareObjectPlanetBox((uint)objectLevel, (uint)boxLevel);
                    var gettingSquareObjectExp = destroyed ? planetBox.GiveToLoseSquareObjectExp : planetBox.GiveToSquareObjectExp;
                    return gettingSquareObjectExp;
                });

                var dbuser = userList.First(userdb => userdb.userId == user.Key);
                uint maxLevel = 1;
                int maxExp = 0;
                int usedExp = 0;
                int spentGold = 0;
                for (uint i = 1; i < dbuser.objectLevel; i++ )
                {
                    var exp = Data.GetSquareObject(i).NeedExpForNextLevelUp;
                    usedExp += exp;
                    if (maxExp + exp < totalExp)
                    {
                        maxLevel = i+1;
                        maxExp += exp;
                    }
                    else
                    {
                        spentGold += Data.GetSquareObject(i).NeedGoldForNextLevelUp;
                    }
                }

                dbuser.fixedObjectExp = totalExp - maxExp;
                dbuser.fixedObjectLevel = maxLevel;
                dbuser.usedObjectExp = usedExp;
                dbuser.wrongLevel = dbuser.fixedObjectLevel < dbuser.objectLevel;
                dbuser.distLevel = (int)(dbuser.objectLevel - dbuser.fixedObjectLevel);
                dbuser.objectGold = spentGold;

            }

            foreach (var user in groupInvadedElements)
            {
                var totalExp = user.Sum(square => {
                    var monsterId = uint.Parse(square.fields["fields"]["MonsterId"].ToString());
                    var monster = Data.GetSquareObjectMonster(monsterId);
                    return monster?.GiveToPlanetAgencyExp ?? 0;
                });

                var dbuser = userList.First(userdb => userdb.userId == user.Key);
                uint maxLevel = 1;
                int maxExp = 0;
                int usedAgencyExp = 0;
                int spentGold = 0;

                for (uint i = 1; i < dbuser.agencyLevel ; i++)
                {
                    var exp = Data.GetSquareObjectPlanetAgency(i).NeedExpForNextLevelUp;
                    usedAgencyExp += exp;
                    if (maxExp + exp < totalExp)
                    {
                        maxLevel = i+1;
                        maxExp += exp;
                    }
                    else
                    {
                        spentGold += Data.GetSquareObjectPlanetAgency(i).NeedGoldForNextLevelUp;
                    }
                }
                dbuser.fixedAgencyExp = (int)totalExp - maxExp;
                dbuser.fixedAgencyLevel = maxLevel;
                dbuser.usedAgencyExp = usedAgencyExp;
                dbuser.wrongAgencyLevel = dbuser.fixedAgencyLevel < dbuser.agencyLevel;
                dbuser.distAgencyLevel = (int)(dbuser.agencyLevel - dbuser.fixedAgencyLevel);
                dbuser.agencyGold = spentGold;

                maxLevel = 1;
                maxExp = 0;
                int usedCoreExp = 0;
                spentGold = 0;
                for (uint i = 1; i < dbuser.coreLevel ; i++)
                {
                    var exp = Data.GetSquareObjectPlanetCore(i).NeedExpForNextLevelUp;
                    usedCoreExp += exp;
                    if (maxExp + exp < totalExp)
                    {
                        maxLevel = i+1;
                        maxExp += exp;
                    }
                    else
                    {
                        spentGold += Data.GetSquareObjectPlanetCore(i).NeedGoldForNextLevelUp;
                    }
                }

                dbuser.fixedCoreExp = (int)totalExp - maxExp;
                dbuser.fixedCoreLevel = maxLevel;
                dbuser.usedCoreExp = usedCoreExp;
                dbuser.wrongCoreLevel = dbuser.fixedCoreLevel < dbuser.coreLevel;
                dbuser.distCoreLevel = (int)(dbuser.coreLevel - dbuser.fixedCoreLevel);
                dbuser.coreGold = spentGold;

                dbuser.totalGold = dbuser.agencyGold + dbuser.coreGold + dbuser.objectGold;
            }


            var rewardOrdinary = 99306u;
            var rewardGroup = userList.GroupBy(user => user.totalGold)
                                      .OrderBy(group => group.Key)
                                      .Select((group, index) => {
                                          if (group.Key != 0)
                                              foreach (var user in group)
                                                  user.rewardId = rewardOrdinary + (uint)index;
                                          return group.Key;
                                      })
                                      .ToList();





            if (Directory.Exists(".\\list\\") == false)
                Directory.CreateDirectory(".\\list\\");

            var filePath = Path.Combine(".\\list\\", "list.csv");



            var fields = typeof(dbSquareEntitiy).GetFields();
            var column = string.Join("|", fields.Select(field => field.Name));

            List<string> lines = new List<string>() { column };
            lines.AddRange(  userList.Select(user => string.Join("|", fields.Select(field => field.GetValue(user).ToString()))    ));

            await File.WriteAllLinesAsync(filePath,lines);


            var queries = userList.Select(user =>

                "UPDATE square_object_schedule SET " 
                + " objectExp = " + user.fixedObjectExp + ", objectLevel = " + (user.wrongLevel ? user.fixedObjectLevel : user.objectLevel)
                + ",coreExp = " + user.fixedCoreExp + ", coreLevel = " + (user.wrongCoreLevel ? user.fixedCoreLevel : user.coreLevel)
                + ",agencyExp = " + user.fixedAgencyExp + ", agencyLevel = " + (user.wrongAgencyLevel ? user.fixedAgencyLevel : user.agencyLevel)
                + " WHERE userId = " + user.userId + ";"
        //        +
        //        "UPDATE users SET gold = gold + " + user.totalGold
        //        + " WHERE userId = " + user.userId + ";"
            );

            var updateQuery = string.Join("",queries);

            await File.WriteAllLinesAsync(Path.Combine(".\\list\\", "update_exp.sql"), queries);



            var rewardResult = rewardGroup.Select((reward, index) => reward + " GD|" + (rewardOrdinary + (uint)index));
            await File.WriteAllLinesAsync(Path.Combine(".\\list\\", "rewards.csv"), rewardResult);


            var userRewardList = userList.Where(user=>user.rewardId !=0).Select(user =>
            {
                var sql = "insert into knightrun.user_mail_private(userId, isDeleted, startDateTime, endDateTime, mailTitle, mailText, isRead, mailIcon, productId)"
                         + " values(" + user.userId + ", 0, '2020-07-20 00:00:00', '2020-10-20 23:59:59', '스퀘어오브젝트 환급', '스퀘어 오브젝트 레벨업 환급', 0, 'thumb_mail_icon_001',"
                         + user.rewardId + ");";
                return sql;
            });
            await File.WriteAllLinesAsync(Path.Combine(".\\list\\", "update_user_reward.sql"), userRewardList);


            using (var sqldb = new DBContext(11))
            {
                var resultCount = 0;
               await sqldb.BeginTransactionCallback(async () =>
               {
            //       resultCount =await sqldb.ExecuteNonQueryAsync(updateQuery);
                   return true;
               });

                Console.WriteLine("Result : " + resultCount);
            }
        }


    }
}
