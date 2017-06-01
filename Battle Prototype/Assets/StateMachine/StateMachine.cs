using UnityEngine;
using System.Collections;

public class StateMachine : MonoBehaviour {
	public State startingState = null;
	private IState currentState = null;

	void Start() {
		SetCurrentState(startingState);
	}

	public void SetCurrentState(IState next) {
		if (next != null && next != currentState) {
			next.StateBegin();
            IState prevState = currentState;
			currentState = next;

            if (prevState != null)
            {
                prevState.StateEnd();
            }
		}
	}

	void Update () {
		if (currentState != null) {
			SetCurrentState(currentState.UpdateState(Time.deltaTime) ?? currentState);
		}
	}
}
