using SubstanceMeltingTemps.Substance;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SubstanceMeltingTemps
{
    public class Instrument:List<MeasuredSubstance>, IXmlSerializable
    {
        public int Id { get; private set; } = -1;
        public float NominalValuesStandardDeviation { get; private set; } = 0;
        public float MeasuredValuesStandardDeviation { get; private set; } = 0;
        public float Covariance { get; private set; } = 0;
        public float CorrelationCoefficient { get; private set; } = 0;
        public float Alpha { get; private set; } = 0;
        public float Beta { get; private set; } = 0;

        [XmlIgnore]
        public static readonly Instrument TestInstrument = new(new List<MeasuredSubstance>{
            new MeasuredSubstance("H2O",0f,0.1f),
            new MeasuredSubstance("Ga",29.8f,31.6f),
            new MeasuredSubstance("Biphenyl",69.2f, 70.5f),
            new MeasuredSubstance("Benzoeacid",122.4f,127.6f),
            new MeasuredSubstance("KNO3",128.7f, 126.5f),
            new MeasuredSubstance("In",156.6f, 156.5f),
            new MeasuredSubstance("RbNO3(trig>kub)",164.2f, 162.5f),
            new MeasuredSubstance("Sn",231.9f, 236.7f),
            new MeasuredSubstance("Bi",271.4f, 269.5f),
            new MeasuredSubstance("KClO4",300.8f, 299.3f) 
        });

        public Instrument(IEnumerable<MeasuredSubstance> collection, int id = 1) :base(collection)
        {
            CheckId(id);
            Id = id;
            Update();
        }

        public Instrument(int id = 1) : base()
        {
            CheckId(id);
            Id = id;
            Update();
        }

        private Instrument() 
        {
        }

        public new void Add(MeasuredSubstance measuredSubstance)
        {
            base.Add(measuredSubstance);
            Update();
        }
        public new void Remove(MeasuredSubstance measuredSubstance)
        {
            base.Remove(measuredSubstance);
            Update();
        }

            public void Update()
        {
            var nominalValues = this.Select(v => v.NominalMeltingTemperatureC).ToList();
            var measuredValues = this.Select(v => v.MeasuredMeltingTemperatureC).ToList();
            if (nominalValues.Count == 0 || measuredValues.Count == 0) 
            {
                return;
            }
            NominalValuesStandardDeviation = Utilities.StandardDeviation(nominalValues);
            MeasuredValuesStandardDeviation = Utilities.StandardDeviation(measuredValues);
            Covariance = Utilities.Covariance(nominalValues,measuredValues);
            CorrelationCoefficient = Covariance / (NominalValuesStandardDeviation* MeasuredValuesStandardDeviation);
            Beta = CorrelationCoefficient * MeasuredValuesStandardDeviation / NominalValuesStandardDeviation;
            Alpha = measuredValues.Average() - Beta * nominalValues.Average();
        }

        public float LinearRegression(float x) 
        {
            return Beta* x +Alpha;
        }

        private static void CheckId(int id) 
        {
            if (id <= 0)
            {
                throw new ArgumentException("Instrument id can't be less that 1");
            }
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            while (reader.Read())
            {
                if (reader.Name == "Substance")
                {
                    Add(new MeasuredSubstance());
                    this.Last().ReadXml(reader);
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Instrument");
            writer.WriteAttributeString("Id", Id.ToString());
            foreach (var value in this) 
            {
                value.WriteXml(writer);
            }
            writer.WriteEndElement();
        }
    }
}
