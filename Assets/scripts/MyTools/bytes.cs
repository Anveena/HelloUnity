using System;

namespace MyTools
{
    public class Bytes
    {
        public static int ToInt32BigEndian(byte[] data, int startIndex)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length < startIndex + 4)
                throw new ArgumentException("数据长度不足");

            // 确保大端序
            return (data[startIndex] << 24) |
                   (data[startIndex + 1] << 16) |
                   (data[startIndex + 2] << 8) |
                   (data[startIndex + 3]);
        }
    }
}