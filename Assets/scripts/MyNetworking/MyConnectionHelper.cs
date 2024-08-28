using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Photon.Client;

namespace MyNetworking
{
    public class MyConnectionHelper : MonoBehaviour
    {
        private static MyConnectionHelper _instance;
        private static readonly object Lock = new();
        private string _logText;
        private readonly object _logTextLock = new();
        private TcpClient _client;

        public static MyConnectionHelper Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("MyConnectionHelper");
                        _instance = go.AddComponent<MyConnectionHelper>();
                        DontDestroyOnLoad(go);
                    }
                }

                return _instance;
            }
        }

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public async Task<string> TryConnectTo(string ip, int port)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(ip, port);
                return "连接成功";
            }
            catch (Exception ex)
            {
                return "连接失败: " + ex.Message;
            }
        }

        public void StartReceiveLoop(ICommandCallback cbk)
        {
            MyParser parser = new MyParser(cbk);
            byte[] header = new byte[4];
            byte[] buffer = new byte[65536];
            Stream stream = _client.GetStream();
            while (true)
            {
                int bytesRead = stream.Read(header, 0, header.Length);
                if (bytesRead == 0)
                {
                    Console.WriteLine("连接关闭");
                    break;
                }

                if (bytesRead != 4)
                {
                    throw new Exception("读取的头部信息长度不足");
                }

                int messageLength = BitConverter.ToInt16(header, 0);
                bytesRead = stream.Read(buffer, 0, messageLength);
                if (bytesRead != messageLength)
                {
                    throw new Exception("读取的消息长度不足");
                }

                int commandCount = buffer[3];
                int padding = 12;
                try
                {
                    for (int i = 0; i < commandCount; i++)
                    {
                        MyCommand command = new MyCommand(buffer, padding, messageLength);
                        padding += command.Length;
                        if (!command.HasPayload)
                        {
                            continue;
                        }

                        parser.ParseCommand(command);
                    }
                }
                catch (Exception e)
                {
                    cbk.PrintAtLogPanel(e.Message,false);
                    continue;
                }
            }
        }

        public string AddTextToLog(string newMessage)
        {
            string tmp;
            lock (_logTextLock)
            {
                _logText = newMessage + Environment.NewLine + _logText;
                tmp = _logText;
            }

            return tmp;
        }

        public string NewTextToLog(string newMessage)
        {
            string tmp;
            lock (_logTextLock)
            {
                _logText = newMessage;
                tmp = _logText;
            }

            return tmp;
        }
    }
}