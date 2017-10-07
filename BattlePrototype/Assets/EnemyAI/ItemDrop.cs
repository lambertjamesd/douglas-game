using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class DropSlot {
	public float probability = 1.0f;
    public string dropGroup = "default";
    public GameObject drop;
	public Vector3 startOffset;
	public Vector3 endOffset;
	public float dropTime = 0.0f;
}

public class ItemDrop : MonoBehaviour {
	public DropSlot[] drops;

    public void Start()
    {
        Damageable damageable = GetComponent<Damageable>();

        if (damageable != null)
        {
            damageable.OnDeath((info) =>
            {
                DoDrops();
            });
        }
    }

	void SpawnDrop(DropSlot slot) {
        if (slot.drop != null)
        {
		    GameObject drop = (GameObject)Instantiate(slot.drop, transform.position + slot.startOffset, Quaternion.identity);

		    if (slot.dropTime > 0.0f) {
			    DropAnimation anim = drop.GetComponent<DropAnimation>();
			    anim.Throw(transform.position + slot.endOffset, slot.dropTime);
		    }
        }
	}

    public void DoDrops()
    {
        Dictionary<string, float> weightSum = new Dictionary<string, float>();

        for (int i = 0; drops != null && i < drops.Length; ++i)
        {
            DropSlot drop = drops[i];
            if (weightSum.ContainsKey(drop.dropGroup))
            {
                weightSum[drop.dropGroup] = weightSum[drop.dropGroup] + drop.probability;
            }
            else
            {
                weightSum[drop.dropGroup] = drop.probability;
            }

        }

        foreach (var singleSum in weightSum)
        {
            float weight = singleSum.Value * Random.value;

            for (int i = 0; i < drops.Length; ++i)
            {
                DropSlot drop = drops[i];

                if (drop.dropGroup == singleSum.Key)
                {
                    if (drop.probability >= weight)
                    {
                        SpawnDrop(drop);
                        break;
                    }
                    else
                    {
                        weight -= drop.probability;
                    }
                }
            }
        }
    }
}
