using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{

    // [코루틴]
    // -> 함수의 상태를 저장/복원 가능하다.
    //      1. 엄청 오래 걸리는 작업을 잠시 멈출 경우
    //      2. 원하는 타이밍에 함수를 잠시 멈췄다가 복원하는 경우

    // -> return타입을 우리가 원하는 타입으로 가능하다.(class도 가능)

    //Coroutine co;

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Managers.UI.ShowSceneUI<UI_Inven>();

        Dictionary<int, Data.Stat > dict = Managers.Data.StatDict;

        //// 4초 후에 폭발한다.
        //co = StartCoroutine("ExplodeAfterSeconds", 4.0f);

        //// 맘이 바껴서 2초후에 폭발을 멈춘다.
        //StartCoroutine("CoStopExplode", 2.0f);
    }

    //IEnumerator CoStopExplode(float seconds)
    //{
    //    Debug.Log("Stop Enter");
    //    yield return new WaitForSeconds(seconds);
    //    Debug.Log("Stop Execute!!");

    //    if( co != null)
    //    {
    //        StopCoroutine(co);
    //        co = null;
    //    }
    //}

    //IEnumerator ExplodeAfterSeconds(float seconds)
    //{
    //    Debug.Log("Explode Enter");
    //    yield return new WaitForSeconds(seconds);
    //    Debug.Log("Explode Execute!!");
    //    co = null;
    //}

    public override void Clear()
    {

    }
}
