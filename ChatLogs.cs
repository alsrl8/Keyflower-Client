using System.Collections.Generic;
using UI;
using UnityEngine;

public class ChatLogs : MonoBehaviour
{
    public static ChatLogs Instance;
    public LinkedList<ChatData> Messages;
    private const int MaxMessageNum = 100;

    public GameObject newChatAlarm;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Messages = new LinkedList<ChatData>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMessage(ChatData chatData)
    {
        Messages.AddLast(chatData);
        if (Messages.Count > MaxMessageNum)
        {
            Messages.RemoveFirst();
        }

        if (ReferenceEquals(ChatManager.Instance, null) || !ChatManager.Instance.IsActive)
        {
            newChatAlarm.SetActive(true);
        }
    }

    public void InactiveNewChatAlarm()
    {
        newChatAlarm.SetActive(false);
    }
}