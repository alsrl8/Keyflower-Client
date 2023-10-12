using UnityEngine;

namespace Piece
{
    public class Meeple : MonoBehaviour
    {
        public GameObject outline;
        private int _count;

        public void ActivateOutline()
        {
            outline.SetActive(true);
        }
        
        public void InactiveOutline()
        {
            outline.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsAnyMeepleDragged()) return;
            else if (IsThisDragged()) return;
            else if (!IsComponentMeeple(other)) return;
            else if (!IsMeepleColorSame(other)) return;
            else if (IsAlreadyTriggeredMeeple()) return;
            
            MouseInputManager.Instance.SetTriggeredMeeple(this);
            ActivateOutline();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsAnyMeepleDragged()) return;
            else if (IsThisDragged()) return;
            else if (!IsComponentMeeple(other)) return;
            else if (!IsMeepleColorSame(other)) return;
            
            MouseInputManager.Instance.SetTriggeredMeeple(null);
            InactiveOutline();
        }
        
        /// <summary>
        /// Check if the collider is meeple or not
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        private bool IsComponentMeeple(Component component)
        {
            return ((1 << component.gameObject.layer) & (1 << LayerMask.NameToLayer("Meeple"))) > 0;
        }

        private bool IsMeepleColorSame(Component component)
        {
            return MeepleManager.Instance.GetMeepleColor(component.name) == MeepleManager.Instance.GetMeepleColor(this.name);
        }

        private bool IsThisDragged()
        {
            return MouseInputManager.Instance.IsThisDraggingMeeple(this.name);
        }

        private bool IsAnyMeepleDragged()
        {
            return MouseInputManager.Instance.IsDraggingMeeple();
        }

        private bool IsAlreadyTriggeredMeeple()
        {
            return !ReferenceEquals(MouseInputManager.Instance.GetTriggeredMeeple(), null);
        }
    }
}