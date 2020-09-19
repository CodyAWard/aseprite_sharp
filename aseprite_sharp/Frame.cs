using System;
using System.Collections.Generic;
using System.Linq;

namespace aseprite_sharp
{
    public class Frame
    {
        public uint BytesInFrame { get; }
        public ushort FrameDuration { get; }

        public IChunk[] Chunks { get; }
        public bool TryGet<T>(out T chunk) where T : class, IChunk
        {
            if (TryGetAll<T>(out var chunks))
            {
                chunk = chunks.FirstOrDefault();
                return chunk != null;
            }

            chunk = null;
            return false;
        }

        public bool TryGetAll<T>(out IEnumerable<T> chunks) where T : class, IChunk
        {
            chunks = Chunks.OfType<T>();
            return chunks.Any();
        }

        private Frame(uint bytesInFrame, ushort frameDuration, IChunk[] chunks)
        {
            BytesInFrame = bytesInFrame;
            FrameDuration = frameDuration;
            Chunks = chunks;
        }

        public static Frame Read(StreamReader reader, ColorDepth colorDepth)
        {
            // DWORD Bytes in this frame
            var bytesInFrame = reader.DWORD();
            // WORD        Magic number(always 0xF1FA)
            var magicNumber = reader.WORD();
            if (magicNumber != 0xF1FA) throw new System.Exception("Invalid Format");

            // WORD Old field which specifies the number of "chunks"
            //             in this frame.If this value is 0xFFFF, we might
            //             have more chunks to read in this frame
            //             (so we have to use the new field)
            var oldNumberOfChunks = reader.WORD();
            // WORD Frame duration(in milliseconds)
            var frameDuration = reader.WORD();
            // BYTE[2]     For future(set to zero)
            reader.BYTES(2);
            // DWORD New field which specifies the number of "chunks"
            //             in this frame(if this is 0, use the old field)
            var newNumberOfChunks = reader.DWORD();
            var numberOfChunks = newNumberOfChunks > 0 ? newNumberOfChunks : oldNumberOfChunks;

            var chunks = new IChunk[numberOfChunks];
            for (int i = 0; i < numberOfChunks; i++)
            {
                // DWORD Chunk size
                var startPos = reader.POS;
                var size = reader.DWORD();
                // WORD Chunk type
                var type = reader.WORD();
                // BYTE[] Chunk data
                switch (type)
                {
                    case 0x2004:
                        chunks[i] = LayerChunk.Read(reader);
                        break;
                    case 0x2005:
                        chunks[i] = CellChunk.Read(reader, colorDepth);
                        break;
                    case 0x2006:
                        chunks[i] = CellExtraChunk.Read(reader);
                        break;
                    case 0x2018:
                        chunks[i] = TagsChunk.Read(reader);
                        break;
                    case 0x2019:
                        chunks[i] = PaletteChunk.Read(reader);
                        break;
                    case 0x2020:
                        chunks[i] = UserDataChunk.Read(reader);
                        break;
                    case 0x2022:
                        chunks[i] = SliceChunk.Read(reader);
                        break;
                    default:
                        // ingoring data
                        reader.BYTES((int)size);
                        break;
                }

                reader.POS = startPos + size;
            }

            return new Frame(bytesInFrame, frameDuration, chunks);
        }
    }
}
