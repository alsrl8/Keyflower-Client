using UnityEngine;

namespace Piece
{
    public class Tile : MonoBehaviour
    {
        public string Color { set; get; }
        
        public GameObject outline;
        public GameObject bidNumLogo;
        public GameObject playNumLogo;
        private int _bidNum;
        private int _playNum;

        private MeshFilter _meepleMeshFilter;

        private void Awake()
        {
            _meepleMeshFilter = transform.Find("BoundMeeple").GetComponent<MeshFilter>();
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
            if (!IsComponentMeeple(other)) return;
            else if (!GameManager.Instance.IsMyTurn()) return;
            
            TileManager.Instance.TriggerTile(this.name);
            ActivateOutline();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsComponentMeeple(other)) return;
            
            TileManager.Instance.UnTriggerTile();
            InactivateOutline();
            
            var meepleID = other.name;
            GameManager.Instance.UnbindMeepleFromTile(meepleID, this.name);
            if (TileManager.Instance.GetNumberOfMeepleByTileID(this.name) == 0)
            {
                TileManager.Instance.SetTileColorNullByID(this.name);
            }
        }

        /// <summary>
        /// Check if the collider is meeple or not
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        private static bool IsComponentMeeple(Component component)
        {
            return ((1 << component.gameObject.layer) & (1 << LayerMask.NameToLayer("Meeple"))) > 0;
        }
    }
}