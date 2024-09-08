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

namespace BKSMTool.Files.File_WEM
{

    //**************************************************//
    //********** Enumeration of each WEM Types *********//
    //**************************************************//

    //Trove use :   210 times FFFE audio always 0x18 fmt size
    //              755 times 0002 audio always 0x18 fmt size
    //              3140 times FFFF audio always 0x42 fmt size
    public enum WemType
    {
        Pcm2 = 0xFFFE,         /* "PCM for Wwise Authoring" */
        Ima = 0x0002,           /* newer Wwise (variable, probably means "platform's ADPCM") */
        Vorbis = 0xFFFF,
        //PCM = 0x0001,         /* older Wwise */
        //IMA2 = 0x0069,        /* older Wwise [Spiderman Web of Shadows (X360), LotR Conquest (PC)] */
        //XWMA = 0x0161,        /* WMAv2 */
        //XWMA_2 = 0x0162,      /* WMAPro */
        //DSP = 0xFFF0,
        //XMA2 = 0x0165,        /* XMA2-chunk XMA (Wwise doesn't use XMA1) */
        //XMA2_2 = 0x0166,      /* fmt-chunk XMA */
        //AAC = 0xAAC0,
        //HEVAG = 0xFFFB,       /* "VAG" */
        //ATRAC9 = 0xFFFC,
        //OPUSNX = 0x3039,      /* renamed from "OPUS" on Wwise 2018.1 */
        //OPUS = 0x3040,
        //OPUSCPR,              // ?????
        //OPUSWW = 0x3041,      /* "OPUS_WEM", added on Wwise 2019.2.3, replaces OPUS */
        //PTADPCM = 0x8311,     /* added on Wwise 2019.1, replaces IMA */
    }
}