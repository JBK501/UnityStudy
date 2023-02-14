using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 네트워크, UI, 사운드...

public class Managers : MonoBehaviour
{
    //Singleton 디자인패턴 
    static Managers s_instance; // static키워드 특성상 유일성이 보장된다.
    static Managers Instance { get { Init(); return s_instance; } }  // 유일한 매니저를 갖고온다.

    DataManager _data = new DataManager();
    InputManager _input = new InputManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UI_Manager _ui = new UI_Manager();

    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
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

            s_instance._data.Init();  // 대부분의 상황에서 항상 들고 있기에 Clear는 하지 않는다.
            s_instance._pool.Init();
            s_instance._sound.Init(); // 사운드인스턴스를 초기화 한다.
        }
    }

    public static void Clear()
    {
        Input.Clear();
        Sound.Clear();
        Scene.Clear();
        UI.Clear();

        Pool.Clear();
    }
}
