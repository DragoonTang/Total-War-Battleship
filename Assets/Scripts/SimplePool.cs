using System.Collections.Generic;
using UnityEngine;

public static class SimplePool
{
    private static Dictionary<GameObject, Queue<GameObject>> poolDict = new Dictionary<GameObject, Queue<GameObject>>();
    private static Dictionary<GameObject, GameObject> instanceToPrefab = new Dictionary<GameObject, GameObject>();

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolDict.ContainsKey(prefab))
        {
            poolDict[prefab] = new Queue<GameObject>();
        }

        GameObject obj;

        if (poolDict[prefab].Count > 0)
        {
            obj = poolDict[prefab].Dequeue();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
        }
        else
        {
            obj = Object.Instantiate(prefab, position, rotation);
            instanceToPrefab[obj] = prefab; // ✨ 自动记录来源
        }

        return obj;
    }

    public static void Despawn(GameObject obj)
    {
        if (!instanceToPrefab.TryGetValue(obj, out GameObject prefab))
        {
            Debug.LogWarning($"[SimplePool] Attempted to despawn untracked object: {obj.name}");
            Object.Destroy(obj); // 安全兜底
            return;
        }

        obj.SetActive(false);
        poolDict[prefab].Enqueue(obj);
    }

    // 可选：预热接口依然可用
    public static void Preload(GameObject prefab, int count)
    {
        if (!poolDict.ContainsKey(prefab))
        {
            poolDict[prefab] = new Queue<GameObject>();
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            instanceToPrefab[obj] = prefab;
            poolDict[prefab].Enqueue(obj);
        }
    }
}
