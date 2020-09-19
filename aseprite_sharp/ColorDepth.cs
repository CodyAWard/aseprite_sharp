
namespace aseprite_sharp
{
    /// <summary>
    /// Color depth (bits per pixel)
    //               32 bpp = RGBA
    //               16 bpp = Grayscale
    //               8 bpp = Indexed
    /// </summary>
    public enum ColorDepth : byte
    {
        RGBA        = 32,
        Grayscale   = 16,
        Indexed     =  8,
    }
}
