using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 네트워크, UI, 사운드...

public class Managers : MonoBehaviour
{
    //Singleton 디자인패턴 
    static Managers s_instance; // static키워드 특성상 유일성이 보장된다.
    static Managers Instance { get { Init(); return s_instance; } }  // 유일한 매니저를 갖고온다.

    InputManager _input = new InputManager();
    ResourceManager _resource = new ResourceManager();
    UI_Manager _ui = new UI_Manager();

    public static InputManager Input { get { return Instance._input; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static UI_Manager UI { get { return Instance._ui; } }
    

    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        _input.OnUpdate();
    }

    static void Init()
    {
        if(s_instance == null)
        {
            // 초기화
            GameObject go = GameObject.Find("@Managers");   // @Managers오브젝트를 찾는다.

            if(go == null)  // 못찾았으면
            {
                go = new GameObject { name = "@Managers" }; // @Managers오브젝트를 생성한다.
                go.AddComponent<Managers>();    // Managers컴포넌트를 추가한다.
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
        }

        
    }
}
