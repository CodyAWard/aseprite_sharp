namespace aseprite_sharp
{
    public class Header
    {
        public uint FileSize { get; }
        public ushort Frames { get; }
        public ushort Width { get; }
        public ushort Height { get; }
        public ColorDepth ColorDepth { get; }
        public bool LayerOpacityIsValid { get; }
        public byte TransparentIndex { get; }
        public byte PixelWidth { get; }
        public byte PixelHeight { get; }
        public byte PixelRatio { get; }
        public short XPosition { get; }
        public short YPosition { get; }
        public ushort GridWidth { get; }
        public ushort GridHeight { get; }

        private Header(
            uint fileSize, 
            ushort frames, 
            ushort width, 
            ushort height, 
            ColorDepth colorDepth, 
            bool layerOpacityIsValid, 
            byte transparentIndex, 
            byte pixelWidth, 
            byte pixelHeight, 
            byte pixelRatio,
            short xPos, 
            short yPos, 
            ushort gridWidth, 
            ushort gridHeight)
        {
            FileSize = fileSize;
            Frames = frames;
            Width = width;
            Height = height;
            ColorDepth = colorDepth;
            LayerOpacityIsValid = layerOpacityIsValid;
            TransparentIndex = transparentIndex;
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
            XPosition = xPos;
            YPosition = yPos;
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            PixelRatio = pixelRatio;
        }

        public static Header Read(StreamReader reader)
        {
            // DWORD       File size
            var fileSize = reader.DWORD();
            // WORD        Magic number (0xA5E0)
            var magicNumber = reader.WORD();
            if (magicNumber != 0xA5E0) throw new System.Exception("Invalid Format");
            // WORD        Frames
            var frames = reader.WORD();
            // WORD        Width in pixels
            var width = reader.WORD();
            // WORD        Height in pixels
            var height = reader.WORD();
            // WORD        Color depth (bits per pixel)
            //               32 bpp = RGBA
            //               16 bpp = Grayscale
            //               8 bpp = Indexed
            var colorDepth = (ColorDepth)reader.WORD();
            // DWORD       Flags:
            //               1 = Layer opacity has valid value
            var layerOpacityHasValidValue = reader.FLAG(reader.DWORD(), 1);
            // WORD        Speed (milliseconds between frame, like in FLC files)
            //             DEPRECATED: You should use the frame duration field
            //             from each frame header
            reader.WORD();
            // DWORD       Set be 0
            reader.DWORD();
            // DWORD       Set be 0    
            reader.DWORD();
            // BYTE        Palette entry (index) which represent transparent color
            //             in all non-background layers (only for Indexed sprites).
            var transparentIndex = reader.BYTE();
            // BYTE[3]     Ignore these bytes
            reader.BYTES(3);
            // WORD        Number of colors (0 means 256 for old sprites)
            reader.WORD();
            // BYTE        Pixel width (pixel ratio is "pixel width/pixel height").
            //             If this or pixel height field is zero, pixel ratio is 1:1
            var pixelWidth = reader.BYTE();
            // BYTE        Pixel height
            var pixelHeight = reader.BYTE();
            byte pixelRatio = 1;
            if (pixelHeight > 0 && pixelWidth > 0)
            {
                pixelRatio = (byte)(pixelWidth / pixelHeight);
            }
            // SHORT       X position of the grid
            var xPos = reader.SHORT();
            // SHORT       Y position of the grid
            var yPos = reader.SHORT();
            // WORD        Grid width (zero if there is no grid, grid size
            //             is 16x16 on Aseprite by default)
            var gridWidth = reader.WORD();
            // WORD        Grid height (zero if there is no grid)
            var gridHeight = reader.WORD();
            // BYTE[84]    For future (set to zero)
            reader.BYTES(84);

            return new Header(
                fileSize,  
                frames, 
                width, 
                height, 
                colorDepth, 
                layerOpacityHasValidValue, 
                transparentIndex, 
                pixelWidth, 
                pixelHeight, 
                pixelRatio,
                xPos, 
                yPos, 
                gridWidth, 
                gridHeight);
        }
    }
}