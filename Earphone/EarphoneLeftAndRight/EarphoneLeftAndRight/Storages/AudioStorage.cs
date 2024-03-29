﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace EarphoneLeftAndRight.Storages;

public static class AudioStorage
{
	private static Dependency.IAudioTest? _AudioTest;
	public static Dependency.IAudioTest AudioTest { get => _AudioTest ??= DependencyService.Get<Dependency.IAudioTest>(); }


	public static async Task RegisterWave(double frequency, Func<int, int, int, (double, bool)> generator, double duration, double ampLeft, double ampRight, int sampleRate = 44100)
	{
		double[] amplifications = new[] { ampLeft, ampRight };
		if (frequency >= sampleRate)
		{
			//You can not play the audio above the sampleRate
			await AudioTest.Register((sample, samplerate, channel) =>
			{
				return (0, false);
			}, duration, sampleRate);

		}
		else if (frequency >= sampleRate * 5.0 / 12.0)
		{
			//If frequency is between 1 and 5/12 (middle of 1/2 and 1/3) * sampleRate, we assume it's 1/2 * sampleRate.
			await AudioTest.Register((sample, actualSampleRate, channel) =>
			{
				//We have to make sure sampleRate is actually accepted.
				if (frequency >= sampleRate)
				{
					return (0, false);
				}
				if (frequency >= actualSampleRate * 5.0 / 12.0 && frequency < actualSampleRate)
				{
					return (amplifications[channel] * (1 - ((sample + 1) % 2) * 2), false);
				}
				else
				{
					return generator(sample, actualSampleRate, channel);
				}
			}, duration, sampleRate);

		}
		else
		{
			await AudioTest.Register(generator, duration, sampleRate);
		}

	}

	public static bool HiDefTested = false;
	public static bool HiDefSupported096kHz = false;
	public static bool HiDefSupported192kHz = false;

	public static async Task TestSupportHiDef()
	{
		if (HiDefTested) return;
		try
		{
			await RegisterWave(200, 0.01, 0, 0, WaveKinds.Square, 96000);
			HiDefSupported096kHz = AudioTest.ActualSampleRate == 96000;
			await RegisterWave(200, 0.01, 0, 0, WaveKinds.Square, 192000);
			HiDefSupported192kHz = AudioTest.ActualSampleRate == 192000;
			HiDefTested = true;
		}
		catch
		{
			HiDefSupported096kHz = false;
			HiDefSupported192kHz = false;
		}
	}

	public enum SweepChanneldOrder
	{
		Both, LeftThenRight, RightThenLeft
	}

