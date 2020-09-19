namespace aseprite_sharp
{
    public class UserDataChunk : IChunk
    {
        public string Text { get; }

        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public byte A { get; }

        public UserDataChunk()
        {
        }

        public UserDataChunk(string text)
        {
            Text = text;
        }

        public UserDataChunk(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static UserDataChunk Read(StreamReader reader)
        {
            //DWORD Flags
            //  1 = Has text
            //  2 = Has color
            var flag = reader.DWORD();
            //+ If flags have bit 1
            //  STRING Text
            if (reader.FLAG(flag, 1)) return new UserDataChunk(reader.STRING());
            //+If flags have bit 2
            //  BYTE Color Red(0 - 255)
            //  BYTE Color Green(0 - 255)
            //  BYTE Color Blue(0 - 255)
            //  BYTE Color Alpha(0 - 255)
            if (reader.FLAG(flag, 2)) return new UserDataChunk(reader.BYTE(), reader.BYTE(), reader.BYTE(), reader.BYTE());

            return new UserDataChunk();
        }
    }
}