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

using System.Drawing;
using System.Runtime.InteropServices;
using System;
using System.Runtime.CompilerServices;

namespace BKSMTool.Miscellaneous
{
    /// <summary>
    /// NativeMethods contains P/Invoke methods to interact with Windows-specific APIs.
    /// It provides functionality to handle mouse tracking, title bar information, and flashing the window.
    /// </summary>
    public static class NativeMethods
    {
        // Windows Message Codes for mouse events in non-client areas
        public const int WmNcMouseMove = 0xA0;
        public const int WmNcMouseHover = 0x2A0;
        public const int WmNcMouseLeave = 0x2A2;

        // TrackMouseEvent constants
        public const int TmeHover = 0x1;
        public const int TmeLeave = 0x2;
        public const int TmeNonClient = 0x10;

        // P/Invoke declaration for tracking mouse events in a window (non-client area)
        [DllImport("user32.dll")]
        public static extern int TrackMouseEvent(ref TrackMouseEventStruct lpEventStructTrack);

        // P/Invoke declaration for retrieving title bar information
        [DllImport("user32.dll")]
        public static extern bool GetTitleBarInfo(IntPtr hWNd, ref TitleBarInfo pti);

        /// <summary>
        /// Gets the rectangle representing the title bar area of the given window.
        /// </summary>
        /// <param name="hWNd">Handle to the window</param>
        /// <returns>A Rectangle representing the title bar area</returns>
        public static Rectangle GetTitleBarRectangle(IntPtr hWNd)
        {
            var info = new TitleBarInfo { cbSize = (uint)Marshal.SizeOf(typeof(TitleBarInfo)) };
            GetTitleBarInfo(hWNd, ref info);
            return info.rcTitleBar.ToRectangle();
        }

        // Struct for handling mouse tracking events in the window (non-client areas)
        [StructLayout(LayoutKind.Sequential)]
        public struct TrackMouseEventStruct
        {
            public uint cbSize;
            public uint dwFlags;
            public IntPtr HWNdTrack;
            public uint dwHoverTime;
        }

