using System.Collections.Generic;
using UnityEngine;

namespace Piece
{
    public enum MeepleActionType
    {
        Bid,
        Play,
    }

    public class MeepleAction
    {
        public MeepleActionType Type { get; set; }
        public string MeepleID { get; set; }
        public string TargetTileID { get; set; }
    }

    public class MeepleActionManager : MonoBehaviour
    {
        public static MeepleActionManager Instance { get; private set; }
        private Dictionary<string, MeepleAction> _actions;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                _actions = new Dictionary<string, MeepleAction>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddMeepleBidAction(string meepleID, string tileID)
        {
            _actions[meepleID] = new MeepleAction
            {
                Type = MeepleActionType.Bid,
                MeepleID = meepleID,
                TargetTileID = tileID,
            };
        }
    }
}