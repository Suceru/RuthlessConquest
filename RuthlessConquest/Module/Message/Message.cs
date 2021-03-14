using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Engine.Serialization;

namespace Game
{
    internal abstract class Message
    {
        static Message()
        {
            TypeInfo[] array = (from t in typeof(Message).Assembly.DefinedTypes
                                where typeof(Message).IsAssignableFrom(t) && !t.IsAbstract
                                orderby t.Name
                                select t).ToArray<TypeInfo>();
            for (int i = 0; i < array.Length; i++)
            {
                MessageTypesByMessageId[i] = array[i];
                MessageIdsByMessageTypes[array[i]] = i;
                Archive.SetTypeSerializationOptions(array[i], false, true);
            }
        }

        public static Message Read(byte[] bytes)
        {
            Message result;
            try
            {
                InputArchive inputArchive;
                if (bytes[0] == 60)
                {
                    inputArchive = new XmlInputArchive(XElement.Parse(Encoding.UTF8.GetString(bytes)), 0);
                }
                else
                {
                    inputArchive = new BinaryInputArchive(new MemoryStream(bytes), 0)
                    {
                        Use7BitInts = true
                    };
                }
                uint num = 0U;
                inputArchive.Serialize("MagicNumber", ref num);
                if (num != MagicNumber)
                {
                    throw new InvalidOperationException(string.Format("Invalid network message magic number {0:X}.", num));
                }
                uint value = 0U;
                inputArchive.Serialize("Version", ref value);
                Version receivedVersion = new Version((int)value);
                if (receivedVersion.Major != VersionsManager.Version.Major || receivedVersion.Minor != VersionsManager.Version.Minor)
                {
                    throw new InvalidOperationException(string.Format("Invalid network message version (version received {0}.{1}, version expected {2}.{3}).", new object[]
                    {
                        receivedVersion.Major,
                        receivedVersion.Minor,
                        VersionsManager.Version.Major,
                        VersionsManager.Version.Minor
                    }));
                }
                byte b = inputArchive.Serialize<byte>("MessageId");
                Type type;
                if (!MessageTypesByMessageId.TryGetValue(b, out type))
                {
                    throw new InvalidOperationException(string.Format("Unknown message id {0}.", b));
                }
                Message message = (Message)inputArchive.Serialize(type.Name, type);
                message.ReceivedVersion = receivedVersion;
                if (LogNetworkMessages)
                {
                    XmlInputArchive xmlInputArchive = inputArchive as XmlInputArchive;
                }
                result = message;
            }
            catch (Exception innerException)
            {
                throw new InvalidOperationException("Received malformed network message.", innerException);
            }
            return result;
        }

        public static byte[] Write(Message message)
        {
            OutputArchive outputArchive;
            if (UseXmlSerialization)
            {
                outputArchive = new XmlOutputArchive(new XElement("Message"), 0);
            }
            else
            {
                outputArchive = new BinaryOutputArchive(new MemoryStream(), 0)
                {
                    Use7BitInts = true
                };
            }
            outputArchive.Serialize("MagicNumber", MagicNumber);
            outputArchive.Serialize("Version", (uint)VersionsManager.Version.Value);
            outputArchive.Serialize("MessageId", (byte)MessageIdsByMessageTypes[message.GetType()]);
            outputArchive.Serialize(message.GetType().Name, message.GetType(), message);
            byte[] result;
            if (UseXmlSerialization)
            {
                result = Encoding.UTF8.GetBytes(((XmlOutputArchive)outputArchive).Node.ToString());
            }
            else
            {
                result = ((MemoryStream)((BinaryOutputArchive)outputArchive).Stream).ToArray();
            }
            if (LogNetworkMessages)
            {
                XmlOutputArchive xmlOutputArchive = outputArchive as XmlOutputArchive;
            }
            return result;
        }

        private static uint MakeFourCC(string s)
        {
            return s[0] + ((uint)s[1] << 8) + ((uint)s[2] << 16) + ((uint)s[3] << 24);
        }

        private static uint MagicNumber = MakeFourCC("RtCq");

        private static Dictionary<int, Type> MessageTypesByMessageId = new Dictionary<int, Type>();

        private static Dictionary<Type, int> MessageIdsByMessageTypes = new Dictionary<Type, int>();

        public static bool UseXmlSerialization = false;

        public static bool LogNetworkMessages = false;

        public Version ReceivedVersion;
    }
}
