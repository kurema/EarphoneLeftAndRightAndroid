using System;

namespace EarphoneLeftAndRight.Helper
{
    public static class FreqConverters
    {

        public static double HzToNoteNumberEqualTemperament(double hz)
        {
            return Math.Log(hz / 440, 2) * 12 + 69;
        }

        public static double NoteNumberToHzEqualTemperament(double noteNumber)
        {
            return Math.Pow(2.0, (noteNumber - 69) / 12.0) * 440;
        }

        public static (int octave, int semitone, double cent) HzToOctaveEqualTemperament(double hz)
        {
            var note = HzToNoteNumberEqualTemperament(hz) - 11.5;
            var oct = (int)Math.Floor(note / 12.0);
            return (oct, (int)Math.Floor(note - oct * 12), ((note % 1) + 1) % 1 * 100 - 50);
        }

        public static (int octave, int semitone, double cent, double noteNumber) HzToOctaveJustIntonation(double hz)
        {
            //need testing
            double baseHz = 440 / JustIntonationTables[9];
            int octave = (int)(Math.Floor(Math.Log(hz / baseHz / JustIntonationTablesMiddle[0], 2)) + 4);
            double relativeToOctaveBase = hz / (Math.Pow(2, octave - 4) * baseHz);
            int semitone;
            for (semitone = 0; semitone < JustIntonationTablesMiddle.Length - 1;)
            {
                if (JustIntonationTablesMiddle[semitone + 1] >= relativeToOctaveBase) break;
                semitone++;
            }
            double semiFraction = relativeToOctaveBase / JustIntonationTables[semitone];
            double cent = Math.Log(semiFraction, 2) * 12 * 100;
            double noteNumberFraction;
            if (semiFraction >= 0)
            {
                noteNumberFraction = Math.Log(semiFraction, (JustIntonationTables.Length - 1 == semitone ? 2 : JustIntonationTables[semitone + 1]) / JustIntonationTables[semitone]);
            }
            else
            {
                noteNumberFraction = Math.Log(semiFraction, JustIntonationTables[semitone] / (semitone == 0 ? JustIntonationTables[^1] / 2.0 : JustIntonationTables[semitone - 1]));
            }
            return (octave, semitone, cent, octave * 12 + semitone + 12 + noteNumberFraction);
        }

        public static double OctaveToHzJustIntonation(int octave, int semitone, double cent)
        {
            double baseHz = 440 / JustIntonationTables[9];
            int octaveShift = (int)Math.Floor(semitone / 12.0);
            octave += octaveShift;
            semitone -= octaveShift * 12;
            double hz = baseHz * Math.Pow(2, octave - 4);
            hz *= JustIntonationTables[semitone];
            hz *= Math.Pow(2, cent/12.0/100);
            return hz;
        }

        public enum SemitoneLocalizeModes
        {
            Both, International, Local, LocalAlt
        }

        public const string SemitoneInternational = "C,C#,D,D#,E,F,F#,G,G#,A,A#,B";
        private static double[] _JustIntonationTablesMiddle;
        public static double[] JustIntonationTablesMiddle
        {
            get
            {
                if (_JustIntonationTablesMiddle is not null) return _JustIntonationTablesMiddle;
                var result = new double[JustIntonationTables.Length];
                result[0] = Math.Sqrt(JustIntonationTables[^1] / 2.0);
                for (int i = 1; i < JustIntonationTables.Length; i++)
                {
                    result[i] = Math.Sqrt(JustIntonationTables[i] * JustIntonationTables[i - 1]);
                }
                return _JustIntonationTablesMiddle = result;
            }
        }

