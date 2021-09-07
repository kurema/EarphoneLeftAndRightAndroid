using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EarphoneLeftAndRight.Dependency
{
    public interface ITextToSpeech
    {
        Task Clear();
        Task SpeakLeft();
        Task SpeakRight();
    }
}
