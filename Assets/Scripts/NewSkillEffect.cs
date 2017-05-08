using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSkillEffect : MonoBehaviour
{
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

    // Use this for initialization
    void Start()
    {
        coroutines = new List<IEnumerator>();
        eventTable = new Dictionary<int, HashSet<int>>();

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
    }

    IEnumerator SpawnAfterSeconds(EffectSpawner spawner)
    {
        yield return new WaitForSeconds(spawner.triggeredTime);
        Spawn(spawner);
    }

    void Spawn(EffectSpawner spawner)
    {
        var go = Instantiate(spawner.prefab);

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
            go.transform.position = spawnTrans.position + spawner.positionOffset;
            go.transform.rotation = Quaternion.Euler(spawner.rotationOffset) *
                spawnTrans.rotation;
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
