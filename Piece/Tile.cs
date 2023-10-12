using UI;
using UnityEngine;

namespace Piece
{
    public class Tile : MonoBehaviour
    {
        public Mesh meepleMeshRed;
        public Mesh meepleMeshBlue;
        public Mesh meepleMeshYellow;
        public Mesh meepleMeshGreen;
        public Mesh meepleMeshPurple;
        public GameObject bidNumLogo;
        public GameObject playNumLogo;
        private int _bidNum;
        private int _playNum;

        private MeshFilter _meepleMeshFilter;

        private void Awake()
        {
            _meepleMeshFilter = transform.Find("BoundMeeple").GetComponent<MeshFilter>();
        }

        public void SetBindMeepleColor(string color)
        {
            switch (color)
            {
                case "Red":
                    SetBindMeepleRed();
                    break;
                case "Blue":
                    SetBindMeepleBlue();
                    break;
                case "Yellow":
                    SetBindMeepleYellow();
                    break;
                default:
                    SetBindMeepleNone();
                    break;
            }
        }

        private void SetBindMeepleNone()
        {
            _meepleMeshFilter.mesh = null;
        }

        private void SetBindMeepleRed()
        {
            _meepleMeshFilter.mesh = meepleMeshRed;
        }

        private void SetBindMeepleBlue()
        {
            _meepleMeshFilter.mesh = meepleMeshBlue;
        }
        
        private void SetBindMeepleYellow()
        {
            _meepleMeshFilter.mesh = meepleMeshYellow;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if the collider is meeple or not
            if (!IsComponentMeeple(other)) return;
            else if (!GameManager.Instance.IsMyTurn()) return;
            
            TileManager.Instance.SetTileActive(this);
            TileManager.Instance.SetOutlinePositionToActiveTile();
            TileManager.Instance.ActiveOutline();
        }

        private void OnTriggerExit(Collider other)
        {
            // Check if the collider is meeple or not
            if (!IsComponentMeeple(other)) return;

            var meepleID = other.name;
            ActionManager.Instance.RemoveMoveMeepleAction(meepleID);
            GameManager.Instance.UnbindMeepleFromTile(meepleID, this.name);
            TileManager.Instance.InactiveOutline();
            TileManager.Instance.SetTileInactive();
        }

        private static bool IsComponentMeeple(Component component)
        {
            return ((1 << component.gameObject.layer) & (1 << LayerMask.NameToLayer("Meeple"))) > 0;
        }

        public void SetBidNum(int bidNum)
        {
            _bidNum = bidNum;
            bidNumLogo.GetComponent<MeshRenderer>().material = MaterialManager.Instance.GetNumberMaterial(bidNum);
        }

        public int GetBidNum()
        {
            return _bidNum;
        }
    }
}