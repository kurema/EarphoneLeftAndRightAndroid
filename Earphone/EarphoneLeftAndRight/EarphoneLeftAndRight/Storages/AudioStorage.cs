using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace EarphoneLeftAndRight.Storages
{
    public static class AudioStorage
    {
        private static Dependency.IAudioTest? _AudioTest;
        public static Dependency.IAudioTest AudioTest { get => _AudioTest ??= DependencyService.Get<Dependency.IAudioTest>(); }


        public static async Task RegisterWave(double frequency, Func<int, int, int, (double, bool)> generator, double duration, double ampLeft, double ampRight, int sampleRate = 44100)
        {
            double[] amplifications = new[] { ampLeft, ampRight };
            if (frequency >= sampleRate)
            {
                //You can not play the audio above the sampleRate
                await AudioTest.Register((sample, samplerate, channel) =>
                {
                    return (0, false);
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
                        return (0, false);
                    }
                    if (frequency >= actualSampleRate * 5.0 / 12.0 && frequency < actualSampleRate)
                    {
                        return (amplifications[channel] * (1 - ((sample + 1) % 2) * 2), false);
                    }
                    else
                    {
                        return generator(sample, actualSampleRate, channel);
                    }
                }, duration, sampleRate);

            }
            else
            {
                await AudioTest.Register(generator, duration, sampleRate);
            }

        }

        public static bool HiDefTested = false;
        public static bool HiDefSupported096kHz = false;
        public static bool HiDefSupported192kHz = false;

        public static async Task TestSupportHiDef()
        {
            if (HiDefTested) return;
            try
            {
                await RegisterWave(200, 0.01, 0, 0, Storages.AudioStorage.WaveKinds.Square, 96000);
                HiDefSupported096kHz = AudioTest.ActualSampleRate == 96000;
                await RegisterWave(200, 0.01, 0, 0, Storages.AudioStorage.WaveKinds.Square, 192000);
                HiDefSupported192kHz = AudioTest.ActualSampleRate == 192000;
                HiDefTested = true;
            }
            catch
            {
                HiDefSupported096kHz = false;
                HiDefSupported192kHz = false;
            }
        }

        public enum SweepChanneldOrder
        {
            Both, LeftThenRight, RightThenLeft
        }

        public static async Task RegisterSweepAsync(double freqFrom, double freqTo, bool exp, double duration, double amp = 0.5, SweepChanneldOrder channeldOrder = SweepChanneldOrder.Both, int sampleRate = 44100)
        {
            switch (channeldOrder)
            {
                case SweepChanneldOrder.Both:
                    if (exp)
                    {
                        var freqFromLog = Math.Log(freqFrom);
                        var freqToLog = Math.Log(freqTo);
                        double freqDifLog = (freqToLog - freqFromLog) / duration;
                        await AudioTest.Register((sample, actualSampleRate, channel) =>
                        {
                            double t = (double)(sample) / actualSampleRate;
                            //Thanks, Wolfram!
                            //https://www.wolframalpha.com/input?i2d=true&i=Integrate%5B2*pi*exp%5C%2840%29a%2Bbt%5C%2841%29%2C%7Bt%2C0%2Cu%7D%5D
                            return (amp * Math.Sin(2.0 * Math.PI * Math.Exp(freqFromLog) * (Math.Exp(freqDifLog * t) - 1) / freqDifLog), false);
                        }, duration, sampleRate);
                    }
                    else
                    {
                        double freqDif = (freqTo - freqFrom) / duration;
                        await AudioTest.Register((sample, actualSampleRate, channel) =>
                        {
                            double t = (double)(sample) / actualSampleRate;
                            //I can handle basic integral. But just in case...
                            //https://www.wolframalpha.com/input?i2d=true&i=Integrate[a%2Bbt%2C{t%2C0%2Cu}]
                            return (amp * Math.Sin(Math.PI * 2 * (freqFrom * t + freqDif * t * t / 2.0)), false);
                        }, duration, sampleRate);
                    }
                    break;
                case SweepChanneldOrder.LeftThenRight:
                case SweepChanneldOrder.RightThenLeft:
                    bool leftStart = channeldOrder == SweepChanneldOrder.LeftThenRight;
                    if (exp)
                    {
                        var freqFromLog = Math.Log(freqFrom);
                        var freqToLog = Math.Log(freqTo);
                        double freqDifLog = (freqToLog - freqFromLog) / duration;
                        await AudioTest.Register((sample, actualSampleRate, channel) =>
                        {
                            double t = (double)(sample) / actualSampleRate;
                            bool isFirst = t < duration;
                            t = isFirst ? t : t - duration;
                            return ((isFirst ^ leftStart ^ (channel == 0) ? amp : 0) * Math.Sin(2.0 * Math.PI * Math.Exp(freqFromLog) * (Math.Exp(freqDifLog * t) - 1) / freqDifLog), false);
                        }, duration * 2, sampleRate);
                    }
                    else
                    {
                        double freqDif = (freqTo - freqFrom) / duration;
                        await AudioTest.Register((sample, actualSampleRate, channel) =>
                        {
                            double t = (double)(sample) / actualSampleRate;
                            bool isFirst = t < duration;
                            t = isFirst ? t : t - duration;
                            return ((isFirst ^ leftStart ^ (channel == 0) ? amp : 0) * Math.Sin(Math.PI * 2 * (freqFrom * t + freqDif * t * t / 2.0)), false);
                        }, duration * 2, sampleRate);

                    }
                    break;
            }
        }

        public static async Task RegisterWave(double frequency, double duration, double ampLeft, double ampRight, WaveKinds kinds = WaveKinds.Sine, int sampleRate = 44100)
        {
            double[] amplifications = new[] { ampLeft, ampRight };
            switch (kinds)
            {
                case WaveKinds.Sine:
                default:
                    await RegisterWave(frequency, (sample, actualSampleRate, channel) =>
                    {
                        double t = (double)(sample) / actualSampleRate;
                        return (amplifications[channel] * Math.Sin(2.0 * Math.PI * frequency * t), false);
                    }, duration, ampLeft, ampRight, sampleRate);
                    break;
                case WaveKinds.Square:
                    await RegisterWave(frequency, (sample, actualSampleRate, channel) =>
                    {
                        double p1 = (sample * frequency) % actualSampleRate;
                        double p2 = p1 / actualSampleRate;
                        return (amplifications[channel] * (p2 < 0.5 ? 1 : -1), (int)p1 == 0);
                    }, duration, ampLeft, ampRight, sampleRate);
                    break;
                case WaveKinds.Sawtooth:
                    await RegisterWave(frequency, (sample, actualSampleRate, channel) =>
                    {
                        double p1 = (sample * frequency) % actualSampleRate;
                        double p2 = p1 / actualSampleRate;
                        return (amplifications[channel] * (-1 + 2 * p2), (int)p1 == 0);
                    }, duration, ampLeft, ampRight, sampleRate);
                    break;
                case WaveKinds.Ramp:
                    await RegisterWave(frequency, (sample, actualSampleRate, channel) =>
                    {
                        double p1 = (sample * frequency) % actualSampleRate;
                        double p2 = p1 / actualSampleRate;

                        //Sample 0 must be 0 for niceCuttingFrame.
                        return (amplifications[channel] * (p2 switch
                        {
                            < 0.25 => -p2 * 4,
                            < 0.75 => -2 + p2 * 4,
                            _ => -p2 * 4 + 4
                        }), (int)p1 == 0);
                    }, duration, ampLeft, ampRight, sampleRate);
                    break;
            }
        }

        public enum ShiftDirection
        {
            LeftToRight, RightToLeft
        }

        public enum WaveKinds
        {
            Sine, Square, Sawtooth, Ramp,
        }


        public static async Task RegisterSignWaveStereoShift(double frequency, double duration, ShiftDirection direction, int sampleRate = 44100)
        {
            if (frequency >= sampleRate)
            {
                //You can not play the audio above the sampleRate
                await AudioTest.Register((sample, samplerate, channel) =>
                {
                    return (0, false);
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
                    return (channel switch
                    {
                        0 => 0.5 + d * directionNumber,
                        1 => 0.5 - d * directionNumber,
                        _ => 0,
                    } * Math.Sin(2.0 * Math.PI * frequency * t), false);
                }, duration, sampleRate);
            }

        }

    }
}
