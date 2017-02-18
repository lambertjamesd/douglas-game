using UnityEngine;
using System.Collections;

public class StateMachine : MonoBehaviour {
	public State startingState = null;
	private IState currentState = null;

	void Start() {
		SetCurrentState(startingState);
	}

	void SetCurrentState(IState next) {
		if (next != null) {
			next.StateBegin();
			currentState = next;
		}
	}

	void Update () {
		if (currentState != null) {
			SetCurrentState(currentState.UpdateState(Time.deltaTime) ?? currentState);
		}
	}
}
