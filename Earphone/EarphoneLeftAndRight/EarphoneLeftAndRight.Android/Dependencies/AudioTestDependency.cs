using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Media;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(EarphoneLeftAndRight.Droid.AudioTestDependency))]
namespace EarphoneLeftAndRight.Droid
{
    public class AudioTestDependency : Dependency.IAudioTest
    {
        public int ActualSampleRate { get => audioTrack?.SampleRate ?? -1; }
        private int frames;
        private int niceCuttingFrame;

        private readonly System.Threading.SemaphoreSlim Semaphore = new System.Threading.SemaphoreSlim(1, 1);

        private AudioTrack audioTrack = null;

        public double CurrentPosition
        {
            get
            {
                if (audioTrack is null) return 0;
                if (audioTrack.State == AudioTrackState.Uninitialized) return 0;
                try
                {
                    return (double)audioTrack.PlaybackHeadPosition / audioTrack.SampleRate;
                }
                catch
                {
                    return 0;
                }
            }
        }


        public async Task Register(Func<int, int, int, (double, bool)> generator, double duration, int sampleRate = 44100)
        {
            //https://white-wheels.hatenadiary.org/entry/20110820/p3
            //https://developer.android.com/reference/android/media/AudioTrack.Builder
            //https://akira-watson.com/android/audiotrack.html
            //https://qiita.com/takahamarn/items/e375a6a3ed806185e540


            await Semaphore.WaitAsync();
            try
            {
                Release();

                short[] buffer = null;
                while (true)
                {
                    await Task.Run(() => { (buffer, niceCuttingFrame) = GenerateBuffer(generator, duration, sampleRate, 2); });
                    //int bufferSizeInBytes = buffer.Length * 2 * 16 / 8;//bufferSize * Channel * (bit per length) / 8bit
                    int bufferSizeInBytes = buffer.Length * 16 / 8;//bufferSize * (bit per length) / 8bit
                    frames = buffer.Length / 2;

                    if (niceCuttingFrame != 0) { }
                    else if (frames <= 2) { niceCuttingFrame = frames; }
                    else
                    {
                        //Repeating in incorrect point makes a small noise.
                        //This cut in nice frame, which is 0 and sign of differential is same.
                        int sl = buffer[2] - buffer[0];
                        int sr = buffer[3] - buffer[1];

                        int min = int.MaxValue;
                        int currentNice = frames;
                        for (int i = frames / 2; i < frames; i++)
                        {
                            var minC = Math.Abs(buffer[i * 2]) + Math.Abs(buffer[i * 2 + 1]);
                            if (minC <= min && (buffer[i * 2] - buffer[i * 2 - 2]) * sl > 0 && (buffer[i * 2 + 1] - buffer[i * 2 - 1]) * sr > 0)
                            {
                                currentNice = i;
                                min = minC;
                            }
                        }
                        niceCuttingFrame = currentNice;
                    }

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                    {
                        audioTrack = new AudioTrack.Builder()
                            .SetAudioAttributes(new AudioAttributes.Builder()
                            .SetUsage(AudioUsageKind.Media)
                            .SetContentType(AudioContentType.Music)
                            .Build()
                            )
                            .SetAudioFormat(new AudioFormat.Builder()
                            .SetEncoding(Android.Media.Encoding.Pcm16bit)
                            .SetSampleRate(sampleRate)
                            .SetChannelMask(ChannelOut.Stereo)
                            .Build()
                            )
                            .SetBufferSizeInBytes(bufferSizeInBytes)
                            .SetTransferMode(AudioTrackMode.Static)
                            .Build();
                    }
                    else
                    {
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
                        audioTrack = new AudioTrack(Stream.Music, sampleRate, ChannelConfiguration.Stereo, Android.Media.Encoding.Pcm16bit, bufferSizeInBytes, AudioTrackMode.Static);
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
                    }
                    if (sampleRate == ActualSampleRate) break;
                    sampleRate = ActualSampleRate;
                    audioTrack.Release();
                    audioTrack.Dispose();
                    audioTrack = null;
                }
                if (Build.VERSION.SdkInt > BuildVersionCodes.Lollipop)
                {
                    //Following documents says it is supported above 21 inclusive, but it failed. Don't know why.
                    //https://developer.android.com/reference/android/media/AudioTrack#write(float[],%20int,%20int,%20int)
                    await audioTrack.WriteAsync(buffer, 0, buffer.Length, WriteMode.NonBlocking);
                }
                else
                {
                    await audioTrack.WriteAsync(buffer, 0, buffer.Length);
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public void Release()
        {
            if (audioTrack is null) return;
            if (audioTrack.State == AudioTrackState.Uninitialized) return;
            try
            {
                audioTrack.Stop();
                audioTrack.Release();
                audioTrack.Dispose();
            }
            catch { }
            audioTrack = null;
            frames = 0;
            niceCuttingFrame = 0;
        }

        public void Stop()
        {
            if (audioTrack is null) return;
            if (audioTrack.State == AudioTrackState.Uninitialized) return;
            audioTrack?.Stop();
        }

        public static (short[], int) GenerateBuffer(Func<int, int, int, (double, bool)> generator, double duration, int sampleRate, int channelCount)
        {
            int samples = (int)(duration * sampleRate);
            var buffer = new short[samples * channelCount];
            int niceCuttingFrame = 0;
            //It's totally OK to run in parallel.
            Parallel.For(0, samples, (i) =>
            {
                bool niceCutting = true;
                for (int channel = 0; channel < channelCount; channel++)
                {
                    var generated = generator(i, sampleRate, channel);
                    buffer[i * channelCount + channel] = (short)(generated.Item1 * short.MaxValue);
                    niceCutting = niceCutting && generated.Item2;
                }
                if (niceCutting && niceCuttingFrame < i) niceCuttingFrame = i;
            });
            return (buffer, niceCuttingFrame);
        }

        public async void Play()
        {
            await Semaphore.WaitAsync();
            try
            {
                if (audioTrack is null) return;
                if (audioTrack.State == AudioTrackState.Uninitialized) return;
                audioTrack.Stop();
                audioTrack.ReloadStaticData();
                audioTrack.Play();
            }
            finally
            {
                Semaphore.Release();
            }
        }

        /// <summary>
        /// Set loop count.
        /// </summary>
        /// <param name="count">1: once, -1: infinite.</param>
        public void SetLoop(int count = 1, bool niceCutting = false)
        {
            if (audioTrack is null) return;
            audioTrack.SetLoopPoints(0, niceCutting ? niceCuttingFrame : frames, count);
        }
    }
}