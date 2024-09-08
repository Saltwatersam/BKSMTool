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
using BKSMTool.Files.File_WEM;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BKSMTool.Files.File_BNK;
using BKSMTool.Files.File_BNK.SECTIONS;
using System;
using System.IO;

namespace BKSMTool.Files
{
    //==================================================================================================================================//
    /// <summary> Handles operations concerning BNK files</summary>
    /// <remarks>
    /// <br/> - Supports Opening, Parsing, Rebuilding and Saving BNK files, Also include assignment of event names to the WEM file
    /// </remarks>
    //==================================================================================================================================//
    public static class BnkFileHandler
    {
        #region Parsing BNK File

        #region Perform Parsing
        //***************************************************************************************************************//
        /// <summary>
        /// Asynchronously performs parsing of the BNK file in two steps: inspection and chunk parsing.
        /// </summary>
        /// <param name="fileStream">The file stream of the BNK file to be parsed.</param>
        /// <returns>A parsed BNK object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the input fileStream is null.</exception>
        /// <exception cref="Exception">Thrown when any error occurs during the parsing process.</exception>
        //***************************************************************************************************************//
        public static async Task<Bnk> PerformParsing(FileStream fileStream)
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream), @"The file to parse cannot be null.");
            }

            var bnk = new Bnk(fileStream);

            // Step 1: Inspect the BNK file
            try
            {
                await Task.Run(() => InspectBnk(bnk));  
            }
            catch
            {
                bnk.Dispose();
                throw;
            }

            // Step 2: Parse the BNK file using the information gathered during inspection
            try
            {
                await Task.Run(() => ParseBnkChunks(bnk));  
            }
            catch
            {
                bnk.Dispose();
                throw;
            }

            return bnk;
        }
        #endregion

        #region Inspect BNK (read file and find chunks)

        //***************************************************************************************************************//
        /// <summary> Inspect and find Sections/Chunks composing the BNK file</summary>
        /// <param name="thisBnk">"BNK" Object used for storing  information of BNK file</param>
        /// <returns>-nothing</returns>
        //***************************************************************************************************************//
        private static void InspectBnk(Bnk thisBnk)
        {
            #region internal properties

            //*************************************//
            //******** internal properties ********//
            //*************************************//
            int baseOffset;

            #endregion

            //Check Length of File
            if (thisBnk.FileStream.Length < 4)
            {
                throw new Exception(".bnk file is too small.");
            }

            //Check first chunk (Must be BKHD)
            if (FileOperations.ReadUint32(thisBnk.FileStream, 0) == (uint)BnkSections.Bkhd)
            {
                baseOffset = 0x00;
            }
            else
            {
                throw new Exception(".bnk file not recognized.");
            }

            long currentChunkOffset = baseOffset;

            #region Find each chunk

            //Reads once linearly the file
            //Create instance of each chunk found and keep track of their order
            //There must be a single type of each chunk inside the file
            while (currentChunkOffset < (thisBnk.FileStream.Length - 4 - 4))
            {
                long lastChunkSize;
                switch (FileOperations.ReadUint32(thisBnk.FileStream, currentChunkOffset))
                {
                    case (uint)BnkSections.Bkhd:

                        #region BKHD

                        if (!(thisBnk.SectionsOrder.Contains((uint)BnkSections.Bkhd)))
                        {
                            uint chunkSize = FileOperations.ReadUint32(thisBnk.FileStream, currentChunkOffset + 4);
                            thisBnk.Instantiate_BKHD_Section();
                            thisBnk.BkhdChunk.StartOffset = currentChunkOffset;
                            thisBnk.BkhdChunk.Size = chunkSize;
                            lastChunkSize = chunkSize;
                        }
                        else
                        {
                            throw new Exception("More than one BKHD chunk found.");
                        }

                        #endregion

                        break;
                    case (uint)BnkSections.Didx:

                        #region DIDX

                        if (!thisBnk.SectionsOrder.Contains((uint)BnkSections.Didx))
                        {
                            uint chunkSize = FileOperations.ReadUint32(thisBnk.FileStream, currentChunkOffset + 4);
                            thisBnk.Instantiate_DIDX_Section();
                            thisBnk.DidxChunk.StartOffset = currentChunkOffset;
                            thisBnk.DidxChunk.Size = chunkSize;
                            lastChunkSize = chunkSize;
                        }
                        else
                        {
                            throw new Exception("More than one DIDX chunk found.");
                        }

                        #endregion

                        break;
                    case (uint)BnkSections.Data:

                        #region DATA

                        if (!thisBnk.SectionsOrder.Contains((uint)BnkSections.Data))
                        {
                            uint chunkSize = FileOperations.ReadUint32(thisBnk.FileStream, currentChunkOffset + 4);
                            thisBnk.Instantiate_DATA_Section();
                            thisBnk.DataChunk.StartOffset = currentChunkOffset;
                            thisBnk.DataChunk.Size = chunkSize;
                            lastChunkSize = chunkSize;
                        }
                        else
                        {
                            throw new Exception("More than one DATA chunk found.");
                        }

                        #endregion

                        break;
                    case (uint)BnkSections.Envs:

                        #region ENVS

                        if (!thisBnk.SectionsOrder.Contains((uint)BnkSections.Envs))
                        {
                            uint chunkSize = FileOperations.ReadUint32(thisBnk.FileStream, currentChunkOffset + 4);
                            thisBnk.Instantiate_ENVS_Section();
                            thisBnk.EnvsChunk.StartOffset = currentChunkOffset;
                            thisBnk.EnvsChunk.Size = chunkSize;
                            lastChunkSize = chunkSize;
                        }
                        else
                        {
                            throw new Exception("More than one ENVS chunk found.");
                        }

                        #endregion

                        break;
                    case (uint)BnkSections.Fxpr:

                        #region FXPR

                        if (!thisBnk.SectionsOrder.Contains((uint)BnkSections.Fxpr))
                        {
                            uint chunkSize = FileOperations.ReadUint32(thisBnk.FileStream, currentChunkOffset + 4);
                            thisBnk.Instantiate_FXPR_Section();
                            thisBnk.FxprChunk.StartOffset = currentChunkOffset;
                            thisBnk.FxprChunk.Size = chunkSize;
                            lastChunkSize = chunkSize;
                        }
                        else
                        {
                            throw new Exception("More than one FXPR chunk found.");
                        }

                        #endregion

                        break;
                    case (uint)BnkSections.Hirc:

                        #region HIRC

                        if (!thisBnk.SectionsOrder.Contains((uint)BnkSections.Hirc))
                        {
                            uint chunkSize = FileOperations.ReadUint32(thisBnk.FileStream, currentChunkOffset + 4);
                            thisBnk.Instantiate_HIRC_Section();
                            thisBnk.HircChunk.StartOffset = currentChunkOffset;
                            thisBnk.HircChunk.Size = chunkSize;
                            lastChunkSize = chunkSize;
                        }
                        else
                        {
                            throw new Exception("More than one HIRC chunk found.");
                        }

                        #endregion

                        break;
                    case (uint)BnkSections.Init:

                        #region INIT

                        if (!thisBnk.SectionsOrder.Contains((uint)BnkSections.Init))
                        {
                            uint chunkSize = FileOperations.ReadUint32(thisBnk.FileStream, currentChunkOffset + 4);
                            thisBnk.Instantiate_INIT_Section();
                            thisBnk.InitChunk.StartOffset = currentChunkOffset;
                            thisBnk.InitChunk.Size = chunkSize;
                            lastChunkSize = chunkSize;
                        }
                        else
                        {
                            throw new Exception("More than one INIT chunk found.");
                        }

                        #endregion

                        break;
                    case (uint)BnkSections.Plat:

                        #region PLAT

                        if (!thisBnk.SectionsOrder.Contains((uint)BnkSections.Plat))
                        {
                            uint chunkSize = FileOperations.ReadUint32(thisBnk.FileStream, currentChunkOffset + 4);
                            thisBnk.Instantiate_PLAT_Section();
                            thisBnk.PlatChunk.StartOffset = currentChunkOffset;
                            thisBnk.PlatChunk.Size = chunkSize;
                            lastChunkSize = chunkSize;
                        }
                        else
                        {
                            throw new Exception("More than one PLAT chunk found.");
                        }

                        #endregion

                        break;
                    case (uint)BnkSections.Stid:

                        #region STID

                        if (!thisBnk.SectionsOrder.Contains((uint)BnkSections.Stid))
                        {
                            uint chunkSize = FileOperations.ReadUint32(thisBnk.FileStream, currentChunkOffset + 4);
                            thisBnk.Instantiate_STID_Section();
                            thisBnk.StidChunk.StartOffset = currentChunkOffset;
                            thisBnk.StidChunk.Size = chunkSize;
                            lastChunkSize = chunkSize;
                        }
                        else
                        {
                            throw new Exception("More than one STID chunk found.");
                        }

                        #endregion

                        break;
                    case (uint)BnkSections.Stmg:

                        #region STMG

                        if (!thisBnk.SectionsOrder.Contains((uint)BnkSections.Stmg))
                        {
                            uint chunkSize = FileOperations.ReadUint32(thisBnk.FileStream, currentChunkOffset + 4);
                            thisBnk.Instantiate_STMG_Section();
                            thisBnk.StmgChunk.StartOffset = currentChunkOffset;
                            thisBnk.StmgChunk.Size = chunkSize;
                            lastChunkSize = chunkSize;
                        }
                        else
                        {
                            throw new Exception("More than one STMG chunk found.");
                        }

                        #endregion

                        break;
                    default:
                        throw new Exception("Unknown chunk found or corrupted file.");
                }

                currentChunkOffset = currentChunkOffset + lastChunkSize + 4 + 4;
            }

            #endregion
        }

        #endregion

        #region Parse Chunks

        //***************************************************************************************************************//
        /// <summary> Parse Sections/Chunks composing the BNK file, previously found during inspection</summary>
        /// <param name="thisBnk">"BNK" Object used for storing  information of BNK file</param>
        /// <returns>-nothing</returns>
        //***************************************************************************************************************//
        private static void ParseBnkChunks(Bnk thisBnk)
        {
            #region Parse and decompose each chunk

            //DIDX chunk (Data Index?) contains the references to the .wem files embedded in the BNK
            //Each audio file is described inside DIDX with 12 bytes
            //4 Bytes : audio file (.wem) ID
            //4 Bytes : offset from the start of DATA chunk, where the audio file (.wem) is stored and begin
            //4 Bytes : length in bytes of the audio file (.wem)

            //Parsing DIDX chunk in first. Since it's elements are used in others chunks 
            if (thisBnk.SectionsOrder.Contains((uint)BnkSections.Didx)) //if a DIDX chunk has been found
            {
                if (thisBnk.DidxChunk.Size % 12 == 0) //check if the DIDX chunk size is a multiple of 12
                {
                    thisBnk.DidxChunk.NumberOfWem =
                        thisBnk.DidxChunk.Size /
                        12; //define the number of WEM                                                                                          
                    //divide the section length by the number of bytes used by each WEM ([ ID: 4By ] + [Offset: 4By ] + [Size: 4By ])
                }
                else
                {
                    throw new Exception("In .bnk file, DIDX chunk seems corrupted.");
                }

                for (uint kk = 0;
                     kk < thisBnk.DidxChunk.NumberOfWem;
                     kk++) //Parse each WEM information's found in the DIDX chunk
                {
                    thisBnk.DidxChunk.WemFilesInfo.Add(new WemInfo(
                        FileOperations.ReadUint32(thisBnk.FileStream,
                            thisBnk.DidxChunk.StartOffset + 4 + 4 + (kk * 12)),
                        FileOperations.ReadUint32(thisBnk.FileStream,
                            thisBnk.DidxChunk.StartOffset + 4 + 4 + (kk * 12) + 4),
                        FileOperations.ReadUint32(thisBnk.FileStream,
                            thisBnk.DidxChunk.StartOffset + 4 + 4 + (kk * 12) + 4 + 4)));
                }
            }

            //Parse each others chunks found
            foreach (var section in thisBnk.SectionsOrder)
            {
                switch (section)
                {
                    case (uint)BnkSections.Bkhd:

                        #region BKHD

                        //BKHD chunk (Bank Header?) contains the version number? and the Audio Bank id ? + ....

                        if (thisBnk.BkhdChunk.Size >= 0x08)
                        {
                            thisBnk.BkhdChunk.VersionNumber = FileOperations.ReadUint32(thisBnk.FileStream,
                                thisBnk.BkhdChunk.StartOffset + 4 + 4);
                            thisBnk.BkhdChunk.Id = FileOperations.ReadUint32(thisBnk.FileStream,
                                thisBnk.BkhdChunk.StartOffset + 4 + 4 + 4);
                        }
                        else
                        {
                            throw new Exception("In .bnk file, BKHD chunk is too small.");
                        }

                        //if some bytes are unmanaged in the end of section, store them
                        if (thisBnk.BkhdChunk.Size > 0x08)
                        {
                            long numberOfBytesLeft = (thisBnk.BkhdChunk.Size - 0x08);
                            thisBnk.BkhdChunk.LeftBytes = FileOperations.ReadBytes(thisBnk.FileStream,
                                thisBnk.BkhdChunk.StartOffset + 4 + 4 + 4 + 4, numberOfBytesLeft);
                        }

                        #endregion

                        break;
                    case (uint)BnkSections.Didx:

                        #region DIDX

                        //nothing
                        //this chunk has been parsed in first if there was one...

                        #endregion

                        break;
                    case (uint)BnkSections.Data:

                        #region DATA

                        //DATA chunk (data? :) ) contains the .wem files not encoded and immediately following each others
                        //each .wem files is padded to 4 bytes but not the last one

                        if (thisBnk.BkhdChunk.VersionNumber <=
                            26) //old games that had not DIDX sections. each audio must be found in a DATA chunk
                        {
                            throw new Exception("old .bnk file version detected.\nIt is not supported.");
                        }

                        if (thisBnk.SectionsOrder.Contains((uint)BnkSections.Didx) &&
                            thisBnk.BkhdChunk.VersionNumber > 26)
                        {
                            for (int ii = 0; ii < thisBnk.DidxChunk.NumberOfWem; ii++)
                            {
                                if ((thisBnk.DataChunk.StartOffset + 4 + 4 +
                                     thisBnk.DidxChunk.WemFilesInfo.ElementAt(ii).StartOffset +
                                     thisBnk.DidxChunk.WemFilesInfo.ElementAt(ii).Size)
                                    <= (thisBnk.DataChunk.StartOffset + 4 + 4 + thisBnk.DataChunk.Size))
                                {
                                    thisBnk.DataChunk.WemFiles.Add(new WemData(
                                        FileOperations.ReadBytes(thisBnk.FileStream,
                                            thisBnk.DataChunk.StartOffset + 4 + 4 +
                                            thisBnk.DidxChunk.WemFilesInfo.ElementAt(ii).StartOffset,
                                            thisBnk.DidxChunk.WemFilesInfo.ElementAt(ii).Size)));
                                }
                                else
                                {
                                    throw new Exception("An embedded .wem audio file is out of scope of DATA chunk.");
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Unable to parse DATA chunk because no DIDX chunk were found.");
                        }

                        #endregion

                        break;
                    case (uint)BnkSections.Envs:

                        #region ENVS

                        //ENVS chunk (Environments?) yet to analyse...

                        //unmanaged bytes, store them
                        thisBnk.EnvsChunk.LeftBytes = FileOperations.ReadBytes(thisBnk.FileStream,
                            thisBnk.EnvsChunk.StartOffset + 4 + 4, thisBnk.EnvsChunk.Size);

                        #endregion

                        break;
                    case (uint)BnkSections.Fxpr:

                        #region FXPR

                        //FXPR chunk (Effects production?) yet to analyse...

                        //unmanaged bytes, store them
                        thisBnk.FxprChunk.LeftBytes = FileOperations.ReadBytes(thisBnk.FileStream,
                            thisBnk.FxprChunk.StartOffset + 4 + 4, thisBnk.FxprChunk.Size);

                        #endregion

                        break;
                    case (uint)BnkSections.Hirc:

                        #region HIRC

                        //contains all the Wwise objects, events, containers to group audio, the references to the audio files, yet to analyse...

                        //unmanaged bytes, store them
                        thisBnk.HircChunk.LeftBytes = FileOperations.ReadBytes(thisBnk.FileStream,
                            thisBnk.HircChunk.StartOffset + 4 + 4, thisBnk.HircChunk.Size);

                        #endregion

                        break;
                    case (uint)BnkSections.Init:

                        #region INIT

                        //INIT chunk (initializer?) yet to analyse...

                        //unmanaged bytes, store them
                        thisBnk.InitChunk.LeftBytes = FileOperations.ReadBytes(thisBnk.FileStream,
                            thisBnk.InitChunk.StartOffset + 4 + 4, thisBnk.InitChunk.Size);

                        #endregion

                        break;
                    case (uint)BnkSections.Plat:

                        #region PLAT

                        //PLAT chunk (...?) yet to analyse...

                        //unmanaged bytes, store them
                        thisBnk.PlatChunk.LeftBytes = FileOperations.ReadBytes(thisBnk.FileStream,
                            thisBnk.PlatChunk.StartOffset + 4 + 4, thisBnk.PlatChunk.Size);

                        #endregion

                        break;
                    case (uint)BnkSections.Stid:

                        #region STID

                        //STID chunk (Sound type ID ?) yet to analyse...
                        //contains a list of all he audio banks referenced in the HIRC section

                        //unmanaged bytes, store them
                        thisBnk.StidChunk.LeftBytes = FileOperations.ReadBytes(thisBnk.FileStream,
                            thisBnk.StidChunk.StartOffset + 4 + 4, thisBnk.StidChunk.Size);

                        #endregion

                        break;
                    case (uint)BnkSections.Stmg:

                        #region STMG

                        //STMG chunk (... ?) yet to analyse...
                        //only found in the Init.bnk

                        //unmanaged bytes, store them
                        thisBnk.StmgChunk.LeftBytes = FileOperations.ReadBytes(thisBnk.FileStream,
                            thisBnk.StmgChunk.StartOffset + 4 + 4, thisBnk.StmgChunk.Size);

                        #endregion

                        break;
                    default:
                        throw new Exception("Unknown Chunk found.");
                }
            }

            #endregion
        }

        #endregion

        #endregion

        #region Gather WEM
        //***************************************************************************************************************//
        /// <summary>
        /// Gather WEM files from the BNK object
        /// </summary>
        /// <param name="thisBnk"></param>
        /// <param name="progressReporter"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        //***************************************************************************************************************//
        public static async Task GatherWem(Bnk thisBnk,IProgress<int> progressReporter)
        {
            await Task.Delay(0);
            if ((thisBnk.DidxChunk != null) && (thisBnk.DidxChunk.WemFilesInfo.Count > 0))
            {
                var progressLock = new object();
                for (var i = 0; i < thisBnk.DidxChunk.WemFilesInfo.Count; i++) //foreach WEM found in the BNK
                {
                    var thisWem = new Wem(thisBnk.DidxChunk.WemFilesInfo[i], thisBnk.DataChunk.WemFiles[i]); //create a new WEM object
                    thisBnk.AddWemFileToLibrary(thisWem); //add it to the BNK object
                    thisBnk.AudioLibrary.Add(new AudioFile(thisWem)); //create a new AudioFile object and add it to the BNK object
                    lock (progressLock)
                    {
                        progressReporter?.Report(1);
                    }
                }
                //return thisBnk.wemLibrary;
            }
            else
            {
                throw new Exception("No WEM found in the BNK.");
            }
        }
        #endregion

        #region Convert WEM to Standard Audio
        public static async Task ConvertWemToStandardAudio(Bnk thisBnk, IProgress<int> progressReporter)
        {
            if(thisBnk.AudioLibrary.Count == 0)
            {
                throw new Exception("No WEM found in the BNK, nothing to convert.");
            }

            var progressLock = new object();
            var conversionTasks = thisBnk.AudioLibrary.Select(audio => Task.Run(() =>
                {
                    audio.RebuildAudio();
                    lock (progressLock)
                    {
                        progressReporter?.Report(1);
                    }
                }))
                .ToList();

            await Task.WhenAll(conversionTasks);
        }
        #endregion

        #region Rebuild BNK

        //***************************************************************************************************************//
        /// <summary> Build back a raw BNK file (stored in byte array) from the BNK object</summary>
        /// <param name="thisBnk">"BNK" Object used for storing  information of BNK file</param>
        /// <returns>byte[] : the BNK raw file</returns>
        //***************************************************************************************************************//
        public static byte[] PerformReBuild(Bnk thisBnk)
        {
            #region internal properties

            //*************************************//
            //******** internal properties ********//
            //*************************************//
            byte[] _ = new byte[4];

            #endregion

            #region Method's Body

            //*************************************//
            //******** Method's Body **************//
            //*************************************//
            using (var dataStream
                   = new MemoryStream())
            {
                for (int hh = 0; hh < thisBnk.SectionsOrder.Count; hh++)
                {
                    switch (thisBnk.SectionsOrder.ElementAt(hh))
                    {
                        case (uint)BnkSections.Bkhd: //rebuild BKHD chunk

                            #region BKHD

                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                (uint)BnkSections.Bkhd); //write the chunk name
                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                thisBnk.BkhdChunk.Size); //write the size of the chunk
                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                thisBnk.BkhdChunk.VersionNumber); //write the version number ?
                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                thisBnk.BkhdChunk.Id); //write the Bank ID ?
                            FileOperations.WriteBytes(dataStream.Length, dataStream,
                                thisBnk.BkhdChunk.LeftBytes); //write the remaining data

                            #endregion

                            break;
                        case (uint)BnkSections.Didx: //rebuild DIDX chunk

                            #region DIDX

                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                (uint)BnkSections.Didx); //write the chunk name
                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                (uint)(thisBnk.WemLibrary.Count * 3) * 4); //write the size of the chunk

                            uint didxOffsetCounter = 0;
                            for (int mm = 0; mm < thisBnk.WemLibrary.Count; mm++) //for each listed audio :
                            {
                                FileOperations.WriteUint32(dataStream.Length, dataStream,
                                    thisBnk.WemLibrary.ElementAt(mm).IdValue); //write the WEM ID
                                FileOperations.WriteUint32(dataStream.Length, dataStream,
                                    didxOffsetCounter); //write the offset

                                byte[] wemData = thisBnk.WemLibrary.ElementAt(mm).Data;
                                if (mm < thisBnk.WemLibrary.Count -
                                    1) //for whatever reason, a left padding is used but not for the last audio
                                {
                                    FileOperations.PadToMultipleOf(ref wemData, 16);
                                }

                                FileOperations.WriteUint32(dataStream.Length, dataStream,
                                    (uint)thisBnk.WemLibrary.ElementAt(mm).Data.Length); //write the size
                                didxOffsetCounter += (uint)wemData.Length; //increment the offset
                            }

                            #endregion

                            break;
                        case (uint)BnkSections.Data: //rebuild DATA chunk

                            #region DATA

                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                (uint)BnkSections.Data); //write the chunk name

                            uint size = 0;
                            for (int oo = 0; oo < thisBnk.WemLibrary.Count; oo++) //calculate the size of the chunk
                            {
                                byte[] bytes = thisBnk.WemLibrary.ElementAt(oo).Data;
                                if (oo < thisBnk.WemLibrary.Count -
                                    1) //for whatever reason, a left padding is used but not for the last audio
                                {
                                    FileOperations.PadToMultipleOf(ref bytes, 16);
                                }

                                size += (uint)bytes.Length;
                            }

                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                size); //write the size of the chunk

                            for (int pp = 0; pp < thisBnk.WemLibrary.Count; pp++) //for each audio :
                            {
                                byte[] bytes = thisBnk.WemLibrary.ElementAt(pp).Data;
                                if (pp < thisBnk.WemLibrary.Count - 1) //add a left padding if needed
                                {
                                    FileOperations.PadToMultipleOf(ref bytes, 16);
                                }

                                FileOperations.WriteBytes(dataStream.Length, dataStream, bytes); //write the audio
                            }

                            #endregion

                            break;
                        case (uint)BnkSections.Envs: //rebuild ENVS chunk

                            #region ENVS

                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                (uint)BnkSections.Envs); //write the chunk name
                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                thisBnk.EnvsChunk.Size); //write the size of the chunk
                            FileOperations.WriteBytes(dataStream.Length, dataStream,
                                thisBnk.EnvsChunk.LeftBytes); //write the remaining data

                            #endregion

                            break;
                        case (uint)BnkSections.Fxpr: //rebuild FXPR chunk

                            #region FXPR

                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                (uint)BnkSections.Fxpr); //write the chunk name
                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                thisBnk.FxprChunk.Size); //write the size of the chunk
                            FileOperations.WriteBytes(dataStream.Length, dataStream,
                                thisBnk.FxprChunk.LeftBytes); //write the remaining data

                            #endregion

                            break;
                        case (uint)BnkSections.Hirc: //rebuild HIRC chunk

                            #region HIRC

                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                (uint)BnkSections.Hirc); //write the chunk name
                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                thisBnk.HircChunk.Size); //write the size of the chunk
                            FileOperations.WriteBytes(dataStream.Length, dataStream,
                                thisBnk.HircChunk.LeftBytes); //write the remaining data

                            #endregion

                            break;
                        case (uint)BnkSections.Init: //rebuild INIT chunk

                            #region INIT

                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                (uint)BnkSections.Init); //write the chunk name
                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                thisBnk.InitChunk.Size); //write the size of the chunk
                            FileOperations.WriteBytes(dataStream.Length, dataStream,
                                thisBnk.InitChunk.LeftBytes); //write the remaining data

                            #endregion

                            break;
                        case (uint)BnkSections.Plat: //rebuild PLAT chunk

                            #region PLAT

                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                (uint)BnkSections.Plat); //write the chunk name
                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                thisBnk.PlatChunk.Size); //write the size of the chunk
                            FileOperations.WriteBytes(dataStream.Length, dataStream,
                                thisBnk.PlatChunk.LeftBytes); //write the remaining data

                            #endregion

                            break;
                        case (uint)BnkSections.Stid: //rebuild STID chunk

                            #region STID

                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                (uint)BnkSections.Stid); //write the chunk name
                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                thisBnk.StidChunk.Size); //write the size of the chunk
                            FileOperations.WriteBytes(dataStream.Length, dataStream,
                                thisBnk.StidChunk.LeftBytes); //write the remaining data

                            #endregion

                            break;
                        case (uint)BnkSections.Stmg: //rebuild STMG chunk

                            #region STMG

                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                (uint)BnkSections.Stmg); //write the chunk name
                            FileOperations.WriteUint32(dataStream.Length, dataStream,
                                thisBnk.StmgChunk.Size); //write the size of the chunk
                            FileOperations.WriteBytes(dataStream.Length, dataStream,
                                thisBnk.StmgChunk.LeftBytes); //write the remaining data

                            #endregion

                            break;
                    }
                }

                return dataStream.ToArray();
            }

            #endregion
        }

        #endregion

        #region Save BNK

        //***************************************************************************************************************//
        /// <summary> Write back the BNK object to the BNK file and update every saved state</summary>
        /// <param name="thisBnk">"BNK" Object used for storing  information of BNK file</param>
        /// <returns>-nothing</returns>
        //***************************************************************************************************************//
        public static async Task SaveChanges(Bnk thisBnk)
        {
            // Save the data to a file
            try
            {
                if (thisBnk != null)
                {
                    byte[] data = await Task.Run(() => PerformReBuild(thisBnk));
                    await Task.Run(() =>
                    {
                        thisBnk.FileStream.SetLength(0);
                        thisBnk.FileStream.Write(data, 0, data.Length);
                    });
                    foreach (Wem wemFile in thisBnk.WemLibrary)
                    {
                        if (wemFile != null && wemFile.IsModified)
                        {
                            wemFile.Save();
                        }
                    }
                }
                else
                {
                    throw new Exception("BNK is null");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Assignment of event names from TXT file

        //***************************************************************************************************************//
        /// <summary> Perform the assignment of event names of WEM audio based on the selected path</summary>
        /// <param name="wemLibrary">"List<WEM></WEM>" list of each WEM objects from the BNK file</param>
        /// <param name="fileTxt">"string" Selected path of the .TXT file containing event names</param>
        /// <param name="cancellationToken"></param>
        /// <param name="progressReporter"></param>
        /// <returns>- nothing</returns>
        //***************************************************************************************************************//
        public static async Task AssignEventNames(List<Wem> wemLibrary, string fileTxt, CancellationToken cancellationToken, IProgress<int> progressReporter = null)
        {
            if (wemLibrary == null || wemLibrary.Count == 0)
            {
                throw new Exception("No WEM files to assign event names to.");
            }

            if (!File.Exists(fileTxt))
            {
                throw new Exception("No .txt file to assign event names from.");
            }

            #region Internal properties
            //*************************************//
            //******** internal properties ********//
            //*************************************//
            var listEachLineInTxt = File.ReadAllLines(fileTxt); //store each lines of the selected .TXT file
            var sectionInMemoryAudioFound = false; //boolean returning if the section "In memory Audio" was found
            var eventNames = new Dictionary<string, string>();
            var progressLock = new object();
            #endregion

            #region Clear precedent event name
            var clearEventNames = Task.Run(() =>
            {
                //=============================================================//
                //**********     Clear precedent assigned file      ***********//
                //=============================================================//
                foreach (var wem in wemLibrary)
                {
                    wem.EventName = null;
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }, cancellationToken);
            #endregion
            #region Gather new event names
            var findNewEventNames = Task.Run(() =>
            {
                //=============================================================//
                //**********   Pass through each line of .TXT file  ***********//
                //=============================================================//
                foreach (var line in listEachLineInTxt)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var typesInLine = line.Split('\t');   //split each element separated by a tab (\t) within line

                    if (!sectionInMemoryAudioFound)
                    {
                        if (!typesInLine[00].Contains("In Memory Audio")) continue;
                        sectionInMemoryAudioFound = true;
                    }
                    else
                    {
                        //=============================================================//
                        //*********** Gather every ID and event name founds ***********//
                        //=============================================================//
                        if (typesInLine.Length >= 3)
                        {
                            eventNames.Add(typesInLine[1], typesInLine[2]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }, cancellationToken);
            #endregion

            clearEventNames.Wait(cancellationToken);
            findNewEventNames.Wait(cancellationToken);

            if (!sectionInMemoryAudioFound)
            {
                progressReporter?.Report(wemLibrary.Count);
                throw new Exception("No section containing audio information's were found.");
            }
            if (eventNames.Count == 0)
            {
                progressReporter?.Report(wemLibrary.Count);
                throw new Exception("No event names found in the .txt file.");
            }

            #region Assign event names to WEM
            var assigningTasks = wemLibrary.Select(wem => Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (eventNames.TryGetValue(wem.IdValue.ToString(), out string name))
                    {
                        wem.EventName = name;
                    }

                    lock (progressLock)
                    {
                        progressReporter?.Report(1);
                    }
                }, cancellationToken))
                .ToList();
            if(cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
            await Task.WhenAll(assigningTasks);
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
            #endregion
        }
        #endregion
    }
}