namespace HeavenMariaBeyond.Service
{
    public class SelectModeStateService : ISelectModeStateService
    {
        public bool IsInSelectMode { get; private set; }

        public event Action<bool> SelectModeChanged = delegate { };

        public void EnterSelectMode()
        {
            SetState(true);
        }

        public void ExitSelectMode()
        {
            SetState(false);
        }

        private void SetState(bool isInSelectMode)
        {
            if (IsInSelectMode == isInSelectMode)
            {
                return;
            }

            IsInSelectMode = isInSelectMode;
            SelectModeChanged(IsInSelectMode);
        }
    }
}
