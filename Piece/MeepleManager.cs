using System.Collections.Generic;
using UnityEngine;

namespace Piece
{
    public class MeepleManager : MonoBehaviour
    {
        public static MeepleManager Instance { get; private set; }
        public GameObject meepleRedPrefab;
        public GameObject meepleBluePrefab;
        public GameObject meepleYellowPrefab;
        public GameObject meepleGreenPrefab;
        public GameObject meeplePurplePrefab;
        public Material meepleTransparentMaterial;
        public GameObject meeplesGroup;
        private Dictionary<string, GameObject> _meepleDictionary;
        private Dictionary<string, MeepleOutline> _meepleOutlineDictionary;
        private Dictionary<string, string> _meepleColorDictionary;
        private Dictionary<string, GameObject> _myMeepleDictionary;
        private Dictionary<string, string> _meepleTileDictionary;

        // Singleton pattern
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Retain the object when switching scenes

                _meepleDictionary = new Dictionary<string, GameObject>();
                _meepleOutlineDictionary = new Dictionary<string, MeepleOutline>();
                _myMeepleDictionary = new Dictionary<string, GameObject>();
                _meepleColorDictionary = new Dictionary<string, string>();
                _meepleTileDictionary = new Dictionary<string, string>();
            }
            else
            {
                Destroy(gameObject); // Destroy additional instance
            }
        }

        public void ActivateMeepleGroup()
        {
            meeplesGroup.SetActive(true);
        }

        public GameObject GetMeepleByID(string meepleID)
        {
            return _meepleDictionary[meepleID];
        }


        public void AddNewMeeple(NewMeepleData meepleData)
        {
            var meepleID = meepleData.meepleID;
            var ownerID = meepleData.ownerID;
            var prefab = GetPrefabWithColor(meepleData.color);
            var position = new[] { 12.26f, 0.19f, 6.33f };
            var rotation = new[] { 0f, 90f, 0f };
            var newMeeple = Instantiate(prefab, new Vector3(position[0], position[1], position[2]), Quaternion.Euler(rotation[0], rotation[1], rotation[2]), meeplesGroup.transform);
            AssignObjectIdToMeeple(newMeeple, meepleID, ownerID);

            if (ownerID == GameManager.Instance.UserID)
            {
                AddMeepleToMe(meepleID, newMeeple);
                var randomX = Random.Range(-5.0f, 5.0f);
                var randomZ = Random.Range(-9.5f, -9.0f);
                MoveManager.Instance.MoveToPosition(newMeeple, new Vector3(randomX, 0.6f, randomZ), 3.5f);
            }
            else
            {
                newMeeple.GetComponent<MeshRenderer>().material = meepleTransparentMaterial;
                newMeeple.SetActive(false);
            }

            _meepleDictionary.Add(meepleID, newMeeple);
            _meepleOutlineDictionary.Add(meepleID, newMeeple.GetComponent<MeepleOutline>());
            _meepleColorDictionary.Add(meepleID, meepleData.color);
        }

        private void AssignObjectIdToMeeple(GameObject meeple, string meepleID, string ownerID)
        {
            meeple.name = meepleID;
            var objectIDComponent = meeple.AddComponent<ObjectID>();
            objectIDComponent.Initialize(meepleID, ownerID);
        }

        private GameObject GetPrefabWithColor(string color)
        {
            switch (color)
            {
                case "Red":
                    return meepleRedPrefab;
                case "Blue":
                    return meepleBluePrefab;
                case "Yellow":
                    return meepleYellowPrefab;
                case "Green":
                    return meepleGreenPrefab;
                case "Purple":
                    return meeplePurplePrefab;
            }

            return null;
        }

        public void AddMeepleToMe(string meepleID, GameObject meeple)
        {
            _myMeepleDictionary.Add(meepleID, meeple);
        }

        public bool IsMeepleBelongToUser(string meepleID)
        {
            return _myMeepleDictionary.ContainsKey(meepleID);
        }

        public string GetTileIDByMeepleID(string meepleID)
        {
            return _meepleTileDictionary.TryGetValue(meepleID, out var value) ? value : "";
        }

        public void ActivateOutline(string meepleID)
        {
            _meepleOutlineDictionary[meepleID].Activate();
        }


        public void InactiveOutline(string meepleID)
        {
            _meepleOutlineDictionary[meepleID].Inactivate();
        }


        public string GetMeepleColor(string meepleID)
        {
            return _meepleColorDictionary[meepleID];
        }

        private void MoveMyMeepleToTile(string meepleID, string tileID)
        {
            var meeple = _meepleDictionary[meepleID];
            var tilePosition = TileManager.Instance.GetTilePosition(tileID);

            var xPosition = tilePosition.x + Random.Range(-0.5f, 0.5f);
            var yPosition = 0.6f;
            var zPosition = tilePosition.z - 1.3f;
            meeple.transform.position = new Vector3(xPosition, yPosition, zPosition);
        }

        private void MoveOtherMeepleToTile(string meepleID, string tileID)
        {
            var meeple = _meepleDictionary[meepleID];
            meeple.SetActive(true);
            var tilePosition = TileManager.Instance.GetTilePosition(tileID);

            var randomLeftOrRight = Random.Range(-1f, 1f);
            if (randomLeftOrRight < 0)
            {
                var randomVar = Random.Range(1.0f, 1.5f);
                var xPosition = tilePosition.x - randomVar;
                var yPosition = 0.6f;
                var zPosition = tilePosition.z - (randomVar * 0.5f / 0.8f);
                MoveManager.Instance.MoveToPosition(meeple, new Vector3(xPosition, yPosition, zPosition), 0f);
            }
            else
            {
                var randomVar = Random.Range(1.0f, 1.5f);
                var xPosition = tilePosition.x + randomVar;
                var yPosition = 0.6f;
                var zPosition = tilePosition.z - (randomVar * 0.5f / 0.8f);
                MoveManager.Instance.MoveToPosition(meeple, new Vector3(xPosition, yPosition, zPosition), 0f);
            }
        }


        public void BindToTile(string meepleID, string tileID)
        {
            _meepleTileDictionary[meepleID] = tileID;

            if (IsMeepleBelongToUser(meepleID))
            {
                MoveMyMeepleToTile(meepleID, tileID);
            }
            else
            {
                MoveOtherMeepleToTile(meepleID, tileID);
            }
        }

        public void UnbindMeeple(string meepleID)
        {
            if (!_meepleTileDictionary.ContainsKey(meepleID)) return;
            _meepleTileDictionary.Remove(meepleID);
        }

        public bool IsMeepleBoundToTile(string meepleID)
        {
            return _meepleTileDictionary.ContainsKey(meepleID);
        }
    }
}