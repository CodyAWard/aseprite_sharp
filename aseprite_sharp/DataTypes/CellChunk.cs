using aseprite_sharp.Reader;

namespace aseprite_sharp.DataTypes
{
    public class CellExtraChunk : IChunk
    {
        public bool PreciesBoundsAreSet{ get; }
        public float PreciseXPosition{ get; }
        public float PreciseYPosition{ get; }
        public float WidthOfTheCellInTheSprite{ get; }
        public float HeightOfTheCellInTheSprite{ get; }

        private CellExtraChunk(bool preciesBoundsAreSet, float preciseXPosition, float preciseYPosition, float widthOfTheCellInTheSprite, float heightOfTheCellInTheSprite)
        {
            PreciesBoundsAreSet = preciesBoundsAreSet;
            PreciseXPosition = preciseXPosition;
            PreciseYPosition = preciseYPosition;
            WidthOfTheCellInTheSprite = widthOfTheCellInTheSprite;
            HeightOfTheCellInTheSprite = heightOfTheCellInTheSprite;
        }

        public static CellExtraChunk Read(AsepriteStreamReader reader)
        {
            // DWORD Flags(set to zero)
            //   1 = Precise bounds are set
            var preciesBoundsAreSet = reader.DWORD() == 1;
            // FIXED       Precise X position
            var preciseXPosition = reader.FIXED();
            // FIXED       Precise Y position
            var preciseYPosition = reader.FIXED();
            // FIXED       Width of the cel in the sprite(scaled in real-time)
            var widthOfTheCellInTheSprite = reader.FIXED();
            // FIXED Height of the cel in the sprite
            var heightOfTheCellInTheSprite = reader.FIXED();
            // BYTE[16]    For future use(set to zero)
            reader.BYTES(16);

            return new CellExtraChunk(preciesBoundsAreSet, preciseXPosition, preciseYPosition, widthOfTheCellInTheSprite, heightOfTheCellInTheSprite);
        }
    }

    public class CellChunk : IChunk
    {
        public bool IsVisible { get; set; }
        public ushort LayerIndex{ get; }
        public short XPosition{ get; }
        public short YPosition{ get; }
        public byte OpacityLevel{ get; }
        public ushort CellType{ get; }
        
        public ushort? Width{ get; }
        public ushort? Height{ get; }
        
        public byte[] Pixels{ get; }

        public ushort? LinkedFramePosition{ get; }

        private CellChunk(ushort layerIndex, short xPosition, short yPosition, byte opacityLevel, ushort cellType)
        {
            LayerIndex = layerIndex;
            XPosition = xPosition;
            YPosition = yPosition;
            OpacityLevel = opacityLevel;
            CellType = cellType;
        }

        private CellChunk(ushort layerIndex, short xPosition, short yPosition, byte opacityLevel, ushort cellType, ushort linkedFramePosition) : this(layerIndex, xPosition, yPosition, opacityLevel, cellType)
        {
            LinkedFramePosition = linkedFramePosition;
        }

        private CellChunk(ushort layerIndex, short xPosition, short yPosition, byte opacityLevel, ushort cellType, ushort width, ushort height, byte[] pixels) : this(layerIndex, xPosition, yPosition, opacityLevel, cellType)
        {
            Width = width;
            Height = height;
            Pixels = pixels;
        }

        public static CellChunk Read(AsepriteStreamReader reader, ColorDepth colorDepth)
        {
            // WORD Layer index(see NOTE.2)
            var layerIndex = reader.WORD();
            // SHORT X position
            var xPosition = reader.SHORT();
            // SHORT       Y position
            var yPosition = reader.SHORT();
            // BYTE Opacity level
            var opacityLevel = reader.BYTE();
            // WORD        Cel type
            var cellType = reader.WORD();
            // BYTE[7]     For future(set to zero)
            reader.BYTES(7);
            // +For cel type = 0(Raw Cel)
            if (cellType == 0)
            {
                //   WORD Width in pixels
                var width = reader.WORD();
                //   WORD Height in pixels
                var height = reader.WORD();
                //   PIXEL[]   Raw pixel data: row by row from top to bottom,
                //             for each scanline read pixels from left to right.
                var depthModifier = 0;
                if (colorDepth == ColorDepth.RGBA) depthModifier = 4;
                else if (colorDepth == ColorDepth.Grayscale) depthModifier = 2;
                else if (colorDepth == ColorDepth.Indexed) depthModifier = 1;

                var pixels = reader.BYTES(width * height * depthModifier); 
                
                return new CellChunk(layerIndex, xPosition, yPosition, opacityLevel, cellType,
                    width, height, pixels);
            }
            // + For cel type = 1(Linked Cel)
            else if (cellType == 1)
            {
                //   WORD      Frame position to link with
                var linkedFramePosition = reader.WORD();
                return new CellChunk(layerIndex, xPosition, yPosition, opacityLevel, cellType,
                    linkedFramePosition);
            }
            // + For cel type = 2(Compressed Image)
            else if (cellType == 2)
            {
                //   WORD      Width in pixels
                var width = reader.WORD();
                //   WORD      Height in pixels
                var height = reader.WORD();
                //   BYTE[]    "Raw Cel" data compressed with ZLIB method 
                var depthModifier = 0;
                if (colorDepth == ColorDepth.RGBA) depthModifier = 4;
                else if (colorDepth == ColorDepth.Grayscale) depthModifier = 2;
                else if (colorDepth == ColorDepth.Indexed) depthModifier = 1;

                var count = width * height * depthModifier;
                var pixels = new byte[count];
                reader.DEFLATE(pixels);

                return new CellChunk(layerIndex, xPosition, yPosition, opacityLevel, cellType, width, height, pixels);
            }
            else
            {
                return new CellChunk(layerIndex, xPosition, yPosition, opacityLevel, cellType);
            }
        }
    }
}
