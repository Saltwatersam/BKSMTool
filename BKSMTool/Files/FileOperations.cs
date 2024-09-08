// Copyright (c) 2024 Saltwatersam
// 
// This file is part of BKSMTool.
// 
// BKSMTool is licensed under the GPLv3 License:
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using System.Text;
using System;
using System.IO;

namespace BKSMTool.Files
{
    /// <summary>
    /// Provides utility methods for file operations such as reading and writing data,
    /// working with byte arrays, and converting file sizes into human-readable formats.
    /// </summary>
    public static class FileOperations
    {
        #region Start Filestream of the Given File

        /// <summary>
        /// Opens a file in a <see cref="FileStream"/> with specified mode, access, and share options.
        /// </summary>
        /// <param name="selectedFilePath">The path of the file to load.</param>
        /// <param name="fileMode">Specifies how the file should be opened.</param>
        /// <param name="fileAccess">Specifies the type of access to the file.</param>
        /// <param name="fileShare">Specifies the level of access others have to the file while it is open.</param>
        /// <returns>A <see cref="FileStream"/> object for the opened file.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while opening the file.</exception>
        public static FileStream StartFileStream(string selectedFilePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                throw new ArgumentException("You did not supply a file path.");
            }

            try
            {
                return new FileStream(selectedFilePath, fileMode, fileAccess, fileShare);
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                throw new FileNotFoundException("The file or directory cannot be found.", ex);
            }
            catch (DriveNotFoundException)
            {
                throw new DriveNotFoundException("The drive specified in 'path' is invalid.");
            }
            catch (PathTooLongException)
            {
                throw new PathTooLongException("'path' exceeds the maximum supported path length.");
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("You do not have permission to access this file.");
            }
            catch (IOException e) when ((e.HResult & 0x0000FFFF) == 32)
            {
                throw new IOException("There is a sharing violation.", e);
            }
            catch (IOException e) when ((e.HResult & 0x0000FFFF) == 80)
            {
                throw new IOException("The file already exists.", e);
            }
            catch (IOException e)
            {
                throw new IOException($"An unexpected I/O error occurred: {e.Message}", e);
            }
        }

        #endregion

        #region Convert File Size to Readable Format

        /// <summary>
        /// Converts a file size in bytes into a human-readable format (e.g., KB, MB, GB).
        /// </summary>
        /// <param name="value">The file size in bytes.</param>
        /// <returns>A string representing the file size with an appropriate unit.</returns>
        public static string SizeToReadableFormat(long value)
        {
            var absoluteValue = Math.Abs(value);
            string suffix;
            double readable;

            if (absoluteValue >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (value >> 50);
            }
            else if (absoluteValue >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (value >> 40);
            }
            else if (absoluteValue >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (value >> 30);
            }
            else if (absoluteValue >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (value >> 20);
            }
            else if (absoluteValue >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (value >> 10);
            }
            else if (absoluteValue >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = value;
            }
            else
            {
                return value.ToString("0 B"); // Byte
            }

            readable /= 1024; // Convert to the fractional value
            return $"{readable:0.##} {suffix}";
        }

        #endregion

        #region Add Left Padding

        /// <summary>
        /// Adds padding to a byte array to make its length a multiple of a given value.
        /// </summary>
        /// <param name="array">The byte array to pad.</param>
        /// <param name="pad">The target multiple for the padding.</param>
        public static void PadToMultipleOf(ref byte[] array, int pad)
        {
            var len = pad * ((array.Length + pad - 1) / pad); // Calculate the new length
            Array.Resize(ref array, len); // Resize the array to the new length
        }

        #endregion

        #region Path Shortener

