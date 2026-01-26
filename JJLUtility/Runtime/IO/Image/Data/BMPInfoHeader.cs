namespace JJLUtility.IO
{
    /// <summary>
    /// Represents the information header of a BMP file, providing metadata about the dimensions,
    /// color format, and compression details of the BMP image.
    /// </summary>
    public struct BMPInfoHeader
    {
        /// <summary>
        /// Specifies the size of the BMP information header in bytes. This value determines the header's
        /// length and, consequently, the presence of additional metadata fields.
        /// </summary>
        public uint Size;

        /// <summary>
        /// Defines the width of the BMP image in pixels. This value determines the horizontal
        /// dimension of the image and is essential for rendering and processing the bitmap.
        /// </summary>
        public int Width;

        /// <summary>
        /// Specifies the height of the BMP image in pixels. A positive value indicates a bottom-up DIB
        /// with the origin at the lower-left corner, while a negative value indicates a top-down DIB
        /// with the origin at the upper-left corner.
        /// </summary>
        public int Height;

        /// <summary>
        /// Specifies the number of color planes in the BMP image. This value is always set to 1
        /// and must not be changed, as it is reserved for future use.
        /// </summary>
        public const uint Planes = 1;

        /// <summary>
        /// Specifies the number of bits per pixel (bpp) used to define the color depth of the image.
        /// This value determines how many bits are allocated for each pixel in the BMP file.
        /// Common values include 1, 4, 8, 16, 24, and 32 bits per pixel.
        /// </summary>
        public ushort BitCount;

        /// <summary>
        /// Represents the compression method used for the BMP image. This enumeration defines
        /// various compression types such as uncompressed, RLE (Run Length Encoding),
        /// and other specialized formats applicable to the BMP file format.
        /// </summary>
        public BMPCompression Compression;

        /// <summary>
        /// Specifies the size of the raw image data in bytes. This value can be used to determine the
        /// actual storage space occupied by the pixel data, including any padding that might be present
        /// due to alignment requirements.
        /// </summary>
        public uint SizeImage;

        /// <summary>
        /// Specifies the horizontal resolution of the image, expressed in pixels per meter.
        /// This value defines the intended resolution for display or printing purposes.
        /// </summary>
        public int XPixelPerMeter;

        /// <summary>
        /// Specifies the vertical resolution of the image in pixels per meter. This value is commonly used for defining the
        /// intended display resolution of the BMP image and may not always match the actual resolution used during rendering.
        /// </summary>
        public int YPixelPerMeter;

        /// <summary>
        /// Indicates the number of colors used in the BMP file's palette. If this value is zero, all colors
        /// available based on the bit depth of the image are used.
        /// </summary>
        public uint ColorUsed;

        /// <summary>
        /// Specifies the number of important colors used in the BMP, typically used to optimize palette size.
        /// If set to zero, all colors are considered important.
        /// </summary>
        public uint ColorImportant;

        /// <summary>
        /// Represents the bitmask used to isolate the red color channel within a pixel.
        /// This value is utilized when processing BMP files using bit fields to define color channels.
        /// </summary>
        public uint RedMask;

        /// <summary>
        /// Represents the bitmask used to isolate the green color channel within a pixel.
        /// This value is utilized when processing BMP files using bit fields to define color channels.
        /// from the stored pixel data.
        /// </summary>
        public uint GreenMask;

        /// <summary>
        /// Represents the bitmask used to isolate the blue color channel within a pixel.
        /// This value is utilized when processing BMP files using bit fields to define color channels.
        /// </summary>
        public uint BlueMask;

        /// <summary>
        /// Represents the bitmask that specifies the alpha channel of the pixel data.
        /// Used to isolate or manipulate the transparency information in the bitmap.
        /// </summary>
        public uint AlphaMask;
    }
}