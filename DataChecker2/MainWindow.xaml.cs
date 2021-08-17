using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataChecker2.Dao.ViewModel;
using DataChecker2.Service;
using Microsoft.Win32;
using MessageBox = System.Windows.MessageBox;

namespace DataChecker2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainVM _myDefaultData;
        private DataCheckService _dataCheckService ;

        public MainWindow()
        {
            InitializeComponent();

            _myDefaultData = new MainVM();
            _dataCheckService = new DataCheckService(_myDefaultData);
            DataContext = _myDefaultData;
        }
        private async void Click_Run(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            _myDefaultData.EnableProcessing = false;
            Task loadingTask = null;
            var tokenSource = new CancellationTokenSource();
            try
            {

                if (_dataCheckService.CheckPath() == false)
                {
                    ShowMessage("데이터 읽기/결과 저장폴더 경로가 올바르지 않습니다.");
                    return;
                }
                    
                if (_dataCheckService.LoadCheckerData() && _dataCheckService.InitChecklist())
                {
                    ShowMessage("데이터 검증을 시작합니다.");


                    var token = tokenSource.Token;
                    var state = "검사중";
                    _myDefaultData.CurrentState = state;
                    loadingTask = Task.Factory.StartNew(() =>
                        {
                            while (token.IsCancellationRequested == false)
                            {
                                if (_myDefaultData.CurrentState.Length > 20)
                                    _myDefaultData.CurrentState = state;

                                _myDefaultData.CurrentState += ".";
                                Task.Delay(500).Wait();
                            }
                        }
                    ,token);

                    await Task.Factory.StartNew(() =>
                    {
                        _dataCheckService.LoadDatas();

                        _dataCheckService.CheckData();
                    });

                }
                else
                {
                //    ShowMessage("데이터 검증 룰(data_checklistex.csv) 형식이 잘못되었습니다. 개발자에게로~~");
                }
            }
            catch (Exception)
            {
                ShowMessage("데이터 검증에 문제가 있습니다. 개발자에게로~~");
            }
            finally
            {
                _myDefaultData.EnableProcessing = true;
                if (loadingTask != null)
                {
                    tokenSource.Cancel();
                    await loadingTask;
                }
                tokenSource.Dispose();
                _myDefaultData.CurrentState = "검사완료";
                _dataCheckService.WriteReport();
            }
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(this, message, null, MessageBoxButton.OK);
        }


        private void Click_SetOutputPath(object sender, RoutedEventArgs e)
        {
            _myDefaultData.OutputPath = OpenFolderDialog("검증결과를 저장할 폴더를 선택하세요.", _myDefaultData.OutputPath);
        }

        private void Click_SetReadPath(object sender, RoutedEventArgs e)
        {
            _myDefaultData.ReadDataPath = OpenFolderDialog("검증할 데이터 폴더를 선택하세요.", _myDefaultData.ReadDataPath);
        }

        private void Click_resultFolder(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", _myDefaultData.OutputPath);
        }

        private void Click_sourceFolder(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", _myDefaultData.ReadDataPath);
        }

        
        private string OpenFolderDialog(string description, string initPath)
        {
            var openDlg = new FolderBrowserDialog();
            openDlg.UseDescriptionForTitle = true;
            openDlg.Description = description;
            openDlg.SelectedPath = initPath;
            if (openDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return openDlg.SelectedPath;
            }
            return initPath;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _myDefaultData.SaveConfig();
        }
    }
}