	public static async Task RegisterSweepSemitoneAsync(double freqFrom, double freqTo, bool exp, double duration, double amp = 0.5, SweepChanneldOrder channeldOrder = SweepChanneldOrder.Both, int sampleRate = 44100, bool invertRight = false)
	{
		//var start = equal ? Helper.FreqConverters.HzToNoteNumberEqualTemperament(freqFrom) : Helper.FreqConverters.HzToOctaveJustIntonation(freqFrom).noteNumber;
		//var end = equal ? Helper.FreqConverters.HzToNoteNumberEqualTemperament(freqTo) : Helper.FreqConverters.HzToOctaveJustIntonation(freqTo).noteNumber;
		var start = Helper.FreqConverters.HzToNoteNumberEqualTemperament(freqFrom);
		var end = Helper.FreqConverters.HzToNoteNumberEqualTemperament(freqTo);

		if (Math.Floor(start) == Math.Floor(end))
		{
			await RegisterWave(Helper.FreqConverters.NoteNumberToHzEqualTemperament(Math.Floor(start)), duration, amp, amp * (invertRight ? -1 : 1), WaveKinds.Sine, sampleRate);
			return;
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
		int noteCounts = (int)Math.Abs(start - end) + 1;
		double[] hzTable = new double[noteCounts + 1];
		for (int i = 0; i <= noteCounts; i++)
		{
			hzTable[i] = Helper.FreqConverters.NoteNumberToHzEqualTemperament(i * Math.Sign(end - start) + start);
		}
		if (exp)
		{
			double[] noteVirtualDuration = new double[noteCounts];
			double len = duration / noteCounts;
			double currentTimeVirtual = 0;
			noteVirtualDuration[0] = 0;
			for (int index = 1; index < noteCounts; index++)
			{
				currentTimeVirtual += len * 2.0 * Math.PI * hzTable[index - 1];
				noteVirtualDuration[index] = currentTimeVirtual;
			}
			await AudioTest.Register((sample, actualSampleRate, channel) =>
			{
				double tActual = (double)sample / actualSampleRate;
				double t = tActual % duration;
				double cnt = Math.Floor(t / len);
				double nn = cnt * Math.Sign(end - start) + start;
				double rem = t - len * cnt;
				double ampActual = channeldOrder switch
				{
					SweepChanneldOrder.LeftThenRight => tActual < duration ^ channel == 1 ? amp : 0,
					SweepChanneldOrder.RightThenLeft => tActual < duration ^ channel == 0 ? amp : 0,
					_ or SweepChanneldOrder.Both => amp,
				} * (channel == 1 && invertRight ? -1 : 1);
				return (ampActual * Math.Sin(Math.PI * 2.0 * rem * hzTable[(int)cnt] + noteVirtualDuration[(int)cnt]), false);
			}, duration * channeldOrder switch { SweepChanneldOrder.LeftThenRight or SweepChanneldOrder.RightThenLeft => 2, _ => 1 }, sampleRate);
			return;
		}
		else
		{
			double secPerHz = duration / Math.Abs(hzTable[^1] - hzTable[0]);
			double[] noteDuration = new double[noteCounts + 1];
			double[] noteDurationVirtual = new double[noteCounts + 1];
			double currentTime = 0;
			double currentTimeVirtual = 0;
			noteDuration[0] = 0;
			noteDurationVirtual[0] = 0;

			for (int i = 0; i < noteCounts; i++)
			{
				var len = Math.Abs(hzTable[i + 1] - hzTable[i]) * secPerHz;
				currentTime += len;
				noteDuration[i + 1] = currentTime;
				currentTimeVirtual += len * 2.0 * Math.PI * hzTable[i];
				noteDurationVirtual[i + 1] = currentTimeVirtual;
			}
			await AudioTest.Register((sample, actualSampleRate, channel) =>
			{
				double tActual = (double)sample / actualSampleRate;
				double t = tActual % duration;
				int cnt = 0;
				while (cnt < noteCounts)
				{
					if (noteDuration[cnt + 1] > t) break;
					cnt++;
				}
				double rem = t - noteDuration[cnt];
				double freq = hzTable[cnt];
				double ampActual = channeldOrder switch
				{
					SweepChanneldOrder.LeftThenRight => tActual < duration ^ channel == 1 ? amp : 0,
					SweepChanneldOrder.RightThenLeft => tActual < duration ^ channel == 0 ? amp : 0,
					_ or SweepChanneldOrder.Both => amp,
				} * (channel == 1 && invertRight ? -1 : 1);
				return (ampActual * Math.Sin(Math.PI * 2.0 * rem * hzTable[cnt] + noteDurationVirtual[cnt]), false);
			}, duration * channeldOrder switch { SweepChanneldOrder.LeftThenRight or SweepChanneldOrder.RightThenLeft => 2, _ => 1 }, sampleRate);
			return;
		}
	}

	public static async Task RegisterSweepAsync(double freqFrom, double freqTo, bool exp, double duration, double amp = 0.5, SweepChanneldOrder channeldOrder = SweepChanneldOrder.Both, int sampleRate = 44100, bool invertRight = false)
	{
		switch (channeldOrder)
		{
			case SweepChanneldOrder.Both:
				if (exp)
				{
					var freqFromLog = Math.Log(freqFrom);
					var freqToLog = Math.Log(freqTo);
					double freqDifLog = (freqToLog - freqFromLog) / duration;
					await AudioTest.Register((sample, actualSampleRate, channel) =>
					{
						double t = (double)(sample) / actualSampleRate;
						//Thanks, Wolfram!
						//https://www.wolframalpha.com/input?i2d=true&i=Integrate%5B2*pi*exp%5C%2840%29a%2Bbt%5C%2841%29%2C%7Bt%2C0%2Cu%7D%5D
						return (amp * (channel == 1 && invertRight ? -1 : 1) * Math.Sin(2.0 * Math.PI * Math.Exp(freqFromLog) * (Math.Exp(freqDifLog * t) - 1) / freqDifLog), false);
					}, duration, sampleRate);
				}
				else
				{
					double freqDif = (freqTo - freqFrom) / duration;
					await AudioTest.Register((sample, actualSampleRate, channel) =>
					{
						double t = (double)(sample) / actualSampleRate;
						//I can handle basic integral. But just in case...
						//https://www.wolframalpha.com/input?i2d=true&i=Integrate[a%2Bbt%2C{t%2C0%2Cu}]
						return (amp * (channel == 1 && invertRight ? -1 : 1) * Math.Sin(Math.PI * 2 * (freqFrom * t + freqDif * t * t / 2.0)), false);
					}, duration, sampleRate);
				}
				break;
			case SweepChanneldOrder.LeftThenRight:
			case SweepChanneldOrder.RightThenLeft:
				bool leftStart = channeldOrder == SweepChanneldOrder.LeftThenRight;
				if (exp)
				{
					var freqFromLog = Math.Log(freqFrom);
					var freqToLog = Math.Log(freqTo);
					double freqDifLog = (freqToLog - freqFromLog) / duration;
					await AudioTest.Register((sample, actualSampleRate, channel) =>
					{
						double t = (double)(sample) / actualSampleRate;
						bool isFirst = t < duration;
						t = isFirst ? t : t - duration;
						return ((isFirst ^ leftStart ^ (channel == 0) ? amp : 0) * (channel == 1 && invertRight ? -1 : 1) * Math.Sin(2.0 * Math.PI * Math.Exp(freqFromLog) * (Math.Exp(freqDifLog * t) - 1) / freqDifLog), false);
					}, duration * 2, sampleRate);
				}
				else
				{
					double freqDif = (freqTo - freqFrom) / duration;
					await AudioTest.Register((sample, actualSampleRate, channel) =>
					{
						double t = (double)(sample) / actualSampleRate;
						bool isFirst = t < duration;
						t = isFirst ? t : t - duration;
						return ((isFirst ^ leftStart ^ (channel == 0) ? amp : 0) * (channel == 1 && invertRight ? -1 : 1) * Math.Sin(Math.PI * 2 * (freqFrom * t + freqDif * t * t / 2.0)), false);
					}, duration * 2, sampleRate);

				}
				break;
		}
	}

	public static async Task RegisterWave(double frequency, double duration, double ampLeft, double ampRight, WaveKinds kinds = WaveKinds.Sine, int sampleRate = 44100)
	{
		double[] amplifications = new[] { ampLeft, ampRight };
		switch (kinds)
		{
			case WaveKinds.Sine:
			default:
				await RegisterWave(frequency, (sample, actualSampleRate, channel) =>
				{
					double t = (double)(sample) / actualSampleRate;
					return (amplifications[channel] * Math.Sin(2.0 * Math.PI * frequency * t), false);
				}, duration, ampLeft, ampRight, sampleRate);
				break;
			case WaveKinds.Square:
				await RegisterWave(frequency, (sample, actualSampleRate, channel) =>
				{
					double p1 = (sample * frequency) % actualSampleRate;
					double p2 = p1 / actualSampleRate;
					return (amplifications[channel] * (p2 < 0.5 ? 1 : -1), (int)p1 == 0);
				}, duration, ampLeft, ampRight, sampleRate);
				break;
			case WaveKinds.Sawtooth:
				await RegisterWave(frequency, (sample, actualSampleRate, channel) =>
				{
					double p1 = (sample * frequency) % actualSampleRate;
					double p2 = p1 / actualSampleRate;
					return (amplifications[channel] * (-1 + 2 * p2), (int)p1 == 0);
				}, duration, ampLeft, ampRight, sampleRate);
				break;
			case WaveKinds.Ramp:
				await RegisterWave(frequency, (sample, actualSampleRate, channel) =>
				{
					double p1 = (sample * frequency) % actualSampleRate;
					double p2 = p1 / actualSampleRate;

					//Sample 0 must be 0 for niceCuttingFrame.
					return (amplifications[channel] * (p2 switch
					{
						< 0.25 => -p2 * 4,
						< 0.75 => -2 + p2 * 4,
						_ => -p2 * 4 + 4
					}), (int)p1 == 0);
				}, duration, ampLeft, ampRight, sampleRate);
				break;
		}
	}

	public enum ShiftDirection
	{
		LeftToRight, RightToLeft
	}

	public enum WaveKinds
	{
		Sine, Square, Sawtooth, Ramp,
	}


	public static async Task RegisterSineWaveStereoShift(double frequency, double duration, ShiftDirection direction, int sampleRate = 44100)
	{
		if (frequency >= sampleRate)
		{
			//You can not play the audio above the sampleRate
			await AudioTest.Register((sample, samplerate, channel) =>
			{
				return (0, false);
			}, duration, sampleRate);

		}//We omit 22.05kHz operation here.
		else
		{
			int directionNumber = direction switch
			{
				ShiftDirection.LeftToRight => 1,
				ShiftDirection.RightToLeft => -1,
				_ => 0,
			};
			await AudioTest.Register((sample, actualSampleRate, channel) =>
			{
				double t = (double)(sample) / actualSampleRate;
				double d = Math.Cos(Math.PI * t / duration) / 2;
				return (channel switch
				{
					0 => 0.5 + d * directionNumber,
					1 => 0.5 - d * directionNumber,
					_ => 0,
				} * Math.Sin(2.0 * Math.PI * frequency * t), false);
			}, duration, sampleRate);
		}

	}

}
