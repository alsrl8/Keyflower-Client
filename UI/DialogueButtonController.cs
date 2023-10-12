using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DialogueButtonController : MonoBehaviour
    {
        public Button closeButton;

        private void Start()
        {
            closeButton.onClick.AddListener(CloseDialogue);
        }

        private void CloseDialogue()
        {
            TileDialogue.Instance.InactivateTileDialogue();
        }
    }
}