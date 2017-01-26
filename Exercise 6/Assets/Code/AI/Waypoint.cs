using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A waypoint to be used for path planning
/// </summary>
public class Waypoint : MonoBehaviour
{
    /// <summary>
    /// Other Waypoints you can get to from this one with a straight-line path
    /// </summary>
    [NonSerialized]
    public List<Waypoint> Neighbors = new List<Waypoint>();

    /// <summary>
    /// Used in path planning; next closest node to the start node
    /// </summary>
    private Waypoint predecessor;

    /// <summary>
    /// Cached list of all waypoints.
    /// </summary>
    static Waypoint[] AllWaypoints;

    /// <summary>
    /// Compute the Neighbors list
    /// </summary>
    internal void Start()
    {
        var position = transform.position;
        if (AllWaypoints == null)
        {
            AllWaypoints = FindObjectsOfType<Waypoint>();
        }

        foreach (var wp in AllWaypoints) 
            if (wp != this && !BehaviorTreeNode.WallBetween(position, wp.transform.position))
                Neighbors.Add(wp);
    }

    /// <summary>
    /// Visualize the waypoint graph
    /// </summary>
    internal void OnDrawGizmos()
    {
        var position = transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, 0.5f);
        foreach (var wp in Neighbors)
            Gizmos.DrawLine(position, wp.transform.position);    
    }

    /// <summary>
    /// Nearest waypoint to specified location that is reachable by a straight-line path.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Waypoint NearestWaypointTo(Vector2 position)
    {
        Waypoint nearest = null;
        var minDist = float.PositiveInfinity;
        for (int i = 0; i < AllWaypoints.Length; i++)
        {
            var wp = AllWaypoints[i];
            var p = wp.transform.position;
            var d = Vector2.Distance(position, p);
            if (d < minDist && !BehaviorTreeNode.WallBetween(p, position))
            {
                nearest = wp;
                minDist = d;
            }
        }
        return nearest;
    }

    /// <summary>
    /// Returns a series of waypoints to take to get to the specified position
    /// </summary>
    /// <param name="start">Starting position</param>
    /// <param name="end">Desired endpoint</param>
    /// <returns></returns>
    public static List<Waypoint> FindPath(Vector2 start, Vector2 end)
    {
       return FindPathAS(NearestWaypointTo(start), NearestWaypointTo(end));    
    }

    /// <summary>
    /// Finds a sequence of waypoints between a specified pair of waypoints.
    /// IMPORTANT: this is a deliberately bad path planner; it's just BFS and doesn't
    /// pay attention to edge lengths.  Your job is to make it find the actually shortest path.
    /// </summary>
    /// <param name="start">Starting waypoint</param>
    /// <param name="end">Goal waypoint</param>
    /// <returns></returns>
    static List<Waypoint> FindPath(Waypoint start, Waypoint end)
    {
        // Do a BFS of the graph
        var q = new Queue<Waypoint>();
        foreach (var wp in AllWaypoints)
            wp.predecessor = null;
        q.Enqueue(start);
        Waypoint node;
        while ((node = q.Dequeue()) != end)
        {
            foreach (var n in node.Neighbors)
            {
                if (n.predecessor == null)
                {
                    q.Enqueue(n);
                    n.predecessor = node;
                }
            }
        }

        // Reconstruct the path
        var path = new List<Waypoint>();
        path.Add(node);
        while (node != start)
        {
            node = node.predecessor;
            path.Insert(0, node);
        }
        return path;
    }

    /// <summary>
    /// Construct the path by A* algorithm
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    static List<Waypoint> FindPathAS(Waypoint start, Waypoint end)
    {
        foreach (var wp in AllWaypoints)
            wp.predecessor = null;
 
        Dictionary<Waypoint, Waypoint> cameFrom = new Dictionary<Waypoint, Waypoint>();
        Dictionary<Waypoint, double> costSoFar = new Dictionary<Waypoint, double>();

        foreach (var wp in AllWaypoints)
            costSoFar[wp] = float.PositiveInfinity;

        var frontier = new PriorityQueue<Weighted_Waypoint>();
        Weighted_Waypoint start_wp = new Weighted_Waypoint(start, 0);
        frontier.Enqueue(start_wp);

        cameFrom[start] = start;
        costSoFar[start] = 0;

        Weighted_Waypoint current;
        while (((current = frontier.Dequeue()) != null) && frontier.Count() >=0)
        {
            if (current.wp.Equals(end))
            {
                break;
            }

            foreach (var next in current.wp.Neighbors)
            {
                double newCost = costSoFar[current.wp] + distance_cost(current.wp, next);
      
                if (newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    double priority = newCost + Heuristic(next, end);
                    frontier.Enqueue(new Weighted_Waypoint(next, priority));
                    cameFrom[next] = current.wp;
                }
            }
        }

        // Reconstruct the path
        var path = new List<Waypoint>();

        Waypoint cur_key = end;
        Waypoint cur_value = start;
        path.Insert(0, cur_key);
        while (cameFrom.TryGetValue(cur_key, out cur_value)&&(cur_key!= cur_value))
        {
            cur_key = cur_value;
            path.Insert(0, cur_key);
        }
        return path;
    }

    /// <summary>
    /// Define IComparable for Weighted_Waypoint
    /// </summary>
    public class Weighted_Waypoint : IComparable<Weighted_Waypoint>
    {
        public Waypoint wp;
        public double priority; // smaller values are higher priority

        public Weighted_Waypoint(Waypoint wp, double priority)
        {
            this.wp = wp;
            this.priority = priority;
        }

        public override string ToString()
        {
            return "(" + wp + ", " + priority.ToString("F1") + ")";
        }

        public int CompareTo(Weighted_Waypoint other)
        {
            if (this.priority < other.priority) return -1;
            else if (this.priority > other.priority) return 1;
            else return 0;
        }
    } // Weighted_Waypoint


    /// <summary>
    /// Calculate the distance between Waypoint a and b
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    static public double distance_cost(Waypoint a, Waypoint b)
    {
        return Vector2.Distance(a.transform.position, b.transform.position);
    }

    /// <summary>
    /// Here we set the Heuristic distance as the straight line between Waypoint a and b
    /// since in 2D space there is no distance shorter than straight distance
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    static public double Heuristic(Waypoint a, Waypoint b)
    {      
       return Vector2.Distance(a.transform.position, b.transform.position);            
    }



    //From:https://visualstudiomagazine.com/Articles/2012/11/01/Priority-Queues-with-C.aspx
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> data;

        public PriorityQueue()
        {
            this.data = new List<T>();
        }

        public void Enqueue(T item)
        {
            data.Add(item);
            int ci = data.Count - 1; // child index; start at end
            while (ci > 0)
            {
                int pi = (ci - 1) / 2; // parent index
                if (data[ci].CompareTo(data[pi]) >= 0) break; // child item is larger than (or equal) parent so we're done
                T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
                ci = pi;
            }
        }

        public T Dequeue()
        {
            // assumes pq is not empty; up to calling code
            int li = data.Count - 1; // last index (before removal)
            T frontItem = data[0];   // fetch the front
            data[0] = data[li];
            data.RemoveAt(li);

            --li; // last index (after removal)
            int pi = 0; // parent index. start at front of pq
            while (true)
            {
                int ci = pi * 2 + 1; // left child index of parent
                if (ci > li) break;  // no children so done
                int rc = ci + 1;     // right child
                if (rc <= li && data[rc].CompareTo(data[ci]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                    ci = rc;
                if (data[pi].CompareTo(data[ci]) <= 0) break; // parent is smaller than (or equal to) smallest child so done
                T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp; // swap parent and child
                pi = ci;
            }
            return frontItem;
        }

        public T Peek()
        {
            T frontItem = data[0];
            return frontItem;
        }

        public int Count()
        {
            return data.Count;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < data.Count; ++i)
                s += data[i].ToString() + " ";
            s += "count = " + data.Count;
            return s;
        }

        public bool IsConsistent()
        {
            // is the heap property true for all data?
            if (data.Count == 0) return true;
            int li = data.Count - 1; // last index
            for (int pi = 0; pi < data.Count; ++pi) // each parent index
            {
                int lci = 2 * pi + 1; // left child index
                int rci = 2 * pi + 2; // right child index

                if (lci <= li && data[pi].CompareTo(data[lci]) > 0) return false; // if lc exists and it's greater than parent then bad.
                if (rci <= li && data[pi].CompareTo(data[rci]) > 0) return false; // check the right child too.
            }
            return true; // passed all checks
        } // IsConsistent
    } // PriorityQueue


























}