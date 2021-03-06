﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTweener : ColorModifierModifier
{
    [Header("Tweening")]
    public Gradient gradient;
    public float tweenTime;

    private float startTime = 0;

    public void Play()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        float t = (tweenTime == 0) ? 1 : Mathf.Clamp01((Time.time - startTime) / tweenTime);
        Color value = gradient.Evaluate(t);

        parameters.setter = value;
        Apply();
        target.Apply();
    }
}
