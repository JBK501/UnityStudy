using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    Stat _stat;

    [SerializeField]
    float _scanRange = 10;

    [SerializeField]
    float _attackRange = 2;

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Monster;
        _stat = gameObject.GetComponent<Stat>();

        // 몬스터 산하에 UI_HPBar컴포넌트가 없다면 
        if(gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);  // 추가한다.
    }

    protected override void UpdateIdle()
    {
        // TODO : 매니저가 생기면 옮길 것

        // 플레이어를 찾는다.
        //GameObject player = GameObject.FindGameObjectWithTag("Player");

        GameObject player = Managers.Game.GetPlayer();
        if (player == null) // 플레이어가 없으면
            return; // 계속 기다린다.
        

        // 플레이어와의 거리를 구한다.
        float distance = (player.transform.position - transform.position).magnitude;

        // 플레이어가 사정거리 안으로 들어오면
        if(distance <= _scanRange)
        {
            _lockTarget = player;   // 락타겟을 지정한다.
            State = Define.State.Moving;    // 추적상태로 변경한다.

            return;
        }
    }

    protected override void UpdateMoving()
    {
        // 플레이어가 사정거리내로 들어오면 공격한다.
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude; // 플레이어와 몬스터 사이의 거리

            if (distance <= _attackRange)   // 사정거리 내에 있으면
            {
                State = Define.State.Skill; // 플레이어 상태를 스킬로 변경한다.

                // 목적지에 도달했으니까 더이상 이동하지 않도록 설정한다.
                NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
                nma.SetDestination(transform.position);
                return;
            }
        }

        // 이동한다.
        Vector3 dir = _destPos - transform.position;  
        if (dir.magnitude < 0.1f)   // 목적지에 도착했으면
        {
            State = Define.State.Idle;  // 정지 상태로 바꾼다.
        }
        else
        {
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
            nma.SetDestination(_destPos);
            nma.speed = _stat.MoveSpeed;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }
    }

    protected override void UpdateSkill()
    {
        // 목표물(플레이어)이 있으면
        if (_lockTarget != null)
        {
            // 방향을 전환한다.
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    void OnHitEvent()
    {
        if(_lockTarget != null)
        {
            // 체력
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            targetStat.OnAttacked(_stat);

            if(targetStat.Hp > 0)   // 살아있으면
            {
                // 목표물(플레이어)과의 거리를 구한다.
                float distance = (_lockTarget.transform.position - transform.position).magnitude;

                if (distance <= _attackRange)   // 공격범위 내에 있으면 
                    State = Define.State.Skill; // 공격한다.
                else // 공격범위에서 벗어나면 추격한다.
                    State = Define.State.Moving;
            }
            else // 죽었으면
            {
                State = Define.State.Idle;
            }
        }
        else
        {
            State = Define.State.Idle;
        }
    }
}
