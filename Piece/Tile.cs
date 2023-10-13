using System.Collections.Generic;
using UnityEngine;

namespace Piece
{
    public class Tile : MonoBehaviour
    {
        public GameObject outline;
        public GameObject bidNumLogo;
        public GameObject playNumLogo;
        private int _bidNum;
        private int _playNum;
        private Dictionary<string, string> _playerBidMeepleDictionary;
        private Dictionary<string, string> _playerPlayMeepleDictionary;

        private MeshFilter _meepleMeshFilter;

        private void Awake()
        {
            _meepleMeshFilter = transform.Find("BoundMeeple").GetComponent<MeshFilter>();
            _playerBidMeepleDictionary = new Dictionary<string, string>();
            _playerPlayMeepleDictionary = new Dictionary<string, string>();
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

        public string GetBidMeepleByPlayerID(string playerID)
        {
            return _playerBidMeepleDictionary.TryGetValue(playerID, out var value) ? value : "";
        }

        public void SetBidMeeple(string playerID, string meepleID)
        {
            _playerBidMeepleDictionary[playerID] = meepleID;
        }

        public void RemoveBidMeeple(string playerID)
        {
            _playerBidMeepleDictionary.Remove(playerID);
        }

        public string GetPlayerMeepleByPlayerID(string playerID)
        {
            return _playerPlayMeepleDictionary.TryGetValue(playerID, out var value) ? value : "";
        }

        public void SetPlayMeeple(string playerID, string meepleID)
        {
            _playerPlayMeepleDictionary[playerID] = meepleID;
        }

        public void RemovePlayMeeple(string playerID)
        {
            _playerPlayMeepleDictionary.Remove(playerID);
        }

        public string GetColor()
        {
            string color = null;
            foreach (var (playerID, meepleID) in _playerBidMeepleDictionary)
            {
                var meepleColor = MeepleManager.Instance.GetMeepleColor(meepleID);
                switch (meepleColor)
                {
                    case "Red" or "Blue" or "Yellow":
                        return meepleColor;
                    case "Green":
                        color = meepleColor;
                        break;
                }
            }

            foreach (var (playerID, meepleID) in _playerPlayMeepleDictionary)
            {
                var meepleColor = MeepleManager.Instance.GetMeepleColor(meepleID);
                switch (meepleColor)
                {
                    case "Red" or "Blue" or "Yellow":
                        return meepleColor;
                    case "Green":
                        color = meepleColor;
                        break;
                }
            }

            return color;
        }
    }
}