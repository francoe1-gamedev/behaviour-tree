namespace BehaviourTree
{
    public enum NodeState
    {
        Exit,
        Run,
        Completed,
        Enter,
        Stop
    }

    public enum NodeExcecuteState
    {
        Continue,
        Fail,
        Success,
    }
}