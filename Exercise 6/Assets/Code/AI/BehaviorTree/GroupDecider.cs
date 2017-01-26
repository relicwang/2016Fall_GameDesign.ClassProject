using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Behavior tree node that choses between child nodes
/// and recursive runs the selected child.
/// </summary>
[Serializable]
public class GroupDecider : BehaviorTreeNode
{
    /// <summary>
    /// Children of this node.  When we run, we recursively run one of them.
    /// </summary>
    public List<BehaviorTreeNode> Children = new List<BehaviorTreeNode>();
    /// <summary>
    /// Child currently selected to run.  We continue to run it until its Tick() method returns false.
    /// </summary>
    private BehaviorTreeNode selected;

    /// <summary>
    /// Policy to use in selecting child to run
    /// </summary>
    public SelectionPolicy Policy = SelectionPolicy.Prioritized;

    public enum SelectionPolicy
    {
        /// <summary>
        /// Take the first child whose Decide() method returns true.
        /// </summary>
        Prioritized,
        /// <summary>
        /// Randomly choose a child whose Decide method returns true.
        /// </summary>
        Random,
        /// <summary>
        /// Run children in order
        /// </summary>
        Sequential,
        /// <summary>
        /// Run children in order, looping forever.
        /// </summary>
        Loop
    }

    /// <summary>
    /// We're not running anymore; recursively deactivate our selected child.
    /// </summary>
    /// <param name="tank">Tank being controlled</param>
    public override void Deactivate(AITankControl tank)
    {
        if (selected)
        {
            selected.Deactivate(tank);
            selected = null;
        }
    }

#if DEBUG
    public override void Activate(AITankControl tank)
    {
        // Check to make sure the subset property is satisfied
        if (!Children.Any(c => c.Decide(tank)))
            Debug.Log(name + " activated without runnable child");
    }
#endif

    /// <summary>
    /// Run our selected child.  If no child is selected, select one.  If can't select one, return false.
    /// </summary>
    /// <param name="tank">Tank being controlled.</param>
    /// <returns>Whether we want to continue running.</returns>
    public override bool Tick(AITankControl tank)
    {
        var newSelection = SelectChild(tank);

        if (newSelection != selected)
        {
            if (selected != null) selected.Deactivate(tank);
            selected = newSelection;
            selected.Activate(tank);

        }

        if (selected != null)
        {
            if (!selected.Tick(tank))
            {
                selected = null;
            }
        }

        if (newSelection == null)
        {
            return false;
        }
        else { return true; }


    }

    /// <summary>
    /// Select a child to run based on the policy.
    /// </summary>
    /// <param name="tank">Tank being controlled</param>
    /// <returns>Child to run, or null if no runnable children.</returns>
    private BehaviorTreeNode SelectChild(AITankControl tank)
    {
        switch (Policy)
        {
            case SelectionPolicy.Prioritized:
                // Fill this in
                foreach (var child in Children)
                {
                    if (child == selected)
                    {
                        return child;
                    }

                    if (child != selected)
                    {
                        if (child.Decide(tank))
                        {
                            return child;
                        }
                    }


                }
                return null;

            default:
                throw new NotImplementedException("Unimplemented policy: " + Policy);
        }
    }

    #region Debugging support
    public override void GetCurrentPath(StringBuilder b)
    {
        base.GetCurrentPath(b);
        if (selected != null)
        {
            b.Append('/');
            selected.GetCurrentPath(b);
        }
    }

    public override void OnDrawBTGizmos(AITankControl tank)
    {
        if (selected)
            selected.OnDrawBTGizmos(tank);
    }

    #endregion
}

