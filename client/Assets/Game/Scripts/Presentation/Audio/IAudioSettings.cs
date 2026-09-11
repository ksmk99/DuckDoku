namespace DuckDoku.Presentation
{
    public interface IAudioSettings
    {
        bool SfxEnabled { get; }

        void SetSfxEnabled(bool enabled);
    }
}
