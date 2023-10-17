using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Piece;

[Serializable]
public class ServerMessage
{
    public string type;
    public string data;

    public override string ToString()
    {
        return $"Type: {type}, Data: {data}";
    }
}

public static class ServerMessageType
{
    public const string CommonMessage = "CommonMessage";
    public const string Register = "Register";
    public const string GameReady = "GameReady";
    public const string TurnChange = "TurnChange";
    public const string SeasonChange = "SeasonChange";
    public const string NewMeeple = "NewMeeple";
    public const string NewTile = "NewTile";
    public const string Chat = "Chat";
    public const string NewPlayer = "NewPlayer";
    public const string MeepleAction = "MeepleAction";
}

[Serializable]
public class ChatData
{
    public string playerID;
    public string chatTime;
    public string content;
}

[Serializable]
public class TurnChangeData
{
    public int turn;
}

[Serializable]
public class SeasonChangeData
{
    public string season;
}

[Serializable]
public class NewMeepleData
{
    public string meepleID;
    public string ownerID;
    public string color;
}

[Serializable]
public class NewTileData
{
    public string tileID;
    public string ownerID;
    public TileInfoData tileInfo;
}

[Serializable]
public class TileInfoData
{
    public string name;
    public string season;
    public bool isUpgraded;
    [JsonProperty("costToUpgrade")] public Dictionary<string, int> CostToUpgrade;
    public DetailTileInfoData basicTileInfo;
    public DetailTileInfoData upgradedTileInfo;
}

[Serializable]
public class DetailTileInfoData
{
    public int point;
    [JsonProperty("cost")] public Dictionary<string, int> Cost;
    [JsonProperty("reward")] public Dictionary<string, int> Reward;
}

[Serializable]
public class NewPlayerData
{
    public string newPlayerID;
    public int totalPlayerNum;
}

[Serializable]
public class GameReadyData
{
    public string playerID;
    public int playerTurn;
}

[Serializable]
public class MeepleActionData
{
    public string playerID;
    public List<DetailMeepleActionData> detailMeepleActions;
}

[Serializable]
public class DetailMeepleActionData
{
    public MeepleActionType type;
    public string meepleID;
    public string targetTileID;
    public int number;
    public List<string> childrenMeepleIDs;
}