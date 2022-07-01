using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;

namespace EarphoneLeftAndRight.ViewModels;

public class BeepSweepViewModel : BaseViewModel
{
    private double _Duration = 10.0;
    public double Duration { get => _Duration; set => SetProperty(ref _Duration, value); }


    private double _FrequencyStart = 20.0;
    public double FrequencyStart
    {
        get => _FrequencyStart; set
        {
            if (value < FrequencyMinimum) return;
            if (value > FrequencyMaximum) return;
            SetProperty(ref _FrequencyStart, value);
        }
    }


    private double _FrequencyEnd = 20000.0;
    public double FrequencyEnd
    {
        get => _FrequencyEnd; set
        {
            if (value < FrequencyMinimum) return;
            if (value > FrequencyMaximum) return;
            SetProperty(ref _FrequencyEnd, value);
        }
    }


    private double _FrequencyMinimum = 5.0;
    public double FrequencyMinimum { get => _FrequencyMinimum; set => SetProperty(ref _FrequencyMinimum, value); }


    private double _FrequencyMaximum = 96000.0;
    public double FrequencyMaximum { get => _FrequencyMaximum; set => SetProperty(ref _FrequencyMaximum, value); }


    private bool _EachChannel = false;
    public bool EachChannel { get => _EachChannel; set => SetProperty(ref _EachChannel, value); }


    private bool _Exponential = true;
    public bool Exponential { get => _Exponential; set => SetProperty(ref _Exponential, value); }


    private bool _Semitone = false;

    public BeepSweepViewModel()
    {
        PlayCommand = new Command(async () =>
        {
            await Storages.AudioStorage.TestSupportHiDef();
            await Storages.AudioStorage.RegisterSweepAsync(this.FrequencyStart, FrequencyEnd, this.Exponential, this.Duration, 0.5, EachChannel ? Storages.AudioStorage.SweepChanneldOrder.LeftThenRight : Storages.AudioStorage.SweepChanneldOrder.Both
                , Storages.AudioStorage.HiDefSupported192kHz ? 192000 : (Storages.AudioStorage.HiDefSupported096kHz ? 96000 : 44100));
            await Task.Run(() => Storages.AudioStorage.AudioTest.Play());
        });

        SetFrequencyCommand = new Command((arg) =>
        {
            var args = arg?.ToString()?.Split(':')?.Select(a => double.Parse(a, System.Globalization.CultureInfo.InvariantCulture)).ToArray();
            if (args[0] >= 0) FrequencyStart = args[0];
            if (args[1] >= 0) FrequencyEnd = args[1];
        });

        SetFrequencyMaxCommand = new Command(() =>
        {
            FrequencyStart = FrequencyMinimum;
            FrequencyEnd = (Storages.AudioStorage.HiDefSupported192kHz ? 192000 : (Storages.AudioStorage.HiDefSupported096kHz ? 96000 : 44100)) / 2.0;
        });

        SetFrequencyInvertCommand = new Command(() =>
        {
            (FrequencyEnd, FrequencyStart) = (FrequencyStart, FrequencyEnd);
        });
    }

    public bool Semitone { get => _Semitone; set => SetProperty(ref _Semitone, value); }

    public ICommand PlayCommand { get; }
    public ICommand SetFrequencyCommand { get; }
    public ICommand SetFrequencyInvertCommand { get; }
    public ICommand SetFrequencyMaxCommand { get; }

    public CurrentHzProvider GetCurrentHzProvider()
    {
        return new CurrentHzProvider()
        {
            Duration = Duration,
            FrequencyEnd = FrequencyEnd,
            FrequencyStart = FrequencyStart,
            Exponential = Exponential,
            EachChannel = EachChannel,
        };
    }

    public class CurrentHzProvider
    {
        public double Duration { get; set; }
        public double FrequencyStart { get; set; }
        public double FrequencyEnd { get; set; }
        public bool Exponential { get; set; }
        public bool EachChannel { get; set; }

        public double ActualDuration => EachChannel ? Duration * 2 : Duration;

        public double SecondsToHz(double sec)
        {
            sec %= Duration;
            if (Exponential)
            {
                double slog = Math.Log(FrequencyStart);
                double elog = Math.Log(FrequencyEnd);
                double dlog = (elog - slog) / Duration;
                return Math.Pow(Math.E, slog + dlog * sec);
            }
            else
            {
                return FrequencyStart + (FrequencyEnd - FrequencyStart) * sec / Duration;
            }
        }
    }
}
