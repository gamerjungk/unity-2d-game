using System.Collections.Generic;
using UnityEngine;

public class PedestrianPoolManager : MonoBehaviour
{
    public static PedestrianPoolManager Instance;

    [Header("보행자 프리팹 종류")]
    public List<GameObject> pedestrianPrefabs;

    [Header("초기 풀 사이즈")]
    public int initialCount = 10;

    private List<GameObject> pooledObjects = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        InitPool();
    }

    void InitPool()
    {
        for (int i = 0; i < initialCount; i++)
        {
            CreateNewObject();
        }
    }

    GameObject CreateNewObject()
    {
        if (pedestrianPrefabs.Count == 0) return null;

        GameObject prefab = pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Count)];
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        pooledObjects.Add(obj);
        return obj;
    }

    public GameObject GetFromPool(Vector3 position)
    {
        GameObject obj = null;

        foreach (var item in pooledObjects)
        {
            if (!item.activeInHierarchy)
            {
                obj = item;
                break;
            }
        }

        if (obj == null)
        {
            obj = CreateNewObject();
            if (obj == null) return null;
        }

        obj.transform.position = position;
        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void AddPedestrianPrefab(GameObject prefab)
    {
        if (!pedestrianPrefabs.Contains(prefab))
        {
            pedestrianPrefabs.Add(prefab);
        }
    }

    public void RemovePedestrianPrefab(GameObject prefab)
    {
        pedestrianPrefabs.Remove(prefab);
    }
}
