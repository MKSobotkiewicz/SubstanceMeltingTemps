using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SubstanceMeltingTemps.Substance
{
    public sealed class MeasuredSubstance : BaseSubstance, IXmlSerializable
    {
        public float MeasuredMeltingTemperatureC { get; private set; } = 0;

        public MeasuredSubstance(string name, float nominalMeltingTemperatureC, float measuredMeltingTemperatureC) : base(name, nominalMeltingTemperatureC)
        {
            MeasuredMeltingTemperatureC = measuredMeltingTemperatureC;
        }

        public MeasuredSubstance(BaseSubstance baseSubstance, float measuredMeltingTemperatureC) : base(baseSubstance)
        {
            MeasuredMeltingTemperatureC = measuredMeltingTemperatureC;
        }

        public MeasuredSubstance(MeasuredSubstance measuredSubstance) :base(measuredSubstance.Name, measuredSubstance.NominalMeltingTemperatureC)
        {
            MeasuredMeltingTemperatureC = measuredSubstance.MeasuredMeltingTemperatureC;
        }

        public MeasuredSubstance() 
        {
        }

        public void Update(string name, float  mominalMeltingTemperatureC, float measuredMeltingTemperatureC)
        {
            Name = name;
            NominalMeltingTemperatureC = mominalMeltingTemperatureC;
            MeasuredMeltingTemperatureC = measuredMeltingTemperatureC;
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            Name = reader.GetAttribute("Name");
            NominalMeltingTemperatureC = float.Parse(reader.GetAttribute("NominalTemp"));
            MeasuredMeltingTemperatureC = float.Parse(reader.GetAttribute("MeasuredTemp"));
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Substance");
            writer.WriteAttributeString("Name", Name.ToString());
            writer.WriteAttributeString("NominalTemp", NominalMeltingTemperatureC.ToString());
            writer.WriteAttributeString("MeasuredTemp", MeasuredMeltingTemperatureC.ToString());
            writer.WriteEndElement();
        }
    }
}
