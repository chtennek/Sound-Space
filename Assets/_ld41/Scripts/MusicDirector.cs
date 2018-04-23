using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicDirector : MonoBehaviour
{
    public bool playOnAwake = false;
    private bool isPlaying = false;

    public NoteChart chart;
    public NoteSpawner spawner;
    public NoteSpawner spawnerBlue;
    public NoteSpawner spawnerYellow;

    public EntitySpawner spawnerMove;
    public EntitySpawner spawnerShot;

    public float travelTime = 4f;

    public float inputWindow = 0.2f;
    public InputReceiver input;
    public Trigger beatTrigger;
    public Trigger offbeatTrigger;

    public AudioSource audio;

    private float startTime;
    private float lastInput;

    private float nextMove;
    private float nextShot;

    private Queue<float> beats;

    private float nextBeat;
    private float beatIncrement;
    private float nextDouble;
    private Queue<float> doubles;

    private float BeatToSeconds { get { return 1 / (chart.bpm / 60); } }
    private float SecondsToBeat { get { return (chart.bpm / 60); } }

    private void Reset()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (playOnAwake == true)
            Play();
    }

    private void Update()
    {
        if (isPlaying == false)
            return;

        if (input != null && input.GetAnyButtonDown())
            lastInput = Time.time;

        float currentBeat = GetBeat(Time.time);
        //beatTrigger.Active = inputWindow > BeatsFrom(currentBeat, 0) * BeatToSeconds;
        //offbeatTrigger.Active = inputWindow > BeatsFrom(currentBeat, 0.5f) * BeatToSeconds;

        // Move
        float nextNoteBeat = nextMove - (inputWindow + travelTime) * SecondsToBeat;
        if (currentBeat >= nextNoteBeat)
        {
            spawnerMove.Spawn();
            nextMove++;
        }

        // Shot
        nextNoteBeat = nextShot - (inputWindow + travelTime) * SecondsToBeat;
        if (currentBeat >= nextNoteBeat)
        {
            spawnerShot.Spawn();
            nextShot++;
        }

        // Custom notes
        if (beats.Count != 0)
        {
            float beat = beats.Peek();
            int count = 0;
            while (beats.Count != 0 && currentBeat >= beats.Peek() - travelTime * SecondsToBeat)
            {
                count++;
                beats.Dequeue();
            }
            Spawn(count, beat);
        }

        //// Constant notes
        //nextNoteBeat = nextBeat - travelTime * SecondsToBeat;
        //if (currentBeat >= nextNoteBeat)
        //{
        //    Spawn();
        //    nextBeat += beatIncrement;
        //}

        //// Custom doubles
        //if (doubles.Count != 0)
        //{
        //    nextNoteBeat = doubles.Peek() - travelTime * SecondsToBeat;
        //    if (currentBeat >= nextNoteBeat)
        //    {
        //        beatIncrement /= 2;
        //        doubles.Dequeue();
        //    }
        //}
    }

    private void Spawn(int count, float beat)
    {
        if (beat % 1 == 0)
            spawner.Spawn(count);
        else if (beat % 1 == 0.5f)
            spawnerBlue.Spawn(count);
        else
            spawnerYellow.Spawn(count);
    }

    private float BeatsFrom(float beat, float other)
    {
        return Mathf.Abs((beat + 0.5f + other) % 1 - 0.5f);
    }

    private float GetBeat(float time)
    {
        return (time - startTime - chart.offset) * SecondsToBeat;
    }

    public void Stop()
    {
        isPlaying = false;

        audio.Stop();
        enabled = false;

        // Hack
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("OnDeath"))
        {
            EntityBehaviour entity = go.GetComponent<EntityBehaviour>();
            if (entity != null)
                entity.OnDeath();
        }
    }

    public void Play()
    {
        isPlaying = true;

        audio.clip = chart.clip;
        audio.Play();
        startTime = Time.time;
        nextMove = chart.startMove;
        nextShot = chart.startMove + 0.5f;

        LoadChart();
        return;

        beats = new Queue<float>();
        foreach (float beat in chart.beats)
            beats.Enqueue(beat);

        doubles = new Queue<float>();
        foreach (float beat in chart.doubles)
            doubles.Enqueue(beat);

        nextBeat = chart.startBeat;
        beatIncrement = chart.beatIncrement;
    }

    private void LoadChart()
    {
        float currentLoopStart = chart.startBeat;
        beats = new Queue<float>();
        foreach (NotePattern p in chart.patterns)
        {
            for (int i = 0; i < p.loops; i++)
            {
                foreach (float beat in p.pattern)
                {
                    beats.Enqueue(currentLoopStart + beat);
                }
                currentLoopStart += p.length;
            }
        }
    }

    private IEnumerator Coroutine_PlayAudio(float delay)
    {
        yield return new WaitForSeconds(delay);
        audio.Play();
    }
}
