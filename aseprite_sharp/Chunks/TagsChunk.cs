namespace aseprite_sharp
{
    public class Tag
    {
        public ushort FromFrame { get; }
        public ushort ToFrame{ get; }
        public LoopDirections LoopDirection{ get; }
        public byte[] Color{ get; }
        public string Name{ get; }

        public Tag(ushort fromFrame, ushort toFrame, LoopDirections loopDirection, byte[] color, string name)
        {
            FromFrame = fromFrame;
            ToFrame = toFrame;
            LoopDirection = loopDirection;
            Color = color;
            Name = name;
        }

        public enum LoopDirections
        {
            Forward  = 0,
            Reverse  = 1,
            PingPong = 2
        }

        public static Tag Read(StreamReader reader)
        {
            //WORD From frame
            var fromFrame = reader.WORD();
            //WORD To frame
            var toFrame = reader.WORD();
            //BYTE Loop animation direction
            //            0 = Forward
            //            1 = Reverse
            //            2 = Ping - pong
            var loopDirection = (LoopDirections)reader.BYTE();
            //BYTE[8]   For future(set to zero)
            reader.BYTES(8);
            //BYTE[3]   RGB values of the tag color
            var color = reader.BYTES(3);
            //BYTE Extra byte(zero)
            reader.BYTE();
            //STRING Tag name
            var name = reader.STRING();

            return new Tag(fromFrame, toFrame, loopDirection, color, name);
        }
    }

    public class TagsChunk : IChunk
    {
        public Tag[] Tags { get; }

        public TagsChunk(Tag[] tags)
        {
            Tags = tags;
        }

        public static TagsChunk Read(StreamReader reader)
        {
            //WORD Number of tags
            var tagCount = reader.WORD();
            //BYTE[8] For future(set to zero)
            reader.BYTES(8);
            //+For each tag
            var tags = new Tag[tagCount];
            for (int i = 0; i < tagCount; i++)
            {
                tags[i] = Tag.Read(reader);
            }

            return new TagsChunk(tags);
        }
    }
}