        /// <summary>
        /// Shrinks a file path to fit within a specified maximum length.
        /// </summary>
        /// <param name="path">The file path to shrink.</param>
        /// <param name="maxLength">The maximum allowed length of the file path.</param>
        /// <returns>A shortened version of the path, if necessary.</returns>
        public static string ShrinkPath(string path, int maxLength)
        {
            var parts = path.Split('\\');
            var output = string.Join("\\", parts, 0, parts.Length);
            var endIndex = parts.Length - 2;
            var startIndex = endIndex / 2;
            var index = startIndex;
            var step = 0;

            while (output.Length >= maxLength && index != 0 && index != endIndex)
            {
                parts[index] = "...";
                output = string.Join("\\", parts);
                step = step >= 0 ? ++step : step * -1;
                index = startIndex + step;
            }

            return output;
        }

        #endregion

        #region Read FourCC

        /// <summary>
        /// Reads a FourCC (four-character code) from a memory stream at a specified offset.
        /// </summary>
        /// <param name="ms">The memory stream to read from.</param>
        /// <param name="startOrigin">The starting position in the stream.</param>
        /// <returns>A character array representing the FourCC code.</returns>
        public static char[] ReadFourCc(MemoryStream ms, long startOrigin)
        {
            if (startOrigin + 4 > ms.Length)
            {
                throw new ArgumentException("Offset is out of range while reading FourCC.");
            }

            var temp = new byte[4];
            ms.Seek(startOrigin, SeekOrigin.Begin);
            _ = ms.Read(temp, 0, 4);
            return Encoding.ASCII.GetChars(temp);
        }

        #endregion

        #region Read Uint32

