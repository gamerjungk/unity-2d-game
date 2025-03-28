using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefabs;    // 사용할 프리팹 여기에 담으세요
    List<GameObject>[] pools;       

    private void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];       
        for (int i = 0; i < pools.Length; i++) pools[i] = new List<GameObject>();
    }

    public GameObject Get(int index)    // 사용법: prefabs에 담은 프리팹을 복제하고 싶을 때 그 프리팹 인덱스를 인자로 넣으면 됩니다. ex) GameManager.inst.pool.Get(1) -> 1번 인덱스 프리팹 복제
    {
        GameObject select = null;
        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if (!select)
        {
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }

        return select;
    }
}