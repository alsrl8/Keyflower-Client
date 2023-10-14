using System.Linq;
using UI;
using UnityEngine;

namespace Piece
{
    public class MouseInputManager : MonoBehaviour
    {
        public static MouseInputManager Instance { get; private set; }

        private Camera _camera;
        private Ray _ray;
        private bool _isDragging;
        private GameObject _currentlyDragging;
        private Meeple _triggeredMeeple;
        private Chest _triggeredChest;

        // Singleton pattern
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Retain the object when switching scenes
            }
            else
            {
                Destroy(gameObject); // Destroy additional instance
            }
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            // Handle the start of a drag operation
            if (Input.GetMouseButtonDown(0))
            {
                if (TileDialogue.Instance.IsTileDialogueAlive()) return;

                StartDragging();
                if (_isDragging)
                {
                    return;
                }

                HandleTileClickEvent();
            }

            // Handle the end of a drag operation
            if (Input.GetMouseButtonUp(0) && _isDragging)
            {
                FinishDragging();
            }

            // Update the position of the object being dragged
            if (Input.GetMouseButton(0) && _isDragging)
            {
                UpdateDragging();
            }
        }

        private void StartDragging()
        {
            _ray = _camera.ScreenPointToRay(Input.mousePosition);

            var layerMask = 1 << LayerMask.NameToLayer(GetDraggableLayerName());
            var hits = new RaycastHit[100];
            var hitCount = Physics.RaycastNonAlloc(_ray, hits, Mathf.Infinity, layerMask);
            if (hitCount == 0) return;

            GameObject clickedMeeple = null;
            for (var i = 0; i < hitCount; i++)
            {
                var hit = hits[i];
                var meepleID = hit.collider.gameObject.name;
                if (!MeepleManager.Instance.IsMeepleBelongToUser(meepleID)) continue;
                else if (MeepleManager.Instance.IsWinningOnTile(meepleID)) continue;
                clickedMeeple = hit.collider.gameObject;
                break;
            }

            if (ReferenceEquals(clickedMeeple, null)) return; // If no player meeples were found among hits, return early

            _isDragging = true;
            _currentlyDragging = clickedMeeple;

            MoveSlightlyAboveDraggingObject();
            Cursor.visible = false;
            MeepleManager.Instance.ActivateOutline(_currentlyDragging.name);
            var triggeredTileID = MeepleManager.Instance.GetTileIDByMeepleID(_currentlyDragging.name);
            if (!ReferenceEquals(triggeredTileID, ""))
            {
                TileManager.Instance.ActivateOutline(triggeredTileID);
                TileManager.Instance.TriggerTile(triggeredTileID);
            }
            
            ReleaseDraggingMeeple();
        }

        private void MoveSlightlyAboveDraggingObject()
        {
            var position = _currentlyDragging.transform.position + Vector3.up * 0.4f;
            _currentlyDragging.transform.position = position;
        }


        private void HandleTileClickEvent()
        {
            // Handle tile click event
            var layerMask = 1 << LayerMask.NameToLayer(GetTileLayerName());
            if (!Physics.Raycast(_ray, out var hit, Mathf.Infinity, layerMask)) return;

            var tileID = hit.collider.gameObject.name;
            var tileInfo = TileManager.Instance.GetTileInfoByTileID(tileID);
            TileDialogue.Instance.SetTileInfo(tileInfo);
            TileDialogue.Instance.ActivateTileDialogue();
        }

        private void UpdateDragging()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerMask.GetMask("Ground"))) return;
            var newPosition = new Vector3(hit.point.x, 1.0f, hit.point.z);

            // Convert world position to viewport position
            var viewportPos = _camera.WorldToViewportPoint(newPosition);

            // Ensure it's within the defined area before updating position
            if (viewportPos.x < 0f || viewportPos.x > 1f || viewportPos.y < 0f || viewportPos.y > 1f || !(viewportPos.z >= 0))
            {
                return;
            }

            _currentlyDragging.transform.position = newPosition;
        }

        private void FinishDragging()
        {
            var transformPosition = _currentlyDragging.transform.position;
            _currentlyDragging.transform.position = new Vector3(transformPosition.x, 0.6f, transformPosition.z);
            Cursor.visible = true;

            // Handle triggered meeple before tile.
            if (!ReferenceEquals(_triggeredMeeple, null))
            {
                DropOnMeepleEvent();
            }
            else if (!ReferenceEquals(_triggeredChest, null))
            {
                DropOnChestEvent();
            }
            else if (TileManager.Instance.IsTileTriggered())
            {
                DropOnTileEvent();
            }

            MeepleManager.Instance.InactiveOutline(_currentlyDragging.name);
            _currentlyDragging = null;
            _isDragging = false;
        }

        private void DropOnMeepleEvent()
        {
            _currentlyDragging.TryGetComponent<Meeple>(out var draggingMeeple);
            if (draggingMeeple.Number + _triggeredMeeple.Number > 6) return;
            _triggeredMeeple.GroupMeeple(draggingMeeple);
            _triggeredMeeple.InactiveOutline();
            SetTriggeredMeeple(null);
        }

        private void DropOnChestEvent()
        {
            _currentlyDragging.TryGetComponent<Meeple>(out var meeple);
            if (meeple.Number == 1)
            {
                MoveManager.Instance.MoveToMySide(_currentlyDragging, 0f);
            }
            else
            {
                meeple.Number = 1;
                MoveManager.Instance.MoveToMySide(_currentlyDragging, 0f);
                foreach (var childMeeple in meeple.ChildrenIDs.Select(childMeepleID => MeepleManager.Instance.GetMeepleByID(childMeepleID)))
                {
                    childMeeple.gameObject.SetActive(true);
                    childMeeple.transform.position = _currentlyDragging.transform.position;
                    MoveManager.Instance.MoveToMySide(childMeeple.gameObject, 0f);
                }

                meeple.ChildrenIDs.Clear();
            }

            Chest.Instance.InactivateOutline();
            SetTriggeredChest(null);
        }

        private void DropOnTileEvent()
        {
            var tileID = TileManager.Instance.GetTriggeredTileID();
            if (IsDraggingMeepleHasSameColorWithTile() && IsDraggingMeepleNumBiggerThanTileBidNum(tileID))
            {
                HandleGroupToBidMeeple(tileID);
            }
            else
            {
                MoveManager.Instance.MoveToMySide(_currentlyDragging, 0f);
            }

            TileManager.Instance.InactiveOutline(tileID);
            TileManager.Instance.UnTriggerTile();
        }

        private void ReleaseDraggingMeeple()
        {
            var meepleID = _currentlyDragging.name;
            var tileID = MeepleManager.Instance.GetTileIDByMeepleID(meepleID);
            if (tileID is "") return;
            GameManager.Instance.UnBidMeepleFromTile(meepleID, tileID);
            MeepleActionManager.Instance.RemoveMeepleAction(meepleID);
        }

        private void HandleGroupToBidMeeple(string tileID)
        {
            var draggingMeepleID = _currentlyDragging.name;
            var tilePosition = TileManager.Instance.GetTilePositionByID(tileID);
            var bidMeepleID = TileManager.Instance.GetMyMeepleID(tileID);

            if (draggingMeepleID == bidMeepleID)
            {
                MeepleManager.Instance.GetMeepleByID(draggingMeepleID).transform.position = new Vector3(tilePosition.x, 0.6f, tilePosition.z - 1.2f);
                return;
            }

            if (bidMeepleID is "")
            {
                GameManager.Instance.BidMeepleToActiveTile(draggingMeepleID);
                MeepleActionManager.Instance.AddMeepleBidAction(draggingMeepleID, tileID);
                _currentlyDragging.transform.position = new Vector3(tilePosition.x, 0.6f, tilePosition.z - 1.2f);
            }
            else
            {
                _currentlyDragging.TryGetComponent<Meeple>(out var draggingMeeple);
                var myMeeple = MeepleManager.Instance.GetMeepleByID(bidMeepleID);
                if (draggingMeeple.Number + myMeeple.Number > 6) return;
                TileManager.Instance.SetBidNumByTileID(tileID, draggingMeeple.Number);
                myMeeple.GroupMeeple(draggingMeeple);
                MeepleActionManager.Instance.AddMeepleBidAction(myMeeple.name, tileID);
            }
        }


        private static string GetDraggableLayerName() // Currently it's handling only Meeple objects.
        {
            return "Meeple";
        }

        private string GetTileLayerName()
        {
            return "Tile";
        }

        private bool IsDraggingMeepleHasSameColorWithTile()
        {
            var meepleColor = MeepleManager.Instance.GetMeepleColor(_currentlyDragging.name);
            if (meepleColor == "Green") return true;
            var tileColor = TileManager.Instance.GetColorOfTriggeredTile();
            return ReferenceEquals(tileColor, null) || tileColor == meepleColor;
        }

        private bool IsDraggingMeepleNumBiggerThanTileBidNum(string tileID)
        {
            var meepleNum = MeepleManager.Instance.GetMeepleByID(_currentlyDragging.name).Number;
            var bidNum = TileManager.Instance.GetBidNumByTileID(tileID);
            return bidNum < meepleNum;
        }

        public bool IsDraggingMeeple()
        {
            return _isDragging;
        }

        public bool IsThisDraggingMeeple(string meepleID)
        {
            return !ReferenceEquals(_currentlyDragging, null) && _currentlyDragging.name == meepleID;
        }

        public Meeple GetTriggeredMeeple()
        {
            return _triggeredMeeple;
        }

        public void SetTriggeredMeeple(Meeple triggeredMeeple)
        {
            _triggeredMeeple = triggeredMeeple;
        }

        public Chest GetTriggeredChest()
        {
            return _triggeredChest;
        }

        public void SetTriggeredChest(Chest triggeredChest)
        {
            _triggeredChest = triggeredChest;
        }
    }
}