using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationVariable
{
    public string name;
    public Vector2 direction;
    public float minValue;
    public float maxValue;
    public float triggerValue;
    public bool wrap;
    public bool floor;
    public float[] mapping;
    public float currentValue;
    public string dependsOn;
    public float minDependValue;
    public float maxDependValue;
}

[System.Serializable]
public class AnimatedVaraibles
{
    public string namedScalar;
    public float fixedScalar = 1.0f;
    public string targetVariable;
}

public class SpritesheetAnimator : MonoBehaviour {
    public List<AnimationVariable> variables = new List<AnimationVariable>();
    public List<AnimatedVaraibles> animated = new List<AnimatedVaraibles>();
    private Dictionary<string, int> indexMapping = new Dictionary<string, int>();

    public SpriteRenderer spriteRenderer;
    public Texture2D image;
    public Vector2 spriteSize;
    public Vector2 baseOffset;
    public bool useUnscaledTime = false;

    public float GetValue(string name)
    {
        if (indexMapping.ContainsKey(name))
        {
            return variables[indexMapping[name]].currentValue;
        }
        else
        {
            return 0.0f;
        }
    }

    public void SetValue(string name, float value)
    {
        if (indexMapping.ContainsKey(name))
        {
            AnimationVariable variable = variables[indexMapping[name]];

            if (variable.wrap)
            {
                variable.currentValue = (value - variable.minValue) % (variable.maxValue - variable.minValue) + variable.minValue;
            }
            else
            {
                variable.currentValue = Mathf.Clamp(value, variable.minValue, variable.maxValue);
            }
        }
    }

    public float GetTriggerValue(string name)
    {
        if (indexMapping.ContainsKey(name))
        {
            return variables[indexMapping[name]].triggerValue;
        }
        else
        {
            return 0.0f;
        }
    }
    
	public void Start ()
    {
        for (int i = 0; i < variables.Count; ++i)
        {
            indexMapping[variables[i].name] = i;
        }
	}

    private bool IsVariableActive(AnimationVariable variable)
    {
        if (string.IsNullOrEmpty(variable.dependsOn))
        {
            return true;
        }
        else
        {
            if (indexMapping.ContainsKey(variable.dependsOn))
            {
                AnimationVariable dependsOn = variables[indexMapping[variable.dependsOn]];
                return dependsOn.currentValue >= variable.minDependValue && dependsOn.currentValue <= variable.maxDependValue && IsVariableActive(dependsOn);
            }
            else
            {
                return false;
            }
        }
    }
	
	public void LateUpdate () {
        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        foreach (AnimatedVaraibles variable in animated)
        {
            float timeScalar = variable.fixedScalar * deltaTime;

            if (!string.IsNullOrEmpty(variable.namedScalar))
            {
                timeScalar *= GetValue(variable.namedScalar);
            }
            
            SetValue(variable.targetVariable, GetValue(variable.targetVariable) + timeScalar);
        }

        Vector2 offset = baseOffset;

        for (int i = 0; i < variables.Count; ++i)
        {
            AnimationVariable variable = variables[i];
            if (IsVariableActive(variable))
            {
                float useValue = (variable.floor ? Mathf.Floor(variable.currentValue) : variable.currentValue);

                if (variable.mapping != null)
                {
                    int valueAsInt = Mathf.FloorToInt(useValue);

                    if (valueAsInt >= 0 && valueAsInt < variable.mapping.Length)
                    {
                        useValue += variable.mapping[valueAsInt] - valueAsInt;
                    }
                }

                offset += variable.direction * useValue;
            }
        }

        spriteRenderer.sprite = Sprite.Create(image, new Rect(offset, spriteSize), Vector2.one * 0.5f, 32.0f, 0, SpriteMeshType.FullRect);
    }
}
