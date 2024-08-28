using UnityEngine;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using TMPro;
using MyNetworking;
using Photon.Client;
using Photon.Client.StructWrapping;

public class MyGamingPage : MonoBehaviour, ICommandCallback
{
    public TMP_Text logPanel;
    private NetworkStream _stream;
    public RectTransform contentRectTransform;
    private string _logText;
    private float _myX;
    private float _myY;

    public void PrintAtLogPanel(string message, bool cleanHistory)
    {
        _logText = cleanHistory
            ? MyConnectionHelper.Instance.NewTextToLog(message)
            : MyConnectionHelper.Instance.AddTextToLog(message);
    }

    private void OnPlayerMoveTo(float x, float y)
    {
        _myX = x;
        _myY = y;
    }

    public void OnEventData(MyCommand command, EventData eventData)
    {
        if (command.Flags != 1 || command.Payload[0] != 0xf3)
        {
            // PrintAtLogPanel("Flags|Payload[0]不常见:" + command, false);
        }
        if (!eventData.Parameters.TryGetValue(252, out object code) || code is not short intValue)
        {
            // PrintAtLogPanel("OnEventData:没有252," + eventData.Parameters.ToStringFull(), false);
            return;
        }

        switch (intValue)
        {
            case 73:
                //这是逼叨叨的说话的 不用处理
                break;
            // case 201:
            //     // {(Byte)0=(Int32[])System.Int32[], (Byte)1=(Int16[])System.Int16[], (Byte)252=(Int16)201}
            //     print("干啥的");
            //     break;
            // case 202:
            //     print("干啥的");
            //     break;
            // case 203:
            //     print("干啥的");
            //     break;
            // case 204:
            //     print("干啥的");
            //     break;
            // case 399:
            //     print("干啥的");
            //     break;
            default:
                // Debug.LogWarning("OnEventData:" + code + "|||||||||||" + eventData.ToStringFull());
                break;
        }
    }

    public void OnOperationResponse(MyCommand command, OperationResponse operationResponse)
    {
        if (!operationResponse.Parameters.TryGetValue(253, out object code) || code is not short intValue)
        {
            PrintAtLogPanel("OnOperationResponse:没有253," + operationResponse.Parameters.ToStringFull(), false);
            return;
        }

        switch (intValue)
        {
            case 16:
                // 穿越地图
                break;
            case 366:
                // print("干啥的");
                break;
            default:
                // Debug.LogWarning("OnOperationResponse:" + code + "|||||||||||" + operationResponse.Parameters.ToStringFull());
                break;
        }
        // Debug.LogWarning("OnOperationResponse:" + operationResponse.Parameters.ToStringFull());
    }

    public void OnOperationRequest(MyCommand command, OperationRequest operationRequest)
    {
        if (!operationRequest.Parameters.TryGetValue(253, out object code) || code is not short intValue)
        {
            PrintAtLogPanel("OnOperationRequest:没有253," + operationRequest.Parameters.ToStringFull(), false);
            return;
        }

        switch (intValue)
        {
            case 16:
                // 穿越地图
                break;
            case 184:
                print("干啥的");
                break;
            case 366:
                print("干啥的");
                break;
            default:
                Debug.LogWarning("OnOperationRequest:" + code + "|||||||||||" + operationRequest.Parameters.ToStringFull());
                break;
        }
        Debug.LogWarning("OnOperationRequest:" + operationRequest.Parameters.ToStringFull());
    }

    public void OnMessage(MyCommand command,object message)
    {
        if (message != null)
        {
            Debug.LogWarning("OnMessage:" + message);
        }
        else
        {
            Debug.LogWarning("OnMessage:NULL");
        }
    }

    private void Update()
    {
        logPanel.text = _logText;
        var textRect = logPanel.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, logPanel.preferredHeight);
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, logPanel.preferredHeight);
    }

    void CommandReceivedCallback(MyCommand command)
    {
        Debug.Log(command.ToString());

        // PrintAtLogPanel(command.ToString(), false);
    }

    void Start()
    {
        PrintAtLogPanel("新页面已经加载", false);
        try
        {
            Task.Run(() =>
            {
                try
                {
                    MyConnectionHelper.Instance.StartReceiveLoop(this);
                }
                catch (Exception e)
                {
                    PrintAtLogPanel("接收消息出错: " + e.Message, false);
                }
            });
        }
        catch (Exception e)
        {
            PrintAtLogPanel("接收消息出错: " + e.Message, false);
        }
    }
}