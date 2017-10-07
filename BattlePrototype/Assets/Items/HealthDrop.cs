using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : MonoBehaviour
{
    public float healthAmount = 10.0f;

    public void OnTriggerEnter2D(Collider2D collider)
    {
        Damageable damageable = collider.gameObject.GetComponent<Damageable>();

        if (damageable != null && damageable.CurrentHealth != damageable.MaxHealth)
        {
            damageable.Heal(healthAmount);
            Destroy(gameObject);
        }

    }
}
