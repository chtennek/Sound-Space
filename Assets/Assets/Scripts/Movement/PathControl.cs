﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathControl : MonoBehaviour
{
    [Header("Path")]
    public bool loopInitialPoints = false;
    public PathPoint[] initialPoints = new PathPoint[0];

    private Queue<PathPoint> points = new Queue<PathPoint>();
    private Vector3 anchorPosition;
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private PathPoint current;
    private float currentStartTime;
    private float currentCompleteTime;
    private float nextStartTime;

    public float EndTime { get { return points.Count == 0 ? nextStartTime : Mathf.Infinity; } }
    public int Count
    {
        get
        {
            return (Time.time >= nextStartTime) ? 0 : points.Count + 1;
        }
    }

    public void AddWaypoint(PathPoint point)
    {
        point.position -= anchorPosition;
        points.Enqueue(point);

        if (Time.time >= nextStartTime)
            ApplyNextWaypoint(); // [TODO] See if we need ProcessEvents() here
    }

    private void InitializePath()
    {
        if (initialPoints != null)
            foreach (PathPoint point in initialPoints)
                points.Enqueue(point);
    }

    private void Start()
    {
        anchorPosition = transform.position;
        InitializePath();
    }

    private void Update()
    {
        UpdateTransform();
        ProcessEvents();

        if (loopInitialPoints == true && points.Count == 0)
            InitializePath();

        if (Time.time >= nextStartTime)
        {
            ApplyNextWaypoint();
            ProcessEvents();
        }
    }

    private void ProcessEvents()
    {
        if (current != null)
        {
            float t1 = Mathf.InverseLerp(currentStartTime, currentCompleteTime, Time.time - Time.deltaTime);
            float t2 = Mathf.InverseLerp(currentStartTime, currentCompleteTime, Time.time);
            if (Mathf.Approximately(t1, 1) || Mathf.Approximately(currentStartTime, currentCompleteTime))
            {
                t1 = 1 + Time.time - Time.deltaTime - currentCompleteTime;
                t2 = 1 + Time.time - currentCompleteTime;
            }

            current.RunEvents(t1, t2);
        }
    }

    private void UpdateTransform()
    {
        if (current == null)
            return;
        float t0 = (Time.time - currentStartTime) / (currentCompleteTime - currentStartTime);
        float t1 = t0;
        if (current.approachCurve != null)
            current.approachCurve.Evaluate(Mathf.Clamp(t0, 0, 1));

        // [TODO] add curvature capabilities
        transform.position = Vector3.Lerp(lastPosition, anchorPosition + current.position, t1);
        if (current.affectRotation)
            transform.rotation = Quaternion.Lerp(lastRotation, current.rotation, t1);
    }

    private void ApplyNextWaypoint()
    {
        current = points.Count > 0 ? points.Dequeue() : null;
        if (current == null)
            return;

        lastPosition = transform.position;
        lastRotation = transform.rotation;
        currentStartTime = Time.time;
        currentCompleteTime = Time.time + current.GetTravelTimeFrom(transform.position - anchorPosition);
        nextStartTime = currentCompleteTime + current.waitTime;
    }
}
