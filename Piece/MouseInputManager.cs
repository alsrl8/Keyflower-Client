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
        private Vector3 _dragStartPosition;

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

            GameObject playerMeeple = null;
            for (var i = 0; i < hitCount; i++)
            {
                var hit = hits[i];
                var meepleID = hit.collider.gameObject.name;
                if (!MeepleManager.Instance.IsMeepleBelongToUser(meepleID)) continue;
                playerMeeple = hit.collider.gameObject;
                break;
            }

            if (ReferenceEquals(playerMeeple, null)) return; // If no player meeples were found among hits, return early

            _isDragging = true;
            _currentlyDragging = playerMeeple;
            _dragStartPosition = _currentlyDragging.transform.position;

            MoveSlightlyAboveDraggingObject();
            Cursor.visible = false;
            MeepleManager.Instance.ActivateOutline(_currentlyDragging.name);
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
            if (!_isDragging) return;

            var transformPosition = _currentlyDragging.transform.position;
            _currentlyDragging.transform.position = new Vector3(transformPosition.x, 0.6f, transformPosition.z);
            Cursor.visible = true;

            if (TileManager.Instance.IsAnyTileActivate())
            {
                if (IsDraggableToThisTile())
                {
                    var meepleID = _currentlyDragging.name;
                    var tileID = TileManager.Instance.GetActiveTileID();
                    GameManager.Instance.BindMeepleAndActiveTile(meepleID);
                    MeepleActionManager.Instance.AddMeepleBidAction(meepleID, tileID);
                }
                else
                {
                    _currentlyDragging.transform.position = _dragStartPosition;
                }

                TileManager.Instance.InactiveOutline();
                TileManager.Instance.SetActiveTileMeshNull();
            }
            
            MeepleManager.Instance.InactiveOutline(_currentlyDragging.name);
            _currentlyDragging = null;
            _isDragging = false;
        }

        private static string GetDraggableLayerName() // Currently it's handling only Meeple objects.
        {
            return "Meeple";
        }

        private string GetTileLayerName()
        {
            return "Tile";
        }

        private bool IsDraggableToThisTile()
        {
            var meepleColor = MeepleManager.Instance.GetMeepleColor(_currentlyDragging.name);
            if (meepleColor == "Green") return true;

            var initialTileColor = TileManager.Instance.GetInitialMeepleColorOfActiveTile();
            return initialTileColor == "" || initialTileColor == meepleColor;
        }
    }
}