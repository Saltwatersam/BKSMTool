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

using RevorbStd;
using BKSMTool.Properties;
using ManagedBass;
using ManagedBass.Enc;
using System.Runtime.InteropServices;
using BKSMTool.Files.File_Audio.RIFF_WAVE.IMA_ADPCM;
using BKSMTool.Files.File_Audio.RIFF_WAVE.PCM;
using System;
using System.IO;
using BKSMTool.audio_player_converter;


namespace BKSMTool.Files.File_Audio
{
    /// <summary>
    /// Provides operations for handling various audio file formats, including extracting, converting, and retrieving audio data properties.
    /// Implements <see cref="IDisposable"/> for proper resource management.
    /// </summary>
    public class AudioOperations : IDisposable
    {
        #region ENUMERATOR OF AUDIO FORMAT
        /// <summary>
        /// Enumeration of supported audio types/formats.
        /// </summary>
        public enum AudioTypeFormat
        {
            Unknown,
            OggVorbis,
            WavPcm,
            WavImaAdpcm,
            Mp3,
            WemVorbis,
            WemImaAdpcm,
            WemPcm,
        }
        #endregion

        #region PROPERTIES
        private bool _isBassInitialized;
        // Flag to indicate whether the object has been disposed
        private bool _disposed;
        #endregion

        #region CONSTUCTOR
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioOperations"/> class and attempts to initialize the ManagedBass library.
        /// </summary>
        public AudioOperations()
        {
            // Attempt to initialize ManagedBass
            try
            {
                _ = BassInstancing.Instance;
                _isBassInitialized = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to initialize Bass.", ex);
            }
        }
        #endregion

