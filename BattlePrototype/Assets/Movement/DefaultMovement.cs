using UnityEngine;
using System.Collections;

public enum MovementMode {
	Direct,
	Sliding,
}

public class DefaultMovement : MonoBehaviour {
	private Rigidbody2D forBody;
	private Vector2 targetVelocity;
    private Vector2 lastPosition;

	public Damageable damageKnockback;
	public bool canKnockback = true;
	public float maxAcceleration = 2.0f;
	public MovementMode movementMode = MovementMode.Sliding;
	public float slideTimer = 0.0f;
	public Animator moveAnimator;
	public Transform direction = null;
	private int lockRotation = 0;
	public bool animationSpeed = false;

	public LayerMask iceLayers;
	public float iceAcceleration = 2.0f;
    public Vector2 startDirection = Vector2.down;

	public void LockRotation() {
		++lockRotation;
	}

	public void UnlockRotation() {
		--lockRotation;
	}

	public void Start() {
		forBody = GetComponent<Rigidbody2D>();

        lastPosition = transform.position;

        if (damageKnockback != null) {
			damageKnockback.FilterDamage((source, damageable) => {
				if (source.KnockbackDistance > 0.0f) {
					Knockback(source.Direction * source.KnockbackDistance);
				}
				return source;
			}, false);
		}

        SetDirection(startDirection);
	}

	public void FixedUpdate() {
		float acceleration = maxAcceleration;
		bool isSliding = movementMode == MovementMode.Sliding || slideTimer > 0.0f;

		if (iceLayers != 0) {
			if (Physics2D.OverlapPoint(transform.position, iceLayers)) {
				isSliding = true;
				acceleration = iceAcceleration;
			}
		}

        Vector2 currentPosition = transform.position;

        if (moveAnimator != null)
        {
            float speed = Time.fixedDeltaTime == 0.0f ? 0.0f : (currentPosition - lastPosition).magnitude / Time.fixedDeltaTime;

            moveAnimator.SetBool("Moving", speed != 0.0f);

            if (animationSpeed)
            {
                moveAnimator.SetFloat("Speed", speed);
            }
        }

        lastPosition = currentPosition;

        if (isSliding) {
			Vector2 offset = TargetVelocity - Velocity;
			float accel = acceleration * Time.fixedDeltaTime;

			if (offset.sqrMagnitude <= accel * accel) {
				Velocity = TargetVelocity;
			} else {
				Velocity = Velocity + offset.normalized * accel;
			}

			slideTimer -= Time.fixedDeltaTime;
		} else {
			float currentSpeed = Velocity.magnitude;
			float targetSpeed = targetVelocity.magnitude;

			float actualSpeed = Mathf.Min(targetSpeed, currentSpeed + acceleration * Time.fixedDeltaTime);

			if (actualSpeed == 0.0f) {
				Velocity = Vector2.zero;
			} else {
				Velocity = targetVelocity * actualSpeed / targetSpeed;
			}
		}
	}

	public Vector2 TargetVelocity {
		get {
			return targetVelocity;
		}

		set {
			targetVelocity = value;

			if (lockRotation == 0) {
                SetDirection(value);
			}
		}
	}

	public Vector2 Velocity {
		get {
			return forBody.velocity;
		}

		set {
			forBody.velocity = value;
		}
	}

    public void SetDirection(Vector2 target)
    {
        Vector2 dir;
        Vector3 currentDir = direction != null ? direction.TransformDirection(Vector3.right) : Vector3.right;
        int facing = -1;

        if (Mathf.Abs(target.x) >= Mathf.Abs(target.y))
        {
            if (target.x == 0.0f || Mathf.Abs(target.x) == Mathf.Abs(target.y) && Vector3.Dot(currentDir, new Vector3(target.x, target.y, 0.0f)) > 0.0f)
            {
                dir = Vector2.zero;
            }
            else
            {
                dir = new Vector2(Mathf.Sign(target.x), 0.0f);
                facing = target.x > 0 ? 3 : 1;
            }
        }
        else
        {
            dir = new Vector2(0.0f, Mathf.Sign(target.y));
            facing = target.y > 0 ? 2 : 0;
        }

        if (moveAnimator != null && facing != -1)
        {
            moveAnimator.SetInteger("Direction", facing);
        }

        if (direction != null && dir != Vector2.zero)
        {
            direction.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(Vector3.forward, dir));
            Vector3 localPos = direction.localPosition;
            localPos.z = dir.x + dir.y;
            direction.localPosition = localPos;
        }
    }

	public bool Knockback(Vector2 offset) {
		if (canKnockback) {
			Velocity = Velocity - offset * Vector2.Dot(Velocity, offset) / offset.sqrMagnitude;
			Vector2 velocityOffset = new Vector2(
				Mathf.Sqrt(2.0f * Mathf.Abs(offset.x) * maxAcceleration) * Mathf.Sign(offset.x),
				Mathf.Sqrt(2.0f * Mathf.Abs(offset.y) * maxAcceleration) * Mathf.Sign(offset.y)
			);
			Velocity = Velocity + velocityOffset;
			slideTimer = Mathf.Max(slideTimer, velocityOffset.magnitude / maxAcceleration);
			return true;
		} else {
			return false;
		}
	}
}
