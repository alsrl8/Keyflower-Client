using System.Collections.Generic;
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
        private Tile _activeTile;
        private GameObject _outlineObj;
        private Dictionary<string, GameObject> _tileDictionary;
        private Dictionary<string, Tile> _tileScriptDictionary;
        private Dictionary<string, TileInfoData> _tileInfoDictionary;
        private Dictionary<string, HashSet<string>> _tileMeepleDictionary;
        private Dictionary<string, string> _tileBidPlayerDictionary;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                CreateOutline();
                _tileDictionary = new Dictionary<string, GameObject>();
                _tileScriptDictionary = new Dictionary<string, Tile>();
                _tileInfoDictionary = new Dictionary<string, TileInfoData>();
                _tileMeepleDictionary = new Dictionary<string, HashSet<string>>();
                _tileBidPlayerDictionary = new Dictionary<string, string>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddNewTile(NewTileData tileData)
        {
            var tilePosition = GetTileInitialPositionByOrder(_tileDictionary.Count);
            var newTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, tilesGroup.transform);
            newTile.name = tileData.tileID;
            _tileDictionary.Add(tileData.tileID, newTile);
            _tileScriptDictionary.Add(tileData.tileID, newTile.transform.GetComponent<Tile>());
            _tileInfoDictionary.Add(tileData.tileID, tileData.tileInfo);
            _tileMeepleDictionary.Add(tileData.tileID, new HashSet<string>());
        }

        public void BindMeepleToTile(string tileID, string meepleID)
        {
            _tileMeepleDictionary[tileID].Add(meepleID);
            
            var meepleColor = MeepleManager.Instance.GetMeepleColor(meepleID);
            _tileScriptDictionary[tileID].SetBindMeepleColor(meepleColor);
        }

        public void BindMeepleToTileByID(string meepleID, string tileID)
        {
            _tileMeepleDictionary[tileID].Add(meepleID);
        }

        public void UnbindMeepleFromTile(string tileID, string meepleID)
        {
            _tileMeepleDictionary[tileID].Remove(meepleID);

            if (_tileMeepleDictionary[tileID].Count == 0)
            {
                _tileScriptDictionary[tileID].SetBindMeepleColor("");
            }
        }

        private void CreateOutline()
        {
            var outlinePreFabTransform = tileOutlinePreFab.transform;
            var tileObjectGroup = GameObject.Find("Tiles");
            _outlineObj = Instantiate(tileOutlinePreFab, outlinePreFabTransform.position, outlinePreFabTransform.rotation, tileObjectGroup.transform);
            _outlineObj.GetComponent<Renderer>().material = tileOutlineMaterial;
            var originScale = gameObject.transform.localScale;
            _outlineObj.transform.localScale = new Vector3(originScale.x * 2.7f, originScale.y * 2.5f, originScale.z * 2.8f); // Adjust size
            _outlineObj.name = "Outline";
            _outlineObj.layer = 0;
            _outlineObj.SetActive(false);
        }

        public void SetTileActive(Tile tile)
        {
            _activeTile = tile;
        }

        public void SetTileInactive()
        {
            _activeTile = null;
        }

        public bool IsAnyTileActivate()
        {
            return !ReferenceEquals(_activeTile, null);
        }

        public void ActiveOutline()
        {
            _outlineObj.SetActive(true);
        }

        public void SetOutlinePositionToActiveTile()
        {
            _outlineObj.transform.position = _activeTile.transform.position;
        }

        public void InactiveOutline()
        {
            _outlineObj.SetActive(false);
        }

        public string GetInitialMeepleColorOfActiveTile()
        {
            return GetInitialMeepleColor(_activeTile.name);
        }

        public string GetActiveTileID()
        {
            return _activeTile.name;
        }

        public string GetInitialMeepleColor(string tileID)
        {
            var initialColor = "";
            var boundMeepleSet = _tileMeepleDictionary[tileID];
            foreach (var boundMeepleID in boundMeepleSet)
            {
                var meepleColor = MeepleManager.Instance.GetMeepleColor(boundMeepleID);
                if (meepleColor == "Green") continue;
                initialColor = meepleColor;
            }

            return initialColor;
        }

        public Vector3 GetTilePosition(string tileID)
        {
            var tile = _tileDictionary[tileID];
            return tile.transform.position;
        }

        public TileInfoData GetTileInfoByTileID(string tileID)
        {
            return _tileInfoDictionary[tileID];
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

        public void SetBidNum(string tileID, int bidNum)
        {
            _tileScriptDictionary[tileID].SetBidNum(bidNum);
        }

        public void SetBidPlayer(string tileID, string playerID)
        {
            _tileBidPlayerDictionary[tileID] = playerID;
        }

        public bool IsBidToPlayer(string tileID)
        {
            return _tileBidPlayerDictionary[tileID] == GameManager.Instance.UserID;
        }

        public int GetBidNum(string tileID)
        {
            return _tileScriptDictionary[tileID].GetBidNum();
        }
    }
}