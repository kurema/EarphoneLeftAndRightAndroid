using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EarphoneLeftAndRight.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayPage : ContentPage
    {
        private Dependency.ITextToSpeech tts;


        public PlayPage()
        {
            InitializeComponent();
        }
    }
}