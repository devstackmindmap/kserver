using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KnightUWP.Dao
{
    [Table(Name ="users")]
    public class Users : Dao<Users>
    {
        uint _userId;
        public uint userId
        {
            get { return _userId; }
            set { _userId = value; NotifyPropertyChanged(); }

        }

        int _gold;
        public int gold
        {
            get { return _gold; }
            set { _gold = value; NotifyPropertyChanged(); }

        }

        int _gem;
        public int gem
        {
            get { return _gem; }
            set { _gem = value; NotifyPropertyChanged(); }

        }

        uint _level;
        public uint level
        {
            get { return _level; }
            set { _level = value; NotifyPropertyChanged(); }

        }

        UInt64 _exp;
        public UInt64 exp
        {
            get { return _exp; }
            set { _exp = value; NotifyPropertyChanged(); }

        }
    }
}
