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
        private TileMesh _activeTileMesh;
        private GameObject _outlineObj;
        private Dictionary<string, GameObject> _tileDictionary;
        private Dictionary<string, TileInfoData> _tileInfoDictionary;
        private Dictionary<string, HashSet<string>> _tileMeepleDictionary;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                CreateOutline();
                _tileDictionary = new Dictionary<string, GameObject>();
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
            var newTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, tilesGroup.transform);
            newTile.name = tileData.tileID;
            _tileDictionary.Add(tileData.tileID, newTile);
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

        public void SetActiveTileMesh(TileMesh tileMesh)
        {
            _activeTileMesh = tileMesh;
        }

        public void SetActiveTileMeshNull()
        {
            _activeTileMesh = null;
        }

        public bool IsAnyTileActivate()
        {
            return !ReferenceEquals(_activeTileMesh, null);
        }

        public void ActiveOutline()
        {
            _outlineObj.SetActive(true);
        }

        public void SetOutlinePositionNearByActiveTileMesh()
        {
            _outlineObj.transform.position = _activeTileMesh.transform.position;
        }

        public void InactiveOutline()
        {
            _outlineObj.SetActive(false);
        }

        public string GetInitialMeepleColorOfActiveTile()
        {
            return GetInitialMeepleColor(_activeTileMesh.name);
        }

        public string GetActiveTileID()
        {
            return _activeTileMesh.name;
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
    }
}