using UnityEngine;
using System.Collections;

public interface IState {
	void StateBegin();
    IState UpdateState(float deltaTime);
}

public class State : MonoBehaviour, IState {
	public virtual void StateBegin() {

	}

	public virtual IState UpdateState(float deltaTime) {
		return null;
	}
}
