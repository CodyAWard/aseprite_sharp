namespace aseprite_sharp
{
    public class SliceKey
    {
        public uint FrameNumber { get; }
        public long XOrigin { get; }
        public long YOrigin { get; }
        public uint Width { get; }
        public uint Height { get; }
        public long CenterX { get; }
        public long CenterY { get; }
        public uint CenterWidth { get; }
        public uint CenterHeight { get; }
        public long PivotX { get; }
        public long PivotY { get; }

        public SliceKey(uint frameNumber, long xOrigin, long yOrigin, uint width, uint height, long centerX, long centerY, uint centerWidth, uint centerHeight, long pivotX, long pivotY)
        {
            FrameNumber = frameNumber;
            XOrigin = xOrigin;
            YOrigin = yOrigin;
            Width = width;
            Height = height;
            CenterX = centerX;
            CenterY = centerY;
            CenterWidth = centerWidth;
            CenterHeight = centerHeight;
            PivotX = pivotX;
            PivotY = pivotY;
        }

        public static SliceKey Read(StreamReader reader, int flag)
        {
            //   DWORD     Frame number(this slice is valid from
            //             this frame to the end of the animation)
            var frameNumber = reader.DWORD();
            //   LONG Slice X origin coordinate in the sprite
            var xOrigin = reader.LONG();
            //   LONG Slice Y origin coordinate in the sprite
            var yOrigin = reader.LONG();
            //   DWORD Slice width(can be 0 if this slice hidden in the
            //        animation from the given frame)
            var width = reader.DWORD();
            //   DWORD Slice height
            var height = reader.DWORD();

            long centerX = 0;
            long centerY = 0;
            long pivotX = 0; 
            long pivotY = 0;

            uint centerWidth = 0;
            uint centerHeight = 0;

            //     + If flags have bit 1
            if (reader.FLAG(flag, 1))
            {
                //         LONG Center X position(relative to slice bounds)
                centerX = reader.LONG();
                //         LONG Center Y position
                centerY = reader.LONG();
                //         DWORD Center width
                centerWidth = reader.DWORD();
                //         DWORD   Center height
                centerHeight = reader.DWORD();
            }
            //     + If flags have bit 2
            if (reader.FLAG(flag, 2))
            {
                //         LONG Pivot X position(relative to the slice origin)
                pivotX = reader.LONG();
                //         LONG Pivot Y position(relative to the slice origin)
                pivotY = reader.LONG();
            }

            return new SliceKey(frameNumber, xOrigin, yOrigin, width, height, centerX, centerY, centerWidth, centerHeight, pivotX, pivotY);
        }
    }

    public class SliceChunk : IChunk
    {
        public string Name { get; }
        public SliceKey[] Keys { get; }

        public SliceChunk(string name, SliceKey[] keys)
        {
            Name = name;
            Keys = keys;
        }

        public static SliceChunk Read(StreamReader reader)
        {
            // DWORD Number of "slice keys"
            var keyCount = reader.DWORD();
            // DWORD Flags
            //               1 = It's a 9-patches slice
            //               2 = Has pivot information
            var flag = reader.DWORD();
            // DWORD Reserved
            reader.DWORD();
            // STRING Name
            var name = reader.STRING();
            // + For each slice key
            var keys = new SliceKey[keyCount];
            for (int i = 0; i < keyCount; i++)
            {
                keys[i] = SliceKey.Read(reader, (int)flag);
            }

            return new SliceChunk(name, keys);
        }
    }
}
