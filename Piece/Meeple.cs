using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Piece
{
    public class Meeple : MonoBehaviour
    {
        public GameObject outline;
        public GameObject numberLogo;

        private int _number;
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                SetNumberLogo(_number);
            }
        }

        public List<string> ChildrenIDs { get; set; } // TODO 이거 필요한지 검토
        private MeshRenderer _numberLogo;

        private void Awake()
        {
            Number = 1;
            ChildrenIDs = new List<string>();
            _numberLogo = numberLogo.GetComponent<MeshRenderer>();
        }

        public void ActivateOutline()
        {
            outline.SetActive(true);
        }

        public void InactiveOutline()
        {
            outline.SetActive(false);
        }

        private void SetNumberLogo(int num)
        {
            if (num <= 1)
            {
                numberLogo.SetActive(false);
            }
            else
            {
                var numberMaterial = MaterialManager.Instance.GetNumberMaterial(this.Number);
                numberLogo.SetActive(true);
                _numberLogo.material = numberMaterial;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsAnyMeepleDragged()) return;
            else if (IsThisDragged()) return;
            else if (!IsComponentMeeple(other)) return;
            else if (!IsMeepleColorSame(other)) return;
            else if (IsAlreadyTriggeredMeeple()) return;
            else if (MeepleManager.Instance.IsAttachedToTile(this.name)) return;

            MouseInputManager.Instance.SetTriggeredMeeple(this);
            ActivateOutline();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsAnyMeepleDragged()) return;
            else if (IsThisDragged()) return;
            else if (!IsComponentMeeple(other)) return;
            else if (!IsMeepleColorSame(other)) return;
            else if (MeepleManager.Instance.IsAttachedToTile(other.name)) return;

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

        public void GroupMeeple(Meeple other)
        {
            this.Number += other.Number;
            ChildrenIDs.Add(other.name);
            foreach (var otherChild in other.ChildrenIDs)
            {
                ChildrenIDs.Add(otherChild);
            }
            other.Number = 1;
            other.ChildrenIDs.Clear();
            other.gameObject.SetActive(false);
        }
    }
}