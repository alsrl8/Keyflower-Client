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
            var validationResult = ActionManager.Instance.ValidateMeepleBidAction();
            if (!validationResult) return;  // TODO Bid에 필요한 미플 수가 부족하면 턴을 진행시키지 않음(니가 치워)
            ActionManager.Instance.SendCurrentTurnMeepleMoveAction();
            SetButtonEnable(false);
        }

        public void SetButtonEnable(bool enable)
        {
            _button.interactable = enable;
        }
    }
}