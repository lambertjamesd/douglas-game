using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerMovement : State {
	public float speed = 2.0f;
	public SwordSwing sword;
    public ReloadGun reload;
    public UsingSights useSights;
	public Transform direction;
	public float bowSpeed = 2.0f;
    public float interactDistance = 0.5f;
    public float interactRadius = 0.5f;

	public AnimationController animator;

	public DamageInfo swordDamage;

	public InventorySlot inventory;

	private bool swingingSword = false;

	private DefaultMovement movement;

	void Start () {
		movement = GetComponent<DefaultMovement>();
	}

    public IState CommonMovement(float deltaTime, float speed)
    {
        if (deltaTime == 0.0f)
        {
            return null;
        }

        float horz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        movement.TargetVelocity = new Vector2(horz, vert).normalized * speed;

        State maybeResult = inventory.checkUseItems();

        if (maybeResult != null)
        {
            return maybeResult;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            return new DoCoroutine(SwingSword(), this);
        }

        if (inventory.HasAGun() && Input.GetButtonDown("Reload"))
        {
            return reload;
        }

        if (Input.GetButtonDown("CycleWeapon") && inventory.HasAGun())
        {
            inventory.NextGun();
        }

        if (!inventory.HasAGun())
        {
            animator.SetInteger("GunType", 0);
        }
        else
        {
            GunStats stats = inventory.GetCurrentGun();
            animator.SetInteger("GunType", stats.animationIndex);
        }

        if (Input.GetButtonDown("Submit"))
        {
            var overlaps = Physics2D.OverlapCircleAll(direction.TransformPoint(Vector3.right * interactDistance), interactRadius);

            ScriptInteraction interaction = overlaps.Select(collider => collider.gameObject.GetComponent<ScriptInteraction>())
                .Where(value => value != null)
                .FirstOrDefault(x => true);

            if (interaction != null)
            {
                return new Interact(this, interaction.interact());
            }
        }

        return null;
    }
	
	public override IState UpdateState(float deltaTime)
    {
        if (Input.GetButton("UseSights"))
        {
            return useSights;
        }

        return CommonMovement(deltaTime, speed);
    }

	IEnumerator SwingSword() {
		if (!swingingSword) {
            movement.TargetVelocity = Vector2.zero;
			movement.LockRotation();
			DamageSource source = new DamageSource(swordDamage, direction.TransformDirection(Vector3.right), transform.position);
			swingingSword = true;
			IEnumerator swordSwing = sword.Swing(hit => {
                Projectile projectile = hit.gameObject.GetComponent<Projectile>();

                if (projectile != null)
                {
                    Vector2 normal = projectile.transform.position - transform.position;

                    projectile.Redirect(Vector2.Reflect(projectile.Velocity, (normal - projectile.Velocity.normalized * 3.0f).normalized), LayerMask.NameToLayer("L1: Player Weapon"));
                }
                else
                {
                    Damageable target = hit.gameObject.GetComponent<Damageable>();
				
				    if (target != null) {
					    target.Damage(source);
				    }
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
