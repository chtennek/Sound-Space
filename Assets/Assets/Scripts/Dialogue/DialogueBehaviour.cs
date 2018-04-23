using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBehaviour : MonoBehaviour
{
    // [TODO] actor and audio handling
    //public UnityEvent OnAdvance;
    //public UnityEvent OnTextComplete;

    public Dialogue dialogue;

    [Header("General")]
    public float autoAdvanceAfter = Mathf.Infinity;
    public bool playOnAwake = true;

    [Header("Display Options")]
    public float textSpeed = 10f; // characters per second
    public bool fadeText = false;
    public float fadeTime = 1f;
    public Gradient fadeGradient;

    [Header("Components")]
    public string buttonName;
    [SerializeField]
    private InputReceiver input;

    [SerializeField]
    private Text text;
    [SerializeField]
    private TextMesh textMesh;

    Queue<Line> lines;
    Line currentLine;
    IEnumerator current;

    private string Text
    {
        get
        {
            if (text != null)
                return text.text;

            if (textMesh != null)
                return textMesh.text;

            return "";
        }
        set
        {
            if (text != null)
                text.text = value;

            if (textMesh != null)
                textMesh.text = value;
        }
    }

    private Color TextColor
    {
        get
        {
            if (text != null)
                return text.material.color;

            if (textMesh != null)
                return textMesh.color;

            return Color.clear;
        }
        set
        {
            if (text != null)
                text.color = value;

            if (textMesh != null)
                textMesh.color = value;
        }
    }

    public void Stop()
    {
        lines.Clear();
        if (current != null)
            StopCoroutine(current);
    }

    public void FlushOrNext()
    {
        if (Text != currentLine.text)
            Flush();
        else
            Next();
    }

    public void Flush()
    {
        Text = currentLine.text;
    }

    public void Next()
    {
        Text = "";
        if (current != null)
            StopCoroutine(current);
        if (lines.Count == 0)
            return;

        currentLine = lines.Dequeue();
        current = Coroutine_Display(currentLine);
        StartCoroutine(current);
    }

    public void Load(Dialogue d)
    {
        dialogue = d;
        //Text = "";

        lines = new Queue<Line>();
        foreach (Line line in dialogue.lines)
            lines.Enqueue(line);
    }

    private void Reset()
    {
        if (input == null)
            input = GetComponent<InputReceiver>();

        if (text == null)
            text = GetComponent<Text>();

        if (textMesh == null)
            textMesh = GetComponent<TextMesh>();
    }

    private void Awake()
    {
        Load(dialogue);
        if (playOnAwake == true)
            Next();
    }

    private void Update()
    {
        if (input != null && input.GetButtonDown(buttonName))
            FlushOrNext();
    }

    private IEnumerator Coroutine_Display(Line line)
    {
        if (textSpeed == Mathf.Infinity)
        {
            Flush();
        }
        else
        {
            while (Text.Length < currentLine.text.Length)
            {
                Text = currentLine.text.Substring(0, Text.Length + 1);
                yield return new WaitForSecondsRealtime(1 / textSpeed);
            }
        }

        if (fadeText == true)
        {
            float startFade = Time.time;
            while (true)
            {
                float t = Mathf.InverseLerp(startFade, startFade + fadeTime, Time.time);
                TextColor = fadeGradient.Evaluate(t);
                if (t >= 1)
                    break;
                yield return null;
            }
        }

        if (autoAdvanceAfter != Mathf.Infinity)
        {
            float waitTime = autoAdvanceAfter + (currentLine.playTime - currentLine.text.Length / textSpeed);
            yield return new WaitForSecondsRealtime(waitTime);
            Next();
        }
    }
}
