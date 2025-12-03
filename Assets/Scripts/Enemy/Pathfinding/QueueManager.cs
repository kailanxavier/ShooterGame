using System;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public static QueueManager Instance;

    private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    private Pathfinder pathfinder;
    private bool active;

    private void Awake()
    {
        Instance = this;
        pathfinder = FindFirstObjectByType<Pathfinder>();
    }

    public static void RequestPath(Vector3 start, Vector3 end, Action<List<Vector3>> ctx)
    {
        PathRequest nRequest = new PathRequest(start, end, ctx);
        Instance.pathRequestQueue.Enqueue(nRequest);
        Instance.AttemptNextProcess();
    }

    private void AttemptNextProcess()
    {
        if (!active && pathRequestQueue.Count > 0)
        {
            PathRequest request = pathRequestQueue.Dequeue();
            active = true;

            List<Vector3> path = pathfinder.FindPath(request.start, request.end);
            request.ctx(path);

            active = false;
            AttemptNextProcess();
        }
    }

    private struct PathRequest
    {
        public Vector3 start;
        public Vector3 end;
        public Action<List<Vector3>> ctx;

        public PathRequest(Vector3 start, Vector3 end, Action<List<Vector3>> ctx)
        {
            this.start = start;
            this.end = end;
            this.ctx = ctx;
        }
    }
}