        private static double[] _JustIntonationTables;
        public static double[] JustIntonationTables => _JustIntonationTables ??= new double[]
        {
            //http://www.enjoy.ne.jp/~k-ichikawa/junseiritsu2.html : A# may be wrong.
            //http://www.takuichi.net/hobby/edu/sonic_wave/temperament_just_intonation/temp_just.pdf
            //https://tabatalabo.com/%E9%9F%B3%E5%BE%8B%E3%81%A8%E3%81%AF%EF%BC%9F
            1,//C
            16.0/15.0,//C#
            9.0/8.0,//D
            6.0/5.0,//D#
            5.0/4.0,//E
            4.0/3.0,//F
            45.0/32.0,//F#
            3.0/2.0,//G
            8.0/5.0,//G#
            5.0/3.0,//A
            16.0/9.0,//A#
            15.0/8.0,//B
        };

        public static bool SemitoneLocalizationSupported(string text) => text != SemitoneInternational && text != "NULL";
        public static bool CentLocalizationSupported => CentInternational != Resx.AppResources.Helper_Semitone_Cent;
        public const string CentInternational = "{0} cent(s)";

        public static string LocalizeCent(double cent, SemitoneLocalizeModes mode)
        {
            static string convertCent(double cent, string text)
            {
                var round = Math.Round(cent);
                const string Format = "+#;-#;+0;";
                if (-1.0 <= round && round <= 1.0)
                {
                    text = System.Text.RegularExpressions.Regex.Replace(text, @"\(.+\)", "");
                    text = text.Replace("[", "").Replace("]", "");
                    return string.Format(text, round.ToString(Format));
                }
                else
                {
                    text = System.Text.RegularExpressions.Regex.Replace(text, @"\[.+\]", "");
                    text = text.Replace("(", "").Replace(")", "");
                    return string.Format(text, round.ToString(Format));
                }
            }
            switch (mode)
            {
                case SemitoneLocalizeModes.Both:
                case SemitoneLocalizeModes.International:
                default:
                    return convertCent(cent, CentInternational);
                case SemitoneLocalizeModes.Local:
                case SemitoneLocalizeModes.LocalAlt:
                    return convertCent(cent, Resx.AppResources.Helper_Semitone_Cent);
            }
        }

        public static (string semitone, string cent) HzToLocalized(double hz, SemitoneLocalizeModes mode, bool justIntonation = false)
        {
            if (justIntonation)
            {
                var (oct, semi, cent, _) = HzToOctaveJustIntonation(hz);
                return OctaveToLocalized(oct, semi, cent, mode);
            }
            else
            {
                var (oct, semi, cent) = HzToOctaveEqualTemperament(hz);
                return OctaveToLocalized(oct, semi, cent, mode);
            }
        }

        public static (string semitone, string cent) OctaveToLocalized(int octave, int semitone, double cent, SemitoneLocalizeModes mode)
        {
            string[] names = SemitoneInternational.Split(',');
            string semitoneText;
            switch (mode)
            {
                case SemitoneLocalizeModes.Both:
                default:
                    if (SemitoneLocalizationSupported(Resx.AppResources.Helper_Semitone_Main))
                    {
                        string[] namesLocal = Resx.AppResources.Helper_Semitone_Main.Split(',');
                        semitoneText = $"{names[semitone]}{octave} / {namesLocal[semitone]}{octave}";
                    }
                    else
                    {
                        semitoneText = $"{names[semitone]}{octave}";
                    }
                    break;
                case SemitoneLocalizeModes.International:
                    semitoneText = $"{names[semitone]}{octave}";
                    break;
                case SemitoneLocalizeModes.Local:
                    {
                        string[] namesLocal = Resx.AppResources.Helper_Semitone_Main.Split(',');
                        if (namesLocal.Length >= 12) semitoneText = $"{namesLocal[semitone]}{octave}"; else semitoneText = $"{names[semitone]}{octave}";
                    }
                    break;
                case SemitoneLocalizeModes.LocalAlt:
                    {
                        string[] namesLocal = Resx.AppResources.Helper_Semitone_Alt.Split(',');
                        if (namesLocal.Length >= 12) semitoneText = $"{namesLocal[semitone]}{octave}"; else semitoneText = $"{names[semitone]}{octave}";
                    }
                    break;
            }
            return ($"{semitoneText}", LocalizeCent(cent, mode));
        }
    }
}
