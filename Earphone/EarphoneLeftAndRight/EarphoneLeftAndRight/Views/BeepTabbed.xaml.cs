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
    public partial class BeepTabbed : TabbedPage
    {
        public BeepTabbed()
        {
            InitializeComponent();
        }

        private void PianoView_KeyTapped(object sender, PianoView.KeyTappedEventArgs args)
        {
            if (sender is not Views.PianoView piano) return;
            if (piano.BindingContext is not ViewModels.BeepFreqViewModel vm) return;
            if (vm.JustIntonation)
            {
                var (a, _, _, _) = Helper.FreqConverters.HzToOctaveJustIntonation(vm.Frequency);
                int tr = (int)Math.Truncate(args.PressedKey / 12.0);
                vm.Frequency = Helper.FreqConverters.OctaveToHzJustIntonation(a + tr, args.PressedKey - tr * 12, 0);
            }
            else
            {
                vm.Frequency = Helper.FreqConverters.NoteNumberToHzEqualTemperament(Math.Floor((Helper.FreqConverters.HzToNoteNumberEqualTemperament(vm.Frequency) + 0.5) / 12) * 12.0 + args.PressedKey);
            }
        }

        private void TapGestureRecognizer_Tapped_ChangeLocalizationMode(object sender, EventArgs e)
        {
            if ((sender as BindableObject)?.BindingContext is not ViewModels.BeepFreqViewModel vm) return;
            bool locSupported = Helper.FreqConverters.SemitoneLocalizationSupported(Resx.AppResources.Helper_Semitone_Main);
            bool locAltSupported = Helper.FreqConverters.SemitoneLocalizationSupported(Resx.AppResources.Helper_Semitone_Alt);

            switch (vm.SemitoneLocalizeMode)
            {
                case Helper.FreqConverters.SemitoneLocalizeModes.Both:
                    if (Helper.FreqConverters.CentLocalizationSupported) vm.SemitoneLocalizeMode = Helper.FreqConverters.SemitoneLocalizeModes.International;
                    else vm.JustIntonation = !vm.JustIntonation;
                    break;
                case Helper.FreqConverters.SemitoneLocalizeModes.International:
                    if (locSupported) vm.SemitoneLocalizeMode = Helper.FreqConverters.SemitoneLocalizeModes.Local;
                    else
                    {
                        vm.SemitoneLocalizeMode = Helper.FreqConverters.SemitoneLocalizeModes.Both;
                        vm.JustIntonation = !vm.JustIntonation;
                    }
                    break;
                case Helper.FreqConverters.SemitoneLocalizeModes.Local:
                    if (locAltSupported) vm.SemitoneLocalizeMode = Helper.FreqConverters.SemitoneLocalizeModes.LocalAlt;
                    else
                    {
                        vm.SemitoneLocalizeMode = Helper.FreqConverters.SemitoneLocalizeModes.Both;
                        vm.JustIntonation = !vm.JustIntonation;
                    }
                    break;
                case Helper.FreqConverters.SemitoneLocalizeModes.LocalAlt:
                default:
                    vm.SemitoneLocalizeMode = Helper.FreqConverters.SemitoneLocalizeModes.Both;
                    vm.JustIntonation = !vm.JustIntonation;
                    break;
            }
        }
    }
}