        /// <summary>
        /// Reads a 32-bit unsigned integer from a file stream at a specified offset.
        /// </summary>
        /// <param name="fs">The file stream to read from.</param>
        /// <param name="startOrigin">The starting position in the stream.</param>
        /// <returns>The read 32-bit unsigned integer.</returns>
        public static uint ReadUint32(FileStream fs, long startOrigin)
        {
            if (startOrigin + 4 > fs.Length)
            {
                throw new ArgumentException("Offset reading is out of range.");
            }

            var temp = new byte[4];
            fs.Seek(startOrigin, SeekOrigin.Begin);
            _ = fs.Read(temp, 0, 4);
            return ((uint)temp[3] << 24) | ((uint)temp[2] << 16) | ((uint)temp[1] << 8) | temp[0];
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer from a memory stream at a specified offset.
        /// </summary>
        /// <param name="ms">The memory stream to read from.</param>
        /// <param name="startOrigin">The starting position in the stream.</param>
        /// <returns>The read 32-bit unsigned integer.</returns>
        public static uint ReadUint32(MemoryStream ms, long startOrigin)
        {
            if (startOrigin + 4 > ms.Length)
            {
                throw new ArgumentException("Offset reading is out of range.");
            }

            var temp = new byte[4];
            ms.Seek(startOrigin, SeekOrigin.Begin);
            _ = ms.Read(temp, 0, 4);
            return ((uint)temp[3] << 24) | ((uint)temp[2] << 16) | ((uint)temp[1] << 8) | temp[0];
        }

        #endregion

        #region Read Uint16

        /// <summary>
        /// Reads a 16-bit unsigned integer from a file stream at a specified offset.
        /// </summary>
        /// <param name="fs">The file stream to read from.</param>
        /// <param name="startOrigin">The starting position in the stream.</param>
        /// <returns>The read 16-bit unsigned integer.</returns>
        public static ushort ReadUint16(FileStream fs, long startOrigin)
        {
            if (startOrigin + 2 > fs.Length)
            {
                throw new ArgumentException("Offset reading is out of range.");
            }

            var temp = new byte[2];
            fs.Seek(startOrigin, SeekOrigin.Begin);
            _ = fs.Read(temp, 0, 2);
            return (ushort)((temp[1] << 8) | temp[0]);
        }

        /// <summary>
        /// Reads a 16-bit unsigned integer from a memory stream at a specified offset.
        /// </summary>
        /// <param name="ms">The memory stream to read from.</param>
        /// <param name="startOrigin">The starting position in the stream.</param>
        /// <returns>The read 16-bit unsigned integer.</returns>
        public static ushort ReadUint16(MemoryStream ms, long startOrigin)
        {
            if (startOrigin + 2 > ms.Length)
            {
                throw new ArgumentException("Offset reading is out of range.");
            }

            var temp = new byte[2];
            ms.Seek(startOrigin, SeekOrigin.Begin);
            _ = ms.Read(temp, 0, 2);
            return (ushort)((temp[1] << 8) | temp[0]);
        }

        #endregion

        #region Read Defined Byte Length

        /// <summary>
        /// Reads a defined number of bytes from a file stream.
        /// </summary>
        /// <param name="fs">The file stream to read from.</param>
        /// <param name="startOrigin">The starting position in the stream.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>A byte array containing the read data.</returns>
        public static byte[] ReadBytes(FileStream fs, long startOrigin, long length)
        {
            if (startOrigin + length > fs.Length)
            {
                throw new ArgumentException("Offset reading is out of range.");
            }

            var temp = new byte[length];
            fs.Seek(startOrigin, SeekOrigin.Begin);
            _ = fs.Read(temp, 0, temp.Length);
            return temp;
        }

        /// <summary>
        /// Reads a defined number of bytes from a memory stream.
        /// </summary>
        /// <param name="ms">The memory stream to read from.</param>
        /// <param name="startOrigin">The starting position in the stream.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>A byte array containing the read data.</returns>
        public static byte[] ReadBytes(MemoryStream ms, long startOrigin, long length)
        {
            if (startOrigin + length > ms.Length)
            {
                throw new ArgumentException("Offset reading is out of range.");
            }

            var temp = new byte[length];
            ms.Seek(startOrigin, SeekOrigin.Begin);
            _ = ms.Read(temp, 0, temp.Length);
            return temp;
        }

        #endregion

        #region Write Defined Byte Length

        /// <summary>
        /// Writes a defined number of bytes to a memory stream.
        /// </summary>
        /// <param name="startOrigin">The starting position in the stream.</param>
        /// <param name="ms">The memory stream to write to.</param>
        /// <param name="bytesToWrite">The byte array to write.</param>
        public static void WriteBytes(long startOrigin, MemoryStream ms, byte[] bytesToWrite)
        {
            ms.Seek(startOrigin, SeekOrigin.Begin);
            ms.Write(bytesToWrite, 0, bytesToWrite.Length);
        }

        #endregion

        #region Write Uint32

        /// <summary>
        /// Writes a 32-bit unsigned integer to a memory stream.
        /// </summary>
        /// <param name="startOrigin">The starting position in the stream.</param>
        /// <param name="ms">The memory stream to write to.</param>
        /// <param name="uint32ToWrite">The 32-bit unsigned integer to write.</param>
        public static void WriteUint32(long startOrigin, MemoryStream ms, uint uint32ToWrite)
        {
            var buffer = new byte[4];
            buffer[3] = (byte)(uint32ToWrite >> 24);
            buffer[2] = (byte)(uint32ToWrite >> 16);
            buffer[1] = (byte)(uint32ToWrite >> 8);
            buffer[0] = (byte)(uint32ToWrite >> 0);
            ms.Seek(startOrigin, SeekOrigin.Begin);
            ms.Write(buffer, 0, 4);
        }

        #endregion

        #region Write Uint16

        /// <summary>
        /// Writes a 16-bit unsigned integer to a memory stream.
        /// </summary>
        /// <param name="startOrigin">The starting position in the stream.</param>
        /// <param name="ms">The memory stream to write to.</param>
        /// <param name="uint16ToWrite">The 16-bit unsigned integer to write.</param>
        public static void WriteUint16(long startOrigin, MemoryStream ms, ushort uint16ToWrite)
        {
            var buffer = new byte[2];
            buffer[1] = (byte)(uint16ToWrite >> 8);
            buffer[0] = (byte)(uint16ToWrite >> 0);
            ms.Seek(startOrigin, SeekOrigin.Begin);
            ms.Write(buffer, 0, 2);
        }

        #endregion
    }
}