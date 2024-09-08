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
using BKSMTool.Files.File_Audio;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.IO;

namespace BKSMTool.Files
{
    /// <summary>
    /// Handles the replacement of WEM files from a given audio source.
    /// Provides functionality for converting a standard audio file to WEM format.
    /// </summary>
    public static class WemReplacementHandler
    {
        /// <summary>
        /// Converts a given audio file into a WEM format file.
        /// </summary>
        /// <param name="newAudioPath">The file path of the audio to be converted.</param>
        /// <param name="cancellationToken">Cancellation token to allow the operation to be canceled.</param>
        /// <param name="progressReporter">Progress reporter to report the progress of the conversion process.</param>
        /// <returns>A byte array containing the WEM file data.</returns>
        /// <exception cref="Exception">Thrown when no valid path is provided.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the operation is canceled by the user.</exception>
        public static async Task<byte[]> GetNewWemFile(string newAudioPath, CancellationToken cancellationToken, IProgress<int> progressReporter)
        {
            // Ensure the method remains async while processing the cancellation token.
            await Task.Delay(0, cancellationToken);

            if (string.IsNullOrEmpty(newAudioPath))
            {
                throw new ArgumentException("No path provided.");
            }

            // If the cancellation is requested early in the process.
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            FileStream fsNewAudio;
            byte[] wemToOutput;

            //*************************************//
            // STEP 1 : OPEN AUDIO FILE FOR READING //
            //*************************************//
            #region Open New Audio File
            try
            {
                fsNewAudio = FileOperations.StartFileStream(newAudioPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception ex)
            {
                progressReporter?.Report(1);
                throw new IOException($"Error opening new audio file: {ex.Message}", ex);
            }
            #endregion

            // Check if the operation was canceled after opening the file.
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            progressReporter?.Report(1);

            //*************************************//
            // STEP 2 : CONVERT AUDIO TO WEM FORMAT //
            //*************************************//
            #region Convert Audio to WEM
            try
            {
                var newAudioData = FileOperations.ReadBytes(fsNewAudio, 0, fsNewAudio.Length);
                using (var audioOperations = new AudioOperations())
                {
                    wemToOutput = audioOperations.ConvertStandardAudioToWemPcm(newAudioData);
                }
            }
            catch (Exception ex)
            {
                progressReporter?.Report(1);
                throw new InvalidOperationException($"Error converting audio to WEM format: {ex.Message}", ex);
            }
            #endregion

            progressReporter?.Report(1);

            //*************************************//
            // STEP 3 : FINALIZE AND RETURN WEM FILE //
            //*************************************//
            #region Finalize and Return
            fsNewAudio.Close();

            // Check if the operation was canceled before returning the WEM data.
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            return wemToOutput;
            #endregion
        }
    }
}