        // Struct for retrieving title bar information
        [StructLayout(LayoutKind.Sequential)]
        public struct TitleBarInfo
        {
            public const int CChildrenTitlebar = 5;  // Constant for child windows count
            public uint cbSize;
            public Rect rcTitleBar;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CChildrenTitlebar + 1)]
            public uint[] RgState;
        }

        // Struct for handling rectangle data
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left, Top, Right, Bottom;

            /// <summary>
            /// Converts the Rect structure to a Rectangle.
            /// </summary>
            /// <returns>A Rectangle structure</returns>
            public Rectangle ToRectangle() => Rectangle.FromLTRB(Left, Top, Right, Bottom);
        }

        #region Flashing Window until Focused
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FlashWInfo pWFi);

        // Struct for flashing the window
        [StructLayout(LayoutKind.Sequential)]
        public struct FlashWInfo
        {
            public uint cbSize;
            public IntPtr hWNd;
            public uint dwFlags;
            public uint uCount;
            public uint dwTimeout;
        }

        // Enum for specifying flash window flags
        [Flags]
        public enum FlashWindowFlags : uint
        {
            FlashWAll = 3,
            FlashWTimerNoFg = 12
        }

        /// <summary>
        /// Flashes the window to attract attention, using the default timeout.
        /// </summary>
        /// <param name="hWNd">Handle to the window to flash</param>
        public static void FlashWindow(IntPtr hWNd)
        {
            var flashInfo = new FlashWInfo
            {
                cbSize = Convert.ToUInt32(Marshal.SizeOf(typeof(FlashWInfo))),
                hWNd = hWNd,
                dwFlags = (uint)(FlashWindowFlags.FlashWAll | FlashWindowFlags.FlashWTimerNoFg),
                uCount = uint.MaxValue,  // Flash indefinitely
                dwTimeout = 0            // Use default system timeout
            };

            FlashWindowEx(ref flashInfo);
        }
        #endregion

        #region File Dialogs
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public struct ComdlgFilterspec
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszName;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszSpec;
        }
        #region Constants

        public const uint FosOverwritePrompt = 0x00000002;
        public const uint FosStrictFileTypes = 0x00000004;
        public const uint FosNoChangeDir = 0x00000008;
        public const uint FosPickFolders = 0x00000020;
        public const uint FosForceFileSystem = 0x00000040;
        public const uint FosAllNonStorageItems = 0x00000080;
        public const uint FosNoValidate = 0x00000100;
        public const uint FosAllowMultiselect = 0x00000200;
        public const uint FosPathMustExist = 0x00000800;
        public const uint FosFileMustExist = 0x00001000;
        public const uint FosCreatePrompt = 0x00002000;
        public const uint FosShareAware = 0x00004000;
        public const uint FosNoReadOnlyReturn = 0x00008000;
        public const uint FosNoTestFileCreate = 0x00010000;
        public const uint FosHideMruPlaces = 0x00020000;
        public const uint FosHidePinnedPlaces = 0x00040000;
        public const uint FosNoDereferenceLinks = 0x00100000;
        public const uint FosOkButtonNeedsInteraction = 0x00200000;
        public const uint FosDoNotAddToRecent = 0x02000000;
        public const uint FosForceShowHidden = 0x10000000;
        public const uint FosDefaultNoMiniMode = 0x20000000;
        public const uint FosForcePreviewPaneOn = 0x40000000;
        public const uint FosSupportStreamableItems = unchecked(0x80000000);

        public const uint SOk = 0x00000000;

        //public const uint SigDnNormalDisplay = 0x00000000;   //Name
        //public const uint SigDnParentRelativeParsing = 0x80018001;   //Name
        //public const uint SigDnDesktopAbsoluteParsing = 0x80028000;   //Full Name
        //public const uint SigDnParentRelativeEditing = 0x80031001;   //Name
        //public const uint SigDnDesktopAbsoluteEditing = 0x8004c000;   //Full Name
        public const uint SigDnFileSysPath = 0x80058000;   //Full Name
        //public const uint SigDnUrl = 0x80068000;   //Name like a URL
        //public const uint SigDnParentRelativeForAddressBar = 0x8007c001;   //Name
        //public const uint SigDnParentRelative = 0x80080001;   //Name

        #endregion

        #region COM

        [ComImport, ClassInterface(ClassInterfaceType.None), TypeLibType(TypeLibTypeFlags.FCanCreate), Guid("C0B4E2F3-BA21-4773-8DBA-335EC946EB8B")]
        public class FileSaveDialogRCW { }

        [ComImport, ClassInterface(ClassInterfaceType.None), TypeLibType(TypeLibTypeFlags.FCanCreate), Guid("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7")]
        public class FileOpenDialogRCW { }



        [ComImport, Guid("42F85136-DB7E-439C-85F1-E4075D135FC8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IFileDialog
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            [PreserveSig()]
            uint Show([In, Optional] IntPtr hWndOwner); //IModalWindow 

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFileTypes([In] uint cFileTypes, [In, MarshalAs(UnmanagedType.LPArray)] ComdlgFilterspec[] rgFilterSpec);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFileTypeIndex([In] uint iFileType);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetFileTypeIndex(out uint piFileType);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint Advise([In, MarshalAs(UnmanagedType.Interface)] IntPtr pFde, out uint pdwCookie);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint Unadvise([In] uint dwCookie);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetOptions([In] uint fos);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetOptions(out uint fos);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, uint fDap);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint Close([MarshalAs(UnmanagedType.Error)] uint hr);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetClientGuid([In] ref Guid guid);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint ClearClientData();

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pfilter);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppEnum);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);
        }

        [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IShellItem
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint BindToHandler([In] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out IntPtr ppvOut);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetDisplayName([In] uint sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);   //, out IntPtr ppszName   //, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
        }

        [ComImport, Guid("b63ea76d-1f85-456f-a19c-48159efa858b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IShellItemArray
        {
            // Not supported: IBindCtx
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, out IntPtr ppvOut);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetPropertyStore([In] int flags, [In] ref Guid riid, out IntPtr ppv);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetPropertyDescriptionList([In] ref Guid riid, out IntPtr ppv);   //[In] ref PROPERTYKEY keyType, [In] ref Guid riid

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);   //[In] SIATTRIBFLAGS dwAttribFlags, [In] uint sfgaoMask

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetCount(out uint pdwNumItems);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetItemAt([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            // Not supported: IEnumShellItems (will use GetCount and GetItemAt instead)
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
        }

        #endregion

        //[DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        //internal static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);
        #endregion
    }
}
