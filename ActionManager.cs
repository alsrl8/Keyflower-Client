using System.Collections.Generic;
using Piece;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager Instance { get; private set; }
    private Dictionary<string, string> _moveMeepleActionDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _moveMeepleActionDictionary = new Dictionary<string, string>();
    }

    private void ClearMeepleAction()
    {
        _moveMeepleActionDictionary.Clear();
    }

    public void AddMoveMeepleAction(string meepleID, string tileID)
    {
        _moveMeepleActionDictionary[meepleID] = tileID;
    }

    public void RemoveMoveMeepleAction(string meepleID)
    {
        if (!_moveMeepleActionDictionary.ContainsKey(meepleID)) return;
        _moveMeepleActionDictionary.Remove(meepleID);
    }

    public bool ValidateMeepleBidAction()
    {
        // Check if number of moved meeples are enough to participating in bidding.
        var tileMeepleNumDictionary = new Dictionary<string, int>();
        foreach (var (meepleID, tileID) in _moveMeepleActionDictionary)
        {
            tileMeepleNumDictionary.TryAdd(tileID, 0);
            tileMeepleNumDictionary[tileID] += 1;
        }

        foreach (var (tileID, bidNum) in tileMeepleNumDictionary)
        {
            if (bidNum <= TileManager.Instance.GetBidNum(tileID)) return false;
        }
        return true;
    }
    
    public void SendCurrentTurnMeepleMoveAction()
    {
        // Check if number of moved meeples are enough to participating in bidding.
        var tileMeepleNumDictionary = new Dictionary<string, int>();
        foreach (var (meepleID, tileID) in _moveMeepleActionDictionary)
        {
            tileMeepleNumDictionary.TryAdd(tileID, 0);
            tileMeepleNumDictionary[tileID] += 1;
        }

        // Send meeples' movement to another players.
        var actions = new List<PlayerActionData>();
        foreach (var (meepleID, tileID) in _moveMeepleActionDictionary)
        {
            actions.Add(new PlayerActionData
            {
                type = PlayerActionType.MoveMeeple,
                data = JsonUtility.ToJson(new MoveMeepleData
                {
                    meepleID = meepleID,
                    tileID = tileID,
                })
            });
        }

        // Set bid num and bid player onto tile.
        foreach (var (tileID, meepleCnt) in tileMeepleNumDictionary)
        {
            TileManager.Instance.SetBidNum(tileID, meepleCnt);
            TileManager.Instance.SetBidPlayer(tileID, GameManager.Instance.UserID);
        }

        // Send bid information to another players.
        foreach (var (tileID, meepleCnt) in tileMeepleNumDictionary)
        {
            actions.Add(new PlayerActionData
            {
                type = PlayerActionType.SetTileBidNum,
                data = JsonUtility.ToJson(new SetTileBidNumData
                {
                    tileID = tileID,
                    bidNum = meepleCnt,
                })
            });
        }

        var serverMessage = new ServerMessage
        {
            type = ServerMessageType.EndPlayerAction,
            data = JsonUtility.ToJson(new EndPlayerActionData
            {
                playerID = GameManager.Instance.UserID,
                actions = actions,
            })
        };
        WebSocketClient.Instance.SendMessageToServer(serverMessage);
        ClearMeepleAction();
    }
}

public class PlayerAction
{
    public string PlayerID { get; set; }
}

public class MoveMeepleAction : PlayerAction
{
    public void MyFunc()
    {
        Debug.Log($"{PlayerID}");
    }
}