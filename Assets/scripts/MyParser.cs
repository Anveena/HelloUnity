using System;
using System.Collections.Generic;
using UnityEngine;

// public class MyParser : PhotonParser
// {
//     private IMessageHandler _mainMessageHandler;
//
//     public MyParser(IMessageHandler handler)
//     {
//         _mainMessageHandler = handler;
//     }
//
//     static string GetFormattedString(Dictionary<byte, object> parameters)
//     {
//         // 创建一个列表来存储每个键值对的格式化字符串
//         List<string> formattedPairs = new List<string>();
//
//         foreach (var kvp in parameters)
//         {
//             // 格式化键值对
//             string formattedPair = $"key:{kvp.Key} value:{kvp.Value}";
//             formattedPairs.Add(formattedPair);
//         }
//
//         // 将所有格式化的键值对连接成一个字符串，并返回
//         return string.Join(", ", formattedPairs);
//     }
//
//     protected override void OnRequest(byte operationCode, Dictionary<byte, object> parameters)
//     {
//         _mainMessageHandler.PrintAtLogPanel("code:" + operationCode + "OnRequest:" + GetFormattedString(parameters), false);
//         OperationCodes code = parameters.TryGetValue(253, out var codeObj)
//             ? (OperationCodes)Convert.ToInt16(codeObj)
//             : 0;
//         {
//         }
//         switch (code)
//         {
//             case (OperationCodes)366:
//                 // HandleLocalPlayerMovement(parameters);
//                 break;
//             case (OperationCodes)16:
//                 break;
//             default:
//                 _mainMessageHandler.PrintAtLogPanel("OnRequest" + code, false);
//                 break;
//         }
//     }
//
//     protected override void OnResponse(byte operationCode, short returnCode, string debugMessage,
//         Dictionary<byte, object> parameters)
//     {
//         _mainMessageHandler.PrintAtLogPanel("code:" + operationCode + "OnResponse:" + GetFormattedString(parameters), false);
//         // OperationCodes code = parameters.TryGetValue(253, out var codeObj)
//         //     ? (OperationCodes)Convert.ToInt16(codeObj)
//         //     : 0;
//         // if (code != (OperationCodes)operationCode)
//         // {
//         //     Debug.LogError($"Invalid operation code: {code}");
//         // }
//         // switch (code)
//         // {
//         //     case (OperationCodes)2:
//         //         string mapStr = parameters.TryGetValue(8, out var mapObj) ? Convert.ToString(mapObj) : string.Empty;
//         //         _logger.PrintAtLogPanel("Join:" + mapStr, false);
//         //         break;
//         //     case (OperationCodes)16:
//         //         _logger.PrintAtLogPanel("OnResponse" + parameters.Values.Count, false);
//         //         break;
//         //     default:
//         //         _logger.PrintAtLogPanel("OnResponse" + code, false);
//         //         break;
//         // }
//     }
//
//     protected override void OnEvent(byte code, Dictionary<byte, object> parameters)
//     {
//         _mainMessageHandler.PrintAtLogPanel("code:" + code + "OnEvent:" + GetFormattedString(parameters), false);
//     }
// }