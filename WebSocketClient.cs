using Newtonsoft.Json;
using Piece;
using UI;
using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    public static WebSocketClient Instance { get; private set; }
    private WebSocket _webSocket;

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
        var webSocketServerAddress = GetWebSocketServerAddress();
        _webSocket = new WebSocket(webSocketServerAddress);

        // Events
        _webSocket.OnMessage += OnMessageReceived;
        _webSocket.Connect();
    }

    private void OnMessageReceived(object sender, MessageEventArgs e)
    {
        var serverMessage = JsonUtility.FromJson<ServerMessage>(e.Data);
        Debug.Log("Received message from server: " + e.Data);
        GameManager.Instance.Actions.Enqueue(() =>
        {
            switch (serverMessage.type)
            {
                case ServerMessageType.CommonMessage:
                    break;
                case ServerMessageType.OtherPlayerAction:
                    var otherPlayerActionData = JsonUtility.FromJson<OtherPlayerActionData>(serverMessage.data);
                    HandleOtherPlayerAction(otherPlayerActionData);
                    break;
                case ServerMessageType.TurnChange:
                    var turnChangeData = JsonUtility.FromJson<TurnChangeData>(serverMessage.data);
                    HandleTurnChange(turnChangeData.turn);
                    break;
                case ServerMessageType.NewMeeple:
                    NewMeeple(serverMessage.data);
                    break;
                case ServerMessageType.NewTile:
                    NewTile(serverMessage.data);
                    break;
                case ServerMessageType.Register:
                    var userID = GetIdFromServerMessageData(serverMessage.data);
                    SetUserID(userID);
                    break;
                case ServerMessageType.NewPlayer:
                    var newPlayerData = JsonUtility.FromJson<NewPlayerData>(serverMessage.data);
                    break;
                case ServerMessageType.SeasonChange:
                    var seasonChangeData = JsonUtility.FromJson<SeasonChangeData>(serverMessage.data);
                    HandleSeasonChange(seasonChangeData.season);
                    break;
                case ServerMessageType.GameReady:
                    var gameReadyData = GetGameReadyDataFromServerMessageData(serverMessage.data);
                    HandleGameStart(gameReadyData);
                    break;
            }
        });
    }

    private void OnApplicationQuit()
    {
        _webSocket?.Close();
    }

    public void SendMessageToServer(ServerMessage serverMessage)
    {
        if (!_webSocket.IsAlive) return;

        var data = JsonUtility.ToJson(serverMessage);
        _webSocket.Send(data);
    }

    private string GetWebSocketServerAddress()
    {
        return "ws://192.168.0.6:8080";
    }

    public void RegisterClient()
    {
        SendMessageToServer(new ServerMessage
        {
            type = ServerMessageType.Register,
        });
    }

    private string GetIdFromServerMessageData(string data)
    {
        return data;
    }

    private void SetUserID(string userID)
    {
        GameManager.Instance.Actions.Enqueue(() => { GameManager.Instance.UserID = userID; });
    }

    private void NewMeeple(string data)
    {
        var newMeepleData = JsonUtility.FromJson<NewMeepleData>(data);
        GameManager.Instance.Actions.Enqueue(() => { MeepleManager.Instance.AddNewMeeple(newMeepleData); });
    }

    private void NewTile(string data)
    {
        var newTileData = JsonConvert.DeserializeObject<NewTileData>(data);
        GameManager.Instance.Actions.Enqueue(() => { TileManager.Instance.AddNewTile(newTileData); });
    }

    private void HandleTurnChange(int turn)
    {
        GameManager.Instance.Actions.Enqueue(() => { GameManager.Instance.SetCurrentTurn(turn); });
    }

    private void HandleSeasonChange(string season)
    {
        switch (season)
        {
            case "Summer":
                RoundManager.Instance.StartSummer();
                break;
            case "Autumn":
                RoundManager.Instance.StartAutumn();
                break;
            case "Winter":
                RoundManager.Instance.StartWinter();
                break;
        }    
    }

    private GameReadyData GetGameReadyDataFromServerMessageData(string data)
    {
        var gameReadyData = JsonUtility.FromJson<GameReadyData>(data);
        return gameReadyData;
    }

    private void HandleGameStart(GameReadyData gameReadyData)
    {
        var myTurn = gameReadyData.playerTurn;
        GameManager.Instance.GameStart(myTurn);
    }

    private void HandleOtherPlayerAction(OtherPlayerActionData otherPlayerActionData)
    {
        foreach (var actionData in otherPlayerActionData.actions)
        {
            switch (actionData.type)
            {
                case PlayerActionType.MoveMeeple:
                    var moveMeepleData = JsonUtility.FromJson<MoveMeepleData>(actionData.data);
                    if (MeepleManager.Instance.IsMeepleBelongToUser(moveMeepleData.meepleID)) continue; // Don't move meeple if it belongs to this player.
                    GameManager.Instance.BidMeepleToTile(moveMeepleData.meepleID, moveMeepleData.tileID);
                    break;
            }
        }
    }
}