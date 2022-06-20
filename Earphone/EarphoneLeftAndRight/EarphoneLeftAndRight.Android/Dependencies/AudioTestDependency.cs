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

        private AudioTrack audioTrack = null;
        public async Task Register(Func<int, int, int, double> generator, double duration, int sampleRate = 44100)
        {
            //https://white-wheels.hatenadiary.org/entry/20110820/p3
            //https://developer.android.com/reference/android/media/AudioTrack.Builder
            //https://akira-watson.com/android/audiotrack.html
            //https://qiita.com/takahamarn/items/e375a6a3ed806185e540

            //You don't know sampleRate will be accepted. but new AudioTrack() requires bufferSizeInBytes which depend on sampleRate.
            //~So we assume sampleRate will be accepted. 44.1kHz will be basically OK anywhere, but not sure about the other.~
            //We use `while(true){}`

            Release();

            short[] buffer=null;
            while (true)
            {
                await Task.Run(() => { buffer = GenerateBuffer(generator, duration, sampleRate, 2); });
                int bufferSizeInBytes = buffer.Length * 2 * 16 / 8;//bufferSize * Channel * (bit per length) / 8bit
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
            await audioTrack.WriteAsync(buffer, 0, buffer.Length, WriteMode.NonBlocking);
        }

        public void Release()
        {
            if (audioTrack is null) return;
            audioTrack.Stop();
            audioTrack.Release();
            audioTrack.Dispose();
            audioTrack = null;
        }

        public void Stop()
        {
            audioTrack?.Stop();
        }

        public static short[] GenerateBuffer(Func<int, int, int, double> generator, double duration, int sampleRate, int channelCount)
        {
            int samples = (int)(duration * sampleRate);
            var buffer = new short[samples * channelCount];
            //It's totally OK to run in parallel.
            //Parallel.For(0, samples, (i) =>
            //{
            //    for (int channel = 0; channel < channelCount; channel++)
            //    {
            //        buffer[i * channelCount + channel] = (short)(generator(i, sampleRate, channel) * short.MaxValue);
            //    }
            //});
            for(int i = 0; i < samples; i++)
            {
                for (int channel = 0; channel < channelCount; channel++)
                {
                    buffer[i * channelCount + channel] = (short)(generator(i, sampleRate, channel) * short.MaxValue);
                }
            }
            return buffer;
        }

        public void Play()
        {
            if (audioTrack is null) return;
            if (audioTrack.PlayState == PlayState.Playing)
            {
                audioTrack.Stop();
                //audioTrack.Flush();
                //audioTrack.ReloadStaticData();
            }
            audioTrack.Stop();
            audioTrack.ReloadStaticData();
            audioTrack.Play();
        }
    }
}