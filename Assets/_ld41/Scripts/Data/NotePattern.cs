using UnityEngine;

[CreateAssetMenu(fileName = "Note Pattern", menuName = "LD41/Note Pattern")]
public class NotePattern : ScriptableObject
{
    public int loops;
    public float length;
    public float[] pattern;
}