        #region METHODS
        #region Get audio format
        /// <summary>
        /// Determines the type of audio file from its byte array data.
        /// </summary>
        /// <param name="audioData">The byte array containing the audio data.</param>
        /// <returns>An <see cref="AudioTypeFormat"/> indicating the detected audio format.</returns>
        public AudioTypeFormat GetAudioFileType(byte[] audioData)
        {
            if (audioData == null)
            {
                throw new Exception("No data.");
            }

            //====================================================================================================//
            //briefly check if the audio data is Mp3
            //====================================================================================================//
            if (audioData.Length >= 3)
            {
                //briefly check if the audio data is an MP3
                if ((audioData[0] == 'I' && audioData[1] == 'D' && audioData[2] == '3') ||
                   (audioData[0] == 0xFF && (audioData[1] & 0xE0) == 0xE0))
                {
                    return AudioTypeFormat.Mp3;
                }
                //else check if the audio data is WAV/WEM/OGG
            }
            else
            {
                throw new Exception("Data is too small.");
            }

            //====================================================================================================//
            //briefly check if the audio data is a WAV
            //====================================================================================================//
            if (audioData.Length >= 36)
            {
                //briefly check if the audio data is a WAV
                if (audioData[0] == 'R' && audioData[1] == 'I' && audioData[2] == 'F' && audioData[3] == 'F')
                {
                    if (audioData[8] == 'W' && audioData[9] == 'A' && audioData[10] == 'V' && audioData[11] == 'E')
                    {
                        if (audioData[12] == 'f' && audioData[13] == 'm' && audioData[14] == 't' && audioData[15] == ' ')
                        {
                            // Check the format in the 'fmt ' chunk
                            int formatCode = BitConverter.ToInt16(audioData, 20);

                            if (formatCode == 0x01) // PCM
                            {
                                if (BitConverter.ToInt16(audioData, 34) != 16 && BitConverter.ToInt16(audioData, 34) != 24)
                                {
                                    throw new Exception("Unsupported bit depth.");
                                }
                                if (BitConverter.ToInt32(audioData, 24) < 300 || BitConverter.ToInt32(audioData, 24) > 192000)
                                {
                                    throw new Exception("Unsupported sample rate.");
                                }
                                return AudioTypeFormat.WavPcm;
                            }
                            if (formatCode == 0x11) // IMA-ADPCM
                            {
                                return AudioTypeFormat.WavImaAdpcm;
                            }
                            //throw new Exception($Unknown/Unsupported WAV format: 0x{formatCode.ToString("X2")}.");
                            //it is possible that the audio is a WEM, check WEM format
                        }
                        else
                        {
                            throw new Exception("RIFF \"fmt \" sub-chunk not found.");
                        }
                    }
                    else
                    {
                        throw new Exception("RIFF \"WAVE\" chunk not found.");
                    }
                }
                //else check if the audio data is WEM/OGG
            }
            else
            {
                throw new Exception("Data is too small.");
            }

            //====================================================================================================//
            //briefly check if the audio data is a WEM
            //====================================================================================================//
            if (audioData.Length >= 22)
            {
                //briefly check if the audio data is a WEM
                if (audioData[0] == 'R' && audioData[1] == 'I' && audioData[2] == 'F' && audioData[3] == 'F')
                {
                    if (audioData[8] == 'W' && audioData[9] == 'A' && audioData[10] == 'V' && audioData[11] == 'E')
                    {
                        if (audioData[12] == 'f' && audioData[13] == 'm' && audioData[14] == 't' && audioData[15] == ' ')
                        {
                            // Check the format in the 'fmt ' chunk
                            ushort formatCode = (ushort)BitConverter.ToInt16(audioData, 20);
                            if (formatCode == 0xFFFF)   //check wave format (0xFFFF = Vorbis)
                            {
                                return AudioTypeFormat.WemVorbis;
                            }
                            if (formatCode == 0x0002)   //check wave format (0x0002 = IMA-ADPCM)
                            {
                                return AudioTypeFormat.WemImaAdpcm;
                            }
                            if (formatCode == 0xFFFE)   //check wave format (0xFFFE = PCM)
                            {
                                return AudioTypeFormat.WemPcm;
                            }
                            throw new Exception($"Unsupported WEM format: 0x{formatCode:X2}.");
                        }
                        else
                        {
                            throw new Exception("RIFF \"fmt \" sub-chunk not found.");
                        }
                    }
                    else
                    {
                        throw new Exception("RIFF \"WAVE\" chunk not found.");
                    }
                }
                //else check if the audio data is OGG
            }
            else
            {
                throw new ArgumentException("Data is too small.");
            }

            //====================================================================================================//
            //briefly check if the audio data is an OGG
            //====================================================================================================//
            if (audioData.Length >= 36)
            {
                //briefly check if the audio data is an OGG
                if (audioData[0] == 'O' && audioData[1] == 'g' && audioData[2] == 'g' && audioData[3] == 'S')
                {
                    // Check for Vorbis in Ogg
                    if (audioData[29] == 'v' && audioData[30] == 'o' && audioData[31] == 'r' && audioData[32] == 'b' && audioData[33] == 'i' && audioData[34] == 's')
                    {
                        return AudioTypeFormat.OggVorbis;
                    }
                    // Check for Opus in Ogg
                    else if (audioData[28] == 'O' && audioData[29] == 'p' && audioData[30] == 'u' && audioData[31] == 's' && audioData[32] == 'H' && audioData[33] == 'e' && audioData[34] == 'a' && audioData[35] == 'd')
                    {
                        throw new Exception("Opus format is not supported.");
                    }
                }
                throw new Exception("Unknown audio type.");
            }
            else
            {
                throw new Exception("Data is too small.");
            }
        }
        #endregion

        #region Duration to Human Readable Format
        /// <summary>
        /// Converts a duration in seconds into a human-readable format (hh:mm:ss or mm:ss).
        /// </summary>
        /// <param name="value">The duration in seconds.</param>
        /// <returns>A string representing the formatted duration.</returns>
        public string DurationToReadableFormat(double value)
        {
            var duration = TimeSpan.FromSeconds(value);

            if (duration.Milliseconds >= 500)
            {
                duration = duration.Add(TimeSpan.FromSeconds(1));
            }

            return duration.ToString(duration.TotalHours >= 1 ? @"hh\:mm\:ss" : @"mm\:ss");
        }
        #endregion

