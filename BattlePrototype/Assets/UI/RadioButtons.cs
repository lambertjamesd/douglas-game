using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RadioOption
{
    public Button button;
}

public class RadioButtons : MonoBehaviour {

    public RadioOption[] buttons;
    public Vector3 selectedOffset;

    public delegate void OnChange(int selection);

    private int selected = -1;

    private List<OnChange> changeCallbacks = new List<OnChange>();

    public int Selected
    {
        get
        {
            return selected;
        }
    }

    public void Change(OnChange change)
    {
        changeCallbacks.Add(change);
    }

    public void SetSelected(int index)
    {
        if (index != selected)
        {
            if (selected >= 0 && selected < buttons.Length)
            {
                buttons[selected].button.transform.localPosition -= selectedOffset;
            }

            selected = index;

            if (selected >= 0 && selected < buttons.Length)
            {
                buttons[selected].button.transform.localPosition += selectedOffset;
            }

            changeCallbacks.ForEach((callback) => callback(selected));
        }
    }

    void Listen(RadioOption option, int index)
    {
        option.button.onClick.AddListener(() => SetSelected(index));
    }
    
	void Start () {
        for (int i = 0; i < buttons.Length; ++i)
        {
            Listen(buttons[i], i);
        }

        SetSelected(0);
	}
	
}
