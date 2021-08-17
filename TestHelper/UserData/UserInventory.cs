using AkaDB.MySql;
using AkaEnum;
using System;
using System.Collections.Generic;

namespace TestHelper
{

    public class UserInventoryChecker
    {
        public uint _userId;
        public UserInventory _beforeInventory = new UserInventory();
        public UserInventory _afterInventory = new UserInventory();

        public UserInventoryChecker(uint userId)
        {
            _userId = userId;
        }

        public void SetBeforeInventory()
        {
            SetUser(_beforeInventory);
            SetUnits(_beforeInventory);
            SetCards(_beforeInventory);
            SetWeapons(_beforeInventory);
            SetInfusionBox(_beforeInventory);
        }

        public void SetAfterInventory()
        {
            SetUser(_afterInventory);
            SetUnits(_afterInventory);
            SetCards(_afterInventory);
            SetWeapons(_afterInventory);
            SetInfusionBox(_afterInventory);
        }

        private void SetUser(UserInventory userInventory)
        {
            using (var db = new DBContext(_userId))
            {
                var query = $"SELECT * FROM users WHERE userId={_userId};";
                using (var cursor = db.ExecuteReaderAsync(query))
                {
                    if (cursor.Result.Read() == false)
                        throw new Exception();

                    userInventory.Gold = (int)cursor.Result["gold"];
                    userInventory.Gem = (int)cursor.Result["gem"];
                }
            }
        }

        private void SetUnits(UserInventory userInventory)
        {
            using (var db = new DBContext(_userId))
            {
                var query = $"SELECT * FROM units WHERE userId={_userId};";
                using (var cursor = db.ExecuteReaderAsync(query))
                {
                    userInventory.Units = new Dictionary<uint, UnitInventory>();
                    while (cursor.Result.Read())
                    {
                        var id = (uint)cursor.Result["id"];
                        userInventory.Units.Add(id, new UnitInventory
                        {
                            id = id,
                            count = (int)cursor.Result["count"],
                            level = (uint)cursor.Result["level"]
                        });
                    }
                }
            }
        }

        private void SetCards(UserInventory userInventory)
        {
            using (var db = new DBContext(_userId))
            {
                var query = $"SELECT * FROM cards WHERE userId={_userId};";
                using (var cursor = db.ExecuteReaderAsync(query))
                {
                    userInventory.Cards = new Dictionary<uint, CardInventory>();
                    while (cursor.Result.Read())
                    {
                        var id = (uint)cursor.Result["id"];
                        userInventory.Cards.Add(id, new CardInventory
                        {
                            id = id,
                            count = (int)cursor.Result["count"],
                            level = (uint)cursor.Result["level"]
                        });
                    }
                }
            }
        }

        private void SetWeapons(UserInventory userInventory)
        {
            using (var db = new DBContext(_userId))
            {
                var query = $"SELECT * FROM weapons WHERE userId={_userId};";
                using (var cursor = db.ExecuteReaderAsync(query))
                {
                    userInventory.Weapons = new Dictionary<uint, WeaponInventory>();
                    while (cursor.Result.Read())
                    {
                        var id = (uint)cursor.Result["id"];
                        userInventory.Weapons.Add(id, new WeaponInventory
                        {
                            id = id,
                            count = (int)cursor.Result["count"],
                            level = (uint)cursor.Result["level"]
                        });
                    }
                }
            }
        }

        private void SetInfusionBox(UserInventory userInventory)
        {
            using (var db = new DBContext(_userId))
            {
                var query = $"SELECT * FROM infusion_boxes WHERE userId={_userId};";
                using (var cursor = db.ExecuteReaderAsync(query))
                {

                    userInventory.InfusionBox = new Dictionary<InfusionBoxType, InfusionBoxInventory>();
                    while (cursor.Result.Read())
                    {
                        var infusionBoxType = (InfusionBoxType)(System.SByte)cursor.Result["type"];

                        userInventory.InfusionBox.Add(infusionBoxType, new InfusionBoxInventory {
                            BoxEnergy = (int)cursor.Result["boxEnergy"],
                            Id = (uint)cursor.Result["id"],
                            userBonusEnergy = (int)cursor.Result["userBonusEnergy"],
                            userEnergy = (int)cursor.Result["userEnergy"]
                        });
                    }
                }
            }
        }
    }

    public class UserInventory
    {
        public int Gold;
        public int Gem;

        public Dictionary<uint, UnitInventory> Units;
        public Dictionary<uint, CardInventory> Cards;
        public Dictionary<uint, WeaponInventory> Weapons;

        public Dictionary<InfusionBoxType, InfusionBoxInventory> InfusionBox = new Dictionary<InfusionBoxType, InfusionBoxInventory>();

        public List<uint> skins = new List<uint>();
    }

    public class UnitInventory
    {
        public uint id;
        public uint level;
        public int count;
    }

    public class CardInventory
    {
        public uint id;
        public uint level;
        public int count;
    }

    public class WeaponInventory
    {
        public uint id;
        public uint level;
        public int count;
    }

    public class InfusionBoxInventory
    {
        public uint Id;
        public int BoxEnergy;
        public int userEnergy;
        public int userBonusEnergy;
    }
}
