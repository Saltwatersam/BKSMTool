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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.IO;
using BKSMTool.audio_player_converter;

namespace BKSMTool.Files
{
    //==================================================================================================================================//
    /// <summary> 
    /// Provides functionality for extracting audio files from a BNK archive and saving them in various audio formats.
    /// </summary>
    /// <remarks>
    /// <br/> - Supports extraction to audio file formats OGG (Vorbis), MP3, WAV (PCM), and WEM.
    /// </remarks>
    //==================================================================================================================================//
    public static class AudioExtractionHandler
    {
        #region METHODS

        #region Extract Selected Audio
        //***************************************************************************************************************//
        /// <summary> 
        /// Extracts a single audio file from a BNK archive and writes it to the specified output path.
        /// </summary>
        /// <param name="path">The file path where the audio will be saved, including the filename and extension.</param>
        /// <param name="audio">The <see cref="AudioFile"/> instance representing the audio to extract.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="progressReporter">Optional. A progress handler to report extraction progress.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="TaskCanceledException">Thrown when the extraction operation is canceled.</exception>
        /// <exception cref="Exception">Thrown when the path or audio is null, or an error occurs during extraction.</exception>
        //***************************************************************************************************************//
        public static async Task ExtractSingleAudio(string path, AudioFile audio, CancellationToken cancellationToken, IProgress<int> progressReporter = null)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("No path provided.");
            }
            if (audio == null)
            {
                throw new Exception("No audio file provided.");
            }

            // Internal properties
            Task extractionTask = null;

