using UnityEngine;
using System.Collections;

public class PlayerMovement : State {
	public float speed = 2.0f;
	public SwordSwing sword;
	public Bow bow;
	public BreakerBow breakerBow;
	public Transform direction;
	public float bowSpeed = 2.0f;

	public Animator animator;

	public DamageInfo swordDamage;

	public InventorySlot inventory;

	private bool swingingSword = false;

	private DefaultMovement movement;

	void Start () {
		movement = GetComponent<DefaultMovement>();
	}
	
	public override State UpdateState(float deltaTime) {
		float horz = Input.GetAxisRaw("Horizontal");
		float vert = Input.GetAxisRaw("Vertical");

		movement.TargetVelocity = new Vector2(horz, vert).normalized * speed;

		if (Input.GetButtonDown("Fire1")) {
			return inventory.usePrimaryItem();
		}

		if (Input.GetButtonDown("Fire2")){
			StartCoroutine(SwingSword());
		}

		return null;
	}

	IEnumerator SwingSword() {
		if (!swingingSword) {
			movement.LockRotation();
			DamageSource source = new DamageSource(swordDamage, direction.TransformDirection(Vector3.right));
			swingingSword = true;
			IEnumerator swordSwing = sword.Swing(hit => {
				Damageable target = hit.gameObject.GetComponent<Damageable>();
				
				if (target != null) {
					target.Damage(source);
				}
			});

			while (swordSwing.MoveNext()) {
				yield return swordSwing.Current;
			}

			swingingSword = false;
			
			movement.UnlockRotation();
		}
	}
}
