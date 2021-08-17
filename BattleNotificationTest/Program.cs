using System;
using System.Windows.Forms;

namespace BattleNotificationTest
{
    static class Program
    {
        public static string runMode;
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            runMode = args[0];
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += Application_ApplicationExit;
            Application.Run(new BattleNotificationForm());
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (MatchingServerConnector.Instance != null)
                MatchingServerConnector.Instance.Close();
        }
    }
}
