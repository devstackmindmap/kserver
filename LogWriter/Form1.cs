using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AkaLogger;
using AkaUtility;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using AkaConfig;
using AkaData;
using AkaEnum;


namespace LogWriter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            
            string url = "http://download-dev.akastudio.co.kr/static/assetBundle/test.json";
            //Logger logger = new Logger();

            //logger.User("category:USER", "Login", "아이디", "닉네임", "UDID다", "ADID", 423, 55555, 66666, 7777, 8888, "facebookID", "lobby");
      
            //string responseFromServer = DownloadJson(url);
            
            dynamic json = JsonConvert.DeserializeObject<List<downloadObject>>(DownloadJson(url));
            
            
            foreach(var elmt in json)
            {
                Console.WriteLine("{0}\n",elmt.version.ToString());
            }

                        
            var loader = new FileLoader(FileType.Table, "Dev1", 0);
            var taskResult = loader.GetFileLists();
            Console.WriteLine(loader.ToString());




            //dynamic json2 = JsonConvert.SerializeObject(responseFromServer);

            //downloadObject json = new downloadObject(responseFromServer);

            //foreach (var url in json)
            //{
            //    Console.WriteLine(url);
            //}

            //reader.Close();
            //dataStream.Close();
            //response.Close();

        }

        private void funnel_Click(object sender, EventArgs e)
        {
            AkaLogger.Logger logger = new AkaLogger.Logger();


            logger.Analytics("테스트", "jhkim", "UDID", "유디아이디", "downloadfile", "파일이름", "patch", "패치이름");
            //logger.Analytics("메시지.", "로그이름", "컬럼이름", "컬럼값", "downloadfile", "파일이름", "patch", "패치이름");
            // 로그이름 : 로그를 분류를 위한 키
            // 이후 쌍으로 구성시 데이터를 쌓을수 있음.. 
            // 컬럼이름 : 데이터 컬럼 이름
            // 컬럼값 : 컬럼의 데이터값
            // 주로 분류가 지정하지 않은 상태에서 로그 형태를 확정하기 전에 사용함.

            logger.User("카테고리유저", "Login", "아이디", "닉네임", "UDID다", "ADID", 423, 55555, 66666, 7777, 8888, "facebookID", "lobby");
            //logger.User("메시지.", "Login", "아이디", "닉네임", "UDID다", "ADID", UserLevel-value, FreeGold-value, PaidGold-Value, freeGem-value, paidGem-values, 8888, "소셜계정", "lobby/PVP");
            logger.User("카테고리유저", "Login", "아이디", "닉네임", "UDID다", "ADID", 423, 55555, 66666, 7777, 8888, "facebookID", "lobby"); //로그인
            logger.User("카테고리유저", "Logout", "아이디", "닉네임", "UDID다", "ADID", 423, 55555, 66666, 7777, 8888, "facebookID"); // 로그아웃
            logger.User("카테고리유저", "createID", "아이디", "닉네임", "UDID다", "ADID", "Korea", "verizon", "IOS", "Korean");  //아이디 생성
            logger.User("카테고리유저", "dropID", "아이디", "닉네임", "UDID다", "ADID", "Korea", "verizon", "android", "Korean"); //탈퇴
            logger.User("카테고리유저", "Login", "아이디", "닉네임", "UDID다", "ADID", 423, 55555, 66666, 7777, 8888, "facebookID", "PVP");
        }

        public class downloadObject
        {
            public int version { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string runMode { get; set; }
            public string mdChecksum { get; set; }
        }

        private byte[] DownloadCsv(string url)
        {
            using (var webClient = new System.Net.WebClient())
            {
                var csvBytes = webClient.DownloadData(url);
                return csvBytes;
            }
        }

        private string DownloadJson(string url)
        {
            using (var webClient = new System.Net.WebClient())
            {
                var jsonData = webClient.DownloadString(url);

                return jsonData;
            }
        }
    }
}
