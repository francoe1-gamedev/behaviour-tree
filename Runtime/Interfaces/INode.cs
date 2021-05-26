namespace BehaviourTree.Interfaces
{
    public interface INode
    {
        int Id { get; }

        string GetName();

        void SetName(string name);

        string GetDescription();

        NodeState GetState();

        NodeExcecuteState GetExecutionState();

        void Enter(INode node);

        void Exit();

        void Tick();

        void Stop();

        void Reset();

        bool Compare(INode node);
    }
}