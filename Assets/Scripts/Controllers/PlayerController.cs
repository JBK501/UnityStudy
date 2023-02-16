using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 1. 위치 벡터
// 2. 방향 벡터
//struct MyVector
//{
//    public float x;
//    public float y;
//    public float z;


//    public float magnitude { get { return Mathf.Sqrt( x * x + y * y + z * z); } }
//    public MyVector normalized { get { return new MyVector(x / magnitude, y / magnitude, z / magnitude); } }    // 단위벡터로 방향에 대한 정보를 구한다.

//    public MyVector(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }

//    public static MyVector operator +(MyVector a, MyVector b)
//    {
//        return new MyVector(a.x + b.x, a.y + b.y, a.z + b.z);
//    }
//    public static MyVector operator -(MyVector a, MyVector b)
//    {
//        return new MyVector(a.x - b.x, a.y - b.y, a.z - b.z);
//    }

//    public static MyVector operator *(MyVector a, float d)
//    {
//        return new MyVector(a.x * d, a.y * d, a.z * d);
//    }
//}

//// 위치 벡터
//MyVector to = new MyVector(10.0f, 0.0f, 0.0f);
//MyVector from = new MyVector(5.0f, 0.0f, 0.0f);

//// 방향 벡터는 두 가지 정보를 얻을 수 있다.
//// 1. 거리(크기)    5   magnitude
//// 2. 실제 방향     ->  normalized (가리키는 방향은 똑같지만 크기가 1인 단위벡터를 구한다)

//MyVector dir = to - from; // {5.0f, 0.0f, 0.0f}

//dir = dir.normalized;   //{1.0f, 0.0f, 0.0f }   

//MyVector newPos = from + dir * _speed;


public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
        Skill,
    }

    int _mask = (1 << (int)Define.Layer.Ground | (1 << (int)Define.Layer.Monster));

    //[SerializeField]    // 유니티 상에서 멤버값을 표시한다.
    //float _speed = 10.0f;
    PlayerStat _stat;

    //bool _moveToDest = false;
    Vector3 _destPos;

    [SerializeField]
    PlayerState _state = PlayerState.Idle;

    GameObject _lockTarget;

    public PlayerState State
    {
        get { return _state; }
        set
        {
            _state = value;

            Animator anim = GetComponent<Animator>();
            switch (_state)
            {
                case PlayerState.Die:
                    break;
                case PlayerState.Idle:
                    anim.CrossFade("WAIT", 0.1f);
                    break;
                case PlayerState.Moving:
                    anim.CrossFade("RUN", 0.1f);
                    break;
                case PlayerState.Skill:
                    anim.CrossFade("ATTACK", 0.1f, -1, 0);
                    break;
            }
        }
    }

    void Start()
    {
        

        _stat = gameObject.GetComponent<PlayerStat>();

        //Managers.Input.KeyAction -= OnKeyboard; // 다른 데서 키보드 입력했을 경우를 방지한다.
        //Managers.Input.KeyAction += OnKeyboard;

        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);

    }

    void UpdateDie()
    {
        // 아무것도 못함
    }

    void UpdateMoving()
    {
        // 몬스터가 플레이어와의 사정거리보다 가까우면 공격한다.
        if(_lockTarget != null)
        {
            float distance = (_destPos - transform.position).magnitude; // 플레이어와 몬스터 사이의 거리

            if(distance <= 1)   // 사정거리 내에 있으면
            {
                State = PlayerState.Skill; // 플레이어 상태를 스킬로 변경한다.
                return;
            }
        }

        Vector3 dir = _destPos - transform.position;  // 이동한다.
        if (dir.magnitude < 0.1f)   // 목적지에 도착했으면
        {
            State = PlayerState.Idle;  // 정지 상태로 바꾼다.
        }
        else
        {
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();

            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            nma.Move(dir.normalized * moveDist);

            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.green);
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))
            {
                if(Input.GetMouseButton(0) == false)
                    State = PlayerState.Idle;
                return;
            }

            //transform.position += dir.normalized * moveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }

        //// 애니메이션 처리
        //Animator anim = GetComponent<Animator>();
        //// 현재 게임 상태에 대한 정보를 넘겨준다.
        //anim.SetFloat("speed", _stat.MoveSpeed);
    }


    void UpdateIdle()
    {

    }

    void UpdateSkill()
    {
        if(_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    void OnHitEvent()
    {
        if(_lockTarget != null)
        {
            // TODO
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            Stat myStat = gameObject.GetComponent<PlayerStat>();

            int damage = Mathf.Max(0, myStat.Attack - targetStat.Defense);

            Debug.Log(damage);

            targetStat.Hp -= damage;
        }

        if(_stopSkill)
        {
            State = PlayerState.Idle;
        }
        else
        {
            State = PlayerState.Skill;
        }
    }

    void Update()
    {
        switch (State)
        {
            case PlayerState.Die:
                UpdateDie();
                break;
            case PlayerState.Moving:
                UpdateMoving();
                break;
            case PlayerState.Idle:
                UpdateIdle();
                break;
            case PlayerState.Skill:
                UpdateSkill();
                break;
        }
    }

    //void OnKeyboard()
    //{
    //    //_yAngle += Time.deltaTime * 100.0f;

    //    // + - delta
    //    //transform.Rotate(new Vector3(0.0f, Time.deltaTime * 100.0f, 0.0f));

    //    //transform.rotation = Quaternion.Euler(new Vector3(0.0f, _yAngle, 0.0f));

    //    // World -> Local
    //    // transform.TransformDirection();

    //    // Local -> World
    //    // transform.InverseTransformDirection();

    //    //transform
    //    if (Input.GetKey(KeyCode.W))
    //    {
    //        // LookRotation
    //        // 특정 방향을 바라보게 만든다.

    //        // Slerp
    //        // 부드럽게 방향을 처리한다.

    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
    //        transform.position += Vector3.forward * Time.deltaTime * _speed;
    //    }

    //    if (Input.GetKey(KeyCode.S))
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.2f);
    //        transform.position += Vector3.back * Time.deltaTime * _speed;
    //    }

    //    if (Input.GetKey(KeyCode.A))
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
    //        transform.position += Vector3.left * Time.deltaTime * _speed;
    //    }

    //    if (Input.GetKey(KeyCode.D))
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
    //        transform.position += Vector3.right * Time.deltaTime * _speed;
    //    }

    //    _moveToDest = false;
    //}



    bool _stopSkill = false;
    void OnMouseEvent(Define.MouseEvent evt)
    {
       switch(State)
        {
            case PlayerState.Idle:
                OnmouseEvent_IdleRun(evt);
                break;
            case PlayerState.Moving:
                OnmouseEvent_IdleRun(evt);
                break;
            case PlayerState.Skill:
                {
                    if (evt == Define.MouseEvent.PointerUp)
                        _stopSkill = true;
                }
                break;
        }
    }

    void OnmouseEvent_IdleRun(Define.MouseEvent evt)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);

        // Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                {
                    if (raycastHit)
                    {
                        _destPos = hit.point;
                        State = PlayerState.Moving;
                        _stopSkill = false;

                        if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                        {
                            _lockTarget = hit.collider.gameObject;
                        }
                        else
                        {
                            _lockTarget = null;
                        }
                    }
                }
                break;
            case Define.MouseEvent.Press:
                {
                    if (_lockTarget == null && raycastHit)
                        _destPos = hit.point;     
                }
                break;
            case Define.MouseEvent.PointerUp:
                _stopSkill = true;
                break;
        }
    }
}
