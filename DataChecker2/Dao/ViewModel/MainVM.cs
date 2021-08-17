using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using DataChecker2.Dao.Enums;

namespace DataChecker2.Dao.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        private readonly ConcurrentDictionary<string,object> _dataModel = new ConcurrentDictionary<string, object>();


        public event PropertyChangedEventHandler PropertyChanged;


        public MainVM()
        {
            SelectReadData = true;
            RunModes = Enum.GetNames(typeof(RunMode));
            SelectedRunMode = RunModes[0];
            SelectIndexDiffTarget = RunModes[0];
            EnableProcessing = true;
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AkaDataChecker");
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            path = Path.Combine(path, "config");
            if (File.Exists(path))
            {
                var pathlist = File.ReadAllLines(path);
                if (pathlist?.Length > 0)
                    ReadDataPath = pathlist[0];

                if (pathlist?.Length > 1)
                    OutputPath = pathlist[1];
            }
        }

        public void SaveConfig()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AkaDataChecker");
            path = Path.Combine(path, "config");
            File.WriteAllLines(path, new string[] { ReadDataPath?.ToString() ?? "", OutputPath?.ToString() ?? "" });
        }

        protected void SetValue(object setValue, [System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            _dataModel.AddOrUpdate(propertyName, setValue, (key,val)=>setValue);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected T GetValue<T>([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            return (T)_dataModel.GetOrAdd(propertyName, default(T));
        }

        public string[] RunModes { get; }


        public string ProcessState
        {
            get => GetValue<string>();
            set => SetValue(value);
        }


        public string CurrentState
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public bool EnableProcessing
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public bool SelectReadData
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public bool SelectReadRunMode
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public string ReadDataPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string SelectedRunMode
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string OutputPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public bool DoDiffDeployedRunMode
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public string SelectIndexDiffTarget
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

    }
}
