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
using System;
using System.IO;

namespace BKSMTool.Files.File_Audio.RIFF_WAVE
{
    /// <summary>
    /// Represents an extensible audio wave format, extending the basic <see cref="AudioWaveFormatEx"/> class to handle additional
    /// audio format information such as valid bits per sample, channel mask, and sub format.
    /// </summary>
    public class AudioWaveFormatExtensible : AudioWaveFormatEx
    {
        #region Properties

        /// <summary>
        /// Bits of precision, calculated as:
        /// (((wBlockAlign - (4 * wChannels)) * 8) / (wBitsPerSample * wChannels)) + 1.
        /// </summary>
        protected ushort WValidBitsPerSample;

        /// <summary>
        /// Bitmask representing which channels are present in the stream.
        /// </summary>
        protected uint WChannelMask;

        /// <summary>
        /// The GUID representing the format's sub format.
        /// </summary>
        protected Guid SubFormat;

        #endregion

        #region Methods
        /// <summary>
        /// Parses and extracts audio information from the RIFF WAVE header contained in the provided audio byte array.
        /// </summary>
        /// <param name="audio">A byte array containing the RIFF WAVE audio data.</param>
        /// <exception cref="Exception">Thrown if the audio data does not contain a valid RIFF WAVE header or is incomplete.</exception>
        public void ExtractAudioRiffExtensible(byte[] audio)
        {
            if (audio.Length < 0x08)
            {
                throw new Exception("Audio size is too small to have a header.");
            }

            using (var ms = new MemoryStream(audio))
            {
                long offset = 0;
                var dataChunkFound = false;

                // Check if the file has a valid RIFF header
                var fourCcRead = FileOperations.ReadFourCc(ms, offset);
                if (fourCcRead[0] != 'R' || fourCcRead[1] != 'I' || fourCcRead[2] != 'F' || fourCcRead[3] != 'F')
                {
                    throw new Exception($"Unrecognized header: '{fourCcRead}'.");
                }
                offset += 4;

                // Check the size indicated in the header
                var fileSize = FileOperations.ReadUint32(ms, offset);
                if (fileSize + 8 != ms.Length)
                {
                    throw new Exception("RIFF WAVE audio defined size is incorrect.");
                }
                if (fileSize < 0x12)
                {
                    throw new Exception("RIFF WAVE audio defined size is too small.");
                }
                offset += 4;

                // Check for WAVE format
                fourCcRead = FileOperations.ReadFourCc(ms, offset);
                if (fourCcRead[0] != 'W' || fourCcRead[1] != 'A' || fourCcRead[2] != 'V' || fourCcRead[3] != 'E')
                {
                    throw new Exception($"Unrecognized WAVE format: '{fourCcRead}'.");
                }
                offset += 4;

                // Check for the fmt chunk
                fourCcRead = FileOperations.ReadFourCc(ms, offset);
                if (fourCcRead[0] != 'f' || fourCcRead[1] != 'm' || fourCcRead[2] != 't' || fourCcRead[3] != ' ')
                {
                    throw new Exception($"Unrecognized fmt chunk: '{fourCcRead}'.");
                }
                offset += 4;

                // Read fmt chunk size
                var fmtSize = FileOperations.ReadUint32(ms, offset);
                if (fmtSize < 0x10)
                {
                    throw new Exception("fmt chunk size is not correct.");
                }
                offset += 4;

                // Read the audio format information
                WFormatTag = FileOperations.ReadUint16(ms, offset);
                offset += 2;

                NChannels = FileOperations.ReadUint16(ms, offset);
                offset += 2;

                NSamplesPerSec = FileOperations.ReadUint32(ms, offset);
                offset += 4;

                NAvgBytesPerSec = FileOperations.ReadUint32(ms, offset);
                offset += 4;

                NBlockAlign = FileOperations.ReadUint16(ms, offset);
                offset += 2;

                WBitsPerSample = FileOperations.ReadUint16(ms, offset);
                offset += 2;

                // Read extra data if present
                if (fmtSize >= 0x12)
                {
                    CbSize = FileOperations.ReadUint16(ms, offset);
                    offset += 2;
                }

                if (fmtSize != 0x10 + 0x02 + CbSize)
                {
                    throw new Exception("fmt chunk size is incorrect, does not include the correct size of extra data.");
                }

                // Handle extra data if available
                if (CbSize >= 0x06)
                {
                    WValidBitsPerSample = FileOperations.ReadUint16(ms, offset);
                    offset += 2;

                    WChannelMask = FileOperations.ReadUint32(ms, offset);
                    offset += 4;

                    // Skip the remaining extra data if present
                    offset += CbSize - 6;
                }
                else
                {
                    offset += CbSize;
                }

                // Search for the data chunk
                while (offset + 4 < ms.Length)
                {
                    fourCcRead = FileOperations.ReadFourCc(ms, offset);
                    if (fourCcRead[0] == 'd' && fourCcRead[1] == 'a' && fourCcRead[2] == 't' && fourCcRead[3] == 'a')
                    {
                        offset += 4;
                        dataChunkFound = true;
                        break;
                    }
                    offset += 4;

                    if (offset + 4 >= ms.Length) continue;
                    var chunkSize = FileOperations.ReadUint32(ms, offset);
                    offset += 4 + chunkSize;
                }

                if (!dataChunkFound)
                {
                    throw new Exception("No data chunk found.");
                }

                // Read the size of the data chunk
                DataSize = FileOperations.ReadUint32(ms, offset);
                offset += 4;

                if (DataSize + offset > ms.Length)
                {
                    throw new Exception("Data size is incorrect.");
                }
                if (DataSize == 0)
                {
                    throw new Exception("Data size is 0.");
                }

                // Read the data
                Data = FileOperations.ReadBytes(ms, offset, DataSize);
            }
        }
        #endregion
    }
}
