using UnityEngine;

public class AudioModifier : MonoBehaviour
{
    public GameValue gv;
    public AudioLowPassFilter lowPass;
    public AnimationCurve map;

    public float min = 500;
    public float max = 22000;

    private void Update()
    {
        if (gv == null)
            return;

        lowPass.cutoffFrequency = Mathf.Lerp(min, max, map.Evaluate(gv.ValuePercent));
    }
}
