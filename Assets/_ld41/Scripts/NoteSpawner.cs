using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : EntitySpawner
{
    public GameValue[] lanes;

    public void Spawn(int count)
    {
        if (count == 0)
            return;

        List<int> bucket = new List<int>();
        for (int i = 0; i < lanes.Length; i++)
            bucket.Add(i);

        for (int j = 0; j < count; j++)
        {
            if (bucket.Count == 0)
                break;

            int pick = Random.Range(0, bucket.Count);
            int i = bucket[pick];
            bucket.RemoveAt(pick);

            SpawnAtLane(i);
        }
    }

    public override void Spawn()
    {
        if (lanes.Length == 0)
        {
            base.Spawn();
            return;
        }

        int i = Random.Range(0, lanes.Length);
        SpawnAtLane(i);
    }

    private void SpawnAtLane(int i)
    {
        GameValue lane = lanes[i];
        Vector3 offset = lane.transform.parent.parent.parent.position - Vector3.right * 2;
        Transform entity = _Spawn(velocity, offset);

        LanePileEffector obj = entity.GetComponent<LanePileEffector>();
        obj.target = lane;
    }
}
