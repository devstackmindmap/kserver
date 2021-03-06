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

        private void CheckRogulelikeData(int tabDepth)
        {
            string target = "roguelike";
            string target_kor = "스테이지_로그라이크";
            ShowMessage(target_kor + " 데이터 검사", tabDepth);

            var checkFiles = _checkFiles[target];

            var result = CheckCommonData(checkFiles);

            AddErrResult("참조 속성 오류", target_kor, result);
        }

    }
}