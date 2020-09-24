using aseprite_sharp.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aseprite_sharp
{
    public struct Config
    {
        public string Directory;
        public bool WatchFiles;
    }

    public struct NamedSprite
    {
        public string Name;
        public Aseprite Sprite;

        public NamedSprite(string name, Aseprite sprite)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Sprite = sprite ?? throw new ArgumentNullException(nameof(sprite));
        }
    }

    public class AsepriteParser
    {
        public IEnumerable<NamedSprite> GetAllSprites()
        {
            var count = aseprites.Count;
            var sprites = new NamedSprite[count];
            var names = aseprites.Keys.ToArray();
            var values = aseprites.Values.ToArray();
            for (int i = 0; i < count; i++)
            {
                var name = names[i];
                var sprite = values[i];
                sprites[i] = new NamedSprite(name, sprite);
            }

            return sprites;
        }

        private readonly Config config;
        private readonly Dictionary<string, Aseprite> aseprites;
        private readonly FileSystemWatcher fileSystemWatcher;

        public AsepriteParser(Config config)
        {
            this.config = config;
            if (!Directory.Exists(config.Directory)) throw new DirectoryNotFoundException(config.Directory);

            aseprites = new Dictionary<string, Aseprite>();

            fileSystemWatcher = new FileSystemWatcher(config.Directory)
            {
                NotifyFilter = NotifyFilters.LastAccess
                                           | NotifyFilters.LastWrite
                                           | NotifyFilters.FileName
                                           | NotifyFilters.DirectoryName,

                EnableRaisingEvents = config.WatchFiles
            };

            fileSystemWatcher.Changed += (obj, arg) => Load();

            Load();
        }

        public bool TryGetSprite(string name, out Aseprite sprite) => aseprites.TryGetValue(name, out sprite);

        private void Load()
        {
            if (!Directory.Exists(config.Directory)) throw new DirectoryNotFoundException(config.Directory);

            foreach (var fileName in Directory.GetFiles(config.Directory))
            {
                var extension = Path.GetExtension(fileName);
                if (extension != ".ase" && extension != ".aseprite")
                    continue;

                var name = Path.GetFileNameWithoutExtension(fileName);
                aseprites.TryGetValue(name, out var sprite);
                if (sprite == null)
                {
                    sprite = new Aseprite();
                    aseprites[name] = sprite;
                }

                AsepriteReader.ReadFromFile(fileName, sprite);
            }
        }
    }
}
