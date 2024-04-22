using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SubstanceMeltingTemps
{
    public class InstrumentsFile:List<Instrument>
    {
        public void ToXml(string path)
        {
            using (var xmlWriter = XmlWriter.Create(path)) 
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("InstrumentsList");
                foreach (var instrument in this) 
                {
                    instrument.WriteXml(xmlWriter);
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
        }

        public static InstrumentsFile FromXml(string path) 
        {
            var instrumentsFile = new InstrumentsFile();
            using (var xmlReader = XmlReader.Create(path))
            {
                xmlReader.MoveToContent();
                while (xmlReader.Read()) 
                {
                    if (xmlReader.Name == "Instrument") 
                    {
                        instrumentsFile.Add(new Instrument());
                        instrumentsFile.Last().ReadXml(xmlReader);
                    }
                }
            }
            return instrumentsFile;
        }

        public new bool Add(Instrument instrument) 
        {
            if (this.Where(v => v.Id == instrument.Id).Count() > 0) 
            {
                return false;
            }
            base.Add(instrument);
            return true;
        }
    }
}
