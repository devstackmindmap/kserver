

using System;
using System.Net;
using System.Net.Sockets;

namespace AkaLogger
{
    public static class DomainExceptionHandler
    {
        private static string _unhandleSuffix;
        private static string _fceSuffix;

        private static QueueLogger _firstChanceLogger = new QueueLogger();

        public static void Unintialize()
        {
            _firstChanceLogger.Dispose();
        }

        public static void Initialize()
        {
            AppDomain domain = AppDomain.CurrentDomain;
            var applicationName = domain.FriendlyName;
            var hostname = "";

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    hostname = ip.ToString();
                    break;
                }
            }

            _firstChanceLogger.Start();

            _unhandleSuffix = $"{applicationName}:{hostname} - ";
            _fceSuffix = $"{applicationName}:{hostname} - ";

            domain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e)
            {
                Exception ex = (Exception)e.ExceptionObject;
                AkaLogger.Logger.Instance().Fatal(_unhandleSuffix + ex.Message ,ex);

            };

            domain.FirstChanceException += delegate (object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
            {
                Exception ex = (Exception)e.Exception;

                _firstChanceLogger.Log(_fceSuffix + ex.Message, ex);
            };
        }
    }

}