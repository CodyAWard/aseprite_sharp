using System;

namespace aseprite_sharp
{
    public class LayerChunk : IChunk
    {
        public LayerFlags Flags { get; }
        public LayerTypes Type { get; }
        public ushort ChildLevel { get; }
        public BlendModes BlendMode { get; }
        public byte Opacity { get; }
        public string Name { get; }

        private LayerChunk(LayerFlags flags, LayerTypes type, ushort childLevel, BlendModes blendMode, byte opacity, string name)
        {
            Flags = flags;
            Type = type;
            ChildLevel = childLevel;
            BlendMode = blendMode;
            Opacity = opacity;
            Name = name;
        }

        [Flags]
        public enum LayerFlags
        {
            Visible = 1,
            Editable = 2,
            LockMovement = 4,
            Background = 8,
            PreferLinkedCells = 16,
            TheLayerGroupShouldBeDisplayedCollapsed = 32,
            TheLayerIsAReferenceLayer = 64,
        }

        public enum LayerTypes
        {
            NormalImage = 0,
            Group = 1,
        }

        public enum BlendModes
        {
            Normal = 0,
            Multiply = 1,
            Screen = 2,
            Overlay = 3,
            Darken = 4,
            Lighten = 5,
            ColorDodge = 6,
            ColorBurn = 7,
            HardLight = 8,
            SoftLight = 9,
            Difference = 10,
            Exclusion = 11,
            Hue = 12,
            Saturation = 13,
            Color = 14,
            Luminosity = 15,
            Addition = 16,
            Subtract = 17,
            Divide = 18
        }

        public static LayerChunk Read(StreamReader reader)
        {
            // WORD Flags:
            //               1 = Visible
            //               2 = Editable
            //               4 = Lock movement
            //               8 = Background
            //               16 = Prefer linked cels
            //               32 = The layer group should be displayed collapsed
            //               64 = The layer is a reference layer
            var flags = (LayerFlags)reader.WORD();

            // WORD        Layer type
            //               0 = Normal(image) layer
            //               1 = Group
            var type = (LayerTypes)reader.WORD();
            // WORD Layer child level(see NOTE.1)
            var childLevel = reader.WORD();
            // WORD Default layer width in pixels(ignored)
            reader.WORD();
            // WORD Default layer height in pixels(ignored)
            reader.WORD();
            // WORD Blend mode(always 0 for layer set)
            //                     Normal = 0
            //               Multiply = 1
            //               Screen = 2
            //               Overlay = 3
            //               Darken = 4
            //               Lighten = 5
            //               Color Dodge = 6
            //               Color Burn = 7
            //               Hard Light = 8
            //               Soft Light = 9
            //               Difference = 10
            //               Exclusion = 11
            //               Hue = 12
            //               Saturation = 13
            //               Color = 14
            //               Luminosity = 15
            //               Addition = 16
            //               Subtract = 17
            //               Divide = 18
            var blendMode = (BlendModes)reader.WORD();
            // BYTE Opacity
            var opacity = reader.BYTE();
            //               Note: valid only if file header flags field has bit 1 set
            // BYTE[3]     For future(set to zero)
            reader.BYTES(3);
            // STRING Layer name
            var name = reader.STRING();

            return new LayerChunk(flags, type, childLevel, blendMode, opacity, name);
        }
    }
}