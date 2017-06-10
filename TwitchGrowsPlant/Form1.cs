using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Media;
using System.Diagnostics;
using System.Timers;

namespace TwitchGrowsPlant
{
    public partial class Form1 : Form
    {
        #region Globals
        private static string UserName = "twitch_grows_a_plant";
        private static string Password = "oauth:gp4eotvzywddewgk2xysjiihlgicaj";
        private IrcClient irc = new IrcClient("irc.chat.twitch.tv", 6667, UserName, Password);
        NetworkStream ServerStream = default(NetworkStream);
        string ReadData = "";
        Thread ChatThread;

        // Timer
        private System.Timers.Timer pingTimer;

        // Color Control
        int lR = 4;
        int lG = 2;
        int lB = 2;

        int rR = 0;
        int rG = 0;
        int rB = 0;

        public static Random random = new Random();

        // Difficulty
        public static int multiple = 30;
        private static int lenient = 31; //leniency should probably be a multiple of the multiplier
                                         // in-order to make any difference.
        #endregion

        public Form1()
        {
            InitializeComponent();

            label2.Text = "???";
            NotifyGuessChange(); // Set label and stuff
            RandomizeChatColor(); // Set Random color AND label and stuff
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            irc.JoinRoom("e018s");
            ChatThread = new Thread(GetMessage);
            ChatThread.Start();

            // Initialize Timers
            pingTimer = new System.Timers.Timer();
            pingTimer.Elapsed += (pingTimer, ea) => PingTimeEvent(sender, ea, this);
            pingTimer.Interval = 5000;
            pingTimer.Enabled = true;

        }

        private void GetMessage()
        {
            ServerStream = irc.tcpClient.GetStream();
            int buffsize = 0;
            byte[] inStream = new byte[10025];
            buffsize = irc.tcpClient.ReceiveBufferSize;

            while(true)
            {
                try
                {
                    ReadData = irc.ReadMessage();
                    msg();
                }
                catch (Exception e)
                {

                }
            }
        }

