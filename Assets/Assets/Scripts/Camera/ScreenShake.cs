using DG.Tweening;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float shakeTime = 0.25f;

    public void Shake()
    {
        transform.DOShakePosition(shakeTime);
    }
}
