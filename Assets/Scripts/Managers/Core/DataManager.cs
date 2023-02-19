using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 해당 인터페이스를 적용한 경우 반드시 MakeDict를 구현하도록 한다.
public interface ILoader<key, Value>
{
    Dictionary<key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.Stat> StatDict { get; private set; } = new Dictionary<int, Data.Stat>();

    public void Init()
    {
        StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();
    }

    // Loader는 Key와 Value를 지니고 있는 ILoader를 반드시 지니고 있어야 한다.
    Loader LoadJson<Loader, key, Value>(string path) where Loader : ILoader<key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        Loader data = JsonUtility.FromJson<Loader>(textAsset.text);

        return data;
    }
}
