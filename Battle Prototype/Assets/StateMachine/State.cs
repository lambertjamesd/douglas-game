using UnityEngine;
using System.Collections;

public interface IState {
	void StateBegin();
	State UpdateState(float deltaTime);
}

public class State : MonoBehaviour, IState {
	public virtual void StateBegin() {

	}

	public virtual State UpdateState(float deltaTime) {
		return null;
	}
}
