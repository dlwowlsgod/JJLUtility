namespace JJLUtility.IO
{
    /// <summary>
    /// Represents the file header of a BMP (Bitmap) file.
    /// </summary>
    /// <remarks>
    /// The BMP file header contains information about the file, such as its type,
    /// size, and the offset where the pixel data begins in the file.
    /// It is used to identify and interpret a BMP file's structure during file processing.
    /// </remarks>
    public struct BMPFileHeader
    {
        /// <summary>
        /// Represents the type of the BMP (Bitmap) file header.
        /// </summary>
        /// <remarks>
        /// This constant defines the signature of the BMP file format, typically used to identify
        /// and validate the file as a BMP. Its value is 0x4D42, representing the characters 'BM'
        /// in ASCII, a common indicator for BMP files.
        /// </remarks>
        public const ushort Type = 0x4D42;

        /// <summary>
        /// Specifies the size of the BMP (Bitmap) file.
        /// </summary>
        /// <remarks>
        /// This field indicates the total size of the BMP file header structure. It is a critical
        /// part used for validating the integrity and consistency of the BMP file. Typically,
        /// its value is extracted from the binary content of the file during the parsing process.
        /// </remarks>
        public uint Size;

        /// <summary>
        /// Represents the offset, in bytes, from the beginning of the BMP file to the start of the pixel data.
        /// </summary>
        /// <remarks>
        /// This value indicates where the actual image data begins in the BMP file. It is critical for
        /// loading the bitmap image as it determines the location of the pixel array. The offset typically
        /// accounts for the BMP file header and the DIB header.
        /// </remarks>
        public uint Offset;
    }
}