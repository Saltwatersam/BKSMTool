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

namespace BKSMTool.Files.File_Audio.RIFF_WAVE.PCM
{
    /// <summary> 
    /// Handles the conversion of WEM files (PCM format) to standard audio (PCM format) and vice versa. 
    /// </summary>
    /// <remarks> 
    /// <b>WEM:</b>
    /// <br/> - Uses RIFF wave format files and customizes the Extensible sub-chunk of the 'fmt ' chunk.
    /// <br/> - Inside the Extensible sub-chunk, it uses a serialized configuration instead of <c>WChannelMask</c> and <c>SubFormat</c>.
    /// </remarks>
    public class WemPcm : AudioWaveFormatExtensible, IDisposable
    {
        /// <summary>
        /// Enum for standard speaker positions used in WEM files.
        /// </summary>
        [Flags]
        public enum SpeakerPositions : uint
        {
            FrontLeft = 0x1,            // L
            FrontRight = 0x2,           // R
            FrontCenter = 0x4,          // C
            LowFrequency = 0x8,         // LFE
            BackLeft = 0x10,            // RL
            BackRight = 0x20,           // RR
            BackCenter = 0x100,         // RC
            SideLeft = 0x200,           // SL
            SideRight = 0x400,          // SR
            TopCenter = 0x800,          // HC
            TopFrontLeft = 0x1000,      // HL
            TopFrontCenter = 0x2000,    // HLC
            TopFrontRight = 0x4000,     // HR
            TopBackLeft = 0x8000,       // HRL
            TopBackCenter = 0x10000,    // HRC
            TopBackRight = 0x20000,     // HRR
            Reserved = 0x00040000       // T ?
        }

        #region Properties
        // Flag to indicate whether the object has been disposed
        private bool _disposed;
        #endregion

        #region Methods

        #region Convert WEM audio format PCM to standard PCM
        //***************************************************************************************************************//
        /// <summary> 
        /// Converts a WEM audio array (PCM format) to a standard audio array (PCM format).
        /// </summary>
        /// <param name="audio">A byte array containing the WEM audio in PCM format.</param>
        /// <returns>A byte array containing the converted standard audio in PCM format.</returns>
        /// <exception cref="Exception">Thrown when the input data is invalid or conversion fails.</exception>
        //***************************************************************************************************************//
        public byte[] ConvertToStandardPcm(byte[] audio)
        {
            try
            {
                ExtractAudioRiffExtensible(audio);
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid WaveFormat file. " + ex.Message);
            }

            if (NChannels == 0)
            {
                throw new Exception("Invalid WEM (PCM) file. No channel configuration found.");
            }
            if (NSamplesPerSec == 0)
            {
                throw new Exception("Invalid WEM (PCM) file. No sample rate found.");
            }
            if (WBitsPerSample == 0)
            {
                throw new Exception("Invalid WEM (PCM) file. No bits per sample found.");
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

            using (var convertedAudioStream = new MemoryStream())
            {
                convertedAudioStream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(Data.Length + 0x26), 0, 4);
                convertedAudioStream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);
                convertedAudioStream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(0x12), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(0x01), 0, 2);
                convertedAudioStream.Write(BitConverter.GetBytes(NChannels), 0, 2);
                convertedAudioStream.Write(BitConverter.GetBytes(NSamplesPerSec), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(NAvgBytesPerSec), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(NBlockAlign), 0, 2);
                convertedAudioStream.Write(BitConverter.GetBytes(WBitsPerSample), 0, 2);
                convertedAudioStream.Write(BitConverter.GetBytes(0x02), 0, 2);
                convertedAudioStream.Write(Encoding.ASCII.GetBytes("data"), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(Data.Length), 0, 4);
                convertedAudioStream.Write(Data, 0, Data.Length);

