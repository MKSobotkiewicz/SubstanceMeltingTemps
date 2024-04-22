using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstanceMeltingTemps.Substance
{
    internal static class Utilities
    {
        public static float CtoF(float temperatureInC) 
        {
            return temperatureInC * 1.8f + 32f;
        }

        public static float FtoC(float temperatureInF)
        {
            return ((temperatureInF -32)*5f)/9f;
        }
        public static float StandardDeviation(IEnumerable<float> values)
        {
            var average = values.Average();
            return (float)Math.Sqrt(values.Average(v => (float)Math.Pow(v - average, 2f)));
        }

        public static float Covariance(IEnumerable<float> valuesX, IEnumerable<float> valuesY) 
        {
            if (valuesX.Count() != valuesY.Count()) 
            {
                throw new ArgumentException("valuesX and caluesY lengths can't be diffrent");
            }
            var averageX = valuesX.Average();
            var averageY = valuesY.Average();
            return  (valuesX.Select(v => v - averageX).Zip(valuesY.Select(v => v - averageY),(x, y) => Tuple.Create(x, y)).Select(v=>v.Item1*v.Item2)).Sum()/ valuesX.Count();
        }
    }
}
