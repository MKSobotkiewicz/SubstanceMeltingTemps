using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SubstanceMeltingTemps.Substance
{
    public class BaseSubstance
    {
        public string Name { get; protected set; } = "";
        public float NominalMeltingTemperatureC { get; protected set; } = 0;

        [XmlIgnore]
        public static readonly List<BaseSubstance> PredefinedSubstances = new List<BaseSubstance> {
            new BaseSubstance("H2O",0f),
            new BaseSubstance("Ga",29.8f),
            new BaseSubstance("Biphenyl",69.2f),
            new BaseSubstance("Benzoeacid",122.4f),
            new BaseSubstance("KNO3",128.7f),
            new BaseSubstance("In",156.6f),
            new BaseSubstance("RbNO3(trig>kub)",164.2f),
            new BaseSubstance("Sn",231.9f),
            new BaseSubstance("Bi",271.4f),
            new BaseSubstance("KClO4",300.8f)
        };

        public BaseSubstance(string name, float nominalMeltingTemperatureC)
        {
            Name = name;
            NominalMeltingTemperatureC = nominalMeltingTemperatureC;
        }

        public BaseSubstance(BaseSubstance baseSubstance)
        {
            Name = baseSubstance.Name;
            NominalMeltingTemperatureC = baseSubstance.NominalMeltingTemperatureC;
        }
        protected BaseSubstance()
        {
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
