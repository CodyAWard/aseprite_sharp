using System.IO;
using System.IO.Compression;
using System.Text;

namespace aseprite_sharp.Reader
{
    /// <summary>
    /// Allows for reading the stream via the documentations methodology
    /// https://github.com/aseprite/aseprite/blob/master/docs/ase-file-specs.md
    /// </summary>
    public class AsepriteStreamReader
    {
        private readonly BinaryReader reader;

        public long POS
        {
            get => reader.BaseStream.Position;
            set => reader.BaseStream.Position = value;
        }

        public AsepriteStreamReader(Stream stream)
        {
            reader = new BinaryReader(stream);
        }

        /// <summary>
        /// BYTE: An 8-bit unsigned integer value
        /// </summary>
        public byte BYTE() { return reader.ReadByte(); }
        /// <summary>
        /// WORD: A 16-bit unsigned integer value
        /// </summary>
        public ushort WORD() { return reader.ReadUInt16(); }
        /// <summary>
        /// SHORT: A 16-bit signed integer value
        /// </summary>
        public short SHORT() { return reader.ReadInt16(); }
        /// <summary>
        /// DWORD: A 32-bit unsigned integer value
        /// </summary>
        public uint DWORD() { return reader.ReadUInt32(); }
        /// <summary>
        /// LONG: A 32-bit signed integer value
        /// </summary>
        public int LONG() { return reader.ReadInt32(); }
        /// <summary>
        /// FIXED: A 32-bit fixed point (16.16) value
        /// </summary>
        public float FIXED() { return reader.ReadSingle(); }
        /// <summary>
        ///  STRING:
        ///         WORD: string length(number of bytes)
        ///         BYTE[length]: characters(in UTF-8) The '\0' character is not included.
        /// </summary>
        public string STRING() { return Encoding.UTF8.GetString(BYTES(WORD())); }
        /// <summary>
        /// BYTE[n]: "n" bytes.
        /// </summary>
        public byte[] BYTES(int number) { return reader.ReadBytes(number); }

        public void SEEK(int number) { reader.BaseStream.Position += number; }

        public bool FLAG(uint value, int flag) => FLAG((int)value, flag);
        public bool FLAG(int value, int flag)
        {
            return (value & (1 << flag)) != 0;
        }

        public int DEFLATE(byte[] decompressInto)
        {
            SEEK(2);
            var deflateStream = new DeflateStream(reader.BaseStream, CompressionMode.Decompress);
            return deflateStream.Read(decompressInto, 0, decompressInto.Length);
        }
    }
}