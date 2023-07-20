// http://accord-framework.net/
using System;
using System.Numerics;


namespace Project1.Process
{
    public class Transformate
    {
        private static float MULTILIER_8BIT = 1f / 128f;
        //private static float MULTILIER_16BIT = 1f / 32768f;

        /// <summary>
        /// Konversja byte[] na Complex[] ( Complex - liczba zespolona )
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Complex[] ByteToComplex(byte[] bytes)
        {
            Complex[] fftComplex = new Complex[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
                fftComplex[i] = new Complex((bytes[i] - 128) * MULTILIER_8BIT, 0.0);
            return fftComplex;
        }
        /// <summary>
        /// Zwraca wartość mocy dla szukanej częstotliwości
        /// Czym większa wartość tym czestotliwość próbej jest bliżej do szukanej
        /// </summary>
        /// <param name="samples">tablica próbek</param>
        /// <param name="freq">czestotliwość porównywana</param>
        /// <param name="sampleRate">częstotliwość próbki</param>
        /// <returns></returns>
        public static double GoertzelFilter(float[] samples, double freq, int sampleRate)
        {
            double sPrev = 0.0;
            double sPrev2 = 0.0;
            int i;
            double normalizedfreq = freq / sampleRate;
            double coeff = 2 * Math.Cos(2 * Math.PI * normalizedfreq);
            for (i = 0; i < samples.Length; i++)
            {
                double s = samples[i] + coeff * sPrev - sPrev2;
                sPrev2 = sPrev;
                sPrev = s;
            }
            return sPrev2 * sPrev2 + sPrev * sPrev - coeff * sPrev * sPrev2;
        }
        /// <summary>
        /// Discrete Fourier Transformations
        /// -> dużo wolniejsze od FFT
        /// </summary>
        /// <param name="data"></param>

        private static int Reverse(int i)
        {
            i = (i & 0x55555555) << 1 | (int)((uint)i >> 1) & 0x55555555;
            i = (i & 0x33333333) << 2 | (int)((uint)i >> 2) & 0x33333333;
            i = (i & 0x0f0f0f0f) << 4 | (int)((uint)i >> 4) & 0x0f0f0f0f;
            i = i << 24 | (i & 0xff00) << 8 | (int)((uint)i >> 8) & 0xff00 | (int)((uint)i >> 24);
            return i;
        }
        /// <summary>
        /// Fast Fourier Transformations
        /// </summary>
        /// <param name="data"></param>
        public static void FFT(Complex[] complex)
        {
            int n = complex.Length;

            int levels = (int)Math.Floor(Math.Log(n, 2));

            if (1 << levels != n)
                throw new ArgumentException("Length is not a power of 2");

            var cosTable = new double[n / 2];
            var sinTable = new double[n / 2];
            for (int i = 0; i < n / 2; i++)
            {
                cosTable[i] = Math.Cos(2 * Math.PI * i / n);
                sinTable[i] = Math.Sin(2 * Math.PI * i / n);
            }

            for (int i = 0; i < complex.Length; i++)
            {
                int j = unchecked((int)((uint)Reverse(i) >> 32 - levels));

                if (j > i)
                {
                    var temp = complex[i];
                    complex[i] = complex[j];
                    complex[j] = temp;
                }
            }

            for (int size = 2; size <= n; size *= 2)
            {
                int halfsize = size / 2;
                int tablestep = n / size;

                for (int i = 0; i < n; i += size)
                {
                    for (int j = i, k = 0; j < i + halfsize; j++, k += tablestep)
                    {
                        int h = j + halfsize;
                        double re = complex[h].Real;
                        double im = complex[h].Imaginary;

                        double tpre = +re * cosTable[k] + im * sinTable[k];
                        double tpim = -re * sinTable[k] + im * cosTable[k];

                        double rej = complex[j].Real;
                        double imj = complex[j].Imaginary;

                        complex[h] = new Complex(rej - tpre, imj - tpim);
                        complex[j] = new Complex(rej + tpre, imj + tpim);
                    }
                }
                if (size == n)
                    break;
            }
        }

        public static double GetMaxFrequencyVector(Complex[] fft, int sampleRate)
        {
            int n = (int)Math.Ceiling((fft.Length + 1) / 2.0);

            var v = fft[0].SquaredMagnitude() / fft.Length;
            var vi = 0;

            for (int i = 1; i < n; i++)
            {
                var tv = fft[i].SquaredMagnitude() * 2.0 / fft.Length;
                if (tv > v)
                {
                    v = tv;
                    vi = i;
                }
            }

            return vi * sampleRate / (double)fft.Length;
        }

        public static double[] GetPowerSpectrum(Complex[] fft)
        {
            if (fft == null)
                throw new ArgumentNullException("fft");

            int n = (int)Math.Ceiling((fft.Length + 1) / 2.0);

            double[] mx = new double[n];

            mx[0] = fft[0].SquaredMagnitude() / fft.Length;

            for (int i = 1; i < n; i++)
                mx[i] = fft[i].SquaredMagnitude() * 2.0 / fft.Length;

            return mx;
        }
    }

    public static class ComplexExtenstion
    {
        public static double SquaredMagnitude(this Complex value)
            => value.Magnitude * value.Magnitude;
    }
}
