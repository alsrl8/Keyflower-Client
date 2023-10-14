using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Piece
{
    public class Tile : MonoBehaviour
    {
        public GameObject outline;
        public GameObject bidNumLogo;
        public GameObject playNumLogo;
        private MeshRenderer _bidNumLogo;
        private MeshRenderer _playNumLogo;
        private int _bidNum;
        public int BidNum
        {
            get => _bidNum;
            set
            {
                _bidNum = value;
                if (_bidNum == 0)
                {
                    bidNumLogo.SetActive(false);
                }
                else
                {
                    bidNumLogo.SetActive(true);
                    var numberMaterial = MaterialManager.Instance.GetNumberMaterial(BidNum);
                    _bidNumLogo.material = numberMaterial;
                }
            }
        }

        private int _playNum;
        private Dictionary<string, string> _playerBidMeepleDictionary;
        private Dictionary<string, string> _playerPlayMeepleDictionary;

        private MeshFilter _meepleMeshFilter;

        private void Awake()
        {
            _meepleMeshFilter = transform.Find("BoundMeeple").GetComponent<MeshFilter>();
            _playerBidMeepleDictionary = new Dictionary<string, string>();
            _playerPlayMeepleDictionary = new Dictionary<string, string>();
            _bidNumLogo = bidNumLogo.GetComponent<MeshRenderer>();
            _playNumLogo = playNumLogo.GetComponent<MeshRenderer>();
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
            BidNum = MeepleManager.Instance.GetMeepleByID(meepleID).Number;
        }

        public void RemoveBidMeeple(string playerID)
        {
            _playerBidMeepleDictionary.Remove(playerID);
            var maxBidNum = 0;
            foreach (var (_, bidMeepleID) in _playerBidMeepleDictionary)
            {
                var meepleNum = MeepleManager.Instance.GetMeepleByID(bidMeepleID).Number;
                if (maxBidNum < meepleNum)
                {
                    maxBidNum = meepleNum;
                }
            }
            BidNum = maxBidNum;
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