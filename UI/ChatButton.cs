using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class ChatButton : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.interactable = true;
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            if (!ReferenceEquals(ChatManager.Instance, null) && ChatManager.Instance.IsActive) return;
            SceneManager.LoadScene("ChatScene", LoadSceneMode.Additive);
        }
    }
}