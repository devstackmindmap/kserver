using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;
using System.Collections.Concurrent;
using SuperSocket.ClientEngine;
using System.Threading;
using AkaEnum.Battle;
using System.Collections.ObjectModel;
using KnightUWP.Servicecs.Protocol;
using Windows.UI.Xaml;
using AkaData;
using Windows.UI.Xaml.Media;
using System.ComponentModel;
using KnightUWP.Servicecs;
using Microsoft.Toolkit.Uwp.Helpers;

namespace KnightUWP.Dao
{
    public class UnitInfo :  INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            });
        }

        private void NotifyPropertyChanged_Status([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (VOProvider.Instance.EnableClientState == false)
                return;

            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            });
        }


        public UnitInfo()
        {
        }

        public string Name { get; set; }
        public uint UnitId { get; set; }

        public ImageSource Image { get; set; }

        public uint Level { get; set; }

        int _shield;
        public int Shield
        {
            get { return _shield; }
            set { _shield = value <=0 ? 0 : value; NotifyPropertyChanged_Status(); }
        }

        int _hp;
        public int Hp
        {
            get { return _hp; }
            set { _hp = value <= 0 ? 0 : value; NotifyPropertyChanged_Status(); }
        }

        bool isDeath;
        public bool IsDeath
        {
            get { return isDeath; }
            set { isDeath = value; NotifyPropertyChanged_Status(); }
        }

        public string Skin { get; internal set; }
    }
}
