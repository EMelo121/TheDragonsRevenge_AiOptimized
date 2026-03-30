using UnityEngine;
using System;

[Serializable]
public class BattleTauntRequest
{
    public string enemyType;
    public string enemyStyle;
    public int enemyLevel;
    public string areaName;
    public bool isBoss;
    public int playerLevel;
    public string battlePhase;
}

[Serializable]
public class BattleTauntResponse
{
    public string taunt;
}