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

        public bool Support096kHz => Storages.AudioStorage.HiDefSupported096kHz;

        public bool Support192kHz => Storages.AudioStorage.HiDefSupported192kHz;

        private bool _IsPianoVisible = true;
        public bool IsPianoVisible { get => _IsPianoVisible; set => SetProperty(ref _IsPianoVisible, value); }

        public string LocalizedCentP0 => Helper.FreqConverters.LocalizeCent(0, this.SemitoneLocalizeMode);
        public string LocalizedCentP1 => Helper.FreqConverters.LocalizeCent(+1, this.SemitoneLocalizeMode);
        public string LocalizedCentM1 => Helper.FreqConverters.LocalizeCent(-1, this.SemitoneLocalizeMode);

        private DateTime LastPlayed = new DateTime(2000, 1, 1);

        public async void TestSupportHiDef()
        {
            await Storages.AudioStorage.TestSupportHiDef();
        }

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
        public double FrequencyRounded
        {
            get => _Frequency; set
            {
                if (value <= 0) return;
                value = Math.Min(Math.Max(value, FrequencyMinimum), FrequencyMaximum);
                //if (value >= 1000) value = Math.Round(value);
                //else value = Math.Round(value / 0.1) * 0.1;
                value = Math.Round(value);
                if (Math.Round(Frequency) == value) return;
                if (Math.Abs(value - Frequency) < 1.0) return;
                if (_Frequency == value) return;
                SetProperty(ref _Frequency, value);
                OnPropertyChanged(nameof(FrequencyHumanReadable));
                OnPropertyChanged(nameof(Frequency));
                OnPropertyChanged(nameof(FrequencyName));
                OnPropertyChanged(nameof(FrequencyNameCent));
                ReplayIfNeeded();
            }
        }

        public double Frequency
        {
            get => _Frequency;
            set
            {
                if (value <= 0) return;
                value = Math.Min(Math.Max(value, FrequencyMinimum), FrequencyMaximum);
                if (Frequency == value) return;
                SetProperty(ref _Frequency, value);
                OnPropertyChanged(nameof(FrequencyHumanReadable));
                OnPropertyChanged(nameof(FrequencyRounded));
                OnPropertyChanged(nameof(FrequencyName));
                OnPropertyChanged(nameof(FrequencyNameCent));
                ReplayIfNeeded();
            }
        }

        public string FrequencyHumanReadable
        {
            get
            {
                var freq = Frequency;
                if (freq < 1000) return $"{freq:0.#} Hz";
                else if (freq < 10000) return $"{freq / 1000:0.###} kHz";
                else if (freq < 100000) return $"{freq / 1000:0.##} kHz";
                else return $"{freq:0.#} kHz";
            }
        }

        public string FrequencyName
        {
            get
            {
                return Helper.FreqConverters.HzToLocalized(Frequency, SemitoneLocalizeMode, JustIntonation).semitone;
            }
        }

        public string FrequencyNameCent
        {
            get
            {
                return Helper.FreqConverters.HzToLocalized(Frequency, SemitoneLocalizeMode, JustIntonation).cent;
            }
        }


        private bool _JustIntonation = false;
        public bool JustIntonation
        {
            get => _JustIntonation; set
            {
                SetProperty(ref _JustIntonation, value);
                OnPropertyChanged(nameof(FrequencyName));
                OnPropertyChanged(nameof(FrequencyNameCent));
            }
        }

        private Helper.FreqConverters.SemitoneLocalizeModes _SemitoneLocalizeMode;
        public Helper.FreqConverters.SemitoneLocalizeModes SemitoneLocalizeMode
        {
            get => _SemitoneLocalizeMode; set
            {
                SetProperty(ref _SemitoneLocalizeMode, value);
                OnPropertyChanged(nameof(FrequencyName));
                OnPropertyChanged(nameof(FrequencyNameCent));
                OnPropertyChanged(nameof(LocalizedCentP0));
                OnPropertyChanged(nameof(LocalizedCentP1));
                OnPropertyChanged(nameof(LocalizedCentM1));
            }
        }


        private bool _OppositePhase;
        public bool OppositePhase
        {
            get => _OppositePhase; set
            {
                SetProperty(ref _OppositePhase, value);
                ReplayIfNeeded();
            }
        }

        public BeepFreqViewModel()
        {
            AddFrequencyCommand = new Command((arg) =>
            {
                if (!double.TryParse(arg?.ToString(), out double value)) return;
                var freq = Frequency;
                double scale = 1.0;
                if (freq < 1000) scale = 1.0;
                else scale = Math.Round(Math.Pow(10, Math.Floor(Math.Log10(Frequency)) - 3));
                Frequency = Math.Round(freq / scale) * scale + scale * value;
            });

            MultiplyFrequencyCommand = new Command(arg =>
            {
                if (!double.TryParse(arg?.ToString(), out double value)) return;
                if (JustIntonation)
                {
                    var (a, b, c, _) = Helper.FreqConverters.HzToOctaveJustIntonation(Frequency);
                    var ab = a * 12 + b + (int)Math.Truncate(value);
                    this.Frequency = Helper.FreqConverters.OctaveToHzJustIntonation((int)Math.Floor(ab / 12.0), ab % 12, c + (value - Math.Truncate(value)) * 100);
                }
                else
                {
                    Frequency *= Math.Pow(2.0, value / 12.0);
                }
            });

            PlayCommand = new Command(async _ =>
            {
                await Play();
            });

            StopCommand = new Command(async_ =>
            {
                Stop();
            });

            SetBalanceCommand = new Command(arg =>
            {
                var value = double.Parse(arg.ToString());
                Balance = value;
            });

            SetCentCommand = new Command(arg =>
            {
                double value;
                if (!double.TryParse(arg.ToString(), out value)) value = 0;
                if (JustIntonation)
                {
                    var (a, b, _, _) = Helper.FreqConverters.HzToOctaveJustIntonation(Frequency);
                    this.Frequency = Helper.FreqConverters.OctaveToHzJustIntonation(a, b, value);
                }
                else
                {
                    this.Frequency = Helper.FreqConverters.NoteNumberToHzEqualTemperament(Math.Round(Helper.FreqConverters.HzToNoteNumberEqualTemperament(Frequency)) + (value / 100));
                }
            });

            SetPianoVisibleCommand = new Command(arg =>
            {
                var key = arg?.ToString()?.ToUpperInvariant();
                if (key == "TRUE") this.IsPianoVisible = true;
                else if (key == "TOGGLE") IsPianoVisible = !IsPianoVisible;
                else IsPianoVisible = false;
            });

            TestSupportHiDef();
        }

        public async Task Play()
        {
            LastPlayed = DateTime.Now;
            int phaseInt = OppositePhase ? -1 : 1;
            bool uneven = Frequency % 1 != 0;
            double duration = uneven ? 2 : 1;
            int sampleRate = 44100;
            if (Frequency > 8000 && this.Support096kHz) sampleRate = 96000;
            if (Frequency > 10000 && Support192kHz) sampleRate = 192000;
            try
            {
                await Storages.AudioStorage.RegisterWave(this.Frequency, duration, 0.5 - 0.5 * Balance, (0.5 + 0.5 * Balance) * phaseInt, WaveKind, sampleRate);
                Storages.AudioStorage.AudioTest.SetLoop(-1, uneven);
                await Task.Run(() => { try { Storages.AudioStorage.AudioTest.Play(); } catch { IsPlaying = false; } });
                IsPlaying = true;
            }
            catch { IsPlaying = false; }
        }

        public void Stop()
        {
            try
            {
                Storages.AudioStorage.AudioTest.Stop();
            }
            catch { }
            IsPlaying = false;
        }

        public async void ReplayIfNeeded()
        {
            if (!IsPlaying) return;
            if ((DateTime.Now - LastPlayed).TotalSeconds < 0.1) return;

            {
                Stop();
                await Play();
            }
        }

        private bool _IsPlaying = false;
        public bool IsPlaying { get => _IsPlaying; set => SetProperty(ref _IsPlaying, value); }


        private double _Balance;//Between -1 and 1
        public double Balance
        {
            get => _Balance; set
            {
                if (Balance > 1 || Balance < -1) return;
                if (Balance == value) return;
                SetProperty(ref _Balance, value);
                ReplayIfNeeded();
            }
        }

        public ICommand AddFrequencyCommand { get; }
        public ICommand MultiplyFrequencyCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand SetBalanceCommand { get; }
        public ICommand SetPianoVisibleCommand { get; }
        public ICommand SetCentCommand { get; }

        private Storages.AudioStorage.WaveKinds _WaveKind = Storages.AudioStorage.WaveKinds.Sine;
        public Storages.AudioStorage.WaveKinds WaveKind
        {
            get => _WaveKind; set
            {
                SetProperty(ref _WaveKind, value);
                ReplayIfNeeded();
            }
        }


        public Storages.AudioStorage.WaveKinds[] WaveKindCandidates { get; } = new[] {
            Storages.AudioStorage.WaveKinds.Sine,
            Storages.AudioStorage.WaveKinds.Square,
            Storages.AudioStorage.WaveKinds.Sawtooth,
            Storages.AudioStorage.WaveKinds.Ramp,
        };

    }
}
