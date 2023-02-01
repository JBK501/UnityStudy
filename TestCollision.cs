using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision  @ {collision.gameObject.name} !");
    }

    
    private void OnTriggerEnter(Collider other)
    {
       Debug.Log($"Trigger @ {other.gameObject.name} !");
    }

    void Start()
    {
        
    }

    void Update()
    {
        // Local <-> World <-> Viewport(화면 비율에 대해서 몇 %를 차지하는지 표시한다) <-> Screen(한 픽셀 좌표를 기준으로 표현)

        //Debug.Log(Input.mousePosition);     // Screen좌표 표현
        //Debug.Log(Camera.main.ScreenToViewportPoint(Input.mousePosition)); // Viewport좌표 표현

        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);


            // 레이어를 설정한 다음 원하는 애들만 레이캐스팅을 한다.
            LayerMask mask = LayerMask.GetMask("Monster") | LayerMask.GetMask("Wall");
            //int mask = (1 << 8) | (1 << 9); 


            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100.0f,mask))
            {
                Debug.Log($"Raycast Camera @ {hit.collider.gameObject.tag}");
            }
        }

        //if (Input.GetMouseButtonDown(0))
        //{

        //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        //    Vector3 dir = mousePos - Camera.main.transform.position;
        //    dir = dir.normalized;

        //    Debug.DrawRay(Camera.main.transform.position, dir * 100.0f, Color.red, 1.0f);

        //    RaycastHit hit;
        //    if (Physics.Raycast(Camera.main.transform.position, dir, out hit, 100.0f))
        //    {
        //        Debug.Log($"Raycast Camera @ {hit.collider.gameObject.name}");
        //    }
        //}
    }
}
