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

namespace BKSMTool.Files.File_Audio.RIFF_WAVE
{
    /// <summary>
    /// Represents the basic structure for an audio wave format (WAV), including properties such as format tag, channels, sample rate, and data size.
    /// This class serves as a base class for more specific audio formats, like <see cref="AudioWaveFormatExtensible"/>.
    /// </summary>
    public class AudioWaveFormatEx
    {
        #region Properties

        /// <summary>
        /// The audio format code (e.g., PCM [0x0001], IMA-ADPCM [0x0011], etc.).
        /// </summary>
        protected ushort WFormatTag;

        /// <summary>
        /// The number of audio channels (e.g., mono, stereo).
        /// </summary>
        protected ushort NChannels;

        /// <summary>
        /// The sample rate (in samples per second).
        /// </summary>
        protected uint NSamplesPerSec;

        /// <summary>
        /// The average number of bytes per second, used for buffer size estimation.
        /// </summary>
        protected uint NAvgBytesPerSec;

        /// <summary>
        /// The number of bytes per audio block or frame, calculated as:
        /// <code>NChannels * WBitsPerSample / 8</code> (rounded up to the nearest byte).
        /// </summary>
        protected ushort NBlockAlign;

        /// <summary>
        /// The number of bits per sample per channel (without padding).
        /// </summary>
        protected ushort WBitsPerSample;

        /// <summary>
        /// The size of any extra data following this structure. For standard formats, this is 0.
        /// </summary>
        protected ushort CbSize = 0;

        /// <summary>
        /// The size of the audio data chunk in bytes.
        /// </summary>
        protected uint DataSize;

        /// <summary>
        /// The audio data stored in the data chunk.
        /// </summary>
        protected byte[] Data;

        #endregion

        #region Methods
        // None
        #endregion
    }


    //**********************************//
    // STANDARD PCM DATA FORMAT        //
    //**********************************//
    // DATA VISUALIZATION
    // Example: 6 bits per sample, 2 channels

    //since we have 6 bits per sample, in order to store it to a byte, we need to pad it to 8 bits
    //blockAlign     =     nChannels * wBitsPerSample / 8     =     2 * 6 / 8 = 1.5      =     2 (rounded up to the nearest byte)

    //|             SAMPLE            |             SAMPLE            |             SAMPLE            |             SAMPLE            |
    //|      Sample 1 (channel 1)     |      Sample 1 (channel 2)     |      Sample 2 (channel 1)     |      Sample 2 (channel 2)     |
    //|                       |PADDING|                       |PADDING|                       |PADDING|                       |PADDING|
    //|-----------------------|-------|-----------------------|-------|-----------------------|-------|-----------------------|-------|
    //|   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
    //| 0   1   2   3   4   5   6   7 | 0   1   2   3   4   5   6   7 | 0   1   2   3   4   5   6   7 | 0   1   2   3   4   5   6   7 |
    //|   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | 
    //|-------------------------------|-------------------------------|-------------------------------|-------------------------------|
    //|                               |                               |                                                               |
    //|                               |                               |                                                               |
    //|                        FRAME 1 (Block)                        |                        FRAME 2 (Block)                        |
}
