namespace JJLUtility.IO
{
    /// <summary>
    /// Defines the compression methods used in BMP image files.
    /// </summary>
    public enum BMPCompression : uint
    {
        /// <summary>
        /// Specifies that no compression is used, meaning the BMP image data is stored as plain, uncompressed RGB pixels.
        /// </summary>
        BI_RGB = 0,

        /// <summary>
        /// Specifies that the BMP image data is compressed using 8-bit run-length encoding (RLE).
        /// This compression method is primarily used for images with an 8-bit color depth (256 colors).
        /// </summary>
        BI_RLE8 = 1,

        /// <summary>
        /// Specifies that the BMP image uses 4-bit run-length encoding (RLE) compression.
        /// This method compresses the image by encoding sequences of the same color or by using escape sequences for uncompressed data.
        /// </summary>
        BI_RLE4 = 2,

        /// <summary>
        /// Specifies that the bitmap uses bit fields to define the RGB and optionally alpha pixel masks.
        /// This compression method allows for custom pixel formats by specifying mask bit fields for color components.
        /// </summary>
        BI_BITFIELDS = 3,

        /// <summary>
        /// Specifies that the BMP image data is compressed using the JPEG (Joint Photographic Experts Group) compression algorithm.
        /// </summary>
        BI_JPEG = 4,

        /// <summary>
        /// Specifies that the BMP image data is compressed using the PNG image compression method.
        /// </summary>
        BI_PNG = 5,

        /// <summary>
        /// Specifies that the BMP image uses alpha bitfields for defining transparency,
        /// alongside the color masks. This compression method is used for images with explicit alpha channel support.
        /// </summary>
        BI_ALPHABITFIELDS = 6,

        /// <summary>
        /// Specifies that the BMP image uses CMYK color format and no compression is applied, representing the image's color components
        /// directly as Cyan, Magenta, Yellow, and Black.
        /// </summary>
        BI_CMYK = 11,

        /// <summary>
        /// Specifies that the image is compressed using RLE (run-length encoding) for CMYK data, utilizing 8 bits per pixel.
        /// </summary>
        BI_CMYKRLE8 = 12,

        /// <summary>
        /// Specifies that the BMP image data is compressed using run-length encoding (RLE) with 4 bits per pixel in CMYK color format.
        /// </summary>
        BI_CMYKRLE4 = 13
    }
}