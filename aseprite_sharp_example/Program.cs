using aseprite_sharp;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using StreamReader = aseprite_sharp.StreamReader;

namespace aseprite_sharp_example
{
    class Program
    {
        static void Main(string[] args)
        {
            var startTime = DateTime.Now;
            Console.WriteLine(startTime.TimeOfDay + " starting aseprite# read");

            Aseprite aseprite = null;

            var path = "C:/Users/codyw/Documents/Projects/art/aseprite_test_01.aseprite";
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                var streamReader = new StreamReader(fileStream);
                aseprite = AsepriteReader.Read(streamReader);
            }

            var endTime = DateTime.Now;
            var elapsed = endTime - startTime;
            Console.WriteLine(endTime.TimeOfDay + " completed aseprite# read in " + elapsed);

            var index = 0;

            PaletteChunk palette = null;
            foreach (var frame in aseprite.Frames)
            {
                palette = frame.TryGet<PaletteChunk>();
                if (palette != null) break;
            }

            foreach (var frame in aseprite.Frames)
            {
                foreach (var cell in frame.TryGetAll<CellChunk>())
                {
                    var brush = new SolidBrush(Color.White);
                    using (Bitmap b = new Bitmap((int)cell.Width.Value, (int)cell.Height.Value))
                    {
                        using (Graphics g = Graphics.FromImage(b))
                        {
                            for (int x = 0; x < cell.Width.Value; x++)
                            {
                                for (int y = 0; y < cell.Height.Value; y++)
                                {
                                    var pixel = cell.Pixels[x + (y * cell.Width.Value)];
                                    if (pixel < palette.Entries.Length)
                                    {
                                        var color = palette.Entries[pixel];
                                        brush.Color = Color.FromArgb(color.R, color.G, color.B, color.A);
                                    }
                                    else
                                    {
                                        brush.Color = Color.Transparent;
                                    }

                                    g.FillRectangle(brush, x, y, 1, 1);
                                }
                            }
                        }

                        b.Save("C:/Users/codyw/Documents/Projects/art/tests/" + index + ".png", ImageFormat.Png);
                        index++;
                    }
                }
            }
        }
    }
}
