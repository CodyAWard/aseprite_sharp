using aseprite_sharp.DataTypes;
using System;
using System.IO;

namespace aseprite_sharp.Reader
{
    public static class AsepriteReader
    {
        /// <summary>
        /// Reads an Aseprite File from the given path
        /// 
        /// .ase and .aseprite files are acceptable
        /// 
        /// maxFileAccessAttempts is useful due to file
        /// saving while accessing live files
        /// </summary>
        public static void ReadFromFile(string path, Aseprite sprite, int maxFileAccessAttempts = 10)
        {
            sprite.Clear();

            if (!File.Exists(path))
            {
                return;
            }

            var fileAccessAttempts = 0;
            while (fileAccessAttempts < maxFileAccessAttempts)
            {
                try
                {
                    using (var stream = new FileStream(path, FileMode.Open))
                    {
                        var reader = new AsepriteStreamReader(stream);
                        ReadFromStream(reader, sprite);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    fileAccessAttempts++;
                }
            }
        }

        private static void ReadFromStream(AsepriteStreamReader reader, Aseprite sprite)
        {
            // Read the ASE header
            var header = Header.Read(reader);
            var frames = new FrameData[header.Frames];
            for (int i = 0; i < header.Frames; i++)
            {
                frames[i] = FrameData.Read(reader, header.ColorDepth);
            }

            sprite.Update(header, frames);
        }
    }
}