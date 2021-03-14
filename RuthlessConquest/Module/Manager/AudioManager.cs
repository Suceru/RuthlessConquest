using System;
using System.Collections.Generic;
using Engine;
using Engine.Audio;

namespace Game
{
    public static class AudioManager
    {
        public static void PlaySound(SoundBuffer soundBuffer, bool avoidRepetition, float volume = 1f, float pitch = 1f, float pan = 0f)
        {
            if (SettingsManager.SfxVolume > 0f)
            {
                float num = volume * SettingsManager.SfxVolume;
                if (avoidRepetition)
                {
                    AudioManager.LastPlayedData lastPlayedData;
                    LastPlayedTime.TryGetValue(soundBuffer, out lastPlayedData);
                    if (Time.FrameStartTime > lastPlayedData.Time + 0.03 || num > lastPlayedData.Volume)
                    {
                        LastPlayedTime[soundBuffer] = new AudioManager.LastPlayedData
                        {
                            Time = Time.FrameStartTime,
                            Volume = num
                        };
                        new Sound(soundBuffer, num, pitch, pan, false, true).Play();
                        return;
                    }
                }
                else
                {
                    new Sound(soundBuffer, num, pitch, pan, false, true).Play();
                }
            }
        }

        private static Dictionary<SoundBuffer, AudioManager.LastPlayedData> LastPlayedTime = new Dictionary<SoundBuffer, AudioManager.LastPlayedData>();

        private struct LastPlayedData
        {
            public double Time;

            public float Volume;
        }
    }
}
