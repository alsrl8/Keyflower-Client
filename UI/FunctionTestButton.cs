using Piece;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FunctionTestButton : MonoBehaviour
    {
        public static FunctionTestButton Instance { get; private set; }
        private Button _button;

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            MeepleActionManager.Instance.PrintMeepleActionDictionary();
        }
    }
}