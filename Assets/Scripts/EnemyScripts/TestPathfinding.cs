using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TestPathfinding : MonoBehaviour
{

    public Transform target;
    public float speed = 5f;
    public float nextWaypointDistance = 2f;
    Path path;
    int currentWaypoint = 0;
    
    Seeker seeker;
    State currentState;

    public enum State
    {
        Stalk,
        MoveIn,
        Attack,
        Stunned,
        Flee
    }
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void UpdatePath()
    {
       if(seeker.IsDone())
       {
           seeker.StartPath(transform.position, target.position, OnPathComplete);
       }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error) 
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(path == null) 
        {
            return;
        }
        if (currentWaypoint < path.vectorPath.Count) {
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
            Vector2 translation = direction * speed * Time.deltaTime;
            float distance = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
            transform.Translate(translation);
            
            if (distance < nextWaypointDistance && currentWaypoint != path.vectorPath.Count - 1)
            {
                currentWaypoint++;
                
            }
        }
    }
}
