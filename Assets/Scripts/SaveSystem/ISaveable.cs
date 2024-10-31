using System;
using UnityEngine;

public interface ISaveable
{
    public Data GetData();
    public void LoadData(Data data);
}

/// <summary>
/// ALL DATA MUST HAVE [Serializable]
/// </summary>
[Serializable]
public class Data
{
}