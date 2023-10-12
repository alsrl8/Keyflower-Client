using UnityEngine;

namespace Piece
{
    public class MeepleOutline : MonoBehaviour
    {
        public GameObject Outline;

        public void Activate()
        {
            Outline.SetActive(true);
        }
        
        public void Inactivate()
        {
            Outline.SetActive(false);
        }
    }
}