using BehaviourTree.Core;

namespace BehaviourTree.Interfaces
{
    public interface INodeLoopeable
    {
        LoopMode Loop { get; set; }
    }
}