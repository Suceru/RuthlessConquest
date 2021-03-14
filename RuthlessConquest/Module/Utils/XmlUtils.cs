using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Engine.Serialization;

namespace Game
{
    public static class XmlUtils
    {
        public static object GetAttributeValue(XElement node, string attributeName, Type type)
        {
            object attributeValue = GetAttributeValue(node, attributeName, type, null);
            if (attributeValue != null)
            {
                return attributeValue;
            }
            throw new Exception(string.Format("Required XML attribute \"{0}\" not found in node \"{1}\".", attributeName, node.Name));
        }

        public static object GetAttributeValue(XElement node, string attributeName, Type type, object defaultValue)
        {
            XAttribute xattribute = node.Attribute(attributeName);
            if (xattribute != null)
            {
                try
                {
                    return HumanReadableConverter.ConvertFromString(type, xattribute.Value);
                }
                catch (Exception)
                {
                }
                return defaultValue;
            }
            return defaultValue;
        }

        public static T GetAttributeValue<T>(XElement node, string attributeName)
        {
            return (T)GetAttributeValue(node, attributeName, typeof(T));
        }

        public static T GetAttributeValue<T>(XElement node, string attributeName, T defaultValue)
        {
            return (T)GetAttributeValue(node, attributeName, typeof(T), defaultValue);
        }

        public static void SetAttributeValue(XElement node, string attributeName, object value)
        {
            string value2 = HumanReadableConverter.ConvertToString(value);
            XAttribute xattribute = node.Attribute(attributeName);
            if (xattribute != null)
            {
                xattribute.Value = value2;
                return;
            }
            node.Add(new XAttribute(attributeName, value2));
        }

        public static XElement FindChildElement(XElement node, string elementName, bool throwIfNotFound)
        {
            XElement xelement = node.Elements(elementName).FirstOrDefault<XElement>();
            if (xelement != null)
            {
                return xelement;
            }
            if (throwIfNotFound)
            {
                throw new Exception(string.Format("Required XML element \"{0}\" not found in node \"{1}\".", elementName, node.Name));
            }
            return null;
        }

        public static XElement AddElement(XElement parentNode, string name)
        {
            XElement xelement = new XElement(name);
            parentNode.Add(xelement);
            return xelement;
        }

        public static XElement LoadXmlFromTextReader(TextReader textReader, bool throwOnError)
        {
            XElement result;
            using (XmlReader xmlReader = XmlReader.Create(textReader, new XmlReaderSettings
            {
                CheckCharacters = false,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true
            }))
            {
                result = XElement.Load(xmlReader, LoadOptions.None);
            }
            return result;
        }

        public static XElement LoadXmlFromStream(Stream stream, Encoding encoding, bool throwOnError)
        {
            XElement result;
            using (StreamReader streamReader = (encoding != null) ? new StreamReader(stream, encoding) : new StreamReader(stream, true))
            {
                result = LoadXmlFromTextReader(streamReader, throwOnError);
            }
            return result;
        }

        public static XElement LoadXmlFromString(string data, bool throwOnError)
        {
            XElement result;
            using (StringReader stringReader = new StringReader(data))
            {
                result = LoadXmlFromTextReader(stringReader, throwOnError);
            }
            return result;
        }

        public static void SaveXmlToTextWriter(XElement node, TextWriter textWriter, bool throwOnError)
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                Encoding = Encoding.UTF8,
                CloseOutput = true
            }))
            {
                node.Save(xmlWriter);
                xmlWriter.Flush();
            }
        }

        public static void SaveXmlToStream(XElement node, Stream stream, Encoding encoding, bool throwOnError)
        {
            using (TextWriter textWriter = (encoding != null) ? new StreamWriter(stream, encoding) : new StreamWriter(stream))
            {
                SaveXmlToTextWriter(node, textWriter, throwOnError);
            }
        }

        public static string SaveXmlToString(XElement node, bool throwOnError)
        {
            string result;
            using (StringWriter stringWriter = new StringWriter())
            {
                SaveXmlToTextWriter(node, stringWriter, throwOnError);
                result = stringWriter.ToString();
            }
            return result;
        }
    }
}
