using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnCtrl : MonoBehaviour
{
    static private List<GameObject> MonstersOnFeild_List = new List<GameObject>();


    static private MonsterSpawnCtrl instance;
    [Header("Reference")]
    [SerializeField]
    private GameObject monster_prefab;
    [SerializeField]
    private GameObject monsters_parent_obj;
    [SerializeField]
    private float spawnDistance_Max;
    [SerializeField]
    private float spawnDelay_perSec;
    [SerializeField]
    private int MaxMonsterNum;

    private void Awake()
    {
        Singltonization();
        InitiateReference();
    }
    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }
    private void InitiateReference()
    {

    }
    private void Singltonization()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public static MonsterSpawnCtrl Instance()
    {
        return instance;
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (MonstersOnFeild_List.Count < MaxMonsterNum)
            {
                SpawnMonster();
                yield return new WaitForSecondsRealtime(spawnDelay_perSec);
            }
            yield return null;
        }
    }
    private void SpawnMonster()
    {
        GameObject monster = Instantiate(monster_prefab, RandomSpawnPos(), Quaternion.identity, monsters_parent_obj.transform);
        MonstersOnFeild_List.Add(monster);
    }
    public void DeleteMonsterOnList(GameObject target)
    {
        MonstersOnFeild_List.Remove(target);
    }
    private Vector3 RandomSpawnPos()
    {
        float rand_x = Random.Range(-spawnDistance_Max, spawnDistance_Max);
        float rand_z = Random.Range(-spawnDistance_Max, spawnDistance_Max);

        return PlayerCtrl.Instance().transform.position + new Vector3(rand_x, 0, rand_z);
    }

    public List<GameObject> MonsterList()
    {
        return MonstersOnFeild_List;
    }
}
