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
using System.ComponentModel;
using BKSMTool.Files.File_Audio;
using BKSMTool.Files.File_BNK.SECTIONS;
using BKSMTool.Files.File_WEM;
using System;
using System.IO;
using System.Linq;

namespace BKSMTool.Files.File_BNK
{
    /// <summary>
    /// Represents a BNK file, encapsulating its sections (such as BKHD, DIDX, DATA, etc.) and the associated WEM files. 
    /// Implements <see cref="IDisposable"/> to ensure proper resource management and cleanup.
    /// </summary>
    public class Bnk : FileInformation, IDisposable
    {
        // Flag to indicate whether the object has been disposed
        private bool _disposed;

        #region Local Properties
        //********************************************//
        //********** Variables of the Class **********//
        //********************************************//

        /// <summary>
        /// Stores the order of sections in the BNK file.
        /// </summary>
        public readonly List<uint> SectionsOrder;

        /// <summary>
        /// Represents the BKHD chunk of the BNK file.
        /// </summary>
        public Bkhd BkhdChunk;

        /// <summary>
        /// Represents the DIDX chunk of the BNK file.
        /// </summary>
        public Didx DidxChunk;

        /// <summary>
        /// Represents the DATA chunk of the BNK file.
        /// </summary>
        public Data DataChunk;

        /// <summary>
        /// Represents the ENVS chunk of the BNK file.
        /// </summary>
        public Envs EnvsChunk;

        /// <summary>
        /// Represents the FXPR chunk of the BNK file.
        /// </summary>
        public Fxpr FxprChunk;

        /// <summary>
        /// Represents the HIRC chunk of the BNK file.
        /// </summary>
        public Hirc HircChunk;

        /// <summary>
        /// Represents the INIT chunk of the BNK file.
        /// </summary>
        public Init InitChunk;

        /// <summary>
        /// Represents the PLAT chunk of the BNK file.
        /// </summary>
        public Plat PlatChunk;

        /// <summary>
        /// Represents the STID chunk of the BNK file.
        /// </summary>
        public Stid StidChunk;

        /// <summary>
        /// Represents the STMG chunk of the BNK file.
        /// </summary>
        public Stmg StmgChunk;

        /// <summary>
        /// List containing the WEM files referenced in the BNK file.
        /// </summary>
        public List<Wem> WemLibrary = new List<Wem>();

        /// <summary>
        /// List containing the Audio files.
        /// </summary>
        public List<AudioFile> AudioLibrary = new List<AudioFile>();

        #endregion

        #region Constructor
        //*********************************//
        //********** Constructor **********//
        //*********************************//

        /// <summary>
        /// Initializes a new instance of the <see cref="Bnk"/> class and sets up the sections order list.
        /// </summary>
        /// <param name="file">The file stream representing the BNK file to be processed.</param>
        public Bnk(FileStream file) : base(file)
        {
            SectionsOrder = new List<uint>();
        }
        #endregion

        #region Methods

        #region Instantiate sections
        /// <summary>
        /// Instantiates the BKHD section and adds it to the section order.
        /// </summary>
        public void Instantiate_BKHD_Section()
        {
            BkhdChunk = new Bkhd();
            SectionsOrder.Add((uint)BnkSections.Bkhd);
        }

        /// <summary>
        /// Instantiates the DIDX section and adds it to the section order.
        /// </summary>
        public void Instantiate_DIDX_Section()
        {
            DidxChunk = new Didx();
            SectionsOrder.Add((uint)BnkSections.Didx);
        }

        /// <summary>
        /// Instantiates the DATA section and adds it to the section order.
        /// </summary>
        public void Instantiate_DATA_Section()
        {
            DataChunk = new Data();
            SectionsOrder.Add((uint)BnkSections.Data);
        }

        /// <summary>
        /// Instantiates the ENVS section and adds it to the section order.
        /// </summary>
        public void Instantiate_ENVS_Section()
        {
            EnvsChunk = new Envs();
            SectionsOrder.Add((uint)BnkSections.Envs);
        }

        /// <summary>
        /// Instantiates the FXPR section and adds it to the section order.
        /// </summary>
        public void Instantiate_FXPR_Section()
        {
            FxprChunk = new Fxpr();
            SectionsOrder.Add((uint)BnkSections.Fxpr);
        }

        /// <summary>
        /// Instantiates the HIRC section and adds it to the section order.
        /// </summary>
        public void Instantiate_HIRC_Section()
        {
            HircChunk = new Hirc();
            SectionsOrder.Add((uint)BnkSections.Hirc);
        }

        /// <summary>
        /// Instantiates the INIT section and adds it to the section order.
        /// </summary>
        public void Instantiate_INIT_Section()
        {
            InitChunk = new Init();
            SectionsOrder.Add((uint)BnkSections.Init);
        }

        /// <summary>
        /// Instantiates the PLAT section and adds it to the section order.
        /// </summary>
        public void Instantiate_PLAT_Section()
        {
            PlatChunk = new Plat();
            SectionsOrder.Add((uint)BnkSections.Plat);
        }

        /// <summary>
        /// Instantiates the STID section and adds it to the section order.
        /// </summary>
        public void Instantiate_STID_Section()
        {
            StidChunk = new Stid();
            SectionsOrder.Add((uint)BnkSections.Stid);
        }

        /// <summary>
        /// Instantiates the STMG section and adds it to the section order.
        /// </summary>
        public void Instantiate_STMG_Section()
        {
            StmgChunk = new Stmg();
            SectionsOrder.Add((uint)BnkSections.Stmg);
        }
        #endregion

        #region Add WEM File To Library
        /// <summary>
        /// Adds a WEM file to the WEM library and subscribes to its property changed event.
        /// </summary>
        /// <param name="wem">The WEM file to add to the library.</param>
        public void AddWemFileToLibrary(Wem wem)
        {
            wem.PropertyChanged += Wem_PropertyChanged;
            WemLibrary.Add(wem);
        }
        #endregion

        #region Event listeners for WEM
        /// <summary>
        /// Handles changes to the WEM file's properties, specifically monitoring the <see cref="Wem.IsModified"/> property.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments containing the property that changed.</param>
        private void Wem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var wem = (Wem)sender;
            if (e.PropertyName != nameof(Wem.IsModified)) return;
            if (wem.IsModified)
            {
                IsSaved = false;
            }
            else
            {
                if (WemLibrary.Any(w => w.IsModified))
                {
                    IsSaved = false;
                    return;
                }

                IsSaved = true;
            }
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Releases all resources used by the <see cref="Bnk"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Bnk"/> and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                FileStream?.Close();
                FileStream = null;
                FullPath = null;
                Name = null;
                Extension = null;
                ShortenedPath = null;
                SectionsOrder.Clear();

                foreach (var wem in WemLibrary)
                {
                    wem.PropertyChanged -= Wem_PropertyChanged;
                    wem.Dispose();
                }
                WemLibrary.Clear();
                WemLibrary = null;

                BkhdChunk?.Dispose();
                DidxChunk?.Dispose();
                DataChunk?.Dispose();
                EnvsChunk?.Dispose();
                FxprChunk?.Dispose();
                HircChunk?.Dispose();
                InitChunk?.Dispose();
                PlatChunk?.Dispose();
                StidChunk?.Dispose();
                StmgChunk?.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        /// Finalizes the object and ensures proper cleanup of resources.
        /// </summary>
        ~Bnk()
        {
            Dispose(false);
        }
        #endregion
        #endregion
    }
}