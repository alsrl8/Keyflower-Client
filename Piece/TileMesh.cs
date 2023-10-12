using UnityEngine;

namespace Piece
{
    public class TileMesh : MonoBehaviour
    {
        public GameObject bidNumLogo;
        public GameObject playNumLogo;
        private int _bidNum;
        private int _playNum;

        private MeshFilter _meepleMeshFilter;

        private void Awake()
        {
            _meepleMeshFilter = transform.Find("BoundMeeple").GetComponent<MeshFilter>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsComponentMeeple(other)) return;
            else if (!GameManager.Instance.IsMyTurn()) return;

            TileManager.Instance.SetActiveTileMesh(this);
            TileManager.Instance.SetOutlinePositionNearByActiveTileMesh();
            TileManager.Instance.ActiveOutline();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsComponentMeeple(other)) return;

            var meepleID = other.name;
            GameManager.Instance.UnbindMeepleFromTile(meepleID, this.name);
            TileManager.Instance.InactiveOutline();
            TileManager.Instance.SetActiveTileMeshNull();
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