                return convertedAudioStream.ToArray();
            }
        }
        #endregion

        #region Convert Standard PCM to WEM audio format PCM
        /// <summary> 
        /// Converts a standard audio array (PCM format) to a WEM audio array (PCM format).
        /// </summary>
        /// <param name="audio">A byte array containing the standard audio in PCM format.</param>
        /// <returns>A byte array containing the WEM audio in PCM format.</returns>
        /// <exception cref="Exception">Thrown when the input data is invalid or conversion fails.</exception>
        public byte[] GenerateWEM_PCM(byte[] audio)
        {
            try
            {
                ExtractAudioRiffExtensible(audio);
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid WaveFormat file. " + ex.Message);
            }

            if (NChannels == 0)
            {
                throw new Exception("Invalid WEM (PCM) file. No channel configuration found.");
            }
            if (NSamplesPerSec == 0)
            {
                throw new Exception("Invalid WEM (PCM) file. No sample rate found.");
            }
            if (WBitsPerSample == 0)
            {
                throw new Exception("Invalid WEM (PCM) file. No bits per sample found.");
            }

            if (NBlockAlign == 0)
            {
                NBlockAlign = (ushort)(Math.Ceiling((decimal)(NChannels * WBitsPerSample) / 8));
            }

            if (NAvgBytesPerSec == 0)
            {
                NAvgBytesPerSec = NSamplesPerSec * NBlockAlign;
            }

            WValidBitsPerSample = 0;

            // Generate channel mask based on the number of channels
            uint channelMask;
            switch (NChannels)
            {
                case 1:
                    channelMask = (uint)SpeakerPositions.FrontCenter; // Mono
                    break;
                case 2:
                    channelMask = (uint)(SpeakerPositions.FrontLeft | SpeakerPositions.FrontRight); // Stereo
                    break;
                case 3:
                    channelMask = (uint)(SpeakerPositions.FrontLeft | SpeakerPositions.FrontRight | SpeakerPositions.LowFrequency); // 2.1
                    break;
                case 4:
                    channelMask = (uint)(SpeakerPositions.FrontLeft | SpeakerPositions.FrontRight | SpeakerPositions.BackLeft | SpeakerPositions.BackRight); // Quadraphonic
                    break;
                case 5:
                    channelMask = (uint)(SpeakerPositions.FrontLeft | SpeakerPositions.FrontRight | SpeakerPositions.BackLeft | SpeakerPositions.BackRight | SpeakerPositions.LowFrequency); // 4.1
                    break;
                case 6:
                    channelMask = (uint)(SpeakerPositions.FrontLeft | SpeakerPositions.FrontRight | SpeakerPositions.FrontCenter | SpeakerPositions.LowFrequency | SpeakerPositions.BackLeft | SpeakerPositions.BackRight); // 5.1
                    break;
                case 7:
                    channelMask = (uint)(SpeakerPositions.FrontLeft | SpeakerPositions.FrontRight | SpeakerPositions.FrontCenter | SpeakerPositions.LowFrequency | SpeakerPositions.BackLeft | SpeakerPositions.BackRight | SpeakerPositions.BackCenter); // 6.1
                    break;
                case 8:
                    channelMask = (uint)(SpeakerPositions.FrontLeft | SpeakerPositions.FrontRight | SpeakerPositions.FrontCenter | SpeakerPositions.LowFrequency | SpeakerPositions.BackLeft | SpeakerPositions.BackRight | SpeakerPositions.SideLeft | SpeakerPositions.SideRight); // 7.1
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(NChannels), @"Unsupported number of channels.");
            }

            using (var convertedAudioStream = new MemoryStream())
            {
                convertedAudioStream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(Data.Length + 0x2C), 0, 4);
                convertedAudioStream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);
                convertedAudioStream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(0x18), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(0xFFFE), 0, 2); // Extensible format
                convertedAudioStream.Write(BitConverter.GetBytes(NChannels), 0, 2);
                convertedAudioStream.Write(BitConverter.GetBytes(NSamplesPerSec), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(NAvgBytesPerSec), 0, 4);
                convertedAudioStream.Write(BitConverter.GetBytes(NBlockAlign), 0, 2);
                convertedAudioStream.Write(BitConverter.GetBytes(WBitsPerSample), 0, 2);
                convertedAudioStream.Write(BitConverter.GetBytes(0x06), 0, 2); // Extra size
                convertedAudioStream.Write(BitConverter.GetBytes(WValidBitsPerSample), 0, 2);
                convertedAudioStream.Write(BitConverter.GetBytes(channelMask), 0, 4);
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
        /// Releases all resources used by the <see cref="WemPcm"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="WemPcm"/> and optionally releases managed resources.
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
        ~WemPcm()
        {
            Dispose(false);
        }
        #endregion
    }
}