        #region Audio File Type
        #region Extract Most audio to WAV (PCM)
        /// <summary>
        /// Extracts audio data to a WAV file in PCM format.
        /// </summary>
        /// <param name="audioToExtract">The byte array containing the audio data.</param>
        /// <param name="outputPath">The output path where the WAV file will be saved.</param>
        public void ExtractAudioToWav(byte[] audioToExtract, string outputPath)
        {
            GCHandle handle;
            IntPtr dataPtr;
            try
            {
                handle = GCHandle.Alloc(audioToExtract, GCHandleType.Pinned);
                dataPtr = handle.AddrOfPinnedObject();
            }
            catch (Exception e)
            {
                throw new Exception("Failed to pin audio data:\n", e);
            }

            var stream = Bass.CreateStream(dataPtr, 0, audioToExtract.Length, BassFlags.Decode);

            if (stream == 0)
            {
                handle.Free();
                throw new Exception($"Failed to create stream. Error: {Bass.LastError}");
            }

            var encoder = BassEnc.EncodeStart(stream, outputPath, EncodeFlags.PCM | EncodeFlags.AutoFree, null, IntPtr.Zero, 0);

            if (encoder == 0)
            {
                Bass.StreamFree(stream);
                handle.Free();
                throw new Exception($"Failed to start WAV encoder. Error: {Bass.LastError}");
            }
            var buffer = new byte[1048576];
            while (true)
            {
                var bytesRead = Bass.ChannelGetData(stream, buffer, buffer.Length);
                if (bytesRead == 0 || (bytesRead == -1 && Bass.LastError == Errors.Ended))
                {
                    break;
                }

                if (bytesRead != -1 || Bass.LastError == Errors.Ended) continue;
                BassEnc.EncodeStop(stream);
                Bass.StreamFree(stream);
                handle.Free();
                throw new Exception($"Failed to decode portion of data. Error: {Bass.LastError}");
            }

            BassEnc.EncodeStop(encoder);
            Bass.StreamFree(stream);
            handle.Free();
        }
        #endregion
        #region Extract Most audio to OGG (VORBIS)
        /// <summary>
        /// Extracts audio data to an OGG file in Vorbis format.
        /// </summary>
        /// <param name="audioToExtract">The byte array containing the audio data.</param>
        /// <param name="outputPath">The output path where the OGG file will be saved.</param>
        public void ExtractAudioToOgg(byte[] audioToExtract, string outputPath)
        {
            GCHandle handle;
            IntPtr dataPtr;
            try
            {
                handle = GCHandle.Alloc(audioToExtract, GCHandleType.Pinned);
                dataPtr = handle.AddrOfPinnedObject();
            }
            catch (Exception e)
            {
                throw new Exception("Failed to pin audio data:\n", e);
            }

            var stream = Bass.CreateStream(dataPtr, 0, audioToExtract.Length, BassFlags.Decode);

            if (stream == 0)
            {
                handle.Free();
                throw new Exception($"Failed to create stream. Error: {Bass.LastError}");
            }

            var encoder = BassEnc_Ogg.Start(stream, null, EncodeFlags.Default, outputPath);

            if (encoder == 0)
            {
                Bass.StreamFree(stream);
                handle.Free();
                throw new Exception($"Failed to start OGG encoder. Error: {Bass.LastError}");
            }

            var buffer = new byte[1048576];
            while (true)
            {
                var bytesRead = Bass.ChannelGetData(stream, buffer, buffer.Length);
                if (bytesRead == 0 || (bytesRead == -1 && Bass.LastError == Errors.Ended))
                {
                    break;
                }

                if (bytesRead != -1 || Bass.LastError == Errors.Ended) continue;
                BassEnc.EncodeStop(encoder);
                Bass.StreamFree(stream);
                handle.Free();
                throw new Exception($"Failed to decode portion of data. Error: {Bass.LastError}");
            }

            BassEnc.EncodeStop(encoder);
            Bass.StreamFree(stream);
            handle.Free();
        }

