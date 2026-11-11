namespace HeavenMariaBeyond.Service
{
    public interface ISelectModeStateService
    {
        bool IsInSelectMode { get; }
        event Action<bool> SelectModeChanged;
        void EnterSelectMode();
        void ExitSelectMode();
    }
}
