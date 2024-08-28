using System;
using System.Linq;
using Unity.VisualScripting;

namespace MyNetworking
{
    public class MyCommand
    {
        public enum  Source
        {
            Default,
            Unreliable,
            Segmented,
        }

        public Source source;
        public byte[] Payload { get; private set; }
        public bool HasPayload { get; private set; }
        public byte Type { get; private set; }
        public byte ChannelID { get; private set; }
        public byte Flags { get; private set; }

        public int Length { get; private set; }

        // 构造函数，用于解析字节数组并初始化 MyCommand 对象
        public MyCommand(byte[] data, int padding, int length)
        {
            source = Source.Default;
            // 检查数据是否为空或长度是否小于12 + padding
            if (data == null || length < 12 + padding)
            {
                throw new ArgumentException("数据长度无效");
            }

            // 解析数据
            Type = data[0 + padding]; // 命令类型（1字节）
            ChannelID = data[1 + padding]; // 通道ID（1字节）
            Flags = data[2 + padding]; // 命令标志（1字节）

            // 跳过1字节
            // 读取命令长度（4字节，采用小端序）
            Length = MyTools.Bytes.ToInt32BigEndian(data, 4 + padding);

            // 读取序列号（4字节，小端序）
            int sequenceNumber = MyTools.Bytes.ToInt32BigEndian(data, 8 + padding);

            // 有效负载从序列号之后开始，加上padding的长度
            int payloadStartIndex = 12 + padding;
            if (length > payloadStartIndex)
            {
                Payload = data.Skip(payloadStartIndex).Take(Length - 12).ToArray();
                HasPayload = true;
            }
            else
            {
                HasPayload = false;
                Payload = Array.Empty<byte>(); // 如果没有有效负载，初始化为空数组
            }
        }

        public void SetPayloadPadding(int padding)
        {
            Payload = Payload.Skip(padding).Take(Payload.Length - padding).ToArray();
        }

        public void SetNewPayload(byte[] payload)
        {
            Payload = payload;
            Length = payload.Length + 12;
        }

        // 返回 MyCommand 对象的字符串表示
        public override string ToString()
        {
            // 使用十六进制字符串表示有效负载
            string payloadHexString = BitConverter.ToString(Payload).Replace("-", " ");
            return
                $"Type: {Type}, ChannelID: {ChannelID}, Flags: {Flags}, Length: {Length}, Payload: {payloadHexString}";
        }
    }
}