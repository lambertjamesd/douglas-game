using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardNumberSpinner : MonoBehaviour {
    public KeyboardNumberSpinnerDigit digit;
    public RectTransform rectTransform;

    private List<KeyboardNumberSpinnerDigit> digits = new List<KeyboardNumberSpinnerDigit>();
    private int currentDigit = 0;
    private int currentDigitScalar = 1;
    private AxisToButton horz = new AxisToButton();
    private AxisToButton vert = new AxisToButton();
    private int currentValue = 0;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetDigitCount(int count)
    {
        digits.ForEach((digit) => Destroy(digit.gameObject));
        digits.Clear();

        for (var i = 0; i < count; ++i)
        {
            KeyboardNumberSpinnerDigit digitInstance = Instantiate(digit);
            digitInstance.rectTransform.SetParent(rectTransform, true);
            Rect rect = rectTransform.rect;
            digitInstance.rectTransform.localPosition = new Vector2(rect.size.x, 0.0f) * (count - 1 - i);
            digitInstance.SetHighlighted(i == 0);
            digits.Add(digitInstance);
        }
    }

    public void SetValue(int value)
    {
        this.currentValue = value;
        this.UpdateLabel();
    }

    public int GetValue()
    {
        return this.currentValue;
    }

    private void UpdateLabel()
    {
        int loopValue = currentValue;
        for (int i = 0; i < digits.Count; ++i)
        {
            digits[i].label.text = (loopValue % 10).ToString();
            loopValue /= 10;
        }
    }

    public void Update()
    {
        horz.Update(Input.GetAxisRaw("Horizontal"));
        vert.Update(Input.GetAxisRaw("Vertical"));

        if (horz.GetPositiveDown() && currentDigit > 0)
        {
            digits[currentDigit].SetHighlighted(false);
            --currentDigit;
            currentDigitScalar /= 10;
            digits[currentDigit].SetHighlighted(true);
        }

        if (horz.GetNegativeDown() && currentDigit < digits.Count - 1)
        {
            digits[currentDigit].SetHighlighted(false);
            ++currentDigit;
            currentDigitScalar *= 10;
            digits[currentDigit].SetHighlighted(true);
        }

        int mask = currentDigitScalar * 10;

        if (vert.GetPositiveDown())
        {
            currentValue += currentDigitScalar;
            if ((currentValue % mask) / currentDigitScalar == 0)
            {
                currentValue -= mask;
            }
            UpdateLabel();
        }

        if (vert.GetNegativeDown())
        {
            if ((currentValue % mask) / currentDigitScalar == 0)
            {
                currentValue += mask;
            }
            currentValue -= currentDigitScalar;
            UpdateLabel();
        }
    }
}
