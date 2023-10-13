using UnityEngine;

namespace Piece
{
    public class Chest :MonoBehaviour
    {
        public static Chest Instance;
        public GameObject outline;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void ActivateOutline()
        {
            outline.SetActive(true);
        }

        public void InactivateOutline()
        {
            outline.SetActive(false);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!MouseInputManager.Instance.IsDraggingMeeple()) return;
            MouseInputManager.Instance.SetTriggeredChest(this);
            ActivateOutline();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!MouseInputManager.Instance.IsDraggingMeeple()) return;
            MouseInputManager.Instance.SetTriggeredChest(null);
            InactivateOutline();
        }
    }
}