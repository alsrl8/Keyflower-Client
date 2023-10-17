using Piece;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TileDialogue : MonoBehaviour
    {
        public static TileDialogue Instance { get; private set; }
        public GameObject tileDialogueObj;
        private TextMeshProUGUI _tileNameTextComponent;
        private TextMeshProUGUI _tileScoreTextComponent;
        private TextMeshProUGUI _tileOwnerIdTextComponent;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                _tileNameTextComponent = tileDialogueObj.transform.Find("Tile Name").GetComponent<TextMeshProUGUI>();
                _tileScoreTextComponent = tileDialogueObj.transform.Find("Score").GetComponent<TextMeshProUGUI>();
                _tileOwnerIdTextComponent = tileDialogueObj.transform.Find("Owner ID").GetComponent<TextMeshProUGUI>();
                DontDestroyOnLoad(gameObject);
                
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetTileInfo(Tile tile, TileInfoData tileInfo)
        {
            // TODO TileInfo 설정할 때 계절 정보에 따라 계절 아이콘을 수정하는 기능 추가할 것
            _tileOwnerIdTextComponent.text = tile.BidWinner;
            _tileNameTextComponent.text = tileInfo.name;
            _tileScoreTextComponent.text = (!tileInfo.isUpgraded ? tileInfo.basicTileInfo.point : tileInfo.upgradedTileInfo.point).ToString();
        }

        public bool IsTileDialogueAlive()
        {
            return tileDialogueObj.activeSelf;
        }

        public void ActivateTileDialogue()
        {
            tileDialogueObj.SetActive(true);
        }

        public void InactivateTileDialogue()
        {
            tileDialogueObj.SetActive(false);
        }
    }
}