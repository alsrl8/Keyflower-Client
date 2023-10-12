using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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
    public const string MoveMeeple = "MoveMeeple";
    public const string Chat = "Chat";
    public const string NewPlayer = "NewPlayer";
    public const string EndPlayerAction = "EndPlayerAction";
    public const string OtherPlayerAction = "OtherPlayerAction";
}

public static class PlayerActionType
{
    public const string MoveMeeple = "MoveMeeple";
    public const string SetTileBidNum = "SetTileBidNum";
}

[Serializable]
public class MoveMeepleData
{
    public string meepleID;
    public string tileID;
}

[Serializable]
public class SetTileBidNumData
{
    public string tileID;
    public string playerID;
    public int bidNum;
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
public class EndPlayerActionData
{
    public string playerID;
    public List<PlayerActionData> actions;
}

[Serializable]
public class PlayerActionData
{
    public string type;
    public string data;
}

[Serializable]
public class OtherPlayerActionData
{
    public string playerID;
    public List<PlayerActionData> actions;
}