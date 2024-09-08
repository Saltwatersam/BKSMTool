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

using System.Linq;
using System.Threading.Tasks;
using System.Security;
using System.Threading;
using BKSMTool.Files.File_BNK;
using BKSMTool.Miscellaneous;
using System;
using System.IO;
using System.Text;
using BKSMTool.Files;

namespace BKSMTool.CLI.Commands
{
    /// <summary>
    /// Command to extract all audio files from a BNK file.
    /// Supports various options such as specifying the output format, directory, and file naming conventions.
    /// </summary>and
    public class ExtractAllCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractAllCommand"/> class
        /// and sets the command name to "extractAll".
        /// </summary>
        public ExtractAllCommand()
        {
            CommandName = "extractAll";
        }
        /// <summary>
        /// Executes the command to extract all audio files from a BNK file.
        /// Parses the provided arguments and validates them before performing the extraction.
        /// </summary>
        /// <param name="args">The array of arguments passed from the command line interface.</param>
        /// <returns>A task representing the completion of the extraction process.</returns>
        public override async Task Execute(string[] args)
        {
            string bnkPath = null;
            var extension = ".WEM"; // Default extension is WEM
            string outputDirectory = null;
            var naming = "ID"; // Default naming convention
            string infoFilePath = null;

            if (args == null || args.Length == 0)   // No arguments provided
            {
                Console.WriteLine(@"Missing required argument: -p, --path <path>");
                ShowHelp();
                return;
            }

            // Arguments are provided

            #region Parse arguments and their values
            for (var i = 0; i < args.Length; i++)
            {
                string arg;
                string argValue = null;

                if (args[i].Contains("="))      //Handle arguments that integrate values (with '=')
                {
                    var split = args[i].Split(new[] { '=' }, 2);
                    arg = split[0];
                    argValue = split[1];
                }
                else                           //if arguments are separated from their values, then it's just a single argument
                {
                    arg = args[i];
                }

                switch (arg)                    
                {
                    case "-p":
                    case "--path":
                        if(argValue != null)
                        {
                            bnkPath = argValue;
                        }
                        else
                        {
                            if (i + 1 < args.Length)
                            {
                                bnkPath = args[i + 1];
                                i++;
                            }
                            else
                            {
                                Console.WriteLine(@"Missing value <path> for argument: -p, --path");
                                ShowHelp();
                                return;
                            }
                        }
                        break;
                    case "-e":
                    case "--extension":
                        if (argValue != null)
                        {
                            extension = argValue;
                        }
                        else
                        {
                            if (i + 1 < args.Length)
                            {
                                extension = args[i + 1];
                                i++;
                            }
                            else
                            {
                                Console.WriteLine(@"Missing value <ext> for argument: -e, --extension");
                                ShowHelp();
                                return;
                            }
                        }
                        break;
                    case "-o":
                    case "--output":
                        if (argValue != null)
                        {
                            outputDirectory = argValue;
                        }
                        else
                        {
                            if (i + 1 < args.Length)
                            {
                                outputDirectory = args[i + 1];
                                i++;
                            }
                            else
                            {
                                Console.WriteLine(@"Missing value <directory> for argument: -o, --output");
                                ShowHelp();
                                return;
                            }
                        }
                        break;
                    case "-n":
                    case "--name":
                        if(argValue != null)
                        {
                            naming = argValue;
                        }
                        else
                        {
                            if (i + 1 < args.Length)
                            {
                                naming = args[i + 1];
                                i++;
                            }
                            else
                            {
                                Console.WriteLine(@"Missing value <naming> for argument: -n, --name");
                                ShowHelp();
                                return;
                            }
                        }
                        break;
                    case "-i":
                    case "--infoFilePath":
                        if (argValue != null)
                        {
                            infoFilePath = argValue;
                        }
                        else
                        {
                            if (i + 1 < args.Length)
                            {
                                infoFilePath = args[i + 1];
                                i++;
                            }
                            else
                            {
                                Console.WriteLine(@"Missing value <path> for argument: -i, --infoFilePath");
                                ShowHelp();
                                return;
                            }
                        }
                        break;
                    case "-h":
                    case "--help":
                        if (argValue != null)
                        {
                            Console.WriteLine($@"Invalid argument: {argValue}");
                        }
                        else
                        {
                            if (i + 1 < args.Length)
                            {
                                Console.WriteLine($@"Invalid argument: {args[i + 1]}");
                            }
                        }

                        ShowHelp();
                        return;
                    default:
                        Console.WriteLine($@"Invalid argument: {args[i]}");
                        ShowHelp();
                        return;
                }
            }
            #endregion

            #region check if mendatory arguments are provided
            if (bnkPath == null)
            {
                Console.WriteLine(@"Missing required argument: -p, --path <path>");
                ShowHelp();
                return;
            }
            #endregion

            #region check if values of arguments are valid
            #region BNKPath
            try
            {
                ValidatePath(ref bnkPath);
                //check if file extension is .bnk
                if (!string.Equals(Path.GetExtension(bnkPath), ".BNK", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception($"File <\"{bnkPath}\"> is not a .BNK file.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Invalid value <path> for argument: -p, --path");
                Console.WriteLine($@"  -{e.Message}");
                ShowHelp();
                return;
            }
            #endregion
            #region Extension
            extension = extension.ToLowerInvariant();
            switch (extension)
            {
                case ".wem":
                case ".wav":
                case ".ogg":
                case ".mp3":
                    //OK, do nothing
                    break;
                case "originalformat":
                    extension = ".xxx";
                    break;
                default:
                    Console.WriteLine(@"Invalid value <ext> for argument: -e, --extension");
                    ShowHelp();
                    return;
            }
            #endregion
            #region OutputDirectory
            if (outputDirectory == null)
            {
                outputDirectory = Path.GetDirectoryName(bnkPath);
            }
            else
            {
                try
                {
                    ValidateDirectory(outputDirectory);
                }
                catch (Exception e)
                {
                    Console.WriteLine(@"Invalid value <directory> for argument: -o, --output");
                    Console.WriteLine($@"  -{e.Message}");
                    ShowHelp();
                    return;
                }
            }

            #endregion
            #region InfoFilePath
            try
            {
                if (infoFilePath != null)
                {
                    ValidatePath(ref infoFilePath);
                    //check if file extension is .txt
                    if (!string.Equals(Path.GetExtension(infoFilePath), ".TXT", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception($"File <\"{infoFilePath}\"> is not a .TXT file.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Invalid value <path> for argument: -i, --infoFilePath");
                Console.WriteLine($@"  -{e.Message}");
                ShowHelp();
                return;
            }
            #endregion
            #region Naming
            naming = naming.ToLowerInvariant();
            switch (naming)
            {
                case "id":
                    //OK, do nothing since ID are not dependent on any other file
                    break;
                case "event":                   //Event naming requires InfoFilePath
                case "idwithevent":
                    if (infoFilePath == null)
                    {
                        Console.WriteLine(@"Missing required argument: -i, --infoFilePath <path>");
                        ShowHelp();
                        return;
                    }
                    break;
                default:                        //Custom string naming
                    try
                    {                         
                        ValidateFileName(ref naming);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(@"Invalid value <naming> for argument: -n, --name");
                        Console.WriteLine($@"  -{e.Message}");
                        ShowHelp();
                        return;
                    }
                    break;
            }
            #endregion
            #endregion
            // ReSharper disable once AssignNullToNotNullAttribute
            var outputs = Path.Combine(outputDirectory, naming + extension);
            await Extractall(bnkPath, extension, outputs, naming, infoFilePath);
        }
        /// <summary>
        /// Extracts all audio files from the provided BNK file.
        /// Handles file validation, extraction, and conversion.
        /// </summary>
        private static async Task Extractall(string bnkPath, string extension, string outputDirectory, string naming, string infoFilePath)
        {
            #region local properties
            Bnk bnkFile;
            FileStream fileStream;
            #endregion

            #region Resume of inputs given
            Console.WriteLine(new StringBuilder()
                .Append("Inputs given:")
                .AppendLine($"  -BNK file: {bnkPath}")
                .AppendLine($"  -Output format: {extension}")
                .AppendLine($"  -Naming: {naming}")
                .AppendLine($"  -Info file path: {infoFilePath}")
                .AppendLine($"  -Output(s) : {outputDirectory}").ToString());
            #endregion

            #region Open BNK file
            try
            {
                Console.WriteLine($@"Opening file ""{Path.GetFileName(bnkPath)}""...");
                Logger.Log($"Opening file \"{bnkPath}\"...");
                fileStream = FileOperations.StartFileStream(bnkPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                Console.WriteLine(new StringBuilder().Append(@"File successfully opened.").ToString());
                Logger.Log($"File \"{bnkPath}\" successfully opened.");
            }
            catch(Exception e)
            {
                Console.WriteLine(new StringBuilder().Append("Error while opening \"")
                    .Append(bnkPath)
                    .Append("\": \n\n")
                    .Append(e.Message)
                    .Append("\n")
                    .Append(e.StackTrace)
                    .ToString());
                Logger.Log($"Error while opening \"{bnkPath}\": \n\n{e.Message}\n{e.StackTrace}" + "\n\nBank audio file ignored.");
                return;
            }
            #endregion
            #region Parse BNK file
            try
            {
                Logger.Log($"Parsing \"{bnkPath}\"...\n");
                Console.WriteLine($@"Parsing ""{Path.GetFileName(bnkPath)}""...");
                var parsingTask = BnkFileHandler.PerformParsing(fileStream);
                parsingTask.Wait();
                bnkFile = parsingTask.Result;
                Task.Delay(10).Wait();
                Logger.Log("Parsing done.");
                Console.WriteLine(@"Parsing done.");
                #region Log the opening of the file
                var logString = "Chunk founds:";
                if (bnkFile.BkhdChunk != null) logString += "\n- \"BKHD\"";
                if (bnkFile.DataChunk != null) logString += "\n- \"DATA\"";
                if (bnkFile.DidxChunk != null) logString += "\n- \"DIDX\"";
                if (bnkFile.EnvsChunk != null) logString += "\n- \"ENVS\"";
                if (bnkFile.FxprChunk != null) logString += "\n- \"FXPR\"";
                if (bnkFile.HircChunk != null) logString += "\n- \"HIRC\"";
                if (bnkFile.InitChunk != null) logString += "\n- \"INIT\"";
                if (bnkFile.PlatChunk != null) logString += "\n- \"PLAT\"";
                if (bnkFile.StidChunk != null) logString += "\n- \"STID\"";
                if (bnkFile.StmgChunk != null) logString += "\n- \"STMG\"";
                Logger.Log(logString);
                Console.WriteLine(new StringBuilder().Append(logString).Append("\n").ToString());
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(new StringBuilder().Append("Error while parsing \"")
                    .Append(bnkPath)
                    .Append("\": \n\n")
                    .Append(ex.Message)
                    .Append("\n")
                    .Append(ex.StackTrace)
                    .ToString());
                Logger.Log($"Error while parsing \"{bnkPath}\": \n\n{ex.Message}\n{ex.StackTrace}" + "\n\nBank audio file ignored.");
                return;
            }
            #endregion
            #region Check if the BNK file is valid

            if (bnkFile.DidxChunk == null || bnkFile.DidxChunk.WemFilesInfo.Count == 0)
            {
                Console.WriteLine(new StringBuilder().Append("Error while parsing \"")
                    .Append(Path.GetFileName(bnkPath))
                    .Append("\": \n\nThe BNK file does not contain any audio files.")
                    .ToString());
                Logger.Log($"Error while parsing \"{bnkPath}\": \n\nThe BNK file does not contain any audio files." + "\n\nBank audio file ignored.");
                return;
            }
            if(bnkFile.DataChunk == null || bnkFile.DataChunk.WemFiles == null)
            {
                Console.WriteLine(new StringBuilder().Append("Error while parsing \"")
                    .Append(Path.GetFileName(bnkPath))
                    .Append("\": \n\nThe BNK file does not contain any audio data.")
                    .ToString());
                Logger.Log($"Error while parsing \"{bnkPath}\": \n\nThe BNK file does not contain any audio data." + "\n\nBank audio file ignored.");
                return;
            }
            #endregion
            #region Gather WEM files
            try
            {
                Logger.Log($"Gathering WEM files from \"{bnkPath}\"...");
                Console.WriteLine($@"Gathering WEM files from ""{Path.GetFileName(bnkPath)}""...");
                #region Feedback of the onging extraction
                var currentProgress = 0;
                var progressQueue = new ProgressQueue<int>(i =>
                {
                    currentProgress += i;
                    Console.Write(new StringBuilder().Append("\rGathering WEM files from BNK : ")
                        .Append(currentProgress)
                        .Append(" / ")
                        .Append(bnkFile.WemLibrary.Count)
                        .ToString());
                });
                var progress = new Progress<int>(i =>
                {
                    progressQueue.Report(i);
                });
                #endregion
                await BnkFileHandler.GatherWem(bnkFile, progress);
                await progressQueue.WaitForQueueToEmptyAsync();
                await Task.Delay(150);
                Console.WriteLine(new StringBuilder().Append("\nGathering done.\n").ToString());
                Logger.Log($"Gathering done.\nAudio founds:{bnkFile.WemLibrary.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(new StringBuilder().Append("Error while gathering WEM files from \"")
                    .Append(bnkPath)
                    .Append("\": \n\n")
                    .Append(ex.Message)
                    .Append("\n")
                    .Append(ex.StackTrace)
                    .ToString());
                Logger.Log($"Error while gathering WEM files from \"{bnkPath}\": \n\n{ex.Message}\n{ex.StackTrace}");
                return;
            }
            #endregion
            #region Convert WEM files
            try
            {
                Logger.Log($"Converting WEM files from \"{bnkPath}\"...");
                Console.WriteLine(new StringBuilder().Append("Converting WEM files from \"")
                    .Append(Path.GetFileName(bnkPath))
                    .Append("\"...")
                    .ToString());
                #region Feedback of the onging extraction
                var currentProgress = 0;
                var progressQueue = new ProgressQueue<int>(i =>
                {
                    currentProgress += i;
                    Console.Write(new StringBuilder().Append("\rConverting WEM files : ")
                        .Append(currentProgress)
                        .Append(" / ")
                        .Append(bnkFile.WemLibrary.Count)
                        .ToString());
                });
                var progress = new Progress<int>(i =>
                {
                    progressQueue.Report(i);
                });
                #endregion
                await BnkFileHandler.ConvertWemToStandardAudio(bnkFile, progress);
                await progressQueue.WaitForQueueToEmptyAsync();
                await Task.Delay(150);
                Console.WriteLine(new StringBuilder().Append("\nConversion done.\n").ToString());
                Logger.Log("Conversion done.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(new StringBuilder().Append("Error while converting WEM files from \"")
                    .Append(bnkPath)
                    .Append("\": \n\n")
                    .Append(ex.Message)
                    .Append("\n")
                    .Append(ex.StackTrace)
                    .ToString());
                Logger.Log($"Error while converting WEM files from \"{bnkPath}\": \n\n{ex.Message}\n{ex.StackTrace}");
                return;
            }
            #endregion
            #region Assign event names to audio files
            if(!string.IsNullOrEmpty(infoFilePath))
            {
                try
                {
                    Logger.Log($"Assigning event names to audio files from \"{bnkPath}\"...");
                    Console.WriteLine($@"Assigning event names to audio files from ""{Path.GetFileName(bnkPath)}""...");
                    #region Feedback of the onging extraction
                    var currentProgress = 0;
                    var progressQueue = new ProgressQueue<int>(i =>
                    {
                        currentProgress += i;
                        Console.Write(new StringBuilder().Append("\rAssigning event names to audio files : ")
                            .Append(currentProgress)
                            .Append(" / ")
                            .Append(bnkFile.WemLibrary.Count)
                            .ToString());
                    });
                    var progress = new Progress<int>(i =>
                    {
                        progressQueue.Report(i);
                    });
                    #endregion
                    await BnkFileHandler.AssignEventNames(bnkFile.WemLibrary, infoFilePath,new CancellationToken(), progress);
                    await progressQueue.WaitForQueueToEmptyAsync();
                    await Task.Delay(150);
                    Console.WriteLine(new StringBuilder().Append("\nAssignment done.\n").ToString());
                    Logger.Log("Assignment done.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(new StringBuilder()
                        .Append("Error while assigning event names to audio files from \"")
                        .Append(bnkPath)
                        .Append("\": \n\n")
                        .Append(ex.Message)
                        .Append("\n")
                        .Append(ex.StackTrace)
                        .ToString());
                    Logger.Log($"Error while assigning event names to audio files from \"{bnkPath}\": \n\n{ex.Message}\n{ex.StackTrace}");
                    return;
                }
            }
            #endregion
            #region Extracting audio files
            try
            {
                Logger.Log($"Extracting audio files from \"{bnkPath}\"...");
                Console.WriteLine($@"Extracting audio files from ""{Path.GetFileName(bnkPath)}""...");
                #region Feedback of the onging extraction
                var currentProgress = 0;
                var progressQueue = new ProgressQueue<int>(i =>
                {
                    currentProgress += i;
                    Console.Write(new StringBuilder().Append("\rExtracting audio files : ")
                        .Append(currentProgress)
                        .Append(" / ")
                        .Append(bnkFile.WemLibrary.Count)
                        .ToString());
                });
                var progress = new Progress<int>(i =>
                {
                    progressQueue.Report(i);
                });
                #endregion
                await AudioExtractionHandler.ExtractAllAudio(outputDirectory,bnkFile.AudioLibrary,new CancellationToken(), progress);
                await progressQueue.WaitForQueueToEmptyAsync();
                await Task.Delay(150);
                Console.WriteLine(new StringBuilder().Append("\nExtraction done!").ToString());
                Logger.Log("Extraction done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(new StringBuilder().Append("Error while extracting audio files from \"")
                    .Append(bnkPath)
                    .Append("\": \n\n")
                    .Append(ex.Message)
                    .Append("\n")
                    .Append(ex.StackTrace)
                    .ToString());
                Logger.Log($"Error while extracting audio files from \"{bnkPath}\": \n\n{ex.Message}\n{ex.StackTrace}");
            }
            #endregion
        }

        #region validate file name
        /// <summary>
        /// Validate a file name
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="Exception"></exception>
        public void ValidateFileName(ref string name)
        {
            //remove " at the start of the name
            if (!string.IsNullOrEmpty(name) && name.StartsWith("\""))
            {
                name = name.TrimStart('\"');
            }
            //remove " at the end of the name
            if (!string.IsNullOrEmpty(name) && name.EndsWith("\""))
            {
                name = name.TrimEnd('\"');
            }
            //check if the name ends with a space, if so, remove it
            if (!string.IsNullOrEmpty(name) && name.EndsWith(" "))
            {
                name = name.TrimEnd(' ');
            }
            //check if the name starts with a space, if so, remove it
            if (!string.IsNullOrEmpty(name) && name.StartsWith(" "))
            {
                name = name.TrimStart(' ');
            }

            // Check if name is null or empty
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("File name cannot be empty or only white space.");
            }

            // check for invalid characters in file names
            var invalidChars = Path.GetInvalidFileNameChars();

            foreach (var c in invalidChars)
            {
                if (name.Contains(c))
                {
                    throw new Exception($"File name contains invalid characters: \"{c}\"");
                }
            }

            // Check for reserved file names
            string[] reservedNames =
            {
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };
            var upperName = name.ToUpperInvariant();

            if (reservedNames.Any(reservedName => upperName.Equals(reservedName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception($"File name is reserved for windows only : \"{name}\"");
            }
        }
        #endregion

        #region validate full path
        /// <summary>
        /// Validate a file path
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="Exception"></exception>
        private static void ValidatePath(ref string path)
        {
            //remove " at the start of the path
            if (!string.IsNullOrEmpty(path) && path.StartsWith("\""))
            {
                path = path.TrimStart('\"');
            }
            //remove " at the end of the path
            if (!string.IsNullOrEmpty(path) && path.EndsWith("\""))
            {
                path = path.TrimEnd('\"');
            }
            //check if the path ends with a space, if so, remove it
            if (!string.IsNullOrEmpty(path) && path.EndsWith(" "))
            {
                path = path.TrimEnd(' ');
            }
            //check if the path starts with a space, if so, remove it
            if (!string.IsNullOrEmpty(path) && path.StartsWith(" "))
            {
                path = path.TrimStart(' ');
            }

            //check if path is null or empty
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
            {
                throw new Exception("Path cannot be empty or null.");
            }

            // Check for invalid path characters
            var invalidChars = Path.GetInvalidPathChars();
            foreach (var c in invalidChars)
            {
                if (path.Contains(c))
                {
                    throw new Exception($"Path <\"{path}\"> contains invalid characters: \"{c}\"");
                }
            }

            try
            {
                path = Path.GetFullPath(path);
            }
            catch(ArgumentException argEx)
            {
                throw new Exception($"Could not retrieve the absolute path of the file <\"{path}\">. ({argEx.Message})");
            }
            catch(SecurityException secEx)
            {
                throw new Exception($"Access to the file <\"{path}\"> is denied. ({secEx.Message})");
            }
            catch(NotSupportedException notSupEx)
            {
                throw new Exception($"Path <\"{path}\"> contains a colon (':') that is not part of a volume identifier. ({notSupEx.Message})");
            }
            catch (PathTooLongException pathEx)
            {
                throw new Exception($"Path <\"{path}\"> is too long. ({pathEx.Message})");
            }
            catch (Exception e)
            {
                throw new Exception($"Could not retrieve the absolute path of the file <\"{path}\">. ({e.Message})");
            }

            //check if there is a filename in the path
            if (Path.GetFileName(path) == string.Empty)
            {
                throw new Exception($"Path <\"{path}\"> does not contain a valid file name.");
            }

            //check if path is a valid path
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                throw new Exception($"Directory <\"{Path.GetDirectoryName(path)}\"> does not exist.");
            }

            //check if file exists
            if (!File.Exists(path))
            {
                throw new Exception($"File <\"{path}\"> does not exist.");
            }
        }
        #endregion

        #region validate directory
        /// <summary>
        /// Validate a directory path
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="Exception"></exception>
        private static void ValidateDirectory(string path)
        {
            //remove " at the start of the path
            if (!string.IsNullOrEmpty(path) && path.StartsWith("\""))
            {
                path = path.TrimStart('\"');
            }
            //remove " at the end of the path
            if (!string.IsNullOrEmpty(path) && path.EndsWith("\""))
            {
                path = path.TrimEnd('\"');
            }
            //check if the path ends with a space, if so, remove it
            if (!string.IsNullOrEmpty(path) && path.EndsWith(" "))
            {
                path = path.TrimEnd(' ');
            }
            //check if the path starts with a space, if so, remove it
            if (!string.IsNullOrEmpty(path) && path.StartsWith(" "))
            {
                path = path.TrimStart(' ');
            }

            //check if path is null or empty
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
            {
                throw new Exception("Path cannot be empty or null.");
            }

            // Check for invalid path characters
            var invalidChars = Path.GetInvalidPathChars();
            foreach (char c in invalidChars)
            {
                if (path.Contains(c))
                {
                    throw new Exception($"Path <\"{path}\"> contains invalid characters: \"{c}\"");
                }
            }
            //check if there is a filename in the path
            if (Path.GetExtension(path) != string.Empty)
            {
                throw new Exception($"Path <\"{path}\"> contain file.");
            }

            try
            {
                path = Path.GetFullPath(path);
            }
            catch (ArgumentException argEx)
            {
                throw new Exception($"Could not retrieve the absolute path <\"{path}\">. ({argEx.Message})");
            }
            catch (SecurityException secEx)
            {
                throw new Exception($"Access to the path <\"{path}\"> is denied. ({secEx.Message})");
            }
            catch (NotSupportedException notSupEx)
            {
                throw new Exception($"Path <\"{path}\"> contains a colon (':') that is not part of a volume identifier. ({notSupEx.Message})");
            }
            catch (PathTooLongException pathEx)
            {
                throw new Exception($"Path <\"{path}\"> is too long. ({pathEx.Message})");
            }
            catch (Exception e)
            {
                throw new Exception($"Could not retrieve the absolute path <\"{path}\">. ({e.Message})");
            }

            //check if path is a valid path
            if (!Directory.Exists(path))
            {
                throw new Exception($"Directory <\"{path}\"> does not exist.");
            }
        }
        #endregion
        /// <summary>
        /// Displays help information for the extractAll command.
        /// </summary>
        protected override void ShowHelp()
        {
            Console.WriteLine(new StringBuilder()
                .Append("\nUsages:")
                .Append("\n  BKSMTool.exe extractall -p <path> [-e <ext>] [-o <directory>] [-n <naming>] [-i <path>]")
                .Append("\n  BKSMTool.exe extractall [-h]")
                .Append("\n\nOptions:")
                .Append("\n  -p, --path <path>          Path to the BNK file to extract. (Required)")
                .Append("\n  -e, --extension <ext>      Output file format ('.WEM'/'.WAV'/'.OGG'/'.MP3'/'OriginalFormat').")
                .Append("\n                             (Optional, Default is '.WEM'.)")
                .Append("\n  -o, --output <directory>   Directory to save extracted files.")
                .Append("\n                             (Optional, Default is BNK file's directory.)")
                .Append("\n  -n, --name <naming>        Name of extracted audio files. (Optional, Default is 'ID'.)")
                .Append("\n                             -Custom name is supported. Names extracted files with a custom string followed by a number.")
                .Append("\n                             - 'ID'          Names extracted files by their ID.")
                .Append("\n                             - 'Event'       Names extracted files by their event name.")
                .Append("\n                             - 'IDWithEvent' Names extracted files by their ID and event name.")
                .Append("\n                             For 'Event' and 'IDWithEvent' see --infoFilePath argument.")
                .Append("\n  -i, --infoFilePath <path>  Path of the info file containing event audio names of the BNK file. ")
                .Append("\n                             (Only required if name arguments are 'Event' or 'IDWithEvent'.)")
                .Append("\n  -h, --help                 Display this help screen.").ToString());
        }
    }
}
