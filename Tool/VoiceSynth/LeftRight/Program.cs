using System;
using System.Speech.Synthesis;

namespace LeftRight
{
    class Program
    {
        static void Main(string[] args)
        {
            if (OperatingSystem.IsWindows())
            {
                var synth = new SpeechSynthesizer();
                var v = synth.GetInstalledVoices();
                synth.SelectVoice("Microsoft Zira Desktop");
                synth.SetOutputToDefaultAudioDevice();
                synth.SetOutputToWaveFile("left.wav");
                synth.Speak("Left");
                synth.SetOutputToWaveFile("right.wav");
                synth.Speak("Right");
            }
        }
    }
}
