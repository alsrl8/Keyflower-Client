using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Piece
{
    public class TileManager : MonoBehaviour
    {
        public static TileManager Instance { get; private set; }
        public GameObject tilePrefab;
        public GameObject tilesGroup;
        public GameObject tileOutlinePreFab;
        public Material tileOutlineMaterial;
        public Mesh meepleMeshRed;
        public Mesh meepleMeshBlue;
        public Mesh meepleMeshYellow;
        public Mesh meepleMeshGreen;
        public Mesh meepleMeshPurple;
        private Tile _triggeredTile;
        private Dictionary<string, Tile> _tileDictionary;
        private Dictionary<string, TileInfoData> _tileInfoDictionary;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                _tileDictionary = new Dictionary<string, Tile>();
                _tileInfoDictionary = new Dictionary<string, TileInfoData>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddNewTile(NewTileData tileData)
        {
            var tilePosition = GetTileInitialPositionByOrder(_tileDictionary.Count); // TODO 계절별 타일 초기 위치를 수정할 것
            var tileObj = Instantiate(tilePrefab, tilePosition, Quaternion.identity, tilesGroup.transform);
            tileObj.name = tileData.tileID;
            _tileDictionary.Add(tileData.tileID, tileObj.GetComponent<Tile>());
            _tileInfoDictionary.Add(tileData.tileID, tileData.tileInfo);
        }

        public void BidOtherMeepleOnTile(string playerID, string meepleID, string tileID)
        {
            _tileDictionary[tileID].SetBidMeeple(playerID, meepleID);
        }

        public void BidMeepleOnTile(string meepleID, string tileID)
        {
            var playerID = GameManager.Instance.UserID;
            _tileDictionary[tileID].SetBidMeeple(playerID, meepleID);
        }

        public void UnBidFromTile(string tileID)
        {
            var playerID = GameManager.Instance.UserID;
            _tileDictionary[tileID].RemoveBidMeeple(playerID);
        }

        public string GetMyMeepleID(string tileID)
        {
            var tile = _tileDictionary[tileID];
            return tile.GetBidMeepleByPlayerID(GameManager.Instance.UserID);
        }

        public void TriggerTile(string tileID)
        {
            _triggeredTile = _tileDictionary[tileID];
        }

        public void UnTriggerTile()
        {
            _triggeredTile = null;
        }

        public bool IsTileTriggered()
        {
            return !ReferenceEquals(_triggeredTile, null);
        }

        public void ActivateOutline(string tileID)
        {
            _tileDictionary[tileID].ActivateOutline();
        }

        public void InactiveOutline(string tileID)
        {
            _tileDictionary[tileID].InactivateOutline();
        }

        public string GetColorOfTriggeredTile()
        {
            return GetColorByID(_triggeredTile.name);
        }

        public string GetTriggeredTileID()
        {
            return _triggeredTile.name;
        }

        public Vector3 GetTilePositionByID(string tileID)
        {
            return _tileDictionary[tileID].transform.position;
        }

        public string GetColorByID(string tileID)
        {
            return _tileDictionary[tileID].GetColor(); 
        }

        public void SetTileInfoToDialogueByTileID(string tileID)
        {
            var tile = _tileDictionary[tileID];
            var tileInfo = _tileInfoDictionary[tileID];
            TileDialogue.Instance.SetTileInfo(tile, tileInfo);
            
        }

        private Vector3 GetTileInitialPositionByOrder(int order)
        {
            var row = order / 3;
            var col = order % 3;
            float xPosition = 0, zPosition = 0;
            switch (row)
            {
                case 0:
                    zPosition = 2;
                    break;
                case 1:
                    zPosition = -2;
                    break;
                case 2:
                    zPosition = 6;
                    break;
                case 3:
                    zPosition = -6;
                    break;
            }

            switch (col)
            {
                case 0:
                    xPosition = 0;
                    break;
                case 1:
                    xPosition = -5;
                    break;
                case 2:
                    xPosition = 5;
                    break;
            }

            return new Vector3(xPosition, 0.6f, zPosition);
        }

        public int GetBidNumByTileID(string tileID)
        {
            return _tileDictionary[tileID].BidNum;
        }

        public void SetBidMeeple(string meepleID, string tileID)
        {
            var tile = _tileDictionary[tileID];
            var playerID = GameManager.Instance.UserID;
            tile.SetBidMeeple(playerID, meepleID);
        }

        public string GetBidWinnerByTileID(string tileID)
        {
            return _tileDictionary[tileID].BidWinner;
        }

        public List<string> GetBidWinningTileIDs() // TODO Tile Scene 테스트 용도로 만듦
        {
            var tileIDs = new List<string>();
            foreach (var (tileID, tile) in _tileDictionary)
            {
                if (tile.BidWinner == GameManager.Instance.UserID)
                {
                    var tileName = _tileInfoDictionary[tileID].name;
                    tileIDs.Add(tileName);
                } 
            }
            return tileIDs;
        }

        public void SetInactiveSeasonTiles(string season)
        {
            foreach (var (tileID, tile) in _tileDictionary)
            {
                var tileInfo = _tileInfoDictionary[tileID];
                if (tileInfo.season == season)
                {
                    tile.gameObject.SetActive(false);
                    tile.SetInactiveBidMeeples();
                }
            }
        }
        
    }
}