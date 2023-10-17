using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class TileSceneCloseButton : MonoBehaviour
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
            SceneManager.UnloadSceneAsync("TileScene");
        }
    }
}