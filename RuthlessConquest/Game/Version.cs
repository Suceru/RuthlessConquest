using System;
using System.Collections.Generic;
using Engine.Serialization;

namespace Game
{
    public struct Version : IEquatable<Version>, IComparable<Version>
    {
        public byte Major
        {
            get
            {
                return (byte)(this.Value >> 24);
            }
            set
            {
                this.Value = ((this.Value & 16777215) | value << 24);
            }
        }

        public byte Minor
        {
            get
            {
                return (byte)(this.Value >> 16);
            }
            set
            {
                this.Value = ((this.Value & -16711681) | value << 16);
            }
        }

        public byte Build
        {
            get
            {
                return (byte)(this.Value >> 8);
            }
            set
            {
                this.Value = ((this.Value & -65281) | value << 8);
            }
        }

        public byte Revision
        {
            get
            {
                return (byte)this.Value;
            }
            set
            {
                this.Value = ((this.Value & -256) | value);
            }
        }

        public Version(byte[] bytes, int startIndex)
        {
            this.Value = BitConverter.ToInt32(bytes, startIndex);
        }

        public Version(int value)
        {
            this.Value = value;
        }

        public Version(byte major, byte minor, byte build, byte revision)
        {
            this.Value = (major << 24 | minor << 16 | build << 8 | revision);
        }

        public Version GetNetworkProtocolVersion()
        {
            return new Version(this.Major, this.Minor, 0, 0);
        }

        public bool Equals(Version other)
        {
            return this.Value == other.Value;
        }

        public byte[] ToByteArray()
        {
            return BitConverter.GetBytes(this.Value);
        }

        public override string ToString()
        {
            return this.ToString(4);
        }

        public string ToString(int components = 4)
        {
            if (components == 4)
            {
                return string.Format("{0}.{1}.{2}.{3}", new object[]
                {
                    this.Major,
                    this.Minor,
                    this.Build,
                    this.Revision
                });
            }
            if (components == 3)
            {
                return string.Format("{0}.{1}.{2}", this.Major, this.Minor, this.Build);
            }
            if (components == 2)
            {
                return string.Format("{0}.{1}", this.Major, this.Minor);
            }
            if (components == 1)
            {
                return string.Format("{0}", this.Major);
            }
            if (components == 0)
            {
                return "";
            }
            throw new InvalidOperationException();
        }

        public override bool Equals(object obj)
        {
            if (obj is Version)
            {
                Version other = (Version)obj;
                return this.Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Value;
        }

        public int CompareTo(Version other)
        {
            return Comparer<uint>.Default.Compare((uint)this.Value, (uint)other.Value);
        }

        public static bool operator ==(Version v1, Version v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Version v1, Version v2)
        {
            return !v1.Equals(v2);
        }

        public static bool operator <(Version v1, Version v2)
        {
            return v1.Value < v2.Value;
        }

        public static bool operator >(Version v1, Version v2)
        {
            return v1.Value > v2.Value;
        }

        public int Value;

        [HumanReadableConverter(typeof(Version))]
        private class VersionHumanReadableConverter : IHumanReadableConverter
        {
            public string ConvertToString(object value)
            {
                Version version = (Version)value;
                return HumanReadableConverter.ValuesListToString<byte>('.', new byte[]
                {
                    version.Major,
                    version.Minor,
                    version.Build,
                    version.Revision
                });
            }

            public object ConvertFromString(Type type, string data)
            {
                byte[] array = HumanReadableConverter.ValuesListFromString<byte>('.', data);
                if (array.Length == 2)
                {
                    return new Version
                    {
                        Major = array[0],
                        Minor = array[1]
                    };
                }
                if (array.Length == 3)
                {
                    return new Version
                    {
                        Major = array[0],
                        Minor = array[1],
                        Build = array[2]
                    };
                }
                if (array.Length == 4)
                {
                    return new Version
                    {
                        Major = array[0],
                        Minor = array[1],
                        Build = array[2],
                        Revision = array[3]
                    };
                }
                throw new Exception();
            }
        }
    }
}
