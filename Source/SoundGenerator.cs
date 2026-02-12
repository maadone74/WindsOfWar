using Microsoft.Xna.Framework.Audio;
using System;

namespace WindsOfWar
{
    public static class SoundGenerator
    {
        public static SoundEffect CreateNoise(int durationMs)
        {
            int sampleRate = 44100;
            int samples = sampleRate * durationMs / 1000;
            byte[] buffer = new byte[samples * 2];
            Random rand = new Random();
            for (int i = 0; i < samples; i++)
            {
                short value = (short)(rand.Next(-10000, 10000));
                buffer[i * 2] = (byte)(value & 0xFF);
                buffer[i * 2 + 1] = (byte)(value >> 8);
            }
            return new SoundEffect(buffer, sampleRate, AudioChannels.Mono);
        }

        public static SoundEffect CreateTone(int frequency, int durationMs)
        {
            int sampleRate = 44100;
            int samples = sampleRate * durationMs / 1000;
            byte[] buffer = new byte[samples * 2];
            double angle = 0;
            double angleStep = 2 * Math.PI * frequency / sampleRate;

            for (int i = 0; i < samples; i++)
            {
                short value = (short)(Math.Sin(angle) * 10000);
                buffer[i * 2] = (byte)(value & 0xFF);
                buffer[i * 2 + 1] = (byte)(value >> 8);
                angle += angleStep;
            }
            return new SoundEffect(buffer, sampleRate, AudioChannels.Mono);
        }
    }
}
