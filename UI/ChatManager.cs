using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ChatManager : MonoBehaviour
    {
        public static ChatManager Instance;
        public bool IsActive { get; private set; }

        public GameObject chatMessagePrefab;
        public GameObject inputField;
        public Transform contentTransform;
        public ScrollRect scrollRect;

        private TMP_InputField _inputField;
        private const string Me = "Me";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                ShowChatLogs();
                _inputField = inputField.GetComponent<TMP_InputField>();
                ChatLogs.Instance.InactiveNewChatAlarm();
            }
        }

        private void Start()
        {
            SetFocusOnInputBox();
        }

        private void OnEnable()
        {
            IsActive = true;
        }

        private void OnDisable()
        {
            IsActive = false;
        }

        public void SubmitChatMessage(string message)
        {
            if (!Input.GetKey(KeyCode.Return)) return;
            else if (message == "") return;

            var playerID = GameManager.Instance.UserID;
            var chatData = GenerateChatData(message);

            SendMessageToServer(chatData);
            ShowMessageOnChatPanel(chatData);
            ClearInputBox();
            SetFocusOnInputBox();

            ChatLogs.Instance.AddMessage(chatData);
        }

        private ChatData GenerateChatData(string message)
        {
            var playerID = GameManager.Instance.UserID;
            var currentTime = DateTime.Now.ToString("HH:mm:ss");
            return new ChatData
            {
                playerID = playerID,
                content = message,
                chatTime = currentTime,
            };
        }

        private string GenerateFormatMessageFromChatData(ChatData chatData)
        {
            var speaker = chatData.playerID == GameManager.Instance.UserID ? Me : chatData.playerID;
            var formatMessage = $"{speaker}[{chatData.chatTime}]: {chatData.content}";
            return formatMessage;
        }

        private void SendMessageToServer(ChatData chatData)
        {
            WebSocketClient.Instance.SendMessageToServer(new ServerMessage
            {
                type = ServerMessageType.Chat,
                data = JsonUtility.ToJson(chatData)
            });
        }

        public void ShowMessageOnChatPanel(ChatData chatData)
        {
            var messageObj = Instantiate(chatMessagePrefab, contentTransform);
            messageObj.GetComponent<TextMeshProUGUI>().text = GenerateFormatMessageFromChatData(chatData);
            StartCoroutine(ScrollToBottom());
        }

        public void ShowChatLogs()
        {
            foreach (var chatData in ChatLogs.Instance.Messages)
            {
                var messageObj = Instantiate(chatMessagePrefab, contentTransform);
                messageObj.GetComponent<TextMeshProUGUI>().text = GenerateFormatMessageFromChatData(chatData);
            }

            StartCoroutine(ScrollToBottom());
        }

        private void ClearInputBox()
        {
            _inputField.text = "";
        }

        private void SetFocusOnInputBox()
        {
            _inputField.Select();
            _inputField.ActivateInputField();
        }

        private IEnumerator ScrollToBottom()
        {
            yield return new WaitForEndOfFrame();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}