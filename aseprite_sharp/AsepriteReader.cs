using System;

namespace aseprite_sharp
{
    public class Aseprite
    {
        public Header Header { get; }
        public Frame[] Frames { get; }

        public Aseprite(Header header, Frame[] frames)
        {
            Header = header;
            Frames = frames;
        }
    }

    public class AsepriteReader
    {
        public static Aseprite Read(StreamReader reader)
        {
            // Read the ASE header
            var header = Header.Read(reader);
            var frames = new Frame[header.Frames];
            for (int i = 0; i < header.Frames; i++)
            {
                Console.WriteLine("Reading Frame " + i);
                frames[i] = Frame.Read(reader, header.ColorDepth);
            }
           
            return new Aseprite(header, frames);
        }
    }
}
