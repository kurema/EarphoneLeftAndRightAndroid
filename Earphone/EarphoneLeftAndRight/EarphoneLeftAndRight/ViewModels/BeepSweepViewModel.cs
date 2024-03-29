﻿using System;
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
	public double Duration
	{
		get => _Duration; set
		{
			if (value < 0.1) return;
			SetProperty(ref _Duration, value);
		}
	}


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
			if (Semitone) await Storages.AudioStorage.RegisterSweepSemitoneAsync(this.FrequencyStart, FrequencyEnd, this.Exponential, this.Duration, 0.5, EachChannel ? Storages.AudioStorage.SweepChanneldOrder.LeftThenRight : Storages.AudioStorage.SweepChanneldOrder.Both, Storages.AudioStorage.HiDefSupported192kHz ? 192000 : (Storages.AudioStorage.HiDefSupported096kHz ? 96000 : 44100), this.OppositePhase);
			else await Storages.AudioStorage.RegisterSweepAsync(this.FrequencyStart, FrequencyEnd, this.Exponential, this.Duration, 0.5, EachChannel ? Storages.AudioStorage.SweepChanneldOrder.LeftThenRight : Storages.AudioStorage.SweepChanneldOrder.Both, Storages.AudioStorage.HiDefSupported192kHz ? 192000 : (Storages.AudioStorage.HiDefSupported096kHz ? 96000 : 44100), this.OppositePhase);
			await Task.Run(() => Storages.AudioStorage.AudioTest.Play());
		});

		SetFrequencyCommand = new Command((arg) =>
		{
			var args = arg?.ToString()?.Split(':')?.Select(a => double.Parse(a, System.Globalization.CultureInfo.InvariantCulture)).ToArray();
			if (args is null || args.Length < 2) return;
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

		DurationAddCommand = new Command((arg) =>
		{
			if (!double.TryParse(arg.ToString(), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out double t)) return;
			this.Duration += t;
		});
	}

	public bool Semitone { get => _Semitone; set => SetProperty(ref _Semitone, value); }

	private bool _OppositePhase = false;
	public bool OppositePhase { get => _OppositePhase; set => SetProperty(ref _OppositePhase, value); }

	public ICommand PlayCommand { get; }
	public ICommand SetFrequencyCommand { get; }
	public ICommand SetFrequencyInvertCommand { get; }
	public ICommand SetFrequencyMaxCommand { get; }
	public ICommand DurationAddCommand { get; }

	public CurrentHzProvider GetCurrentHzProvider()
	{
		return new CurrentHzProvider()
		{
			Duration = Duration,
			FrequencyEnd = FrequencyEnd,
			FrequencyStart = FrequencyStart,
			Exponential = Exponential,
			EachChannel = EachChannel,
			Semitone = Semitone
		};
	}

	public class CurrentHzProvider
	{
		public double Duration { get; set; }
		public double FrequencyStart { get; set; }
		public double FrequencyEnd { get; set; }
		public bool Exponential { get; set; }
		public bool EachChannel { get; set; }
		public bool Semitone { get; set; }

		public double ActualDuration => EachChannel ? Duration * 2 : Duration;

		public double SecondsToHz(double sec)
		{
			if ((EachChannel && sec > Duration * 2) || ((!EachChannel) && sec > Duration))
			{
				return 0;
			}
			sec %= Duration;
			if (Semitone)
			{
				var start = Helper.FreqConverters.HzToNoteNumberEqualTemperament(FrequencyStart);
				var end = Helper.FreqConverters.HzToNoteNumberEqualTemperament(FrequencyEnd);

				if (Math.Floor(start) == Math.Floor(end))
				{
					return Helper.FreqConverters.NoteNumberToHzEqualTemperament(Math.Floor(start));
				}
				else if (start > end)
				{
					start = Math.Floor(start);
					end = Math.Ceiling(end);
				}
				else
				{
					start = Math.Ceiling(start);
					end = Math.Floor(end);
				}

				if (Exponential)
				{
					return Helper.FreqConverters.NoteNumberToHzEqualTemperament(start + Math.Sign(end - start) * Math.Floor(sec / Duration * (Math.Abs(end - start) + 1)));
				}
				else if (start < end)
				{
					var hzS = Helper.FreqConverters.NoteNumberToHzEqualTemperament(start);
					double hzPerSec = (Helper.FreqConverters.NoteNumberToHzEqualTemperament(end + 1) - hzS) / Duration;
					return Helper.FreqConverters.NoteNumberToHzEqualTemperament(Math.Floor(Helper.FreqConverters.HzToNoteNumberEqualTemperament(hzS + hzPerSec * sec)));
				}
				else
				{
					if (sec == 0) return Helper.FreqConverters.NoteNumberToHzEqualTemperament(start);
					var hzS = Helper.FreqConverters.NoteNumberToHzEqualTemperament(start + 1);
					double hzPerSec = (hzS - Helper.FreqConverters.NoteNumberToHzEqualTemperament(end)) / Duration;
					return Helper.FreqConverters.NoteNumberToHzEqualTemperament(Math.Floor(Helper.FreqConverters.HzToNoteNumberEqualTemperament(hzS - hzPerSec * sec)));

				}
			}
			else
			{
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
}
