using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Json파일의 데이터 명칭이랑 클래스의 필드 명칭이랑 일치해야 한다.
// [Serializable] -> 어떤 형식으로 데이터가 들어올지 데이터 포맷을 참조한다.

#region Stat
[Serializable]  
public class Stat
{
    public int level;
    public int hp;
    public int attack;
}

[Serializable]
public class StatData : ILoader<int, Stat>
{
    public List<Stat> stats = new List<Stat>();

    public Dictionary<int, Stat> MakeDict()
    {
        Dictionary<int, Stat> dict = new Dictionary<int, Stat>();

        // 받아온 데이터를 딕셔너리로 변환한다.
        foreach (Stat stat in stats)
            dict.Add(stat.level, stat);

        return dict;
    }
}
#endregion