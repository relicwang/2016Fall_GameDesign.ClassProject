public class Fire : BehaviorTreeNode
{
    public override bool Tick(AITankControl tank)
    {
        tank.Fire();
        return false;
    }
}
