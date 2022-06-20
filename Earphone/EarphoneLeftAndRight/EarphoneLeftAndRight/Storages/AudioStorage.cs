using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace EarphoneLeftAndRight.Storages
{
    public static class AudioStorage
    {
        private static Dependency.IAudioTest _AudioTest;
        public static Dependency.IAudioTest AudioTest { get => _AudioTest ??= DependencyService.Get<Dependency.IAudioTest>(); }

        public static async Task RegisterSignWave(double frequency, double duration, double ampLeft, double ampRight, int sampleRate = 44100)
        {
            double[] amplifications = new[] { ampLeft, ampRight };
            if (frequency >= sampleRate)
            {
                //You can not play the audio above the sampleRate
                await AudioTest.Register((sample, samplerate, channel) =>
                {
                    return 0;
                }, duration, sampleRate);

            }
            else if (frequency >= sampleRate * 5.0 / 12.0)
            {
                //If frequency is between 1 and 5/12 (middle of 1/2 and 1/3) * sampleRate, we assume it's 1/2 * sampleRate.
                await AudioTest.Register((sample, actualSampleRate, channel) =>
                {
                    //We have to make sure sampleRate is actually accepted.
                    if (frequency >= sampleRate)
                    {
                        return 0;
                    }
                    if (frequency >= actualSampleRate * 5.0 / 12.0 && frequency < actualSampleRate)
                    {
                        return 1 - ((sample + 1) % 2) * 2;
                    }
                    else
                    {
                        double t = (double)(sample) / actualSampleRate;
                        return amplifications[channel] * Math.Sin(2.0 * Math.PI * frequency * t);
                    }
                }, duration, sampleRate);

            }
            else
            {
                await AudioTest.Register((sample, actualSampleRate, channel) =>
                {
                    double t = (double)(sample) / actualSampleRate;
                    return amplifications[channel] * Math.Sin(2.0 * Math.PI * frequency * t);
                }, duration, sampleRate);
            }

        }

        public enum ShiftDirection
        {
            LeftToRight, RightToLeft
        }

        public static async Task RegisterSignWaveStereoShift(double frequency, double duration, ShiftDirection direction, int sampleRate = 44100)
        {
            if (frequency >= sampleRate)
            {
                //You can not play the audio above the sampleRate
                await AudioTest.Register((sample, samplerate, channel) =>
                {
                    return 0;
                }, duration, sampleRate);

            }//We omit 22.05kHz operation here.
            else
            {
                int directionNumber = direction switch
                {
                    ShiftDirection.LeftToRight => 1,
                    ShiftDirection.RightToLeft => -1,
                    _ => 0,
                };
                await AudioTest.Register((sample, actualSampleRate, channel) =>
                {
                    double t = (double)(sample) / actualSampleRate;
                    double d = Math.Cos(Math.PI * t / duration) / 2;
                    return channel switch
                    {
                        0 => 0.5 + d * directionNumber,
                        1 => 0.5 - d * directionNumber,
                        _ => 0,
                    } * Math.Sin(2.0 * Math.PI * frequency * t);
                }, duration, sampleRate);
            }

        }

    }
}
