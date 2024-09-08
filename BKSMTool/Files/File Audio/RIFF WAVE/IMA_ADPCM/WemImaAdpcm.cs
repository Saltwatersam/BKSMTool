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

using System.Collections.Generic;
using System.Text;
using System;
using System.IO;

namespace BKSMTool.Files.File_Audio.RIFF_WAVE.IMA_ADPCM
{
    
    /// <summary> 
    /// Handles the conversion of WEM files (IMA-ADPCM format) to standard audio (IMA ADPCM format) and vice versa.
    /// </summary>
    /// <remarks> 
    /// <b>WEM:</b>
    /// <br/> - Uses RIFF wave format files and customizes the Extensible sub-chunk of the 'fmt ' chunk.
    /// <br/> Inside the Extensible sub-chunk, it uses a serialized configuration instead of <c>WChannelMask</c> and <c>SubFormat</c>.
    /// <br/>
    /// <br/> - (IMA-ADPCM format) Uses code 0x0002 as <c>wFormatTag</c>, while standard IMA-ADPCM uses 0x0011.
    /// <br/>
    /// <br/> - (IMA-ADPCM format) Places data chained by channel when there is more than one channel, while standard IMA-ADPCM places data interleaved.
    /// </remarks>
    public class WemImaAdpcm : AudioWaveFormatExtensible, IDisposable
    {
        #region Properties
        // Implement IDisposable pattern
        private bool _disposed;
        #endregion

        #region Methods

        #region Convert WEM audio format IMA-ADPCM to standard IMA-ADPCM
        /// <summary> 
        /// Converts a WEM audio array (IMA-ADPCM format) to a standard audio array (IMA-ADPCM format).
        /// </summary>
        /// <param name="audio">A byte array containing the WEM audio in IMA-ADPCM format.</param>
        /// <returns>A byte array containing the converted standard audio in IMA-ADPCM format.</returns>
        /// <exception cref="Exception">Thrown when the input data is invalid or conversion fails.</exception>
        public byte[] ConvertToStandardImaAdpcm(byte[] audio)
        {
            //*******************************************//
            // Parse and extract audio from RIFF Wave Extensible //
            //*******************************************//
            try
            {
                ExtractAudioRiffExtensible(audio);
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid RIFF WaveFormat audio -> " + ex.Message);
            }

            //*******************************************//
            // Parsing is done, check parameters found    //
            //*******************************************//
            if (NChannels == 0)
            {
                throw new Exception("No channel configuration found.");
            }
            if (NSamplesPerSec == 0)
            {
                throw new Exception("No sample rate found.");
            }
            if (WBitsPerSample == 0)
            {
                throw new Exception("No bits per sample found.");
            }

            if (NBlockAlign == 0)
            {
                NBlockAlign = (ushort)(Math.Ceiling((decimal)(NChannels * WBitsPerSample) / 8));
            }

            if (NAvgBytesPerSec == 0)
            {
                NAvgBytesPerSec = NSamplesPerSec * NBlockAlign;
            }

            if (WValidBitsPerSample == 0)
            {
                WValidBitsPerSample = (ushort)((((NBlockAlign - (4 * NChannels)) * 8) / (WBitsPerSample * NChannels)) + 1);
            }

            var numberOfSamplesPerChannel = (uint)((Data.Length / NBlockAlign) * WValidBitsPerSample) / NChannels;

            // If audio is multichannel, data must be interleaved back to standard IMA ADPCM since WEM (IMA-ADPCM) places data chained by channel.
            if (NChannels > 1)
            {
                // Size of block per channel
                var blockSizePerChannel = NBlockAlign / NChannels;

                // Number of blocks per channel
                var numBlocksPerChannel = Data.Length / (NBlockAlign / NChannels);

                // Array to store converted data
                using (var convertedData = new MemoryStream())
                {
                    if (Data.Length % (NBlockAlign / NChannels) != 0)
                    {
                        throw new Exception("Data size is not correct.");
                    }

                    var samples = new List<byte[]>();
                    using (var ms = new MemoryStream(Data))
                    {
                        long offset = 0;
                        // Get samples of each channel
                        for (int i = 0; i < numBlocksPerChannel; i++)
                        {
                            samples.Add(FileOperations.ReadBytes(ms, offset, blockSizePerChannel));
                            offset += blockSizePerChannel;
                        }
                    }

                    // Interleave samples
                    for (var i = 0; i < numBlocksPerChannel; i += NChannels)
                    {
                        for (var j = 0; j < blockSizePerChannel / 4; j++)
                        {
                            convertedData.Write(samples[i], j * 4, 4);
                            convertedData.Write(samples[i + 1], j * 4, 4);
                        }
                    }

                    samples.Clear();
                    Data = convertedData.ToArray();
                }
            }

            //******************************************//
            // Rebuild audio to standard IMA ADPCM       //
            //******************************************//
            using (var convertedAudioStream = new MemoryStream())
            {
                convertedAudioStream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(Data.Length + 0x34), 0, 4);
                convertedAudioStream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);
                convertedAudioStream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(0x14), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(0x11), 0, 2);
                convertedAudioStream.Write(BitConverter.GetBytes(NChannels), 0, 2);
                convertedAudioStream.Write(BitConverter.GetBytes(NSamplesPerSec), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(NAvgBytesPerSec), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(NBlockAlign), 0, 2);
                convertedAudioStream.Write(BitConverter.GetBytes(WBitsPerSample), 0, 2);
                convertedAudioStream.Write(BitConverter.GetBytes(0x02), 0, 2);
                convertedAudioStream.Write(BitConverter.GetBytes(WValidBitsPerSample), 0, 2);
                convertedAudioStream.Write(Encoding.ASCII.GetBytes("fact"), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(0x04), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(numberOfSamplesPerChannel), 0, 4);
                convertedAudioStream.Write(Encoding.ASCII.GetBytes("data"), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(Data.Length), 0, 4);
                convertedAudioStream.Write(Data, 0, Data.Length);

                return convertedAudioStream.ToArray();
            }
        }
        #endregion

        #endregion

        #region Dispose

        /// <summary>
        /// Releases all resources used by the <see cref="WemImaAdpcm"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="WemImaAdpcm"/> and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                WValidBitsPerSample = 0;
                WChannelMask = 0;
                SubFormat = Guid.Empty;

                WFormatTag = 0;
                NChannels = 0;
                NSamplesPerSec = 0;
                NAvgBytesPerSec = 0;
                NBlockAlign = 0;
                WBitsPerSample = 0;
                CbSize = 0;
                DataSize = 0;
                Data = null;
            }

            // Dispose unmanaged resources

            _disposed = true;
        }

        /// <summary>
        /// Destructor to ensure resources are released when the object is garbage collected.
        /// </summary>
        ~WemImaAdpcm()
        {
            Dispose(false);
        }

        #endregion
    }
}
