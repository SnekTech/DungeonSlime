using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace MonoGameLibrary.Audio;

public class AudioController : IDisposable
{
    private readonly List<SoundEffectInstance> _activeSoundEffectInstances = [];

    private float _previousSongVolume;
    private float _previousSoundEffectVolume;

    public bool IsMuted { get; private set; }

    public float SongVolume
    {
        get => IsMuted ? 0 : MediaPlayer.Volume;
        set
        {
            if (IsMuted) return;

            MediaPlayer.Volume = Math.Clamp(value, 0, 1);
        }
    }

    public float SoundEffectVolume
    {
        get => IsMuted ? 0 : SoundEffect.MasterVolume;
        set
        {
            if (IsMuted) return;

            SoundEffect.MasterVolume = Math.Clamp(value, 0, 1);
        }
    }

    public bool IsDisposed { get; private set; }

    ~AudioController() => Dispose(false);

    public void Update()
    {
        for (var i = _activeSoundEffectInstances.Count - 1; i >= 0; i--)
        {
            var instance = _activeSoundEffectInstances[i];

            if (instance.State != SoundState.Stopped) continue;

            if (!instance.IsDisposed)
            {
                instance.Dispose();
            }

            _activeSoundEffectInstances.RemoveAt(i);
        }
    }

    public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect,
        float volume = 1, float pitch = 1, float pan = 0, bool isLooped = false)
    {
        var soundEffectInstance = soundEffect.CreateInstance();

        soundEffectInstance.Volume = volume;
        soundEffectInstance.Pitch = pitch;
        soundEffectInstance.Pan = pan;
        soundEffectInstance.IsLooped = isLooped;

        soundEffectInstance.Play();

        _activeSoundEffectInstances.Add(soundEffectInstance);
        return soundEffectInstance;
    }

    public void PlaySong(Song song, bool isRepeating = true)
    {
        if (MediaPlayer.State == MediaState.Playing)
        {
            MediaPlayer.Stop();
        }

        MediaPlayer.Play(song);
        MediaPlayer.IsRepeating = isRepeating;
    }

    public void PauseAudio()
    {
        MediaPlayer.Pause();

        foreach (var soundEffectInstance in _activeSoundEffectInstances)
        {
            soundEffectInstance.Pause();
        }
    }

    public void ResumeAudio()
    {
        MediaPlayer.Resume();

        foreach (var soundEffectInstance in _activeSoundEffectInstances)
        {
            soundEffectInstance.Resume();
        }
    }

    public void MuteAudio()
    {
        _previousSongVolume = MediaPlayer.Volume;
        _previousSoundEffectVolume = SoundEffect.MasterVolume;

        MediaPlayer.Volume = 0;
        SoundEffect.MasterVolume = 0;
        IsMuted = true;
    }

    public void UnmuteAudio()
    {
        MediaPlayer.Volume = _previousSongVolume;
        SoundEffect.MasterVolume = _previousSoundEffectVolume;
        IsMuted = false;
    }

    public void ToggleMute()
    {
        if (IsMuted)
        {
            UnmuteAudio();
        }
        else
        {
            MuteAudio();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (IsDisposed) return;

        if (disposing)
        {
            foreach (var soundEffectInstance in _activeSoundEffectInstances)
            {
                soundEffectInstance.Dispose();
            }

            _activeSoundEffectInstances.Clear();
        }

        IsDisposed = true;
    }
}