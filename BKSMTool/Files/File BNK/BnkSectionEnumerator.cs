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

namespace BKSMTool.Files.File_BNK
{
    #region Enumeration of each sections in a BNK File
    //**************************************************//
    //********** Enumeration of each sections **********//
    //**************************************************//
    internal enum BnkSections : uint //representation in human left to right format. (big endian)
    {
        Bkhd = 0x44484B42,
        Didx = 0x58444944,
        Data = 0x41544144,
        Envs = 0x53564E45,
        Fxpr = 0x52505846,
        Hirc = 0x43524948,
        Init = 0x54494E49,
        Plat = 0x54414C50,
        Stid = 0x44495453,
        Stmg = 0x474D5453
    }
    #endregion
}
