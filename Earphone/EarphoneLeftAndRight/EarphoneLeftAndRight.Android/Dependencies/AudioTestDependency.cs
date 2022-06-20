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

[assembly: Xamarin.Forms.Dependency(typeof(EarphoneLeftAndRight.Droid.AudioTestDependency))]
namespace EarphoneLeftAndRight.Droid
{
    public class AudioTestDependency : Dependency.IAudioTest
    {
        public int SampleRate { get => audioTrack?.SampleRate ?? -1; }

        private AudioTrack audioTrack = null;
        public async System.Threading.Tasks.Task Register(Func<int, int, double> generator, double duration, int sampleRate = 44100)
        {
            //https://white-wheels.hatenadiary.org/entry/20110820/p3
            //https://developer.android.com/reference/android/media/AudioTrack.Builder
            //https://akira-watson.com/android/audiotrack.html

            //You don't know sampleRate will be accepted. but new AudioTrack() requires bufferSizeInBytes which depend on sampleRate.
            //~So we assume sampleRate will be accepted. 44.1kHz will be basically OK anywhere, but not sure about the other.~
            //We use `while(true){}`
            short[] buffer;
            while(true)
            {
                buffer = GenerateBuffer(generator, duration, sampleRate);
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
                        .Build();
                }
                else
                {
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
                    audioTrack = new AudioTrack(Stream.Music, sampleRate, ChannelConfiguration.Stereo, Android.Media.Encoding.Pcm16bit, bufferSizeInBytes, AudioTrackMode.Static);
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
                }
                if (sampleRate == SampleRate) break;
                sampleRate = SampleRate;
                audioTrack.Release();
                audioTrack.Dispose();
                audioTrack = null;
            }
            await audioTrack.WriteAsync(buffer, 0, buffer.Length);
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

        public static short[] GenerateBuffer(Func<int, int, double> generator, double duration, int sampleRate)
        {
            int samples = (int)(duration * sampleRate);
            var buffer = new short[samples * 2];
            for (int i = 0; i < samples; i++)
            {
                buffer[i] = (short)(generator(i, sampleRate) * short.MaxValue);
            }
            return buffer;
        }

        public void Play()
        {
            if (audioTrack is null) return;
            if (audioTrack.PlayState != PlayState.Stopped)
            {
                audioTrack.Stop();
                audioTrack.ReloadStaticData();
            }
            audioTrack.Play();
        }
    }
}