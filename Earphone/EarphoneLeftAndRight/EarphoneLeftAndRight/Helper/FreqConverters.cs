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

        public enum SemitoneLocalizeModes
        {
            Both, International, Local, LocalAlt
        }

        public const string SemitoneInternational = "C,C#,D,D#,E,F,F#,G,G#,A,A#,B";
        public static double[] JustIntonationTables => new double[]
        {
            1,//C
            1*5.0/1.0,//C#
            9.0/8.0,//D
            9.0/8.0*5.0/4.0,//D#
            5.0/4.0,//E
            4.0/3.0,//F
            4.0/3.0*5.0/4.0,//F#
            3.0/2.0,//G
            3.0/2.0*5.0/4.0,//G#
            5.0/3.0,//A
            5.0/3.0*5.0/4.0,//A#
            15.0/8.0,
        };

        public static bool SemitoneLocalizationSupported(string text) => text != SemitoneInternational && text != "NULL";

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
            const string centInternational = "{0} cent(s)";
            switch (mode)
            {
                case SemitoneLocalizeModes.Both:
                case SemitoneLocalizeModes.International:
                default:
                    return convertCent(cent, centInternational);
                case SemitoneLocalizeModes.Local:
                case SemitoneLocalizeModes.LocalAlt:
                    return convertCent(cent, Resx.AppResources.Helper_Semitone_Cent);
            }
        }

        public static (string semitone, string cent) HzToLocalizedEqualTemperament(double hz, SemitoneLocalizeModes mode)
        {
            var (oct, semi, cent) = HzToOctaveEqualTemperament(hz);
            return OctaveToLocalized(oct, semi, cent, mode);
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
