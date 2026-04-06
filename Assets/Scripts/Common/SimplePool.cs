using System.Collections.Generic;
using UnityEngine;

public class SimplePool : MonoSingleton<SimplePool>
{
    private Dictionary<GameObject, Queue<GameObject>> poolDict = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<GameObject, GameObject> instanceToPrefab = new Dictionary<GameObject, GameObject>();

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        if (!poolDict.ContainsKey(prefab))
        {
            poolDict[prefab] = new Queue<GameObject>();
        }

        GameObject obj;
        if (poolDict[prefab].Count > 0)
        {
            obj = poolDict[prefab].Dequeue();
            // 额外检查：防止池子里的物体因为某种意外被销毁了
            if (obj == null) return Spawn(prefab, position, rotation);

            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab, position, rotation);
            instanceToPrefab[obj] = prefab;
        }
        return obj;
    }

    public void Despawn(GameObject obj)
    {
        if (obj == null) return;
        if (!instanceToPrefab.TryGetValue(obj, out GameObject prefab))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        poolDict[prefab].Enqueue(obj);
    }
}