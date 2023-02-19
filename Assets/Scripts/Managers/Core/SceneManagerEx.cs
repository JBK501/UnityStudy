using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx 
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.Scene type)
    {
        // 씬이 바뀔 때 이전 씬의 내용(인풋처리, 사운드처리..)을 모두 없앤다.
        // (메모리 낭비를 막기위해서)
        Managers.Clear();   
        SceneManager.LoadScene(GetsceneName(type));
    }

    string GetsceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
