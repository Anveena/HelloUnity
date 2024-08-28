using System.Collections;
using MyNetworking;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MyConnectingPage : MonoBehaviour
{
    public TMP_InputField addrField;
    public TMP_Text logPanel;
    public Button connectBtn;
    public RectTransform contentRectTransform;

    private void PrintAtLogPanel(string message)
    {
        logPanel.text = MyConnectionHelper.Instance.AddTextToLog(message);
        var textRect = logPanel.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, logPanel.preferredHeight);
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, logPanel.preferredHeight);
    }

    private void OnConnectBtnClicked()
    {
        var address = addrField.text;
        string[] addressParts = address.Split(':');
        if (addressParts.Length == 2)
        {
            string ip = addressParts[0];
            if (int.TryParse(addressParts[1], out int port))
            {
                ConnectToServer(ip, port);
            }
            else
            {
                PrintAtLogPanel("端口号无效");
            }
        }
        else
        {
            PrintAtLogPanel("地址格式错误,应为 ip:port,由于错误的输入,直接连接本机32999端口");
            ConnectToServer("127.0.0.1", 32999);
        }
    }

    void Start()
    {
        Button btn = connectBtn.GetComponent<Button>();
        btn.onClick.AddListener(OnConnectBtnClicked);
    }

    void ConnectToServer(string ip, int port)
    {
        SynchronizationContext ctx = SynchronizationContext.Current;
        PrintAtLogPanel("已经开始尝试连接了,不要点按钮了.虽然简单但是C#实在不想写");
        MyConnectionHelper.Instance.TryConnectTo(ip, port).ContinueWith(task =>
        {
            ctx.Post(_ =>
            {
                var result = task.Result;
                PrintAtLogPanel(result);
                if (result.StartsWith("连接成功"))
                {
                    SceneManager.LoadScene("Gaming");
                }
            },null);
        });
    }
}