using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameReadyButton : MonoBehaviour
    {
        public static GameReadyButton Instance { get; private set; }
        public GameObject chest;
        private Animator _animator;
        private Button _button;
        
        private static readonly int Pushed = Animator.StringToHash("Pushed");
        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            _animator = GetComponent<Animator>();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClick);  
        }

        private void OnButtonClick()
        {
            Debug.Log("Game Ready!");
            _animator.SetTrigger(Pushed);
            chest.SetActive(true);
        }
        
        public void SetButtonEnable(bool enable)
        {
            gameObject.SetActive(enable);
        }
    }
}