using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;

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
        /// Represents the color palettes of a BMP (Bitmap) file.
        /// </summary>
        /// <remarks>
        /// The palettes contain predefined sets of colors used to represent the image
        /// when the BMP file uses color-indexed values. It is primarily utilized in BMP
        /// files with bit depths of 1, 4, or 8 bits per pixel.
        /// </remarks>
        public Color32[] Palettes;

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
        private static BMPFile LoadBMPFile(string filepath)
        {
            //check extension
            if (Path.GetExtension(filepath).ToLower() != ".bmp")
            {
                Debugger.LogError($"File is not .bmp file: {filepath}", Instance, nameof(ImageLoader));
                return null;
                ;
            }

            //check file exists
            if (!File.Exists(filepath))
            {
                Debugger.LogError($"File not found: {filepath}", Instance, nameof(ImageLoader));
                return null;
            }

            //open filestream
            var stream = File.OpenRead(filepath);
            var binaryReader = new BinaryReader(stream);

            if (binaryReader.ReadUInt16() != BMPFileHeader.Type)
            {
                Debugger.LogError($"File is not .bmp file: {filepath}", Instance, nameof(ImageLoader));
                return null;
            }

            //create bmp file
            var bmpFile = new BMPFile();

            //read file header
            bmpFile.FileHeader = LoadBMPFileHeader(binaryReader);

            //read info header
            var bmpInfoHeader = LoadBMPInfoHeader(binaryReader);

            //check bmp compression method
            if (bmpInfoHeader.Compression != BMPCompression.BI_RGB &&
                bmpInfoHeader.Compression != BMPCompression.BI_RLE8 &&
                bmpInfoHeader.Compression != BMPCompression.BI_RLE4 &&
                bmpInfoHeader.Compression != BMPCompression.BI_BITFIELDS &&
                bmpInfoHeader.Compression != BMPCompression.BI_ALPHABITFIELDS)
            {
                Debugger.LogError($"Unsupported .bmp compression method: {filepath}", Instance, nameof(ImageLoader));
                return null;
            }

            //move reader offset to v4 header
            long realOffset = 14 + bmpInfoHeader.Size;
            binaryReader.BaseStream.Seek(realOffset, SeekOrigin.Begin);

            //read bitmasks
            if (bmpInfoHeader.BitCount > 8 && (bmpInfoHeader.Compression == BMPCompression.BI_BITFIELDS ||
                                               bmpInfoHeader.Compression == BMPCompression.BI_ALPHABITFIELDS))
            {
                bmpInfoHeader.RedMask = binaryReader.ReadUInt32();
                bmpInfoHeader.GreenMask = binaryReader.ReadUInt32();
                bmpInfoHeader.BlueMask = binaryReader.ReadUInt32();
                if (bmpInfoHeader.Compression == BMPCompression.BI_ALPHABITFIELDS)
                {
                    bmpInfoHeader.AlphaMask = binaryReader.ReadUInt32();
                }
            }
            else
            {
                bmpInfoHeader.BlueMask = 0x000000FF;
                bmpInfoHeader.GreenMask = 0x0000FF00;
                bmpInfoHeader.RedMask = 0x00FF0000;
                if (bmpInfoHeader.BitCount == 32)
                {
                    bmpInfoHeader.AlphaMask = 0xFF000000;
                }
                else
                {
                    bmpInfoHeader.AlphaMask = 0;
                }
            }

            //read palette
            if (bmpInfoHeader.BitCount <= 8)
            {
                if (bmpInfoHeader.ColorUsed == 0)
                {
                    bmpInfoHeader.ColorUsed = 1u << bmpInfoHeader.BitCount;
                }

                bmpFile.Palettes = new Color32[bmpInfoHeader.ColorUsed];

                for (int i = 0; i < bmpFile.Palettes.Length; i++)
                {
                    bmpFile.Palettes[i] = new Color32
                    {
                        b = binaryReader.ReadByte(),
                        g = binaryReader.ReadByte(),
                        r = binaryReader.ReadByte(),
                        a = 255
                    };
                    binaryReader.ReadByte();
                }
            }

            bmpFile.InfoHeader = bmpInfoHeader;

            //read data
            bool uncompressed = bmpInfoHeader.Compression == BMPCompression.BI_RGB ||
                                bmpInfoHeader.Compression == BMPCompression.BI_BITFIELDS ||
                                bmpInfoHeader.Compression == BMPCompression.BI_ALPHABITFIELDS;

            if (uncompressed)
            {
                //read 32 bit bmp image
                if (bmpInfoHeader.BitCount == 32)
                {
                    if (Load32BitBMPFile(binaryReader, ref bmpFile) == false)
                    {
                        Debugger.LogError(
                            $"File is corrupted: {filepath}\nCompression: {bmpInfoHeader.Compression}\nMode: {bmpInfoHeader.BitCount}bit",
                            Instance, nameof(ImageLoader));
                        return null;
                    }

                    return bmpFile;
                }

                //read 24 bit bmp image
                if (bmpInfoHeader.BitCount == 24)
                {
                    if (Load24BitBMPFile(binaryReader, ref bmpFile) == false)
                    {
                        Debugger.LogError(
                            $"File is corrupted: {filepath}\nCompression: {bmpInfoHeader.Compression}\nMode: {bmpInfoHeader.BitCount}bit",
                            Instance, nameof(ImageLoader));
                        return null;
                    }

                    return bmpFile;
                }

                //read 16 bit bmp image
                if (bmpInfoHeader.BitCount == 16)
                {
                    if (Load16BitBMPFile(binaryReader, ref bmpFile) == false)
                    {
                        Debugger.LogError(
                            $"File is corrupted: {filepath}\nCompression: {bmpInfoHeader.Compression}\nMode: {bmpInfoHeader.BitCount}bit",
                            Instance, nameof(ImageLoader));
                        return null;
                    }

                    return bmpFile;
                }

                //read indexed image
                if (bmpInfoHeader.BitCount <= 8 && bmpFile.Palettes != null)
                {
                    if (LoadIndexedBMPFile(binaryReader, ref bmpFile) == false)
                    {
                        Debugger.LogError(
                            $"File is corrupted: {filepath}\nCompression: {bmpInfoHeader.Compression}\nMode: {bmpInfoHeader.BitCount}bit",
                            Instance, nameof(ImageLoader));
                        return null;
                    }

                    return bmpFile;
                }
            }

            //read RLE compressed image
            if (bmpFile.Palettes != null)
            {
                if (bmpInfoHeader.Compression == BMPCompression.BI_RLE4 && bmpInfoHeader.BitCount == 4)
                {
                    if (LoadRLE4BMPFile(binaryReader, ref bmpFile) == false)
                    {
                        Debugger.LogError(
                            $"File is corrupted: {filepath}\nCompression: {bmpInfoHeader.Compression}\nMode: {bmpInfoHeader.BitCount}bit",
                            Instance, nameof(ImageLoader));
                        return null;
                    }

                    return bmpFile;
                }

                if (bmpInfoHeader.Compression == BMPCompression.BI_RLE8 && bmpInfoHeader.BitCount == 8)
                {
                    if (LoadRLE8BMPFile(binaryReader, ref bmpFile) == false)
                    {
                        Debugger.LogError(
                            $"File is corrupted: {filepath}\nCompression: {bmpInfoHeader.Compression}\nMode: {bmpInfoHeader.BitCount}bit",
                            Instance, nameof(ImageLoader));
                        return null;
                    }

                    return bmpFile;
                }
            }

            Debugger.LogError(
                $"File is corrupted: {filepath}\nCompression: Unknown \nMode: Unknown",
                Instance, nameof(ImageLoader));
            return null;
        }

        private static BMPFileHeader LoadBMPFileHeader(BinaryReader binaryReader)
        {
            var fileHeader = new BMPFileHeader();
            fileHeader.Size = binaryReader.ReadUInt32();
            binaryReader.ReadInt32();
            fileHeader.Offset = binaryReader.ReadUInt32();
            return fileHeader;
        }

        private static BMPInfoHeader LoadBMPInfoHeader(BinaryReader binaryReader)
        {
            var infoHeader = new BMPInfoHeader();
            infoHeader.Size = binaryReader.ReadUInt32();
            infoHeader.Width = binaryReader.ReadInt32();
            infoHeader.Height = binaryReader.ReadInt32();
            binaryReader.ReadInt16();
            infoHeader.BitCount = binaryReader.ReadUInt16();
            infoHeader.Compression = (BMPCompression)binaryReader.ReadUInt32();
            infoHeader.SizeImage = binaryReader.ReadUInt32();
            infoHeader.XPixelPerMeter = binaryReader.ReadInt32();
            infoHeader.YPixelPerMeter = binaryReader.ReadInt32();
            infoHeader.ColorUsed = binaryReader.ReadUInt32();
            infoHeader.ColorImportant = binaryReader.ReadUInt32();

            return infoHeader;
        }

        private static bool Load32BitBMPFile(BinaryReader binaryReader, ref BMPFile bmpFile)
        {
            int width = Mathf.Abs(bmpFile.InfoHeader.Width);
            int height = Mathf.Abs(bmpFile.InfoHeader.Height);
            bmpFile.Pixels = new Color32[width * height];

            long rowCount = (long)width * height * 4;
            
            if (binaryReader.BaseStream.Position + rowCount > binaryReader.BaseStream.Length)
            {
                Debugger.LogError(
                    $"Unexpected end of file.\nHave: {binaryReader.BaseStream.Position + rowCount}\nExpected: {binaryReader.BaseStream.Length}",
                    Instance, nameof(ImageLoader));
                return false;
            }

            int shiftR = GetShiftCount(bmpFile.InfoHeader.RedMask);
            int shiftG = GetShiftCount(bmpFile.InfoHeader.GreenMask);
            int shiftB = GetShiftCount(bmpFile.InfoHeader.BlueMask);
            int shiftA = GetShiftCount(bmpFile.InfoHeader.AlphaMask);

            int bitsR = CountSetBits(bmpFile.InfoHeader.RedMask);
            int bitsG = CountSetBits(bmpFile.InfoHeader.GreenMask);
            int bitsB = CountSetBits(bmpFile.InfoHeader.BlueMask);
            int bitsA = CountSetBits(bmpFile.InfoHeader.AlphaMask);

            uint maxR = (1u << bitsR) - 1;
            if (maxR == 0) maxR = 255;
            uint maxG = (1u << bitsG) - 1;
            if (maxG == 0) maxG = 255;
            uint maxB = (1u << bitsB) - 1;
            if (maxB == 0) maxB = 255;
            uint maxA = (1u << bitsA) - 1;
            if (maxA == 0) maxA = 255;

            for (int y = 0; y < height; y++)
            {
                //flip
                int pixelY = (bmpFile.InfoHeader.Height > 0) ? y : (height - 1 - y);

                for (int x = 0; x < width; x++)
                {
                    uint value = binaryReader.ReadUInt32();

                    byte r = (byte)((((value & bmpFile.InfoHeader.RedMask) >> shiftR) * 255) / maxR);
                    byte g = (byte)((((value & bmpFile.InfoHeader.GreenMask) >> shiftG) * 255) / maxG);
                    byte b = (byte)((((value & bmpFile.InfoHeader.BlueMask) >> shiftB) * 255) / maxB);
                    byte a;

                    if (bitsA > 0)
                    {
                        a = (byte)((((value & bmpFile.InfoHeader.AlphaMask) >> shiftA) * 255) / maxA);
                    }
                    else
                    {
                        a = 255;
                    }

                    bmpFile.Pixels[pixelY * width + x] = new Color32(r, g, b, a);
                }
            }

            return true;
        }


        private static bool Load24BitBMPFile(BinaryReader binaryReader, ref BMPFile bmpFile)
        {
            int width = Mathf.Abs(bmpFile.InfoHeader.Width);
            int height = Mathf.Abs(bmpFile.InfoHeader.Height);

            int rowLength = ((24 * width + 31) / 32) * 4;
            int rowCount = rowLength * height;
            int rowPadding = rowLength - width * 3;
            bmpFile.Pixels = new Color32[width * height];

            if (binaryReader.BaseStream.Position + rowCount > binaryReader.BaseStream.Length)
            {
                Debugger.LogError(
                    $"Unexpected end of file.\nHave: {binaryReader.BaseStream.Position + rowCount}\nExpected: {binaryReader.BaseStream.Length}",
                    Instance, nameof(ImageLoader));
                return false;
            }

            int shiftR = GetShiftCount(bmpFile.InfoHeader.RedMask);
            int shiftG = GetShiftCount(bmpFile.InfoHeader.GreenMask);
            int shiftB = GetShiftCount(bmpFile.InfoHeader.BlueMask);

            int bitsR = CountSetBits(bmpFile.InfoHeader.RedMask);
            int bitsG = CountSetBits(bmpFile.InfoHeader.GreenMask);
            int bitsB = CountSetBits(bmpFile.InfoHeader.BlueMask);

            uint maxR = (1u << bitsR) - 1;
            if (maxR == 0) maxR = 255;
            uint maxG = (1u << bitsG) - 1;
            if (maxG == 0) maxG = 255;
            uint maxB = (1u << bitsB) - 1;
            if (maxB == 0) maxB = 255;

            for (int y = 0; y < height; y++)
            {
                //flip
                int pixelY = (bmpFile.InfoHeader.Height > 0) ? y : (height - 1 - y);

                for (int x = 0; x < width; x++)
                {
                    uint value = (uint)(binaryReader.ReadByte() | binaryReader.ReadByte() << 8 |
                                        binaryReader.ReadByte() << 16);

                    byte r = (byte)((((value & bmpFile.InfoHeader.RedMask) >> shiftR) * 255) / maxR);
                    byte g = (byte)((((value & bmpFile.InfoHeader.GreenMask) >> shiftG) * 255) / maxG);
                    byte b = (byte)((((value & bmpFile.InfoHeader.BlueMask) >> shiftB) * 255) / maxB);

                    bmpFile.Pixels[pixelY * width + x] = new Color32(r, g, b, 255);
                }

                for (int i = 0; i < rowPadding; i++)
                {
                    binaryReader.ReadByte();
                }
            }

            return true;
        }


        private static bool Load16BitBMPFile(BinaryReader binaryReader, ref BMPFile bmpFile)
        {
            int width = Mathf.Abs(bmpFile.InfoHeader.Width);
            int height = Mathf.Abs(bmpFile.InfoHeader.Height);

            int rowLength = ((16 * width + 31) / 32) * 4;
            int rowCount = rowLength * height;
            int rowPadding = rowLength - width * 2;
            bmpFile.Pixels = new Color32[width * height];

            if (binaryReader.BaseStream.Position + rowCount > binaryReader.BaseStream.Length)
            {
                Debugger.LogError(
                    $"Unexpected end of file.\nHave: {binaryReader.BaseStream.Position + rowCount}\nExpected: {binaryReader.BaseStream.Length}",
                    Instance, nameof(ImageLoader));
                return false;
            }

            int shiftR = GetShiftCount(bmpFile.InfoHeader.RedMask);
            int shiftG = GetShiftCount(bmpFile.InfoHeader.GreenMask);
            int shiftB = GetShiftCount(bmpFile.InfoHeader.BlueMask);
            int shiftA = GetShiftCount(bmpFile.InfoHeader.AlphaMask);

            int bitsR = CountSetBits(bmpFile.InfoHeader.RedMask);
            int bitsG = CountSetBits(bmpFile.InfoHeader.GreenMask);
            int bitsB = CountSetBits(bmpFile.InfoHeader.BlueMask);
            int bitsA = CountSetBits(bmpFile.InfoHeader.AlphaMask);

            uint maxR = (1u << bitsR) - 1;
            if (maxR == 0) maxR = 255;
            uint maxG = (1u << bitsG) - 1;
            if (maxG == 0) maxG = 255;
            uint maxB = (1u << bitsB) - 1;
            if (maxB == 0) maxB = 255;
            uint maxA = (1u << bitsA) - 1;
            if (maxA == 0) maxA = 255;

            for (int y = 0; y < height; y++)
            {
                //flip
                int pixelY = (bmpFile.InfoHeader.Height > 0) ? y : (height - 1 - y);

                for (int x = 0; x < width; x++)
                {
                    uint value = (uint)(binaryReader.ReadByte() | binaryReader.ReadByte() << 8);
                    byte r = (byte)((((value & bmpFile.InfoHeader.RedMask) >> shiftR) * 255) / maxR);
                    byte g = (byte)((((value & bmpFile.InfoHeader.GreenMask) >> shiftG) * 255) / maxG);
                    byte b = (byte)((((value & bmpFile.InfoHeader.BlueMask) >> shiftB) * 255) / maxB);
                    byte a;

                    if (bitsA > 0)
                    {
                        a = (byte)((((value & bmpFile.InfoHeader.AlphaMask) >> shiftA) * 255) / maxA);
                    }
                    else
                    {
                        a = 255;
                    }

                    bmpFile.Pixels[pixelY * width + x] = new Color32(r, g, b, a);
                }

                for (int i = 0; i < rowPadding; i++)
                {
                    binaryReader.ReadByte();
                }
            }

            return true;
        }

        private static bool LoadIndexedBMPFile(BinaryReader binaryReader, ref BMPFile bmpFile)
        {
            int width = Mathf.Abs(bmpFile.InfoHeader.Width);
            int height = Mathf.Abs(bmpFile.InfoHeader.Height);

            int rowLength = ((bmpFile.InfoHeader.BitCount * width + 31) / 32) * 4;
            int rowCount = rowLength * height;
            int rowPadding = rowLength - (width * bmpFile.InfoHeader.BitCount + 7) / 8;
            bmpFile.Pixels = new Color32[width * height];
            
            if (binaryReader.BaseStream.Position + rowCount > binaryReader.BaseStream.Length)
            {
                Debugger.LogError(
                    $"Unexpected end of file.\nHave: {binaryReader.BaseStream.Position + rowCount}\nExpected: {binaryReader.BaseStream.Length}",
                    Instance, nameof(ImageLoader));
                return false;
            }

            var bitReader = new BitReader(binaryReader.BaseStream, Encoding.Default, true);
            for (int y = 0; y < height; y++)
            {
                //flip
                int pixelY = (bmpFile.InfoHeader.Height > 0) ? y : (height - 1 - y);
                
                for (int x = 0; x < width; x++)
                {
                    int value = (int)bitReader.ReadBits(bmpFile.InfoHeader.BitCount);
                    if (value >= bmpFile.Palettes.Length)
                    {
                        Debugger.LogError(
                            "Palette index out of range.",
                            Instance, nameof(ImageLoader));
                        return false;
                    }

                    bmpFile.Pixels[pixelY * width + x] = bmpFile.Palettes[value];
                }

                bitReader.ResetBitBuffer();

                for (int i = 0; i < rowPadding; i++)
                {
                    binaryReader.ReadByte();
                }
            }

            return true;
        }

        private static bool LoadRLE4BMPFile(BinaryReader binaryReader, ref BMPFile bmpFile)
        {
            int width = Mathf.Abs(bmpFile.InfoHeader.Width);
            int height = Mathf.Abs(bmpFile.InfoHeader.Height);
            bmpFile.Pixels = new Color32[width * height];

            int x = 0, y = 0, yOffset = 0;

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - 1)
            {
                int count = binaryReader.ReadByte();
                byte d = binaryReader.ReadByte();
                if (count > 0)
                {
                    for (int i = (count / 2); i > 0; i--)
                    {
                        bmpFile.Pixels[x++ + yOffset] = bmpFile.Palettes[(d >> 4) & 0x0F];
                        bmpFile.Pixels[x++ + yOffset] = bmpFile.Palettes[d & 0x0F];
                    }

                    if ((count & 0x01) > 0)
                    {
                        bmpFile.Pixels[x++ + yOffset] = bmpFile.Palettes[(d >> 4) & 0x0F];
                    }
                }
                else
                {
                    if (d == 0)
                    {
                        x = 0;
                        y += 1;
                        yOffset = y * width;
                    }
                    else if (d == 1)
                    {
                        break;
                    }
                    else if (d == 2)
                    {
                        x += binaryReader.ReadByte();
                        y += binaryReader.ReadByte();
                        yOffset = y * width;
                    }
                    else
                    {
                        for (int i = (d / 2); i > 0; i--)
                        {
                            byte d2 = binaryReader.ReadByte();
                            bmpFile.Pixels[x++ + yOffset] = bmpFile.Palettes[(d2 >> 4) & 0x0F];
                            if (x + 1 < width)
                            {
                                bmpFile.Pixels[x++ + yOffset] = bmpFile.Palettes[d2 & 0x0F];
                            }
                        }

                        if ((d & 0x01) > 0)
                        {
                            bmpFile.Pixels[x++ + yOffset] =
                                bmpFile.Palettes[(binaryReader.ReadByte() >> 4) & 0x0F];
                        }

                        if ((((d - 1) / 2) & 1) == 0)
                        {
                            binaryReader.ReadByte();
                        }
                    }
                }
            }

            FlipHeight(ref bmpFile);

            return true;
        }

        private static bool LoadRLE8BMPFile(BinaryReader binaryReader, ref BMPFile bmpFile)
        {
            int width = Mathf.Abs(bmpFile.InfoHeader.Width);
            int height = Mathf.Abs(bmpFile.InfoHeader.Height);
            bmpFile.Pixels = new Color32[width * height];

            int x = 0, y = 0, yOffset = 0;
            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - 1)
            {
                int count = binaryReader.ReadByte();
                byte d = binaryReader.ReadByte();
                if (count > 0)
                {
                    for (int i = count; i > 0; i--)
                    {
                        bmpFile.Pixels[x++ + yOffset] = bmpFile.Palettes[d];
                    }
                }
                else
                {
                    if (d == 0)
                    {
                        x = 0;
                        y += 1;
                        yOffset = y * width;
                    }
                    else if (d == 1)
                    {
                        break;
                    }
                    else if (d == 2)
                    {
                        x += binaryReader.ReadByte();
                        y += binaryReader.ReadByte();
                        yOffset = y * width;
                    }
                    else
                    {
                        for (int i = d; i > 0; i--)
                        {
                            bmpFile.Pixels[x++ + yOffset] = bmpFile.Palettes[binaryReader.ReadByte()];
                        }

                        if ((d & 0x01) > 0)
                        {
                            binaryReader.ReadByte();
                        }
                    }
                }
            }

            FlipHeight(ref bmpFile);
            
            return true;
        }

        private static void FlipHeight(ref BMPFile bmpFile)
        {
            int width = Mathf.Abs(bmpFile.InfoHeader.Width);
            int height = Mathf.Abs(bmpFile.InfoHeader.Height);
            int center = height / 2;

            for (int y = 0; y < center; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var offset0 = y * width;
                    var offset1 = (height - y - 1) * width;
                    (bmpFile.Pixels[offset0], bmpFile.Pixels[offset1]) =
                        (bmpFile.Pixels[offset1], bmpFile.Pixels[offset0]);
                }
            }

            bmpFile.InfoHeader.Height = height;
        }

        private static int GetShiftCount(uint mask)
        {
            if (mask == 0) return 0;

            for (int i = 0; i < 32; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    return i;
                }
            }

            return 0;
        }

        private static int CountSetBits(uint n)
        {
            int count = 0;
            while (n > 0)
            {
                n &= (n - 1);
                count++;
            }

            return count;
        }
    }
}