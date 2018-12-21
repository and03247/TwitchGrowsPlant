using System;
using System.Diagnostics;
using System.Timers;

namespace TwitchToHue
{
    using System.Configuration;
    using System.Net.Sockets;
    using System.Threading;

    internal class Program
    {
        #region Globals
        private readonly string userName = ConfigurationManager.AppSettings["username"];
        private readonly string password = ConfigurationManager.AppSettings["password"];
        private readonly string ip = ConfigurationManager.AppSettings["ip"];
        private readonly string port = ConfigurationManager.AppSettings["port"];
        private readonly string room = ConfigurationManager.AppSettings["room"];
        private IrcClient irc;
        private string readData = "";
        private Thread chatThread;
        private NetworkStream serverStream;
        private readonly HueClient hueClient = new HueClient();
        private const string LightsKeyword = "!lights ";
        private const int CommandTimeout = 15000;

        // Timer
        private System.Timers.Timer timer;

        private bool isTimedOut = false;

        #endregion

        private static void Main(string[] args)
        {
            var program = new Program();
            program.OnStartup();
        }

        private void OnStartup()
        {
            irc = new IrcClient(ip, int.Parse(port), userName, password);

            irc.JoinRoom(room);
            chatThread = new Thread(GetMessage);
            chatThread.Start();

            // Initialize command timeout timer
            timer = new System.Timers.Timer();
            timer.Elapsed += (pingTimer, ea) => PingTimeEvent(this, ea, this);
            timer.Interval = CommandTimeout;
            timer.Enabled = true;
            timer.Start();
        }

        private void Msg() // This is where everything is dealt with in chat: commands, automatic timeouts, etc.
        {
            try
            {
                Debug.WriteLine(readData);
                Console.WriteLine(readData);
                if (readData.Contains("PRIVMSG"))
                {
                    var msgseparator = new[] { "#" + this.room + " :" };
                    var user = readData.Split('!')[0];
                    user = user.Substring(1); // get rid of the starting colon
                    var message = readData.Split(msgseparator, StringSplitOptions.None)[1];

                    Console.WriteLine("Message: " + message);
                    if (message.StartsWith(LightsKeyword) && !isTimedOut)
                    {
                        this.isTimedOut = true;
                        var color = message.Split(' ')[1];
                        var response = hueClient.ChangeLightToColor(color);
                        irc.SendChatMessage(response);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void GetMessage()
        {
            serverStream = irc.TcpClient.GetStream();

            while (true)
            {
                try
                {
                    readData = irc.ReadMessage();
                    Msg();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        #region TimerEvents

        private static void PingTimeEvent(object source, ElapsedEventArgs e, Program program)
        {
            program.isTimedOut = false;
        }

        #endregion

    }
}
