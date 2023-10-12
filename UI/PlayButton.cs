using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayButton : MonoBehaviour
    {
        public static PlayButton Instance { get; private set; }

        private Animator _animator;
        private Button _button;
        private static readonly int Pushed = Animator.StringToHash("Pushed");

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            _animator = GetComponent<Animator>();
            _button = GetComponent<Button>();
            _button.interactable = false;
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            _animator.SetTrigger(Pushed);
            SetButtonEnable(false);
            GameManager.Instance.HandlePlayButton();
        }

        public void SetButtonEnable(bool enable)
        {
            _button.interactable = enable;
        }
    }
}