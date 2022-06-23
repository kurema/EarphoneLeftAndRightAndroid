using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace EarphoneLeftAndRight.ViewModels
{
    public class BeepFreqViewModel : BaseViewModel
    {

        private double _FrequencyMaximum = 20000;
        public double FrequencyMaximum
        {
            get => _FrequencyMaximum; set
            {
                if (value <= FrequencyMinimum) return;
                SetProperty(ref _FrequencyMaximum, value);
            }
        }

        private double _FrequencyMinimum = 20;
        public double FrequencyMinimum
        {
            get => _FrequencyMinimum; set
            {
                if (value >= FrequencyMinimum) return;
                SetProperty(ref _FrequencyMinimum, value);
            }
        }

        private double _Frequency = 440;
        public double Frequency
        {
            get => _Frequency; set
            {
                if (value <= 0) return;
                value = Math.Min(Math.Max(value, FrequencyMinimum), FrequencyMaximum);
                if (value >= 1000) value = Math.Round(value);
                else value = Math.Round(value / 0.01) * 0.01;
                SetProperty(ref _Frequency, value);
                OnPropertyChanged(nameof(FrequencyHumanReadable));
            }
        }

        public string FrequencyHumanReadable
        {
            get
            {
                var freq = Frequency;
                if (freq < 1000) return $"{freq:F0} Hz";
                else if (freq < 10000) return $"{freq / 1000:0.###} kHz";
                else if (freq < 100000) return $"{freq / 1000:0.##} kHz";
                else return $"{freq:0.#} kHz";
            }
        }

        private bool _OppositePhase;
        public bool OppositePhase { get => _OppositePhase; set => SetProperty(ref _OppositePhase, value); }

        public BeepFreqViewModel()
        {
            AddFrequencyCommand = new Command((arg) =>
            {
                if (!double.TryParse(arg.ToString(), out double value)) return;
                var freq = Frequency;
                double scale = 1.0;
                if (freq < 1000) scale = 1.0;
                else scale = Math.Round(Math.Pow(10, Math.Floor(Math.Log10(Frequency)) - 3));
                Frequency = Math.Round(freq / scale) * scale + scale * value;
            });

            PlayCommand = new Command(async _ =>
            {
                int phaseInt = OppositePhase ? -1 : 1;
                bool uneven = Frequency % 1 != 0;
                double duration = uneven ? 2 : 1;
                try
                {
                    await Storages.AudioStorage.RegisterSignWave(this.Frequency, duration, 0.5 - 0.5 * Balance, (0.5 + 0.5 * Balance) * phaseInt);
                    Storages.AudioStorage.AudioTest.SetLoop(-1, uneven);
                    await Task.Run(() => { try { Storages.AudioStorage.AudioTest.Play(); } catch { } });
                }
                catch { }
                IsPlaying = true;
            });

            StopCommand = new Command(async_ =>
            {
                Storages.AudioStorage.AudioTest.Stop();
                IsPlaying = false;
            });

            SetBalanceCommand = new Command(arg =>
            {
                var value = double.Parse(arg.ToString());
                Balance = value;
            });
        }


        private bool _IsPlaying;
        public bool IsPlaying { get => _IsPlaying; set => SetProperty(ref _IsPlaying, value); }


        private double _Balance;//Between -1 and 1
        public double Balance
        {
            get => _Balance; set
            {
                if (Balance > 1 || Balance < -1) return;
                SetProperty(ref _Balance, value);
            }
        }

        public ICommand AddFrequencyCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand SetBalanceCommand { get; }
    }
}