        #endregion
        #region Extract Most audio to MP3
        /// <summary>
        /// Extracts audio data to an MP3 file.
        /// </summary>
        /// <param name="audioToExtract">The byte array containing the audio data.</param>
        /// <param name="outputPath">The output path where the MP3 file will be saved.</param>
        public void ExtractAudioToMp3(byte[] audioToExtract, string outputPath)
        {
            GCHandle handle;
            IntPtr dataPtr;
            try
            {
                handle = GCHandle.Alloc(audioToExtract, GCHandleType.Pinned);
                dataPtr = handle.AddrOfPinnedObject();
            }
            catch (Exception e)
            {
                throw new Exception("Failed to pin audio data:\n", e);
            }

            var stream = Bass.CreateStream(dataPtr, 0, audioToExtract.Length, BassFlags.Decode);

            if (stream == 0)
            {
                handle.Free();
                throw new Exception($"Failed to create stream. Error: {Bass.LastError}");
            }

            var encoder = BassEnc_Mp3.Start(stream, null, EncodeFlags.Default, outputPath);

            if (encoder == 0)
            {
                Bass.StreamFree(stream);
                handle.Free();
                throw new Exception($"Failed to start MP3 encoder. Error: {Bass.LastError}");
            }
            var buffer = new byte[1048576];
            while (true)
            {
                var bytesRead = Bass.ChannelGetData(stream, buffer, buffer.Length);
                if (bytesRead == 0 || (bytesRead == -1 && Bass.LastError == Errors.Ended))
                {
                    break;
                }

                if (bytesRead != -1 || Bass.LastError == Errors.Ended) continue;
                Bass.StreamFree(stream);
                handle.Free();
                throw new Exception($"Failed to decode portion of data. Error: {Bass.LastError}");
            }

            BassEnc.EncodeStop(encoder);
            Bass.StreamFree(stream);
            handle.Free();
        }
        #endregion

        #region Generate WAV File as Data
        /// <summary>
        /// Generates a WAV file with a header using PCM data, sample rate, bit-depth, and channel count.
        /// </summary>
        /// <param name="pcmData">The byte array containing PCM data.</param>
        /// <param name="sampleRate">The sample rate of the audio.</param>
        /// <param name="bitDepth">The bit depth of the audio.</param>
        /// <param name="channels">The number of audio channels.</param>
        /// <returns>A byte array containing the WAV file data.</returns>
        private static byte[] GenerateWavAudio(byte[] pcmData, int sampleRate, short bitDepth, int channels)
        {
            var dataLength = pcmData.Length;
            var fileSize = 38 + dataLength;
            const short extraParamSize = 0; // Set extra param size to 0 if no extra data

            using (var memoryStream = new MemoryStream())
            {
                // RIFF header
                memoryStream.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"), 0, 4);
                memoryStream.Write(BitConverter.GetBytes(fileSize), 0, 4);
                memoryStream.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"), 0, 4);

                // fmt sub chunk
                memoryStream.Write(System.Text.Encoding.ASCII.GetBytes("fmt "), 0, 4);
                memoryStream.Write(BitConverter.GetBytes(18), 0, 4);          // Sub chunk1Size (18 for PCM with extra param size)
                memoryStream.Write(BitConverter.GetBytes((short)1), 0, 2);    // Audio format (1 = PCM)
                memoryStream.Write(BitConverter.GetBytes(channels), 0, 2);
                memoryStream.Write(BitConverter.GetBytes(sampleRate), 0, 4);
                memoryStream.Write(BitConverter.GetBytes(sampleRate * channels * bitDepth / 8), 0, 4); // Byte rate
                memoryStream.Write(BitConverter.GetBytes((short)(channels * bitDepth / 8)), 0, 2); // Block align
                memoryStream.Write(BitConverter.GetBytes(bitDepth), 0, 2);
                memoryStream.Write(BitConverter.GetBytes(extraParamSize), 0, 2); // Extra param size

                // data sub chunk
                memoryStream.Write(System.Text.Encoding.ASCII.GetBytes("data"), 0, 4);
                memoryStream.Write(BitConverter.GetBytes(dataLength), 0, 4);
                memoryStream.Write(pcmData, 0, dataLength);

                return memoryStream.ToArray();
            }
        }
        #endregion

