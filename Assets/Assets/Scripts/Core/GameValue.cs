﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameValue : MonoBehaviour
{
    public string label = "HP";
    public float minValue = 0;
    public float maxValue = Mathf.Infinity;
    public float m_currentValue = 0; // Initial value
    public bool clampMin = true;
    public bool clampMax = true;
    public bool roundToInt = true;
    public float Value
    {
        get { return m_currentValue; }
        set
        {
            m_currentValue = Mathf.Clamp(value, clampMin ? minValue : -Mathf.Infinity, clampMax ? maxValue : Mathf.Infinity);
            if (roundToInt == true)
                m_currentValue = Mathf.Round(m_currentValue);
        }
    }
    public float ValuePercent
    {
        get { return (maxValue == 0) ? 0 : Value / maxValue; }
        set
        {
            Value = value * maxValue;
        }
    }

    [Header("Display")]
    public float m_lerpRatio = 0f;
    public float m_lerpSpeed = 1; // Change display in [Value] amount per tick
    public int m_ticksPerSecond = 10;

    [Space]
    public Text m_numericalDisplay;
    public int m_zeroPadding = 0;
    public string m_tickSound; // Sound played on each tick

    [Space]
    public Image m_canvasDisplay;
    public bool m_overrideColor = false;
    public Gradient m_displayColor;
    protected virtual float FillAmount
    {
        get { return m_canvasDisplay.fillAmount; }
        set { m_canvasDisplay.fillAmount = value; }
    }

    private float m_displayValue;

    private void Reset()
    {
        if (m_numericalDisplay == null)
            m_numericalDisplay = GetComponent<Text>();
        if (m_canvasDisplay == null)
            m_canvasDisplay = GetComponent<Image>();
    }

    private void Start()
    {
        m_displayValue = Value; // If we fail to retrieve a display value, set to Value immediately
        if (m_numericalDisplay != null)
        {
            Single.TryParse(m_numericalDisplay.text, out m_displayValue);
        }
        else if (m_canvasDisplay != null)
        {
            m_displayValue = FillAmount * maxValue;
        }

        IEnumerator current = Coroutine_UpdateDisplay();
        StartCoroutine(current);
    }

    private IEnumerator Coroutine_UpdateDisplay()
    {
        while (true)
        {
            m_displayValue = DetermineNextDisplayValue(m_displayValue);

            if (m_numericalDisplay != null)
            {
                float finalDisplayValue = m_displayValue;
                if (roundToInt == true)
                    finalDisplayValue = Mathf.Round(finalDisplayValue);

                string text = finalDisplayValue.ToString().PadLeft(m_zeroPadding, '0');
                if (m_numericalDisplay.text != text)
                {
                    AudioManager.PlaySound(m_tickSound);
                    m_numericalDisplay.text = text;
                }
            }

            if (m_canvasDisplay != null)
            {
                FillAmount = (maxValue == 0) ? 0 : m_displayValue / maxValue;

                if (m_overrideColor == true)
                {
                    m_canvasDisplay.color = m_displayColor.Evaluate(ValuePercent);
                }
            }

            yield return new WaitForSeconds(1f / m_ticksPerSecond);
        }
    }

    private float DetermineNextDisplayValue(float lastValue)
    {
        float nextValue = Mathf.Lerp(lastValue, Value, m_lerpRatio);

        if (Mathf.Abs(Value - nextValue) <= m_lerpSpeed)
            return Value;
        else if (nextValue < Value)
            return nextValue + m_lerpSpeed;
        else
            return nextValue - m_lerpSpeed;
    }
}
