using System;
using System.Collections.Generic;
using System.Linq;
using MyTools;
using Photon.Client;
using UnityEngine;

namespace MyNetworking
{
    public class MyParser
    {
        class SegmentedPackage
        {
            public int Length;
            public int Wroten;
            public byte[] Buffer;
        }

        private readonly Dictionary<int, SegmentedPackage> _segmentedPkgDic;
        private readonly ICommandCallback _ctx;
        private readonly Photon.Client.Protocol16 _deserializer;

        public MyParser(ICommandCallback ctx)
        {
            _segmentedPkgDic = new();
            _deserializer = new Photon.Client.Protocol16();
            _ctx = ctx;
        }

        public void ParseCommand(MyCommand command)
        {
            switch (command.Type)
            {
                case 1:
                    ParseAckCommand(command);
                    break;
                case 6:
                    ParseReliable(command);
                    break;
                case 7:
                    command.SetPayloadPadding(4);
                    command.source = MyCommand.Source.Unreliable;
                    ParseReliable(command);
                    break;
                case 8:
                    ParseSegmented(command);
                    command.source = MyCommand.Source.Segmented;
                    break;
                default:
                    _ctx.PrintAtLogPanel("未知消息:" + command, false);
                    break;
            }
        }

        private void ParseSegmented(MyCommand command)
        {
            int startSn = Bytes.ToInt32BigEndian(command.Payload, 0);
            int segCount = Bytes.ToInt32BigEndian(command.Payload, 4);
            int segIdx = Bytes.ToInt32BigEndian(command.Payload, 8);
            int totalLength = Bytes.ToInt32BigEndian(command.Payload, 12);
            int offsetOfFullData = Bytes.ToInt32BigEndian(command.Payload, 16);

            if (!_segmentedPkgDic.TryGetValue(startSn, out SegmentedPackage segmentedPackage))
            {
                segmentedPackage = new SegmentedPackage
                {
                    Buffer = new byte[totalLength],
                    Length = totalLength,
                    Wroten = 0
                };
                _segmentedPkgDic[startSn] = segmentedPackage;
            }

            Buffer.BlockCopy(command.Payload,
                20, segmentedPackage.Buffer, offsetOfFullData, command.Payload.Length - 20);
            segmentedPackage.Wroten += command.Payload.Length - 20;
            if (segmentedPackage.Wroten >= segmentedPackage.Length)
            {
                _segmentedPkgDic.Remove(startSn);
                command.SetNewPayload(segmentedPackage.Buffer);
                ParseReliable(command);
            }
        }

        private void ParseReliable(MyCommand command)
        {
            switch (command.Payload[1])
            {
                case 0x02:
                    OperationRequest operationRequest = _deserializer.DeserializeOperationRequest(
                        new StreamBuffer(command.Payload.Skip(2).Take(command.Payload.Length - 2).ToArray()),
                        Protocol.DeserializationFlags.None);
                    _ctx.OnOperationRequest(command, operationRequest);
                    break;
                case 0x03:
                    OperationResponse operationResponse = _deserializer.DeserializeOperationResponse(
                        new StreamBuffer(command.Payload.Skip(2).Take(command.Payload.Length - 2).ToArray()));
                    _ctx.OnOperationResponse(command, operationResponse);
                    break;
                case 0x04:
                    EventData eventData = _deserializer.DeserializeEventData(
                        new StreamBuffer(command.Payload.Skip(2).Take(command.Payload.Length - 2).ToArray()));
                    _ctx.OnEventData(command, eventData);
                    break;
                default:
                    _ctx.PrintAtLogPanel("未知消息:" + command, false);
                    break;
            }
        }

        private void ParseAckCommand(MyCommand command)
        {
            if (command.Length != 20)
            {
                _ctx.PrintAtLogPanel("ParseAckCommand长度特异:" + command, false);
            }

            if (command.Flags == 0) return;
            // 事实上,这个command就是个ack,我看不包含除了ping和ack的任何信息.
            // object obj = _deserializer.DeserializeMessage(
            //     new StreamBuffer(command.Payload.Take(command.Payload.Length).ToArray()));
            // _ctx.OnMessage(obj);
            _ctx.PrintAtLogPanel("ParseAckCommand:" + command, false);
        }
    }
}