            try
            {
                extractionTask = new Task(() =>
                {
                    PerformAudioExtraction(path, audio, cancellationToken);
                    progressReporter?.Report(1);
                });

                progressReporter?.Report(1); // Major step done

                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                extractionTask.Start();
                await extractionTask;
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }
                progressReporter?.Report(1); // Next step
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }
            }
            catch (TaskCanceledException)
            {
                throw new TaskCanceledException();
            }
            catch (Exception ex)
            {
                extractionTask?.Dispose();
                throw new Exception($"Error while extracting audio: \n{ex.Message}");
            }
        }
        #endregion

        #region Extract All Audios
        //***************************************************************************************************************//
        /// <summary> 
        /// Extracts all audio files from a BNK archive and writes them to the specified output path.
        /// </summary>
        /// <param name="path">The file path where the audio will be saved, including the filename and extension.</param>
        /// <param name="audioList">A list of <see cref="AudioFile"/> objects to extract.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="progressReporter">Optional. A progress handler to report extraction progress.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="TaskCanceledException">Thrown when the extraction operation is canceled.</exception>
        /// <exception cref="Exception">Thrown when the path or audioList is null, or an error occurs during extraction.</exception>
        //***************************************************************************************************************//
        public static async Task ExtractAllAudio(string path, List<AudioFile> audioList, CancellationToken cancellationToken, IProgress<int> progressReporter = null)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("No path provided.");
            }

            if (audioList == null || audioList.Count == 0)
            {
                throw new Exception("No audio files provided.");
            }

            var totalAudio = audioList.Count;
            var extractionTasks = new List<Task>();

            try
            {
                for (var audioNumber = 0; audioNumber < totalAudio; audioNumber++)  // Create a task for each audio to extract
                {
                    var currentAudio = audioList[audioNumber];
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);

                    if (fileNameWithoutExtension == "default names (ID - event name)")
                    {
                        fileNameWithoutExtension = currentAudio.AudioName;
                    }
                    else if (fileNameWithoutExtension.ToLowerInvariant() == "idwithevent")
                    {
                        fileNameWithoutExtension = currentAudio.AudioName;
                    }
                    else if (fileNameWithoutExtension.ToLowerInvariant() == "id")
                    {
                        fileNameWithoutExtension = currentAudio.WemId.ToString();
                    }
                    else if (fileNameWithoutExtension.ToLowerInvariant() == "event")
                    {
                        fileNameWithoutExtension = currentAudio.WemEventName;
                    }
                    else
                    {
                        fileNameWithoutExtension = $"{Path.GetFileNameWithoutExtension(path)}_{audioNumber}";
                    }

                    var selectedPath = Path.Combine(Path.GetDirectoryName(path) ?? throw new InvalidOperationException(),
                        fileNameWithoutExtension + Path.GetExtension(path));

                    var extractionTask = new Task(() =>
                    {
                        PerformAudioExtraction(selectedPath, currentAudio, cancellationToken);
                        progressReporter?.Report(1);
                    });

                    extractionTasks.Add(extractionTask);
                }

                foreach (var task in extractionTasks)
                {
                    task.Start();
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }
                await Task.WhenAll(extractionTasks);
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }
            }
            catch (TaskCanceledException)
            {
                throw new TaskCanceledException();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while extracting audio(s): \n{ex.Message}");
            }
        }
        #endregion

        #region Perform Audio Extraction
        //***************************************************************************************************************//
        /// <summary> 
        /// Handles the core process of extracting the audio and writing it to the output file.
        /// </summary>
        /// <param name="path">The full path to the output file, including the filename and extension.</param>
        /// <param name="audio">The <see cref="AudioFile"/> to be extracted.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe during the extraction process.</param>
        //***************************************************************************************************************//
        private static void PerformAudioExtraction(string path, AudioFile audio, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("No path provided.");
            }

            if (audio == null)
            {
                throw new Exception("No audio provided.");
            }

            var directory = Path.GetDirectoryName(path);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                string extension;
                string selectedPath;

                switch (Path.GetExtension(path))
                {
                    case ".xxx":
                        if (audio.Type == AudioType.Ogg)
                        {
                            extension = ".ogg";
                            selectedPath = Path.Combine(directory ?? throw new InvalidOperationException(), fileNameWithoutExtension + extension);
                            using (AudioOperations audioOperation = new AudioOperations())
                            {
                                audioOperation.ExtractAudioToOgg(audio.AudioData, selectedPath);
                            }
                        }
                        else if (audio.Type == AudioType.Wav)
                        {
                            extension = ".wav";
                            selectedPath = Path.Combine(directory ?? throw new InvalidOperationException(), fileNameWithoutExtension + extension);
                            using (var audioOperation = new AudioOperations())
                            {
                                audioOperation.ExtractAudioToWav(audio.AudioData, selectedPath);
                            }
                        }
                        else
                        {
                            throw new Exception("unknown audio format.");
                        }
                        break;
                    case ".wem":
                        extension = ".wem";
                        selectedPath = Path.Combine(directory ?? throw new InvalidOperationException(), fileNameWithoutExtension + extension);
                        using (var fs = new FileStream(selectedPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            fs.Write(audio.SourceFile.Data, 0, audio.SourceFile.Data.Length);
                        }
                        break;
                    case ".ogg":
                        extension = ".ogg";
                        selectedPath = Path.Combine(directory ?? throw new InvalidOperationException(), fileNameWithoutExtension + extension);
                        using (var audioOperation = new AudioOperations())
                        {
                            audioOperation.ExtractAudioToOgg(audio.AudioData, selectedPath);
                        }
                        break;
                    case ".wav":
                        extension = ".wav";
                        selectedPath = Path.Combine(directory ?? throw new InvalidOperationException(), fileNameWithoutExtension + extension);
                        using (var audioOperation = new AudioOperations())
                        {
                            audioOperation.ExtractAudioToWav(audio.AudioData, selectedPath);
                        }
                        break;
                    case ".mp3":
                        extension = ".mp3";
                        selectedPath = Path.Combine(directory ?? throw new InvalidOperationException(), fileNameWithoutExtension + extension);
                        using (var audioOperation = new AudioOperations())
                        {
                            audioOperation.ExtractAudioToMp3(audio.AudioData, selectedPath);
                        }
                        break;
                    default:
                        throw new Exception("Unsupported audio format.");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error while extracting audio: \n{e.Message}");
            }
        }
        #endregion

        #endregion
    }
}