using UnityEngine;
using System.Collections;

public class Sight : MonoBehaviour
{
	public LayerMask canSee;
    public LayerMask sightBlocked;
    public bool damageableOnly = false;

	public virtual Collider2D GetVisibleObject()
    {
		return null;
	}

	public bool canSeeObject(GameObject gameObject)
    {
        Vector2 origin = this.transform.position;
        Vector2 target = gameObject.transform.position;

        RaycastHit2D hit = new RaycastHit2D();

        if (sightBlocked != 0 && (hit = Physics2D.Raycast(origin, target - origin, float.MaxValue, sightBlocked)))
        {
            if ((target - origin).sqrMagnitude > hit.distance * hit.distance)
            {
                return false;
            }
        }

        if (damageableOnly && gameObject.GetComponent<Damageable>() == null)
        {
            return false;
        }

		ObjectVisibility visibility = gameObject.GetComponent<ObjectVisibility>();

		if (visibility != null)
        {
			return (canSee & visibility.visibilityLayers) != 0;
		}

		return true;
	}
}
