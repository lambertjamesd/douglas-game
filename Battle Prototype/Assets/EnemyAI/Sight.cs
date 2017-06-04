using UnityEngine;
using System.Collections;

public class Sight : MonoBehaviour
{
	public LayerMask canSee;
    public bool damageableOnly = false;

	public virtual Collider2D GetVisibleObject()
    {
		return null;
	}

	public bool canSeeObject(GameObject gameObject)
    {
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
