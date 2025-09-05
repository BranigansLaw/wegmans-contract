using System;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Wegmans.POS.DataHub.Util
{
    /// <Summary>
    /// This static generic class is used serializing and deserializing objects
    /// </Summary>
    public static class SerializationUtil<T>
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(T));

        /// <Summary>
        /// Returns deserialized object from XElement input
        /// </Summary>
        public static T Deserialize(XElement doc)
        {
            using (var reader = doc.CreateReader())
            {
                return (T)_serializer.Deserialize(reader);
            }
        }

        /// <Summary>
        /// Returns serialized object as XDocument
        /// </Summary>
        public static XDocument Serialize(T value)
        {
            XDocument doc = new XDocument();
            using (var writer = doc.CreateWriter())
            {
                _serializer.Serialize(writer, value);
            }

            return doc;
        }
    }
}