using UnityEngine;
using System.Collections;

public class BreakerBow : State {
	public float moveSpeed = 1.0f;
	public State nextState;
	public Bow bow;
	public DefaultMovement movement;
	public Transform direction;
	public float minSpeed = 2.0f;
	public float maxSpeed = 4.0f;
	public float minDamage = 1.0f;
	public float maxDamage = 2.0f;
	public float chargeTime = 1.0f;
	public float chargeFrequency = 6.0f;
	public Projectile breakerBolt = null;

	private float currentTime = 0.0f;
	private Flasher currentFlasher = null;
	
	public override IState UpdateState(float deltaTime) {
		float horz = Input.GetAxisRaw("Horizontal");
		float vert = Input.GetAxisRaw("Vertical");

		currentTime += deltaTime;
		
		movement.TargetVelocity = new Vector2(horz, vert) * moveSpeed;

		if (currentFlasher == null) {
			Projectile projectile = bow.Draw();
			currentFlasher = projectile.GetComponent<Flasher>();
		}

		float lerp = Mathf.Min(1.0f, currentTime / chargeTime);
		float indicationLerp = (lerp == 1.0f ? lerp : lerp) * 0.5f;

		currentFlasher.depth = indicationLerp;
		currentFlasher.frequency = indicationLerp * chargeFrequency;

		if (currentTime >= chargeTime && currentTime - deltaTime < chargeTime) {
			bow.LoadProjectile(breakerBolt);
		}

		if (!Input.GetButton("Fire1")){
			bow.Fire(Mathf.Lerp(minSpeed, maxSpeed, lerp));
			currentFlasher = null;
			currentTime = 0.0f;
			return nextState;
		} else {
			return null;
		}
	}
}
