using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSkillEffect : MonoBehaviour
{
    public float timeAlive;

    [System.Serializable]
    public class EffectSpawner
    {
        public GameObject prefab;
        public Transform target;
        public Vector3 positionOffset;
        public Vector3 rotationOffset;
        public bool asChild;
        public bool triggeredByEvent;
        public int eventId;
        public float triggeredTime;
    }

    public EffectSpawner[] spawners;

    public Transform Target { get; set; }

    private List<IEnumerator> coroutines;
    private Dictionary<int, HashSet<int>> eventTable;

    private List<GameObject> spawnedObjs;

    // Use this for initialization
    void Start()
    {
        coroutines = new List<IEnumerator>();
        eventTable = new Dictionary<int, HashSet<int>>();

        spawnedObjs = new List<GameObject>();

        for (int i = 0; i < spawners.Length; i++)
        {
            if (spawners[i].triggeredByEvent)
            {
                if (!eventTable.ContainsKey(spawners[i].eventId))
                {
                    eventTable.Add(spawners[i].eventId, new HashSet<int>());
                }
                eventTable[spawners[i].eventId].Add(i);
            }
            else
            {
                var cr = SpawnAfterSeconds(spawners[i]);
                StartCoroutine(cr);
                coroutines.Add(cr);
            }
        }

        {
            var cr = DestroyAfterSeconds(timeAlive);
            StartCoroutine(cr);
            coroutines.Add(cr);
        }
    }

    IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    IEnumerator SpawnAfterSeconds(EffectSpawner spawner)
    {
        yield return new WaitForSeconds(spawner.triggeredTime);
        Spawn(spawner);
    }

    void Spawn(EffectSpawner spawner)
    {
        var go = Instantiate(spawner.prefab);
        spawnedObjs.Add(go);

        Transform spawnTrans = spawner.target;

        if (null == spawnTrans)
        {
            spawnTrans = Target;
        }

        if (spawner.asChild)
        {
            go.transform.SetParent(spawnTrans);
            go.transform.localPosition = spawner.positionOffset;
            go.transform.localRotation = Quaternion.Euler(spawner.rotationOffset);
        }
        else
        {
            go.transform.position = spawnTrans.position;
            go.transform.rotation = spawnTrans.rotation * Quaternion.Euler(spawner.rotationOffset);
            go.transform.position += spawnTrans.rotation * Vector3.Scale(spawner.positionOffset, transform.lossyScale);
        }
        SkillEffect skillfx = go.GetComponent<SkillEffect>();
        if (null != skillfx)
        {
            skillfx.SetTarget(Target);
        }
    }

    void OnDisable()
    {
        Target = null;

        foreach (var c in coroutines)
        {
            StopCoroutine(c);
        }
        coroutines.Clear();


        foreach (var go in spawnedObjs)
        {
            Destroy(go);
        }
        spawnedObjs.Clear();
    }

    public void OnEventTriggered(int eventId)
    {
        if (eventTable.ContainsKey(eventId))
        {
            foreach (int id in eventTable[eventId])
            {
                Spawn(spawners[id]);
            }
        }
    }
}
