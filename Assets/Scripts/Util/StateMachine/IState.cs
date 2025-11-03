namespace Util.StateMachine
{
    public interface IState
    {
        bool IsActive { get; set; }
        void OnEnter();
        void OnExit();
    }
    
    public interface IState<T> : IState  // Don't need "out" since T should be enums, not possibly inheriting classes.
    {
        T Key { get; }
    }
}