        #region Retrieve Audio Duration
        /// <summary>
        /// Retrieves the duration of an audio file in seconds.
        /// </summary>
        /// <param name="thisAudioFile">The <see cref="AudioFile"/> object containing the audio data.</param>
        /// <returns>The duration of the audio in seconds.</returns>
        public double RetrieveAudioDuration(AudioFile thisAudioFile)
        {
            if (!_isBassInitialized)
            {
                throw new InvalidOperationException("ManagedBass is not initialized.");
            }

            double duration;
            var handle = GCHandle.Alloc(thisAudioFile.AudioData, GCHandleType.Pinned);
            var dataPtr = handle.AddrOfPinnedObject();
            try
            {
                int streamHandle;
                switch (thisAudioFile.Type)
                {
                    case AudioType.Ogg:

                        streamHandle = Bass.CreateStream(dataPtr, 0, thisAudioFile.AudioData.Length, BassFlags.Decode);
                        break;
                    case AudioType.Mp3:
                        handle = GCHandle.Alloc(thisAudioFile.AudioData, GCHandleType.Pinned);
                        dataPtr = handle.AddrOfPinnedObject();
                        streamHandle = Bass.CreateStream(dataPtr, 0, thisAudioFile.AudioData.Length, BassFlags.Decode | BassFlags.Prescan);
                        break;
                    case AudioType.Wav:
                        handle = GCHandle.Alloc(thisAudioFile.AudioData, GCHandleType.Pinned);
                        dataPtr = handle.AddrOfPinnedObject();
                        streamHandle = Bass.CreateStream(dataPtr, 0, thisAudioFile.AudioData.Length, BassFlags.Decode);
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported or unknown audio format");
                }
                if (streamHandle == 0)
                {
                    throw new Exception("Failed to create audio stream." + " (" + Bass.LastError + ")");
                }
                try
                {
                    // Get the length of the stream in bytes
                    var lengthInBytes = Bass.ChannelGetLength(streamHandle);
                    // Calculate the length of the stream in seconds
                    duration = Bass.ChannelBytes2Seconds(streamHandle, lengthInBytes);
                }
                finally
                {
                    Bass.StreamFree(streamHandle);
                }
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
            return duration;
        }
        #endregion
        #region Retrieve Audio Number of Channels
        /// <summary>
        /// Retrieves the number of channels in an audio file.
        /// </summary>
        /// <param name="thisAudioFile">The <see cref="AudioFile"/> object containing the audio data.</param>
        /// <returns>The number of channels in the audio file.</returns>
        public int RetrieveAudioChannels(AudioFile thisAudioFile)
        {
            if (!_isBassInitialized)
            {
                throw new InvalidOperationException("ManagedBass is not initialized.");
            }

            int channels;
            var handle = GCHandle.Alloc(thisAudioFile.AudioData, GCHandleType.Pinned);
            var dataPtr = handle.AddrOfPinnedObject();
            try
            {
                var format = GetAudioFileType(thisAudioFile.AudioData);   //format of the audio
                int streamHandle;
                switch (format)
                {
                    case AudioTypeFormat.OggVorbis:
                        streamHandle = Bass.CreateStream(dataPtr, 0, thisAudioFile.AudioData.Length, BassFlags.Decode);
                        break;
                    case AudioTypeFormat.Mp3:
                        streamHandle = Bass.CreateStream(dataPtr, 0, thisAudioFile.AudioData.Length, BassFlags.Decode | BassFlags.Prescan);
                        break;
                    case AudioTypeFormat.WavPcm:
                        streamHandle = Bass.CreateStream(dataPtr, 0, thisAudioFile.AudioData.Length, BassFlags.Decode);
                        break;
                    case AudioTypeFormat.WavImaAdpcm:
                        streamHandle = Bass.CreateStream(dataPtr, 0, thisAudioFile.AudioData.Length, BassFlags.Decode);
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported or unknown audio format");
                }
                if (streamHandle == 0)
                {
                    throw new Exception("Failed to create audio stream." + " (" + Bass.LastError + ")");
                }
                try
                {
                    // Get information about the sample stream
                    Bass.ChannelGetInfo(streamHandle, out var channelInfo);
                    channels = channelInfo.Channels;
                }
                finally
                {
                    Bass.StreamFree(streamHandle);
                }
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
            // Get the number of channels
            return channels;
        }
        #endregion
        #region Retrieve Audio Sample Rate
        /// <summary>
        /// Retrieves the sample rate of an audio file.
        /// </summary>
        /// <param name="thisAudioFile">The <see cref="AudioFile"/> object containing the audio data.</param>
        /// <returns>The sample rate of the audio file.</returns>
        public int RetrieveAudioSampleRate(AudioFile thisAudioFile)
        {
            if (!_isBassInitialized)
            {
                throw new InvalidOperationException("ManagedBass is not initialized.");
            }

            int sampleRate;
            var handle = GCHandle.Alloc(thisAudioFile.AudioData, GCHandleType.Pinned);
            var dataPtr = handle.AddrOfPinnedObject();
            try
            {
                var format = GetAudioFileType(thisAudioFile.AudioData);   //format of the audio
                int streamHandle;
                switch (format)
                {
                    case AudioTypeFormat.OggVorbis:
                        streamHandle = Bass.CreateStream(dataPtr, 0, thisAudioFile.AudioData.Length, BassFlags.Decode);
                        break;
                    case AudioTypeFormat.Mp3:
                        streamHandle = Bass.CreateStream(dataPtr, 0, thisAudioFile.AudioData.Length, BassFlags.Decode | BassFlags.Prescan);
                        break;
                    case AudioTypeFormat.WavPcm:
                        streamHandle = Bass.CreateStream(dataPtr, 0, thisAudioFile.AudioData.Length, BassFlags.Decode);
                        break;
                    case AudioTypeFormat.WavImaAdpcm:
                        streamHandle = Bass.CreateStream(dataPtr, 0, thisAudioFile.AudioData.Length, BassFlags.Decode);
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported or unknown audio format");
                }
                if (streamHandle == 0)
                {
                    throw new Exception("Failed to create audio stream." + " (" + Bass.LastError + ")");
                }
                try
                {
                    // Get information about the sample stream
                    Bass.ChannelGetInfo(streamHandle, out var channelInfo);

                    sampleRate = channelInfo.Frequency;
                }
                finally
                {
                    Bass.StreamFree(streamHandle);
                }
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
            // Get the sample rate
            return sampleRate;
        }
        #endregion
        #endregion

        #region WEM File Type
        #region Convert WEM audio format to standard audio format
        /// <summary>
        /// Converts WEM audio data to a standard audio format (PCM, Vorbis, etc.).
        /// </summary>
        /// <param name="thisWem">The byte array containing WEM audio data.</param>
        /// <returns>A byte array containing the converted standard audio.</returns>
        public byte[] ConvertWemToStandardAudio(byte[] thisWem)
        {
            byte[] audioToOutput;
            try
            {
                switch (GetAudioFileType(thisWem))
                {
                    case AudioTypeFormat.WemVorbis:
                        using (var wem = new WEMFile(new MemoryStream(thisWem), WEMForcePacketFormat.NoForcePacketFormat))
                        {
                            var temp = wem.GenerateOGG(Resources.packed_codebooks_aoTuV_603, false, false);
                            //audioToOutput = temp;
                            using (var msOgg = new MemoryStream(temp))
                            {
                                using (Stream data = Revorb.Jiggle(msOgg))
                                {
                                    using (var msTemp = new MemoryStream())
                                    {
                                        data.CopyTo(msTemp);
                                        audioToOutput = msTemp.ToArray();
                                    }
                                }
                            }
                        }
                        break;
                    case AudioTypeFormat.WemImaAdpcm:
                        using (var wem = new WemImaAdpcm())
                        {
                            audioToOutput = wem.ConvertToStandardImaAdpcm(thisWem);
                        }
                        break;
                    case AudioTypeFormat.WemPcm:
                        using (var wem = new WemPcm())
                        {
                            audioToOutput = wem.ConvertToStandardPcm(thisWem);
                        }
                        break;
                    case AudioTypeFormat.OggVorbis:
                        throw new Exception("audio is ogg vorbis, not wem.");
                    case AudioTypeFormat.Mp3:
                        throw new Exception("audio is mp3, not wem.");
                    case AudioTypeFormat.WavPcm:
                        throw new Exception("audio is wav pcm, not wem.");
                    case AudioTypeFormat.WavImaAdpcm:
                        throw new Exception("audio is wav ima adpcm, not wem.");
                    default:
                        throw new InvalidOperationException("Unsupported or unknown audio format");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while converting WEM to standard audio: " + e.Message);
            }
            return audioToOutput;
        }

        #endregion
        #region convert standard audio format to WEM (PCM format)
        /// <summary>
        /// Converts a standard audio file (PCM) to WEM format.
        /// </summary>
        /// <param name="audioToConvert">The byte array containing the standard audio data.</param>
        /// <returns>A byte array containing the WEM audio data.</returns>
        public byte[] ConvertStandardAudioToWemPcm(byte[] audioToConvert)
        {
            if (!_isBassInitialized)
            {
                throw new InvalidOperationException("ManagedBass is not initialized.");
            }

            var handle = GCHandle.Alloc(0);
            try
            {
                // Create a stream from the audio data
                var streamHandle = 0;
                var format = GetAudioFileType(audioToConvert);
                IntPtr dataPtr;
                switch (format)
                {
                    case AudioTypeFormat.OggVorbis:
                        handle = GCHandle.Alloc(audioToConvert, GCHandleType.Pinned);
                        dataPtr = handle.AddrOfPinnedObject();
                        streamHandle = Bass.CreateStream(dataPtr, 0, audioToConvert.Length, BassFlags.Decode);
                        break;
                    case AudioTypeFormat.Mp3:
                        handle = GCHandle.Alloc(audioToConvert, GCHandleType.Pinned);
                        dataPtr = handle.AddrOfPinnedObject();
                        streamHandle = Bass.CreateStream(dataPtr, 0, audioToConvert.Length, BassFlags.Decode | BassFlags.Prescan);
                        break;
                    case AudioTypeFormat.WavPcm:
                        handle = GCHandle.Alloc(audioToConvert, GCHandleType.Pinned);
                        dataPtr = handle.AddrOfPinnedObject();
                        streamHandle = Bass.CreateStream(dataPtr, 0, audioToConvert.Length, BassFlags.Decode);
                        break;
                    case AudioTypeFormat.WavImaAdpcm:
                        handle = GCHandle.Alloc(audioToConvert, GCHandleType.Pinned);
                        dataPtr = handle.AddrOfPinnedObject();
                        streamHandle = Bass.CreateStream(dataPtr, 0, audioToConvert.Length, BassFlags.Decode);
                        break;
                    case AudioTypeFormat.WemVorbis:
                        //do nothing, already in WEM format
                        break;
                    case AudioTypeFormat.WemImaAdpcm:
                        //do nothing, already in WEM format
                        break;
                    case AudioTypeFormat.WemPcm:
                        //do nothing, already in WEM format
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported or unknown audio format");
                }
                if (format == AudioTypeFormat.WemPcm || format == AudioTypeFormat.WemImaAdpcm || format == AudioTypeFormat.WemVorbis)
                {
                    return audioToConvert;
                }

                if (streamHandle == 0)
                {
                    throw new InvalidOperationException("Error creating stream: " + Bass.LastError);
                }
                try
                {
                    // Retrieve PCM data
                    var length = (int)Bass.ChannelGetLength(streamHandle);
                    var pcmData = new byte[length];
                    var bytesRead = Bass.ChannelGetData(streamHandle, pcmData, length);

                    if (bytesRead < 0)
                    {
                        throw new InvalidOperationException("Error retrieving PCM data: " + Bass.LastError);
                    }

                    // Get the stream info to determine sample rate, channels, and bit-depth
                    var info = Bass.ChannelGetInfo(streamHandle);
                    var sampleRate = info.Frequency;
                    var channels = info.Channels;
                    const short bitDepth = 16; // Assuming 16-bit PCM

                    // Create WAV file data with header
                    var audioPcm = GenerateWavAudio(pcmData, sampleRate, bitDepth, channels);
                    using (var wem = new WemPcm())
                    {
                        return wem.GenerateWEM_PCM(audioPcm);
                    }
                }
                finally
                {
                    // Cleanup the stream
                    Bass.StreamFree(streamHandle);
                }
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }
        #endregion
        #endregion
        #endregion

        #region Dispose
        /// <summary>
        /// Releases all resources used by the <see cref="AudioOperations"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="AudioOperations"/> and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                if (_isBassInitialized)
                {
                    _isBassInitialized = false;
                }
            }

            // Dispose unmanaged resources

            _disposed = true;
        }
        /// <summary>
        /// Destructor to ensure resources are released when the object is garbage collected.
        /// </summary>
        ~AudioOperations()
        {
            Dispose(false);
        }
        #endregion
    }
}