using System;
using Engine;
using Engine.Audio;
using Engine.Media;

namespace Game
{
    public static class MusicManager
    {
        public static MusicManager.Mix CurrentMix
        {
            get
            {
                return m_currentMix;
            }
            set
            {
                if (value != m_currentMix)
                {
                    m_currentMix = value;
                    m_nextSongTime = 0.0;
                }
            }
        }

        public static void Update()
        {
            if (m_fadeSound != null)
            {
                m_fadeSound.Volume = MathUtils.Min(m_fadeSound.Volume - 1f * Volume * Time.FrameDuration, Volume);
                if (m_fadeSound.Volume <= 0f)
                {
                    m_fadeSound.Dispose();
                    m_fadeSound = null;
                }
            }
            if (m_sound != null && Time.FrameStartTime >= m_fadeStartTime)
            {
                m_sound.Volume = MathUtils.Min(m_sound.Volume + 1f * Volume * Time.FrameDuration, Volume);
            }
            if (m_currentMix == Mix.None || Volume == 0f)
            {
                StopMusic();
                return;
            }
            if (m_currentMix == Mix.Menu)
            {
                if (Time.FrameStartTime >= m_nextSongTime || !IsPlaying)
                {
                    float startPercentage = 0f;
                    if (m_random.Int(1) == 0)
                    {
                        PlayMusic(Sounds.SpeedOfLight, startPercentage);
                    }
                    m_nextSongTime = double.MaxValue;
                    return;
                }
            }
            else if (m_currentMix == Mix.Game)
            {
                StopMusic();
            }
        }

        private static bool IsPlaying
        {
            get
            {
                return m_sound != null && m_sound.State > SoundState.Stopped;
            }
        }

        private static void PlayMusic(StreamingSource source, float startPercentage)
        {
            try
            {
                StopMusic();
                m_fadeStartTime = Time.FrameStartTime + 0.0;
                float volume = (m_fadeSound != null) ? 0f : Volume;
                StreamingSource streamingSource = source.Duplicate();
                streamingSource.Position = (long)(MathUtils.Saturate(startPercentage) * (streamingSource.BytesCount / streamingSource.ChannelsCount / 2L)) / 16L * 16L;
                m_sound = new StreamingSound(streamingSource, volume, 1f, 0f, true, false, 1f);
                m_sound.Play();
            }
            catch
            {
                Log.Warning("Error playing music.");
            }
        }

        private static void StopMusic()
        {
            if (m_sound != null)
            {
                if (m_fadeSound != null)
                {
                    m_fadeSound.Dispose();
                }
                m_fadeSound = m_sound;
                m_sound = null;
            }
        }

        private static float Volume
        {
            get
            {
                return SettingsManager.MusicVolume * 0.4f;
            }
        }

        private const float m_fadeSpeed = 1f;

        private const float m_fadeWait = 0f;

        private static StreamingSound m_fadeSound;

        private static StreamingSound m_sound;

        private static double m_fadeStartTime;

        private static MusicManager.Mix m_currentMix;

        private static double m_nextSongTime;

        private static Engine.Random m_random = new Engine.Random();

        public enum Mix
        {
            None,
            Menu,
            Game
        }
    }
}
