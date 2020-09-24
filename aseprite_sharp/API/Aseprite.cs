using aseprite_sharp.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aseprite_sharp
{
    public class Frame
    {
        public IEnumerable<CellChunk> Cells { get; }
        public int FrameDuration { get; }

        public Frame(IEnumerable<CellChunk> cells, int frameDuration)
        {
            FrameDuration = frameDuration;
            Cells = cells ?? throw new ArgumentNullException(nameof(cells));
        }
    }

    public class Aseprite
    {
        public event EventHandler Updated;

        public Header Header { get; private set; }

        private PaletteChunk paletteChunk;
        private TagsChunk tagsChunk;

        public IEnumerable<PaletteEntry> Palette => paletteChunk.Entries;
        public IEnumerable<Tag> Tags => tagsChunk?.Tags;

        public IEnumerable<Frame> Frames { get; private set; }

        public Aseprite()
        {
            Clear();
        }

        public void Clear()
        {
            Header = null;
            Frames = null;

            paletteChunk = null;
            tagsChunk = null;
        }

        public void Update(Header header, FrameData[] frameData)
        {
            Header = header;

            LayerChunk[] layerChunks = null;
            foreach (var frame in frameData)
            {
                if (paletteChunk == null) frame.TryGet(out paletteChunk);
                if (tagsChunk == null) frame.TryGet(out tagsChunk);

                if (layerChunks == null && frame.TryGetAll<LayerChunk>(out var chunks))
                {
                    layerChunks = chunks.ToArray();
                }
            }

            var frames = new Frame[header.Frames];
            for (int i = 0; i < header.Frames; i++)
            {
                var data = frameData[i];

                data.TryGetAll<CellChunk>(out var chunks);
                var layerCells = chunks.OrderBy(c => c.LayerIndex);

                var visibilityFlag = 1;
                foreach (var cell in layerCells)
                {
                    var flags = (int)layerChunks[cell.LayerIndex].Flags;
                    cell.IsVisible = (flags & (1 << visibilityFlag)) != 0;
                }

                frames[i] = new Frame(layerCells, data.FrameDuration);
            }

            Frames = frames;

            Updated?.Invoke(this, EventArgs.Empty);
        }
    }
}
