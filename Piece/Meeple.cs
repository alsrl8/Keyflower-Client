using UnityEngine;

namespace Piece
{
    public class Meeple : MonoBehaviour
    {
        public GameObject outline;

        public void ActivateOutline()
        {
            outline.SetActive(true);
        }
        
        public void InactiveOutline()
        {
            outline.SetActive(false);
        }
    }
}