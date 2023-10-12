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
        public Mesh meepleMeshRed;
        public Mesh meepleMeshBlue;
        public Mesh meepleMeshYellow;
        public Mesh meepleMeshGreen;
        public Mesh meepleMeshPurple;
        private Tile _triggeredTile;
        private Dictionary<string, Tile> _tileDictionary;
        private Dictionary<string, TileInfoData> _tileInfoDictionary;
        private Dictionary<string, HashSet<string>> _tileMeepleDictionary;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                _tileDictionary = new Dictionary<string, Tile>();
                _tileInfoDictionary = new Dictionary<string, TileInfoData>();
                _tileMeepleDictionary = new Dictionary<string, HashSet<string>>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddNewTile(NewTileData tileData)
        {
            var tilePosition = GetTileInitialPositionByOrder(_tileDictionary.Count);
            var tileObj = Instantiate(tilePrefab, tilePosition, Quaternion.identity, tilesGroup.transform);
            tileObj.name = tileData.tileID;
            _tileDictionary.Add(tileData.tileID, tileObj.GetComponent<Tile>());
            _tileInfoDictionary.Add(tileData.tileID, tileData.tileInfo);
            _tileMeepleDictionary.Add(tileData.tileID, new HashSet<string>());
        }

        public void BindMeepleToTile(string tileID, string meepleID)
        {
            _tileMeepleDictionary[tileID].Add(meepleID);
            
            var meepleColor = MeepleManager.Instance.GetMeepleColor(meepleID);
        }
        
        public void UnbindMeepleFromTile(string tileID, string meepleID)
        {
            _tileMeepleDictionary[tileID].Remove(meepleID);
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
            return _tileDictionary[tileID].Color; 
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

        public string GetTileColorByID(string tileID)
        {
            return _tileDictionary[tileID].Color;
        }

        public void SetTileColorByID(string tileID, string color)
        {
            _tileDictionary[tileID].Color = color;
        }
    }
}