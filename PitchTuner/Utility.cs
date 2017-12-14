using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PitchTuner
{
    public static class Utility
    {
        static double LogNote(double freq)
        {
            double oct = (Math.Log(freq) - Math.Log(261.626)) / Math.Log(2) + 4.0;
            return oct;
        }

        public static void FreqToNote(double freq, out string note, out double cents)
        {
            const int NOTE_COUNT = 12;
            double lnote = LogNote(freq);
            double oct = Math.Floor(lnote);            
            const string note_table = "C C#D D#E F F#G G#A A#B ";
            cents = 1200 * (lnote - oct);
            note = null;
            double offset = 50.0;
            int x = 2;
            StringBuilder sbNote = new StringBuilder(3);         
   
            if (cents < 50)
            {
                sbNote.Append("C ");
            }
            else if (cents >= 1150)
            {
                sbNote.Append("C ");
                cents -= 1200;
                oct++;
            }
            else
            {
                for (int i = 1; i < NOTE_COUNT; i++)
                {
                    if (cents >= offset && cents < (offset + 100))
                    {
                        sbNote.Append(note_table[x]);
                        sbNote.Append(note_table[x + 1]);
                        cents -= (i * 100);
                        break;
                    }
                    offset += 100;
                    x += 2;
                }
            }

            cents = Math.Round(cents, 2);
            sbNote.Append(oct);
            note = sbNote.ToString();
        }
    }
}
