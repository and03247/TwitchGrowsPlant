namespace TwitchToHue
{
    using System.IO;
    using System.Net.Sockets;
    
    public class IrcClient
    {
        private readonly string username;
        private string channel;
        public TcpClient TcpClient;
        private readonly StreamReader inputStream;
        private readonly StreamWriter outputStream;

        // Constructor
        public IrcClient(string ip, int port, string username, string password)
        {
            this.username = username;
            this.TcpClient = new TcpClient(ip, port);
            inputStream = new StreamReader(this.TcpClient.GetStream());
            outputStream = new StreamWriter(this.TcpClient.GetStream());

            outputStream.WriteLine("PASS " + password);
            outputStream.WriteLine("Nick " + username);
            outputStream.WriteLine("USER " + username + " 8 * :" + username);
            outputStream.WriteLine("CAP REQ :twitch.tv/membership");
            outputStream.WriteLine("CAP REQ :twitch.tv/commands");
            outputStream.Flush();
        }

        public void JoinRoom(string channelName)
        {
            channel = channelName;
            outputStream.WriteLine("JOIN #" + channelName);
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
            return inputStream.ReadLine();
        }
    }
}