        private void msg() // THIS is WHERE EVERYTHING IS DEALT WITH IN CHAT
                           // Commands, automatic timeouts, etc.
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(msg));
                }
                else
                {
                    //ChatBox.Text = ChatBox.Text + ReadData.ToString() + Environment.NewLine;
                    Debug.WriteLine(ReadData);
                    if (ReadData.Contains("PRIVMSG"))
                    {
                        string[] msgseparator = new string[] { "#e018s :" }; // Room name
                        string user = ReadData.Split('!')[0];
                        user = user.Substring(1); // get rid of the starging colon
                        string message = ReadData.Split(msgseparator, StringSplitOptions.None)[1];
                        //ChatBox.Text = ChatBox.Text + user + ": " + message + "\n";

                        // Colors
                        // Red
                        if (message.ToLower().Equals("r+") || message.ToLower().Equals("red+"))
                        {
                            IncrementRed();
                        }
                        if (message.ToLower().Equals("r-") || message.ToLower().Equals("red-"))
                        {
                            DecrementRed();
                        }
                        // Green
                        if (message.ToLower().Equals("g+") || message.ToLower().Equals("green+"))
                        {
                            IncrementGreen();
                        }
                        if (message.ToLower().Equals("g-") || message.ToLower().Equals("green-"))
                        {
                            DecrementGreen();
                        }
                        // Blue
                        if (message.ToLower().Equals("b+") || message.ToLower().Equals("blue+"))
                        {
                            IncrementBlue();
                        }
                        if (message.ToLower().Equals("b-") || message.ToLower().Equals("blue-"))
                        {
                            DecrementBlue();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        #region ColorControl
        // Red
        private void IncrementRed()
        {
            if (rR < 255 - multiple)
            {
                rR = rR + multiple;
                NotifyGuessChange();
            }
        }

        private void DecrementRed()
        {
            if (rR >= 0 + multiple)
            {
                rR = rR - multiple;
                NotifyGuessChange();
            }
        }
        // Green
        private void IncrementGreen()
        {
            if (rG < 255 - multiple)
            {
                rG = rG + multiple;
                NotifyGuessChange();
            }
        }

        private void DecrementGreen()
        {
            if (rG >= 0 + multiple)
            {
                rG = rG - multiple;
                NotifyGuessChange();
            }
        }
        // Blue
        private void IncrementBlue()
        {
            if (rB < 255 - multiple)
            {
                rB = rB + multiple;
                NotifyGuessChange();
            }
        }

        private void DecrementBlue()
        {
            if (rB >= 0 + multiple)
            {
                rB = rB - multiple;
                NotifyGuessChange();
            }
        }



        // ETC
        
        private void Reconnect()
        {
            irc = new IrcClient("irc.chat.twitch.tv", 6667, UserName, Password);
            irc.JoinRoom("e018s");
        }
       
        private void NotifyGuessChange()
        {
            GuessBox.BackColor = Color.FromArgb(rR, rG, rB); // Initial guessbox color
            label1.Text = "R(" + rR + "), G(" + rG + "), B(" + rB + ")";

            // Check Guess
            if (InRange())
            {
                RandomizeChatColor();
            }
        }

        private void NotifyChatChange()
        {
            ChatBox.BackColor = Color.FromArgb(lR, lG, lB); // Initial guessbox color
            //label2.Text = "R(" + lR + "), G(" + lG + "), B(" + lB + ")";
        }

        private bool InRange()
        {
            bool winner = true;
            // Check red
            if (!(rR>=lR-lenient && rR<=lR+lenient)) // if not in lenient range
            {
                winner = false;
            }
            // Check green
            if (!(rG >= lG - lenient && rG <= lG + lenient)) // if not in lenient range
            {
                winner = false;
            }
            // Check blue
            if (!(rB >= lB - lenient && rB <= lB + lenient)) // if not in lenient range
            {
                winner = false;
            }

            return winner;
        }

        private void RandomizeChatColor()
        {
            // Generates a random number between 0 and 255 of step size MULTIPLE
            lR = random.Next(0, (255 / multiple) + 1) * multiple;
            lG = random.Next(0, (255 / multiple) + 1) * multiple;
            lB = random.Next(0, (255 / multiple) + 1) * multiple;

            irc.SendChatMessage("Congradulations, you got the color. New Color Generated");

            NotifyChatChange();
        }

        #endregion

        #region TimerEvents

        private static void PingTimeEvent(object source, ElapsedEventArgs e, Form1 form)
        {
            form.irc.PingResponse();
            // Debug
            //form.irc.SendChatMessage("Pinged Ya! " + new Random().Next(0,1000));
            //
        }

        #endregion

        #region OnClicks

        private void button1_Click(object sender, EventArgs e)
        {
            irc.SendChatMessage("Hey Buddy Boy (Sent From My Windows Form App)");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            irc.LeaveRoom();
            ServerStream.Dispose();
            Environment.Exit(0);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        #endregion

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Reconnect();
            irc.SendChatMessage("I Clicked the button !");
        }
    }

    public class IrcClient
    {
        private string username;
        private string channel;

        public TcpClient tcpClient;
        private StreamReader inputStream;
        private StreamWriter outputStream;

        // Constructor
        public IrcClient(string ip, int port, string username, string password)
        {
            tcpClient = new TcpClient(ip, port);
            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());

            outputStream.WriteLine("PASS " + password);
            outputStream.WriteLine("Nick " + username);
            outputStream.WriteLine("USER " + username + " 8 * :" + username);
            outputStream.WriteLine("CAP REQ :twitch.tv/membership");
            outputStream.WriteLine("CAP REQ :twitch.tv/commands");
            outputStream.Flush();
        }

        public void JoinRoom(string channel)
        {
            this.channel = channel;
            outputStream.WriteLine("JOIN #" + channel);
            outputStream.Flush();
        }

        public void LeaveRoom()
        {
            outputStream.Close();
            inputStream.Close();
        }

        public void SendIrcMessage(string message)
        {
            outputStream.WriteLine(message);
            outputStream.Flush();
        }

        public void SendChatMessage(string message)
        {
            SendIrcMessage(":" + username + "!" + username + "@" + username 
                + ".tmi.twitch.tv PRIVMSG #" + channel + " :" + message);
        }

        public void PingResponse()
        {
            SendIrcMessage("PONG tmi.twitch.tv\r\n");
        }

        public string ReadMessage()
        {
            string message = "";
            message = inputStream.ReadLine();
            return message;
        }
    }
}
