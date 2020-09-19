
# aseprite#
### a simple aseprite c# parser

#### Aseprite Resources
https://www.aseprite.org/

https://github.com/aseprite/aseprite/blob/master/docs/ase-file-specs.md

#### Description
Inspired by  Noel Berry's aseprite parser 
https://gist.github.com/NoelFB/778d190e5d17f1b86ebf39325346fcc5

I wanted to take a crack at writing my own Aseprite Parser. This allows you to easily read the header and frame chunks from .ase and .aseprite files in C#

#### Usage
    Aseprite aseprite = null;

    var path = "C:/path/to/my/coolImg.aseprite"; // or .ase
    using (var fileStream = new FileStream(path, FileMode.Open))
    {
          var streamReader = new StreamReader(fileStream);
          aseprite = AsepriteReader.Read(streamReader);
    }
    
    


