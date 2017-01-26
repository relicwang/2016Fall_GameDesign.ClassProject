using System;
using System.Text;
using UnityEngine;

/// <summary>
/// Base class for behavior tree nodes
/// </summary>
[Serializable]
public abstract class BehaviorTreeNode : ScriptableObject
{
    /// <summary>
    /// Which decision procedure to use to decide whether to fire the behavior
    /// </summary>
    public DeciderType Decider = DeciderType.Always;
    /// <summary>
    /// Whether our parent should redecide whether to run when we finish running
    /// </summary>
    public bool ParentRedecidesOnExit;
    /// <summary>
    /// Beahvior won't run unless the tank's AI level is at least this high
    /// </summary>
    public int MinimumLevel;

    /// <summary>
    /// Check if we want to run
    /// </summary>
    /// <param name="tank">The tank being controlled</param>
    /// <returns>True if we want to run</returns>
    public bool Decide(AITankControl tank)
    {
        return tank.AILevel>=MinimumLevel && Decider.Decide(tank);
    }

    /// <summary>
    /// Control the tank.  Called only if this node has been chosen to run.
    /// </summary>
    /// <param name="tank">Tank to control</param>
    /// <returns>True if we want to keep running.</returns>
    public abstract bool Tick(AITankControl tank);

    /// <summary>
    /// Called when node is selected by parent, and before Tick().
    /// Not called again until node is deselected and then later reselected.
    /// </summary>
    /// <param name="tank">Tank being controlled</param>
    public virtual void Activate(AITankControl tank) { }
    /// <summary>
    /// Called when node is deselected.
    /// Not called again, until it is reselected and then deselected again.
    /// </summary>
    /// <param name="tank">Tank being controlled</param>
    public virtual void Deactivate(AITankControl tank) { }

    #region Debugging support
    /// <summary>
    /// Recursively write the currently selected path through the tree into string buffer.
    /// </summary>
    /// <param name="b">String buffer into which to write the string</param>
    public virtual void GetCurrentPath(StringBuilder b)
    {
        if (string.IsNullOrEmpty(name))
            b.Append(this.GetType().Name);
        else
            b.Append(name);
    }

    public virtual void OnDrawBTGizmos(AITankControl tank) { }
    #endregion

    #region Silly Unity stuff
    /// <summary>
    /// Just here to make sure the Player global is initialized.
    /// </summary>
    internal virtual void OnEnable()
    {
        if (Player == null)
            Player = GameObject.FindWithTag("Player").transform;
    }
    #endregion

    #region Static utilities used elsewhere
    public static Transform Player;

    /// <summary>
    /// Check for walls between two points
    /// </summary>
    /// <param name="point1">First point</param>
    /// <param name="point2">Other point</param>
    /// <returns>True if a straight line path between specified points intersects a wall.</returns>
    public static bool WallBetween(Vector3 point1, Vector3 point2)
    {
        return Physics2D.Linecast(point1, point2, (int)Layers.Walls);
    }
    #endregion
}
