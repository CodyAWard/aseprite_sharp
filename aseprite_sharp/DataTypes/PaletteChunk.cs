using aseprite_sharp.Reader;

namespace aseprite_sharp.DataTypes
{
    public class PaletteEntry
    {
        public byte R{ get; }
        public byte G{ get; }
        public byte B{ get; }
        public byte A{ get; }
        public string Name{ get; }

        private PaletteEntry(byte r, byte g, byte b, byte a, string name)
        {
            R = r;
            G = g;
            B = b;
            A = a;
            Name = name;
        }

        public static PaletteEntry Read(AsepriteStreamReader reader, string defaultName)
        {
            //   WORD Entry flags:
            //             1 = Has name
            bool hasName = reader.FLAG(reader.WORD(), 1);
            //   BYTE      Red(0 - 255)
            var r = reader.BYTE();
            //   BYTE Green(0 - 255)
            var g = reader.BYTE();
            //   BYTE Blue(0 - 255)
            var b = reader.BYTE();
            //   BYTE Alpha(0 - 255)
            var a = reader.BYTE();
            //   +If has name bit in entry flags
            //     STRING Color name
            var name = hasName 
                ? reader.STRING() 
                : defaultName;

            return new PaletteEntry(r, g, b, a, name);
        }
    }

    public class PaletteChunk : IChunk
    {
        public PaletteEntry[] Entries { get; }

        private PaletteChunk(PaletteEntry[] entries)
        {
            Entries = entries;
        }

        public static PaletteChunk Read(AsepriteStreamReader reader)
        {
            // DWORD New palette size(total number of entries)
            var entryCount = reader.DWORD();
            // DWORD First color index to change
            var from = reader.DWORD();
            // DWORD Last color index to change
            var to = reader.DWORD();
            // BYTE[8]     For future(set to zero)
            reader.BYTES(8);
            // +For each palette entry in [from, to] range(to - from + 1 entries)
            var entries = new PaletteEntry[entryCount];
            for (int i = 0; i < (to - from) + 1; i++)
            {
                entries[i] = PaletteEntry.Read(reader, $"Color {i}");
            }
          
            return new PaletteChunk(entries);
        }
    }
}
