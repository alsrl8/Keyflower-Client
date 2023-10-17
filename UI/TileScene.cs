using Piece;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TileScene : MonoBehaviour
    {
        private TextMeshProUGUI _playerID;
        private TextMeshProUGUI _ownedTiles;

        private void Awake()
        {
            _playerID = gameObject.transform.Find("PlayerID").GetComponent<TextMeshProUGUI>();
            _ownedTiles = gameObject.transform.Find("OwnedTiles").GetComponent<TextMeshProUGUI>();

            _playerID.text = GameManager.Instance.UserID;
            _ownedTiles.text = string.Join(", ", TileManager.Instance.GetBidWinningTileIDs());
        }
    }
}