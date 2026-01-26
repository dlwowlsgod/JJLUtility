using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace JJLUtility.IO
{
    /// <summary>
    /// Represents a BMP (Bitmap) file and its associated data.
    /// This class contains the file header, information header, and pixel data
    /// of a BMP file.
    /// </summary>
    public class BMPFile
    {
        /// <summary>
        /// Represents the header data of a BMP (Bitmap) file.
        /// </summary>
        /// <remarks>
        /// The file header contains general information about the BMP file,
        /// including its type identifier, file size, and pixel data offset.
        /// </remarks>
        public BMPFileHeader FileHeader;

        /// <summary>
        /// Represents the information header of a BMP (Bitmap) file.
        /// </summary>
        /// <remarks>
        /// The information header contains detailed metadata about the BMP image,
        /// such as its dimensions, color format, compression type, and other image properties.
        /// </remarks>
        public BMPInfoHeader InfoHeader;

        /// <summary>
        /// Represents the pixel data of a BMP (Bitmap) file.
        /// </summary>
        /// <remarks>
        /// The pixel data is stored as an array of Color32 structures, where each element
        /// represents a single pixel in the image. The format and arrangement of the pixel data
        /// depend on the BMP file's bit depth, compression, and dimensions as specified in its headers.
        /// </remarks>
        public Color32[] Pixels;
    }

    public partial class ImageLoader
    {
        public Texture2D LoadBMPImage(string filepath)
        {
            if (Path.GetExtension(filepath).ToLower() != ".bmp")
            {
                Debugger.LogError($"File is not .bmp file: {filepath}", this, nameof(ImageLoader));
                return null;;
            }
            
            if (!File.Exists(filepath))
            {
                Debugger.LogError($"File not found: {filepath}", this, nameof(ImageLoader));
                return null;
            }
            
            //open filestream
            var stream = File.OpenRead(filepath);
            var binaryReader = new BinaryReader(stream);

            if (binaryReader.ReadUInt16() != BMPFileHeader.Type)
            {
                Debugger.LogError($"File is not .bmp file: {filepath}", this, nameof(ImageLoader));
                return null;
            }
            
            //create bmp file
            var bmpFile = new BMPFile();
            
            //read file header
            var bmpFileHeader = new BMPFileHeader();
            
            bmpFileHeader.Size = binaryReader.ReadUInt32();
            binaryReader.ReadInt32();
            bmpFileHeader.Offset = binaryReader.ReadUInt32();
            
            bmpFile.FileHeader = bmpFileHeader;
            
            //read info header
            var bmpInfoHeader = new BMPInfoHeader();
            
            bmpInfoHeader.Size = binaryReader.ReadUInt32();
            bmpInfoHeader.Width = binaryReader.ReadInt32();
            bmpInfoHeader.Height = binaryReader.ReadInt32();
            binaryReader.ReadInt16();
            bmpInfoHeader.BitCount = binaryReader.ReadUInt16();
            bmpInfoHeader.Compression = (BMPCompression) binaryReader.ReadUInt32();
            bmpInfoHeader.SizeImage = binaryReader.ReadUInt32();
            bmpInfoHeader.XPixelPerMeter = binaryReader.ReadInt32();
            bmpInfoHeader.YPixelPerMeter = binaryReader.ReadInt32();
            bmpInfoHeader.ColorUsed = binaryReader.ReadUInt32();
            bmpInfoHeader.ColorImportant = binaryReader.ReadUInt32();

            //read remaining
            int padding = (int)bmpFileHeader.Size - 40;
            if (padding > 0)
            {
                binaryReader.ReadBytes(padding);
            } 
            
            //check bmp compression method
            if (bmpInfoHeader.Compression != BMPCompression.BI_RGB ||
                bmpInfoHeader.Compression != BMPCompression.BI_RLE8 ||
                bmpInfoHeader.Compression != BMPCompression.BI_RLE4 ||
                bmpInfoHeader.Compression != BMPCompression.BI_BITFIELDS ||
                bmpInfoHeader.Compression != BMPCompression.BI_ALPHABITFIELDS)
            {
                Debugger.LogError($"Unsupported .bmp compression method: {filepath}", this, nameof(ImageLoader));
                return null;
            }
            
            //move reader offset
            //(safely find offset (fixed 14 FileHeaderSize bytes) + (variable InfoHeaderSize))
            long offset = 14 + bmpInfoHeader.Size;
            binaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
            
            return null;
        }

        public Material LoadBMPMaterial(string filepath)
        {
            return null;
        }
    }
}