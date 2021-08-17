using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KnightUWP.Dao
{
    [Table(Name ="accounts", DB = "account", DBContext = "AccountDBSetting")]
    public class Accounts : Dao<Accounts>
    {
        uint _userId;
        public uint userId
        {
            get { return _userId; }
            set { _userId = value; NotifyPropertyChanged(); }

        }

        string _nickName ="";
        public string nickName
        {
            get { return _nickName; }
            set { _nickName = value; NotifyPropertyChanged(); }

        }

        string _socialAccount = "";
        public string socialAccount
        {
            get { return _socialAccount; }
            set { _socialAccount = value; NotifyPropertyChanged(); }

        }
        
        DateTime _loginDateTime = DateTime.Now;
        public DateTime loginDateTime
        {
            get { return _loginDateTime; }
            set { _loginDateTime = value; NotifyPropertyChanged(); }

        }

        DateTime _joinDateTime = DateTime.Now;
        public DateTime joinDateTime
        {
            get { return _joinDateTime; }
            set { _joinDateTime = value; NotifyPropertyChanged(); }

        }


        int _currentSeason;
        public int currentSeason
        {
            get { return _currentSeason; }
            set { _currentSeason = value; NotifyPropertyChanged(); }

        }

        uint _maxRankLevel;
        public uint maxRankLevel
        {
            get { return _maxRankLevel; }
            set { _maxRankLevel = value; NotifyPropertyChanged(); }

        }


        int _currentSeasonRankPoint;
        public int currentSeasonRankPoint
        {
            get { return _currentSeasonRankPoint; }
            set { _currentSeasonRankPoint = value; NotifyPropertyChanged(); }

        }

        string _friendCode = "";
        public string friendCode
        {
            get { return _friendCode; }
            set { _friendCode = value; NotifyPropertyChanged(); }

        }

        uint _initDataVersion;
        public uint initDataVersion
        {
            get { return _initDataVersion; }
            set { _initDataVersion = value; NotifyPropertyChanged(); }

        }

    }
}
