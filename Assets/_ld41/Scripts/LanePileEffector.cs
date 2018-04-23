using UnityEngine;

public class LanePileEffector : MonoBehaviour
{
    public GameValue target;

    public EntitySpawner effectSpawner;

    public void LowerPile(Transform other)
    {
        if (other.tag != "Shot")
            return;

        target.Value += 60 - (target.Value % 60);
        if (effectSpawner != null)
            effectSpawner.Spawn();
    }
}
