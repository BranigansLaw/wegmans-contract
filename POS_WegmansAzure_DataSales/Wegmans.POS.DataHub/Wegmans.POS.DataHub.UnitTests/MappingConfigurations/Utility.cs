using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Wegmans.POS.DataHub.UnitTests.MappingConfigurations
{
    internal static class Utility
    {
        private static readonly string[] TestFileList = new string[] { 
            "./TransactionTestCases/TestCase1.xml",
            "./TransactionTestCases/TestCase2.xml",
            "./TransactionTestCases/TestCase3.xml",
            "./TransactionTestCases/TestCase4.xml",
            "./TransactionTestCases/TestCase5.xml",
        };

        /// <summary>
        /// Assert that all same-name fields in <paramref name="obj1"/> and <paramref name="obj2"/> have the same values
        /// </summary>
        public static void AssertSameNameFieldsMatch(object obj1, object obj2, IEnumerable<string> ignoreFields)
        {
            Dictionary<string, PropertyInfo> obj1Fields = new(obj1.GetType().GetProperties().Select(p => KeyValuePair.Create(p.Name, p)));
            Dictionary<string, PropertyInfo> obj2Fields = new(obj2.GetType().GetProperties().Select(p => KeyValuePair.Create(p.Name, p)));
            ISet<string> ignoreFieldsSet = new HashSet<string>(ignoreFields);

            foreach (string sharedPropertyName in obj1Fields.Keys.Intersect(obj2Fields.Keys))
            {
                if (!ignoreFieldsSet.Contains(sharedPropertyName))
                {
                    Assert.Equal(obj1Fields[sharedPropertyName].GetValue(obj1), obj2Fields[sharedPropertyName].GetValue(obj2));
                }
            }
        }

        public static IEnumerable<T> GetTransactionDataFromXml<T>(string stringType, string? originator = null, string? subtype = null, string? subStringType = null)
        {
            ICollection<T> transactionData = new List<T>();
            XmlSerializer serializer = new(typeof(T));

            foreach (string filePath in TestFileList)
            {
                XElement? root = XDocument.Load(filePath).Root;

                if (root is not null)
                {
                    string xPath = $"./TransactionRecord[./StringType = '{stringType}']{(originator != null ? $"[./Originator = '{originator}']" : "")}{(subtype != null ? $"[./SubType = '{subtype}']" : "")}{(subStringType != null ? $"[./SubStringType = '{subStringType}']" : "")}";

                    IEnumerable<XElement> stringType00Transactions = root.XPathSelectElements(xPath);

                    foreach (XElement elem in stringType00Transactions)
                    {
                        string toRead = elem.ToString();
                        toRead = toRead.Replace("TransactionRecord", $"TransactionRecord_{stringType}{(originator != null ? $".{originator}" : "")}{(subtype != null ? $".{subtype}" : "")}{(subStringType != null ? $".{subStringType}" : "")}");
                        using (TextReader reader = new StringReader(toRead))
                        {
                            transactionData.Add((T)(serializer.Deserialize(reader) ?? throw new Exception("Deserialization of XML failed")));
                        }
                    }
                }
            }

            return transactionData;
        }
    }
}
