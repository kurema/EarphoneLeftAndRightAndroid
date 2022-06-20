using System;
using System.Collections.Generic;
using System.Text;

namespace EarphoneLeftAndRight.Dependency;

public interface IAudioTest
{
    int ActualSampleRate { get; }

    System.Threading.Tasks.Task Register(Func<int, int, double> generator, double duration, int sampleRate = 44100);

    void Stop();
    void Play();
    void Release();
}
