using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EarphoneLeftAndRight.Dependency
{
    public interface ITextToSpeech
    {
        bool IsSpeaking { get; }
        Task Clear();
        Task SpeakLeft();
        Task SpeakRight();

        Task<bool> SpeakWithPan(string text, float pan, System.Globalization.CultureInfo cultureInfo);
    }
}
