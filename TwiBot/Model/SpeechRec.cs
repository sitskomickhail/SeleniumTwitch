using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;
using NAudio.Wave;
using System.Threading.Tasks;

namespace TwiBot.Model
{
    public static class SpeechRec
    {
        private static SpeechRecognitionEngine sre;
        private static Grammar grammar;

        static SpeechRec()
        {
            sre = new SpeechRecognitionEngine();
            grammar = new DictationGrammar();
        }

        public static string ConvertMp3ToWavAndRecover(string _inPath_, string _outPath_)
        {
            using (Mp3FileReader mp3 = new Mp3FileReader(_inPath_))
            {
                using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                { WaveFileWriter.CreateWaveFile(_outPath_, pcm); }
            }
            //grammar.Priority = 100;
            sre.LoadGrammar(grammar);
            sre.SetInputToWaveFile(_outPath_);
            sre.BabbleTimeout = new TimeSpan(Int32.MaxValue);
            sre.InitialSilenceTimeout = new TimeSpan(Int32.MaxValue);
            sre.EndSilenceTimeout = new TimeSpan(100000000);
            sre.EndSilenceTimeoutAmbiguous = new TimeSpan(100000000);

            StringBuilder sb = new StringBuilder();
            while (true)
            {
                try
                {
                    var recText = sre.Recognize();
                    if (recText == null)
                    {
                        break;
                    }

                    sb.Append(recText.Text);
                }
                catch (Exception ex)
                {
                    break;
                }
            }
            return sb.ToString();
        }
    }
}
