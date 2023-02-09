using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        if(typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }
        
        return Resources.Load<T>(path);
    }

    public GameObject Instaniate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");

        if(original == null)
        {
            Debug.Log($"Faild to load prefab : {path}");
            return null;
        }

        // 혹시 풀링된 애가 있을까?
        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;


        GameObject go = Object.Instantiate(original, parent);
        //int index = go.name.IndexOf("(Clone)");
        //if (index > 0)
        //    go.name = go.name.Substring(0, index);
        go.name = original.name;

        return go;
    }

    public void Desroy(GameObject go)
    {
        if (go == null)
            return;

        // 풀링이 필요하다면 풀링매니저에 위탁한다.
        Poolable poolable = go.GetComponent<Poolable>();
        if(poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }
}
