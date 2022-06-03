using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

using MFMediaEngine;
using static MFMediaEngine.MediaEngineTools;
using DXGI;
using static DXGI.DXGITools;
using GlobalStructures;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
//using D3D11;

namespace WinUI3_MediaEngine
{
    public class CMediaEngine : IMFMediaEngineNotify, IMFTimedTextNotify, IDisposable // , System.ComponentModel.INotifyPropertyChanged
    {
        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateSolidBrush(int crColor);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool FillRect(IntPtr hdc, [In] ref RECT rect, IntPtr hbrush);

        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateBitmap(int nWidth, int nHeight, uint nPlanes, uint nBitCount, IntPtr lpBits);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeleteObject(IntPtr ho);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PatBlt(IntPtr hdc, int x, int y, int w, int h, int rop);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SetStretchBltMode(IntPtr hdc, int mode);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool StretchBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, int rop);

        public const int BLACKONWHITE = 1;
        public const int WHITEONBLACK = 2;
        public const int COLORONCOLOR = 3;
        public const int HALFTONE = 4;
        public const int MAXSTRETCHBLTMODE = 4;

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetObject(IntPtr hFont, int nSize, out BITMAP bm);

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct BITMAP
        {
            public int bmType;
            public int bmWidth;
            public int bmHeight;
            public int bmWidthBytes;
            public short bmPlanes;
            public short bmBitsPixel;
            public IntPtr bmBits;
        }

        public int RGB(byte r, byte g, byte b)
        {
            return (r) | ((g) << 8) | ((b) << 16);
        }

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern short GetKeyState(int nVirtKey);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetSystemMetrics(int nIndex);

        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint GetDpiForWindow(IntPtr hwnd);

        public const int VK_LSHIFT = 0xA0;
        public const int VK_RMENU = 0xA5;
        public const int VK_LMENU = 0xA4;
        public const int VK_LCONTROL = 0xA2;
        public const int VK_RCONTROL = 0xA3;
        public const int VK_LBUTTON = 0x01;
        public const int VK_RBUTTON = 0x02;
        public const int VK_MBUTTON = 0x04;
        public const int VK_XBUTTON1 = 0x05;
        public const int VK_XBUTTON2 = 0x06;
        public const int VK_ESCAPE = 0x1B;

        public const long STGM_READ = 0x00000000L;
        public const long GENERIC_READ = (0x80000000L);
        public const long GENERIC_WRITE = (0x40000000L);

        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern HRESULT SHCreateStreamOnFile(string pszFile, int grfMode, out System.Runtime.InteropServices.ComTypes.IStream ppstm);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeleteFile(string lpFileName);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdiplusStartup(out IntPtr token, ref StartupInput input, out StartupOutput output);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern void GdiplusShutdown(IntPtr token);

        [StructLayout(LayoutKind.Sequential)]
        public struct StartupInput
        {
            public int GdiplusVersion;             // Must be 1
            // public DebugEventProc DebugEventCallback; // Ignored on free builds
            public IntPtr DebugEventCallback;
            public bool SuppressBackgroundThread;     // FALSE unless you're prepared to call 
                                                      // the hook/unhook functions properly
            public bool SuppressExternalCodecs;       // FALSE unless you want GDI+ only to use
                                                      // its internal image codecs.
            public static StartupInput GetDefault()
            {
                StartupInput result = new StartupInput();
                result.GdiplusVersion = 1;
                // result.DebugEventCallback = null;
                result.SuppressBackgroundThread = false;
                result.SuppressExternalCodecs = false;
                return result;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct StartupOutput
        {
            public IntPtr hook;//not used
            public IntPtr unhook;//not used.
        }

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipCreateFromHDC(IntPtr hdc, out IntPtr graphics);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipDeleteGraphics(IntPtr graphics);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipDrawImageRect(IntPtr graphics, IntPtr image, float x, float y, float width, float height);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipDrawImagePointRect(IntPtr graphics, IntPtr image, float x, float y,
            float srcx, float srcy, float srcwidth, float srcheight, GpUnit srcUnit);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipCreateBitmapFromFile(string filename, out IntPtr bitmap);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipCreateBitmapFromStream(System.Runtime.InteropServices.ComTypes.IStream Stream, out IntPtr bitmap);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipCreateHBITMAPFromBitmap(IntPtr nativeBitmap, out IntPtr hbitmap, int argbBackground);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipCreateBitmapFromScan0(int width, int height, int stride, PixelFormat format, IntPtr scan0, out IntPtr bitmap);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipDisposeImage(IntPtr image);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipSaveImageToFile(IntPtr image, string filename, ref Guid clsidEncoder, IntPtr /*EncoderParameters**/ encoderParams);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipGetImageDimension(IntPtr image, ref float width, ref float height);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipGetImagePixelFormat(IntPtr image, out PixelFormat format);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipBitmapLockBits(IntPtr bitmap, ref GpRect rect, uint flags, PixelFormat format, [In, Out] BitmapData lockedBitmapData);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipBitmapUnlockBits(IntPtr bitmap, BitmapData lockedBitmapData);

        public enum GpStatus : int
        {
            Ok = 0,
            GenericError = 1,
            InvalidParameter = 2,
            OutOfMemory = 3,
            ObjectBusy = 4,
            InsufficientBuffer = 5,
            NotImplemented = 6,
            Win32Error = 7,
            WrongState = 8,
            Aborted = 9,
            FileNotFound = 10,
            ValueOverflow = 11,
            AccessDenied = 12,
            UnknownImageFormat = 13,
            FontFamilyNotFound = 14,
            FontStyleNotFound = 15,
            NotTrueTypeFont = 16,
            UnsupportedGdiplusVersion = 17,
            GdiplusNotInitialized = 18,
            PropertyNotFound = 19,
            PropertyNotSupported = 20,
            ProfileNotFound = 21,
        }

        public enum GpUnit
        {
            UnitWorld,      // 0 -- World coordinate (non-physical unit)
            UnitDisplay,    // 1 -- Variable -- for PageTransform only
            UnitPixel,      // 2 -- Each unit is one device pixel.
            UnitPoint,      // 3 -- Each unit is a printer's point, or 1/72 inch.
            UnitInch,       // 4 -- Each unit is 1 inch.
            UnitDocument,   // 5 -- Each unit is 1/300 inch.
            UnitMillimeter  // 6 -- Each unit is 1 millimeter.
        };

        public enum PixelFormat : int
        {
            PixelFormatIndexed = 0x00010000, // Indexes into a palette
            PixelFormatGDI = 0x00020000, // Is a GDI-supported format
            PixelFormatAlpha = 0x00040000, // Has an alpha component
            PixelFormatPAlpha = 0x00080000, // Pre-multiplied alpha
            PixelFormatExtended = 0x00100000, // Extended color 16 bits/channel
            PixelFormatCanonical = 0x00200000,
            PixelFormatUndefined = 0,
            PixelFormatDontCare = 0,
            PixelFormat1bppIndexed = (1 | (1 << 8) | PixelFormatIndexed | PixelFormatGDI),
            PixelFormat4bppIndexed = (2 | (4 << 8) | PixelFormatIndexed | PixelFormatGDI),
            PixelFormat8bppIndexed = (3 | (8 << 8) | PixelFormatIndexed | PixelFormatGDI),
            PixelFormat16bppGrayScale = (4 | (16 << 8) | PixelFormatExtended),
            PixelFormat16bppRGB555 = (5 | (16 << 8) | PixelFormatGDI),
            PixelFormat16bppRGB565 = (6 | (16 << 8) | PixelFormatGDI),
            PixelFormat16bppARGB1555 = (7 | (16 << 8) | PixelFormatAlpha | PixelFormatGDI),
            PixelFormat24bppRGB = (8 | (24 << 8) | PixelFormatGDI),
            PixelFormat32bppRGB = (9 | (32 << 8) | PixelFormatGDI),
            PixelFormat32bppARGB = (10 | (32 << 8) | PixelFormatAlpha | PixelFormatGDI | PixelFormatCanonical),
            PixelFormat32bppPARGB = (11 | (32 << 8) | PixelFormatAlpha | PixelFormatPAlpha | PixelFormatGDI),
            PixelFormat48bppRGB = (12 | (48 << 8) | PixelFormatExtended),
            PixelFormat64bppARGB = (13 | (64 << 8) | PixelFormatAlpha | PixelFormatCanonical | PixelFormatExtended),
            PixelFormat64bppPARGB = (14 | (64 << 8) | PixelFormatAlpha | PixelFormatPAlpha | PixelFormatExtended),
            PixelFormat32bppCMYK = (15 | (32 << 8)),
            PixelFormatMax = 16
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public sealed class BitmapData
        {
            public uint Width;
            public uint Height;
            public int Stride;
            public int PixelFormat;
            public IntPtr Scan0;
            public int Reserved;
        };


        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct GpRect
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;

            public GpRect()
            {
                X = Y = Width = Height = 0;
            }

            public GpRect(int x, int y, int width, int height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }
        };

        [DllImport("Urlmon.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern HRESULT URLDownloadToCacheFile(IntPtr lpUnkcaller, string szURL, StringBuilder szFileName, uint cchFileName, uint dwReserved, IntPtr pBSC);

        public const int WH_MIN = (-1);
        public const int WH_MSGFILTER = (-1);
        public const int WH_JOURNALRECORD = 0;
        public const int WH_JOURNALPLAYBACK = 1;
        public const int WH_KEYBOARD = 2;
        public const int WH_GETMESSAGE = 3;
        public const int WH_CALLWNDPROC = 4;
        public const int WH_CBT = 5;
        public const int WH_SYSMSGFILTER = 6;
        public const int WH_MOUSE = 7;
        public const int WH_HARDWARE = 8;
        public const int WH_DEBUG = 9;
        public const int WH_SHELL = 10;
        public const int WH_FOREGROUNDIDLE = 11;
        public const int WH_CALLWNDPROCRET = 12;
        public const int WH_KEYBOARD_LL = 13;
        public const int WH_MOUSE_LL = 14;
        public const int WH_MAX = 14;
        public const int WH_MINHOOK = WH_MIN;
        public const int WH_MAXHOOK = WH_MAX;

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MSLLHOOKSTRUCT
        {
            public System.Drawing.Point pt;
            public int mouseData;
            public int flags;
            public int time;
            public uint dwExtraInfo;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MOUSEHOOKSTRUCT
        {
            public POINT pt;
            public IntPtr hwnd;
            public uint wHitTestCode;
            public uint dwExtraInfo;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public uint dwExtraInfo;
        }

        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint GetCurrentThreadId();

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int DrawText(IntPtr hdc, StringBuilder lpStr, int nCount, ref RECT lpRect, int wFormat);

        public const int
            DT_TOP = 0x00000000,
            DT_LEFT = 0x00000000,
            DT_CENTER = 0x00000001,
            DT_RIGHT = 0x00000002,
            DT_VCENTER = 0x00000004,
            DT_BOTTOM = 0x00000008,
            DT_WORDBREAK = 0x00000010,
            DT_SINGLELINE = 0x00000020,
            DT_EXPANDTABS = 0x00000040,
            DT_TABSTOP = 0x00000080,
            DT_NOCLIP = 0x00000100,
            DT_EXTERNALLEADING = 0x00000200,
            DT_CALCRECT = 0x00000400,
            DT_NOPREFIX = 0x00000800,
            DT_INTERNAL = 0x00001000,
            DT_EDITCONTROL = 0x00002000,
            DT_PATH_ELLIPSIS = 0x00004000,
            DT_END_ELLIPSIS = 0x00008000,
            DT_MODIFYSTRING = 0x00010000,
            DT_RTLREADING = 0x00020000,
            DT_WORD_ELLIPSIS = 0x00040000,
            DT_NOFULLWIDTHCHARBREAK = 0x00080000,
            DT_HIDEPREFIX = 0x00100000,
            DT_PREFIXONLY = 0x00200000;

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SetTextColor(IntPtr hdc, int color);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SetBkColor(IntPtr hdc, int color);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int SetBkMode(IntPtr hdc, int mode);

        public const int TRANSPARENT = 1;
        public const int OPAQUE = 2;
        public const int BKMODE_LAST = 2;

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFontIndirect([In, MarshalAs(UnmanagedType.LPStruct)] LOGFONT lplf);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LOGFONT
        {
            public int lfHeight = 0;
            public int lfWidth = 0;
            public int lfEscapement = 0;
            public int lfOrientation = 0;
            public int lfWeight = 0;
            public byte lfItalic = 0;
            public byte lfUnderline = 0;
            public byte lfStrikeOut = 0;
            public byte lfCharSet = 0;
            public byte lfOutPrecision = 0;
            public byte lfClipPrecision = 0;
            public byte lfQuality = 0;
            public byte lfPitchAndFamily = 0;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName = string.Empty;
        }

        /* Font Weights */
        public const int FW_DONTCARE = 0;
        public const int FW_THIN = 100;
        public const int FW_EXTRALIGHT = 200;
        public const int FW_LIGHT = 300;
        public const int FW_NORMAL = 400;
        public const int FW_MEDIUM = 500;
        public const int FW_SEMIBOLD = 600;
        public const int FW_BOLD = 700;
        public const int FW_EXTRABOLD = 800;
        public const int FW_HEAVY = 900;

        public const int DEFAULT_QUALITY = 0;
        public const int DRAFT_QUALITY = 1;
        public const int PROOF_QUALITY = 2;
        public const int NONANTIALIASED_QUALITY = 3;
        public const int ANTIALIASED_QUALITY = 4;
        public const int CLEARTYPE_QUALITY = 5;
        public const int CLEARTYPE_NATURAL_QUALITY = 6;

        public const int DEFAULT_PITCH = 0;
        public const int FIXED_PITCH = 1;
        public const int VARIABLE_PITCH = 2;
        public const int MONO_FONT = 8;

        public const int OUT_DEFAULT_PRECIS = 0;
        public const int OUT_STRING_PRECIS = 1;
        public const int OUT_CHARACTER_PRECIS = 2;
        public const int OUT_STROKE_PRECIS = 3;
        public const int OUT_TT_PRECIS = 4;
        public const int OUT_DEVICE_PRECIS = 5;
        public const int OUT_RASTER_PRECIS = 6;
        public const int OUT_TT_ONLY_PRECIS = 7;
        public const int OUT_OUTLINE_PRECIS = 8;
        public const int OUT_SCREEN_OUTLINE_PRECIS = 9;
        public const int OUT_PS_ONLY_PRECIS = 10;

        public const int CLIP_DEFAULT_PRECIS = 0;
        public const int CLIP_CHARACTER_PRECIS = 1;
        public const int CLIP_STROKE_PRECIS = 2;
        public const int CLIP_MASK = 0xf;
        public const int CLIP_LH_ANGLES = (1 << 4);
        public const int CLIP_TT_ALWAYS = (2 << 4);
        public const int CLIP_DFA_DISABLE = (4 << 4);
        public const int CLIP_EMBEDDED = (8 << 4);

        public const int ANSI_CHARSET = 0;
        public const int DEFAULT_CHARSET = 1;
        public const int SYMBOL_CHARSET = 2;

        /* Font Families */
        public const int FF_DONTCARE = (0 << 4);  /* Don't care or don't know. */
        public const int FF_ROMAN = (1 << 4);  /* Variable stroke width, serifed. */
        /* Times Roman, Century Schoolbook, etc. */
        public const int FF_SWISS = (2 << 4);  /* Variable stroke width, sans-serifed. */
        /* Helvetica, Swiss, etc. */
        public const int FF_MODERN = (3 << 4);  /* Constant stroke width, serifed or sans-serifed. */
        /* Pica, Elite, Courier, etc. */
        public const int FF_SCRIPT = (4 << 4);  /* Cursive, etc. */
        public const int FF_DECORATIVE = (5 << 4);  /* Old English, etc. */

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int MulDiv(int nNumber, int nNumerator, int nDenominator);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        public const int HORZSIZE = 4; /* Horizontal size in millimeters */
        public const int VERTSIZE = 6; /* Vertical size in millimeters */
        public const int HORZRES = 8; /* Horizontal width in pixels */
        public const int VERTRES = 10; /* Vertical height in pixels */
        public const int PHYSICALWIDTH = 110; /* Physical Width in device units */
        public const int PHYSICALHEIGHT = 111; /* Physical Height in device units */
        public const int PHYSICALOFFSETX = 112; /* Physical Printable Area x margin */
        public const int PHYSICALOFFSETY = 113; /* Physical Printable Area y margin */
        public const int LOGPIXELSX = 88;   /* Logical pixels/inch in X */
        public const int LOGPIXELSY = 90;   /* Logical pixels/inch in Y */

        public enum AC : byte
        {
            SRC_OVER = 0,
            SRC_ALPHA = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BLENDFUNCTION
        {
            public AC BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public AC AlphaFormat;
        }

        [DllImport("MSImg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool AlphaBlend(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int hHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, BLENDFUNCTION blendFunction);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, uint usage, ref IntPtr ppvBits, IntPtr hSection, int offset);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPV5HEADER pbmi, uint usage, ref IntPtr ppvBits, IntPtr hSection, int offset);

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            [MarshalAs(UnmanagedType.I4)]
            public int biSize;
            [MarshalAs(UnmanagedType.I4)]
            public int biWidth;
            [MarshalAs(UnmanagedType.I4)]
            public int biHeight;
            [MarshalAs(UnmanagedType.I2)]
            public short biPlanes;
            [MarshalAs(UnmanagedType.I2)]
            public short biBitCount;
            [MarshalAs(UnmanagedType.I4)]
            public int biCompression;
            [MarshalAs(UnmanagedType.I4)]
            public int biSizeImage;
            [MarshalAs(UnmanagedType.I4)]
            public int biXPelsPerMeter;
            [MarshalAs(UnmanagedType.I4)]
            public int biYPelsPerMeter;
            [MarshalAs(UnmanagedType.I4)]
            public int biClrUsed;
            [MarshalAs(UnmanagedType.I4)]
            public int biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            [MarshalAs(UnmanagedType.Struct, SizeConst = 40)]
            public BITMAPINFOHEADER bmiHeader;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public int[] bmiColors;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPV5HEADER
        {
            public int bV5Size;
            public int bV5Width;
            public int bV5Height;
            public short bV5Planes;
            public short bV5BitCount;
            public int bV5Compression;
            public int bV5SizeImage;
            public int bV5XPelsPerMeter;
            public int bV5YPelsPerMeter;
            public int bV5ClrUsed;
            public int bV5ClrImportant;
            public int bV5RedMask;
            public int bV5GreenMask;
            public int bV5BlueMask;
            public int bV5AlphaMask;
            public int bV5CSType;
            public CIEXYZTRIPLE bV5Endpoints;
            public int bV5GammaRed;
            public int bV5GammaGreen;
            public int bV5GammaBlue;
            public int bV5Intent;
            public int bV5ProfileData;
            public int bV5ProfileSize;
            public int bV5Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CIEXYZTRIPLE
        {
            public CIEXYZ ciexyzRed;
            public CIEXYZ ciexyzGreen;
            public CIEXYZ ciexyzBlue;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CIEXYZ
        {
            public int ciexyzX;
            public int ciexyzY;
            public int ciexyzZ;
        }

        public const int BI_RGB = 0;
        public const int BI_RLE8 = 1;
        public const int BI_RLE4 = 2;
        public const int BI_BITFIELDS = 3;
        public const int BI_JPEG = 4;
        public const int BI_PNG = 5;

        public const int DIB_RGB_COLORS = 0;
        public const int DIB_PAL_COLORS = 1;

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int StretchDIBits(IntPtr hdc, int XDest, int YDest, int nDestWidth, int nDestHeight, int XSrc, int YSrc, int nSrcWidth, int nSrcHeight, IntPtr lpBits, ref BITMAPINFO lpBitsInfo, int iUsage, int dwRop);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int StretchDIBits(IntPtr hdc, int XDest, int YDest, int nDestWidth, int nDestHeight, int XSrc, int YSrc, int nSrcWidth, int nSrcHeight, byte[] lpBits, ref BITMAPINFO lpBitsInfo, int iUsage, int dwRop);

        public const int SRCCOPY = 0x00CC0020; /* dest = source                   */
        public const int SRCPAINT = 0x00EE0086; /* dest = source OR dest           */
        public const int SRCAND = 0x008800C6; /* dest = source AND dest          */
        public const int SRCINVERT = 0x00660046; /* dest = source XOR dest          */
        public const int SRCERASE = 0x00440328; /* dest = source AND (NOT dest )   */
        public const int NOTSRCCOPY = 0x00330008; /* dest = (NOT source)             */
        public const int NOTSRCERASE = 0x001100A6; /* dest = (NOT src) AND (NOT dest) */
        public const int MERGECOPY = 0x00C000CA; /* dest = (source AND pattern)     */
        public const int MERGEPAINT = 0x00BB0226; /* dest = (NOT source) OR dest     */
        public const int PATCOPY = 0x00F00021; /* dest = pattern                  */
        public const int PATPAINT = 0x00FB0A09; /* dest = DPSnoo                   */
        public const int PATINVERT = 0x005A0049; /* dest = pattern XOR dest         */
        public const int DSTINVERT = 0x00550009; /* dest = (NOT dest)               */
        public const int BLACKNESS = 0x00000042; /* dest = BLACK                    */
        public const int WHITENESS = 0x00FF0062; /* dest = WHITE                    */
        public const int NOMIRRORBITMAP = unchecked((int)0x80000000); /* Do not Mirror the bitmap in this call */
        public const int CAPTUREBLT = 0x40000000; /* Include layered windows */

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetDIBits(IntPtr hdc, IntPtr hbm, uint start, uint cLines, byte[] lpvBits, ref BITMAPINFO lpbmi, uint usage);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetDIBits(IntPtr hdc, IntPtr hbm, uint start, uint cLines, byte[] lpvBits, ref BITMAPV5HEADER lpbmi, uint usage);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SetDIBitsToDevice(IntPtr hdc, int xDest, int yDest, uint w, uint h, int xSrc, int ySrc,
            uint StartScan, uint cLines, IntPtr lpvBits, ref BITMAPINFO lpbmi, uint ColorUse);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SetDIBitsToDevice(IntPtr hdc, int xDest, int yDest, uint w, uint h, int xSrc, int ySrc,
          uint StartScan, uint cLines, IntPtr lpvBits, ref BITMAPV5HEADER lpbmi, uint ColorUse);       

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool IntersectRect(out RECT lprcDst, ref RECT lprcSrc1, ref RECT lprcSrc2);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PtInRect(ref RECT lprc, POINT pt);

        [DllImport("Kernel32.dll", SetLastError = true, EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);




        private IMFMediaEngine m_pMediaEngine = null;
        private IMFMediaEngineEx m_pMediaEngineEx = null;
        private IntPtr m_pD3D11DevicePtr = IntPtr.Zero;
        private IntPtr m_pD3D11DeviceContextPtr = IntPtr.Zero;
        private IMFDXGIDeviceManager m_pDXGIDeviceManager = null;

        private IntPtr m_hWnd = IntPtr.Zero;
        private FrameworkElement m_VideoContainer = null;
        private Slider m_SliderTime = null;
        private TextBlock m_TextBlockElapsedTime = null;

        private IDXGISwapChain1 m_pDXGISwapChain1 = null;

        private MFARGB m_BorderColor;
        private bool m_bPlaying = false;
        private bool m_bPause = false;
        private bool m_bInitialized = false;
        private IMFMediaEngineClassFactory m_pMediaEngineClassFactory = null;
        private IntPtr m_pUnknownMediaEngineNotify = IntPtr.Zero;
        private IMFAttributes m_pAttributes = null;
        private uint m_nFlags = 0;
        private IntPtr m_initToken = IntPtr.Zero;
        private IntPtr m_hBitmapLogo = IntPtr.Zero;
        private IntPtr m_hBitmapOverlay = IntPtr.Zero;
        private IntPtr m_hBitmapNotes = IntPtr.Zero;
        private bool m_bOverLay = false;
        private bool m_bEffects = false;
        private EFFECT m_nEffects = EFFECT.NONE;
        private bool m_bCapture = false;
        private IntPtr m_hGDIPlusBitmapCapture = IntPtr.Zero;
        private IntPtr m_hBitmapCapture = IntPtr.Zero;
        private WriteableBitmap m_hWriteableBitmapCapture = null;

        private IMFTimedText m_pTimedText = null;
        private bool m_bSubtitles = false;
        private string m_sSubtitlesURL = string.Empty;
        private string m_sSubtitleText = string.Empty;

        private bool m_bFullScreen = false;
        private static int m_hHook = 0;
        private HookProc MouseProcedure;

        private double m_nControlsHeight = 0;

        public enum ME_MODE
        {
            MODE_FRAME_SERVER = 0,
            MODE_RENDERING = 2,
            MODE_AUDIO = 3
        }

        public enum DT_VERTICAL
        {
            TOP = 0,
            CENTER = 1,
            BOTTOM = 2
        }

        public enum EFFECT
        {
            NONE = 0,
            GRAYSCALE = 0x0001,
            INVERT = 0x0002,
            RGB = 0x0004,
            LIGHTEN = 0x0008,
        }
        public byte EFFECT_RGB_INTENSITY_RED = 0;
        public byte EFFECT_RGB_INTENSITY_GREEN = 0;
        public byte EFFECT_RGB_INTENSITY_BLUE = 0;
        public double EFFECT_LIGHTEN_INTENSITY = 0.5;

        public CMediaEngine()
        {
            HRESULT hr = HRESULT.S_OK;
            hr = MFStartup(MF_VERSION);

            StartupInput input = StartupInput.GetDefault();
            StartupOutput output;
            GpStatus nStatus = GdiplusStartup(out m_initToken, ref input, out output);
        }

        HRESULT IMFMediaEngineNotify.EventNotify(MF_MEDIA_ENGINE_EVENT nEvent, IntPtr param1, uint param2)
        {
            HRESULT hr = HRESULT.S_OK;

            switch (nEvent)
            {
                case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_PLAY:
                case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_PLAYING:
                    m_bPlaying = true;
                    m_bPause = false;
                    break;
                case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_PAUSE:
                    m_bPause = true;
                    break;
                case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_ENDED:
                    m_bPlaying = false;
                    m_bPause = false;
                    Notify(nEvent);
                    break;
                //case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_FIRSTFRAMEREADY:
                case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_LOADEDMETADATA:
                    //case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_DURATIONCHANGE:
                    //case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_LOADEDDATA:
                    //case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_CANPLAY:
                    //case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_CANPLAYTHROUGH:
                    //case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_TIMEUPDATE:
                    // Duration = m_pMediaEngine.GetDuration();
                    Notify(nEvent);
                    break;
                //case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_WAITING:
                //    m_bPlaying = false;
                //    m_bPause = false;
                //    break;
                case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_TIMEUPDATE:
                    Notify(nEvent);
                    break;
                case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_SEEKING:
                case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_SEEKED:
                    m_bPlaying = true;
                    break;              
                case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_ERROR:
                    m_bPlaying = false;
                    m_bPause = false;
                    string sError = string.Format("EVENT_ERROR ({0}, 0x{1:X})", param1, param2);
                    DisplayTimedText(sError, true, 10000);
                    break;
            }

            // MF_MEDIA_ENGINE_EVENT_ERROR (4, 0x80070005) : HRESULT: 0x80070005 (E_ACCESSDENIED)
           // System.Diagnostics.Debug.WriteLine("Event : {0} ({1}, 0x{2:X})", nEvent, param1, param2);
            return hr;
        }

        Grid gridContainer = null;
        CImageButton buttonPlayPause = null;
        CImageButton buttonStop = null;
        CImageButton buttonRepeat = null;
        CImageButton buttonFullscreen = null;
        CImageButton buttonSound = null;
        Slider sliderSound = null;

        public HRESULT Initialize(IntPtr hWnd, ME_MODE mode, Microsoft.UI.Xaml.FrameworkElement container, Windows.UI.Color borderColor)
        {
            HRESULT hr = HRESULT.S_OK;

            if (!m_bInitialized)
            {
                m_hWnd = hWnd;
                m_VideoContainer = container;
                m_BorderColor.rgbAlpha = borderColor.A;
                m_BorderColor.rgbRed = borderColor.R;
                m_BorderColor.rgbGreen = borderColor.G;
                m_BorderColor.rgbBlue = borderColor.B;

                m_pMediaEngineClassFactory = (IMFMediaEngineClassFactory)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_MFMediaEngineClassFactory));
                if (m_pMediaEngineClassFactory != null)
                {
                    if (hr == HRESULT.S_OK)
                    {
                        hr = MFCreateAttributes(out m_pAttributes, 1);
                        if (hr == HRESULT.S_OK)
                        {
                            m_pUnknownMediaEngineNotify = Marshal.GetComInterfaceForObject(this, typeof(IMFMediaEngineNotify));
                            hr = m_pAttributes.SetUnknown(MF_MEDIA_ENGINE_CALLBACK, m_pUnknownMediaEngineNotify);

                            if (mode == ME_MODE.MODE_FRAME_SERVER || mode == ME_MODE.MODE_RENDERING)
                            {
                                hr = m_pAttributes.SetUINT32(MF_MEDIA_ENGINE_VIDEO_OUTPUT_FORMAT, (uint)DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM);
                            }
                            if (mode == ME_MODE.MODE_RENDERING)
                            {
                                //IntPtr pUnknownCompositionVisual = Marshal.GetComInterfaceForObject(pDCompositionVisual, typeof(IDCompositionVisual));
                                //hr = pMFAttributes.SetUnknown(MF_MEDIA_ENGINE_PLAYBACK_VISUAL, pUnknownCompositionVisual);

                                hr = m_pAttributes.SetUINT64(MF_MEDIA_ENGINE_PLAYBACK_HWND, (ulong)hWnd);
                            }
                            if (mode == ME_MODE.MODE_FRAME_SERVER || mode == ME_MODE.MODE_AUDIO)
                            {
                                hr = CreateD3D11Device();
                                uint resetToken;
                                hr = MFCreateDXGIDeviceManager(out resetToken, out m_pDXGIDeviceManager);
                                if (hr == HRESULT.S_OK)
                                {
                                    hr = m_pDXGIDeviceManager.ResetDevice(m_pD3D11DevicePtr, resetToken);

                                    if (mode == ME_MODE.MODE_FRAME_SERVER)
                                    {
                                        IntPtr pUnknownDeviceManager = Marshal.GetComInterfaceForObject(m_pDXGIDeviceManager, typeof(IMFDXGIDeviceManager));
                                        hr = m_pAttributes.SetUnknown(MF_MEDIA_ENGINE_DXGI_MANAGER, pUnknownDeviceManager);
                                    }

                                    hr = CreateSwapChain(hWnd);
                                }
                            }

                            //uint nFlags = (uint)MF_MEDIA_ENGINE_CREATEFLAGS.MF_MEDIA_ENGINE_REAL_TIME_MODE | (uint)MF_MEDIA_ENGINE_CREATEFLAGS.MF_MEDIA_ENGINE_WAITFORSTABLE_STATE;
                            m_nFlags = (uint)MF_MEDIA_ENGINE_CREATEFLAGS.MF_MEDIA_ENGINE_REAL_TIME_MODE;
                            //uint nFlags = (uint)MF_MEDIA_ENGINE_CREATEFLAGS.MF_MEDIA_ENGINE_DISABLE_LOCAL_PLUGINS;
                            hr = m_pMediaEngineClassFactory.CreateInstance(m_nFlags, m_pAttributes, out m_pMediaEngine);
                            if (hr == HRESULT.S_OK)
                            {
                                hr = m_pMediaEngine.SetAutoPlay(true);
                                m_pMediaEngineEx = (IMFMediaEngineEx)m_pMediaEngine;
                                RegisterSubtitles();
                            }
                        }
                    }

                    string sLogoURL = "https://docs.microsoft.com/en-us/windows/apps/images/logo-winui.png";
                    StringBuilder sbFileName = new StringBuilder(260);
                    hr = URLDownloadToCacheFile(IntPtr.Zero, sLogoURL, sbFileName, (uint)sbFileName.Capacity, 0x0, IntPtr.Zero);
                    if (hr == HRESULT.S_OK)
                    {
                        IntPtr pBitmap = IntPtr.Zero;
                        System.Runtime.InteropServices.ComTypes.IStream pstm;
                        hr = SHCreateStreamOnFile(sbFileName.ToString(), (int)STGM_READ, out pstm);
                        if (hr == HRESULT.S_OK)
                        {
                            GpStatus nStatus = GdipCreateBitmapFromStream(pstm, out pBitmap);
                            if (nStatus == GpStatus.Ok)
                            {
                                GdipCreateHBITMAPFromBitmap(pBitmap, out m_hBitmapLogo, RGB(Microsoft.UI.Colors.Black.R, Microsoft.UI.Colors.Black.G, Microsoft.UI.Colors.Black.B));
                                Marshal.Release(pBitmap);
                            }
                            SafeRelease(ref pstm);
                        }
                    }

                    string sNotesURL = "https://i.ibb.co/KwKgKWN/Note-Blue.png";
                    hr = URLDownloadToCacheFile(IntPtr.Zero, sNotesURL, sbFileName, (uint)sbFileName.Capacity, 0x0, IntPtr.Zero);
                    if (hr == HRESULT.S_OK)
                    {
                        IntPtr pBitmap = IntPtr.Zero;
                        System.Runtime.InteropServices.ComTypes.IStream pstm;
                        hr = SHCreateStreamOnFile(sbFileName.ToString(), (int)STGM_READ, out pstm);
                        if (hr == HRESULT.S_OK)
                        {
                            GpStatus nStatus = GdipCreateBitmapFromStream(pstm, out pBitmap);
                            if (nStatus == GpStatus.Ok)
                            {
                                GdipCreateHBITMAPFromBitmap(pBitmap, out m_hBitmapNotes, RGB(Microsoft.UI.Colors.White.R, Microsoft.UI.Colors.White.G, Microsoft.UI.Colors.White.B));
                                Marshal.Release(pBitmap);
                            }
                            SafeRelease(ref pstm);
                        }
                    }

                    m_SliderTime = new Slider()
                    {
                        Margin = new Thickness(0, 0, 6, 0),
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        TickFrequency = 0.1,
                        TickPlacement = Microsoft.UI.Xaml.Controls.Primitives.TickPlacement.Inline,
                        StepFrequency = 0.01,
                        SnapsTo = Microsoft.UI.Xaml.Controls.Primitives.SliderSnapsTo.StepValues,
                        Minimum = 0,
                        Maximum = 0
                    };

                    m_TextBlockElapsedTime = new TextBlock()
                    {
                        Margin = new Thickness(0, 0, 6, -2),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        FontSize = 12,
                        Foreground = new SolidColorBrush(Colors.LightBlue)
                    };
                    gridContainer = new Grid();
                    RowDefinition Row = new RowDefinition();
                    Row.Height = new GridLength(1, GridUnitType.Star);
                    gridContainer.RowDefinitions.Add(Row);
                    RowDefinition Row2 = new RowDefinition();
                    Row2.Height = new GridLength(0, GridUnitType.Auto);
                    gridContainer.RowDefinitions.Add(Row2);

                    //var stb_Brush = Application.Current.Resources["SliderThumbBackground"] as SolidColorBrush;
                    Application.Current.Resources["SliderThumbBackground"] = new SolidColorBrush(Colors.DeepSkyBlue);
                    Application.Current.Resources["SliderThumbBackgroundPointerOver"] = new SolidColorBrush(Colors.DeepSkyBlue);
                    Application.Current.Resources["SliderThumbBackgroundPressed"] = new SolidColorBrush(Colors.DeepSkyBlue);

                    // LightGray
                    Application.Current.Resources["SliderTrackFill"] = new SolidColorBrush(Colors.LightBlue);
                    Application.Current.Resources["SliderTrackFillPointerOver"] = new SolidColorBrush(Colors.LightBlue);
                    Application.Current.Resources["SliderTrackFillPressed"] = new SolidColorBrush(Colors.LightBlue);

                    // Elapsed
                    Application.Current.Resources["SliderTrackValueFill"] = new SolidColorBrush(Colors.DodgerBlue);
                    Application.Current.Resources["SliderTrackValueFillPointerOver"] = new SolidColorBrush(Colors.DodgerBlue);
                    Application.Current.Resources["SliderTrackValueFillPressed"] = new SolidColorBrush(Colors.RoyalBlue);

                    //var appBar = new CommandBar()
                    //{
                    //    //Width = 200,
                    //    Height = 36,
                    //    Margin = new Thickness(0, -10, 0, 0),
                    //    HorizontalAlignment = HorizontalAlignment.Center,
                    //    Background = new SolidColorBrush(Colors.RoyalBlue)
                    //};                   

                    //var btnNew = new AppBarButton()
                    //{
                    //    //Content = "Test",
                    //    Width = 60,
                    //    Height = 30,
                    //    HorizontalAlignment = HorizontalAlignment.Center,
                    //    Icon = new SymbolIcon(Symbol.Back)
                    //};                  

                    //appBar.PrimaryCommands.Add(btnNew);
                    //gridContainer.Children.Add(appBar);
                    //appBar.SetValue(Grid.RowProperty, 1);

                    var sp1 = new StackPanel()
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Orientation = Orientation.Horizontal
                    };

                    buttonPlayPause = new CImageButton()
                    {
                        Width = 40,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        //Source = new BitmapImage(new Uri("ms-appx:///Assets/Button_Play_Blue.png"))
                    };
                    buttonPlayPause.SetSource("ms-appx:///Assets/Button_Play_Blue.png", buttonPlayPause.Width, buttonPlayPause.Height);

                    //buttonRewind = new CImageButton()
                    //{
                    //    Width = 40,
                    //    Height = 40,
                    //    HorizontalAlignment = HorizontalAlignment.Center,
                    //    Source = new BitmapImage(new Uri("ms-appx:///Assets/Button_Rewind_Blue2.png"))
                    //};
                    //buttonRewind.SetFunctionLongPress(Rewind);

                    buttonStop = new CImageButton()
                    {
                        Width = 40,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Source = new BitmapImage(new Uri("ms-appx:///Assets/Button_Stop_Blue.png"))
                    };

                    //buttonForward = new CImageButton()
                    //{
                    //    Width = 40,
                    //    Height = 40,
                    //    HorizontalAlignment = HorizontalAlignment.Center,
                    //    Source = new BitmapImage(new Uri("ms-appx:///Assets/Button_Forward_Blue2.png"))
                    //};

                    buttonRepeat = new CImageButton()
                    {
                        Width = 40,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };
                    buttonRepeat.SetSource("ms-appx:///Assets/Button_Repeat_Blue.png", buttonRepeat.Width, buttonRepeat.Height);

                    buttonFullscreen = new CImageButton()
                    {
                        Width = 40,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Source = new BitmapImage(new Uri("ms-appx:///Assets/Button_Fullscreen_Blue.png"))
                    };

                    buttonSound = new CImageButton()
                    {
                        Width = 40,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };
                    buttonSound.SetSource("ms-appx:///Assets/Button_Sound_Blue.png", buttonSound.Width, buttonSound.Height);

                    sliderSound = new Slider()
                    {
                        Margin = new Thickness(6, 0, 0, 0),
                        Width = 100,
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TickFrequency = 0.1,
                        TickPlacement = Microsoft.UI.Xaml.Controls.Primitives.TickPlacement.None,
                        StepFrequency = 0.1,
                        SnapsTo = Microsoft.UI.Xaml.Controls.Primitives.SliderSnapsTo.StepValues,
                        Minimum = 0,
                        Maximum = 1
                    };
                    double nVolume = m_pMediaEngine.GetVolume();
                    sliderSound.Value = nVolume;

                    sp1.Children.Add(buttonPlayPause);
                    sp1.Children.Add(buttonStop);
                    sp1.Children.Add(buttonRepeat);
                    sp1.Children.Add(buttonFullscreen);
                    sp1.Children.Add(buttonSound);
                    sp1.Children.Add(sliderSound);

                    buttonPlayPause.CImageButtonClicked += buttonPlayPause_CImageButtonClicked;
                    //buttonRewind.CImageButtonClicked += buttonRewind_CImageButtonClicked;
                    //buttonForward.CImageButtonClicked += buttonForward_CImageButtonClicked;
                    buttonStop.CImageButtonClicked += buttonStop_CImageButtonClicked;
                    buttonRepeat.CImageButtonClicked += buttonRepeat_CImageButtonClicked;
                    buttonFullscreen.CImageButtonClicked += buttonFullscreen_CImageButtonClicked;
                    buttonSound.CImageButtonClicked += buttonSound_CImageButtonClicked;
                    sliderSound.ValueChanged += SliderSound_ValueChanged;

                    gridContainer.Children.Add(sp1);
                    sp1.SetValue(Grid.RowProperty, 1);

                    gridContainer.Children.Add(m_SliderTime);
                    gridContainer.Children.Add(m_TextBlockElapsedTime);

                    m_SliderTime.SetValue(Grid.RowProperty, 0);
                    m_TextBlockElapsedTime.SetValue(Grid.RowProperty, 0);

                    ((UserControl)m_VideoContainer).Content = gridContainer;

                    TimeSpan ts1 = TimeSpan.FromSeconds(0);
                    string sCurrentTimeText = ts1.ToString("hh\\:mm\\:ss");
                    string sTotalTimeText = ts1.ToString("hh\\:mm\\:ss");
                    m_TextBlockElapsedTime.Text = sCurrentTimeText + "/" + sTotalTimeText;

                    m_SliderTime.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(Slider_ChannelPressed), true);
                    m_SliderTime.AddHandler(UIElement.PointerMovedEvent, new PointerEventHandler(Slider_ChannelMoved), true);
                    m_SliderTime.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(Slider_ChannelReleased), true);

                    sp1.Loaded += (sender, e) =>
                    {
                        double nHeight = ((StackPanel)sender).ActualHeight;
                        m_nControlsHeight += nHeight;
                        m_nControlsHeightOld = m_nControlsHeight;
                    };

                    m_SliderTime.Loaded += (sender, e) =>
                    {
                        double nHeight = ((Slider)sender).ActualHeight;
                        m_nControlsHeight += nHeight;
                        m_nControlsHeightOld = m_nControlsHeight;
                    };

                    //sp1.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
                    //sp1.Arrange(new Windows.Foundation.Rect(0, 0, sp1.DesiredSize.Width, sp1.DesiredSize.Height));
                    //double nSPHeight = sp1.ActualHeight;

                    m_VideoContainer.SizeChanged += M_VideoContainer_SizeChanged;
                    Microsoft.UI.Xaml.Media.CompositionTarget.Rendering += CompositionTarget_Rendering;

                    m_bInitialized = true;
                }
            }
            else
            {
                hr = m_pMediaEngineClassFactory.CreateInstance(m_nFlags, m_pAttributes, out m_pMediaEngine);
                if (hr == HRESULT.S_OK)
                {
                    hr = m_pMediaEngine.SetAutoPlay(true);
                    m_pMediaEngineEx = (IMFMediaEngineEx)m_pMediaEngine;
                    RegisterSubtitles();
                }
            }
            return hr;
        }

        private void SliderSound_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            HRESULT hr = HRESULT.S_OK;
            hr = m_pMediaEngine.SetVolume(((Slider)sender).Value);
        }

        //private void Rewind()
        //{
        //    Console.Beep(1000, 10);
        //}

        private void buttonPlayPause_CImageButtonClicked(object sender, RoutedEventArgs e)
        {
            if (m_pMediaEngine.IsPaused() || m_pMediaEngine.IsEnded())
            {
                Play(true);
            }
            else
            {
                Pause(true);
            }
        }

        // IMFMediaEngineSrcElements does not seem very useful...

        //private CMediaEngineSrcElements pSrcElements = null;

        //public void NewSrcElements()
        //{
        //    HRESULT hr = HRESULT.S_OK;
        //    if (pSrcElements != null)
        //    {
        //        pSrcElements.Dispose();
        //        pSrcElements = null;
        //    }
        //    pSrcElements = new CMediaEngineSrcElements();
        //    hr = pSrcElements.AddElement("E:\\big_buck_bunny.asf", null, null);
        //    hr = pSrcElements.AddElement("E:\\sample_960x400_ocean_with_audio.mp4", null, null);
        //    uint nNbElements = pSrcElements.GetLength();

        //    if (m_pMediaEngine.GetAutoPlay())
        //        hr = m_pMediaEngine.SetAutoPlay(false);
        //    hr = m_pMediaEngine.SetSourceElements(pSrcElements);

        //    //IntPtr pMediaEnginePtr = Marshal.GetComInterfaceForObject(m_pMediaEngine, typeof(IMFMediaEngine));
        //    //// null
        //    //IMFMediaEngineSrcElements pSrcElementsCurrent = Marshal.GetObjectForIUnknown(pMediaEnginePtr) as IMFMediaEngineSrcElements;
        //}

        private void buttonStop_CImageButtonClicked(object sender, RoutedEventArgs e)
        {
            HRESULT hr = HRESULT.S_OK;
            hr = Stop(true);
        }

        private void buttonRepeat_CImageButtonClicked(object sender, RoutedEventArgs e)
        {
            HRESULT hr = HRESULT.S_OK;
            MF_MEDIA_ENGINE_READY nReadyState = m_pMediaEngine.GetReadyState();
            if (nReadyState >= MF_MEDIA_ENGINE_READY.MF_MEDIA_ENGINE_READY_HAVE_FUTURE_DATA)
            {
                bool bLoop = m_pMediaEngine.GetLoop();
                hr = m_pMediaEngine.SetLoop(!bLoop);
                if (bLoop)
                    buttonRepeat.SetSource("ms-appx:///Assets/Button_Repeat_Blue.png");
                else
                    buttonRepeat.SetSource("ms-appx:///Assets/Button_Repeat_Blue_On.png");
            }
        }

        private void buttonFullscreen_CImageButtonClicked(object sender, RoutedEventArgs e)
        {
            HRESULT hr = HRESULT.S_OK;
            hr = SetFullScreen(true);
        }

        private void TestEffects()
        {
            HRESULT hr = HRESULT.S_OK;
            IMFTransform pXVP = (IMFTransform)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_VideoProcessorMFT));

            //IMFMediaEngineEx pMediaEngineEx = (IMFMediaEngineEx)m_pMediaEngine;
            //pMediaEngineEx.EnableHorizontalMirrorMode(true);

            IMFVideoProcessorControl2 pXVPControl2 = (IMFVideoProcessorControl2)pXVP;
            hr = pXVPControl2.SetMirror(MF_VIDEO_PROCESSOR_MIRROR.MIRROR_VERTICAL);
            hr = m_pMediaEngineEx.RemoveAllEffects();
            IntPtr pMFTransform = Marshal.GetComInterfaceForObject(pXVP, typeof(IMFTransform));
            hr = m_pMediaEngineEx.InsertVideoEffect(pMFTransform, true);
            Marshal.Release(pMFTransform);
            //Event: MF_MEDIA_ENGINE_EVENT_ERROR(3, 0x80070057)
            SafeRelease(ref pXVP);
            //SafeRelease(ref pMediaEngineEx);
        }

        private void buttonSound_CImageButtonClicked(object sender, RoutedEventArgs e)
        {
            HRESULT hr = HRESULT.S_OK;
            bool bMuted = m_pMediaEngine.GetMuted();
            hr = m_pMediaEngine.SetMuted(!bMuted);
            if (bMuted)
            {
                buttonSound.SetSource("ms-appx:///Assets/Button_Sound_Blue.png");
                sliderSound.IsEnabled = true;
            }
            else
            {
                buttonSound.SetSource("ms-appx:///Assets/Button_NoSound_Blue.png");
                sliderSound.IsEnabled = false;
            }
        }

        private void buttonRewind_CImageButtonClicked(object sender, RoutedEventArgs e)
        {
            HRESULT hr = HRESULT.S_OK;
            MF_MEDIA_ENGINE_READY nReadyState = m_pMediaEngine.GetReadyState();
            if (nReadyState >= MF_MEDIA_ENGINE_READY.MF_MEDIA_ENGINE_READY_HAVE_FUTURE_DATA)
            {
                double nPlayBackRate = m_pMediaEngine.GetPlaybackRate();
                nPlayBackRate--;
                hr = m_pMediaEngine.SetPlaybackRate(nPlayBackRate);
            }
        }

        private void buttonForward_CImageButtonClicked(object sender, RoutedEventArgs e)
        {
            HRESULT hr = HRESULT.S_OK;
            MF_MEDIA_ENGINE_READY nReadyState = m_pMediaEngine.GetReadyState();
            if (nReadyState >= MF_MEDIA_ENGINE_READY.MF_MEDIA_ENGINE_READY_HAVE_FUTURE_DATA)
            {
                double nPlayBackRate = m_pMediaEngine.GetPlaybackRate();
                nPlayBackRate++;
                //hr = Pause(false);
                hr = m_pMediaEngine.SetPlaybackRate(nPlayBackRate);
                //hr = Play(false);
            }
        }

        private bool bClicked = false;
        private bool bWasPaused = false;

        private void Slider_ChannelReleased(object sender, PointerRoutedEventArgs e)
        {
            HRESULT hr = HRESULT.S_OK;
            var properties = e.GetCurrentPoint((UIElement)sender).Properties;
            //if (properties.IsLeftButtonPressed)
            {
                bClicked = false;
                //hr = SetCurrentTime(((Slider)sender).Value);
                //if (m_pMediaEngineEx.IsPaused())
                //    m_pMediaEngineEx.FrameStep(true);
                //if (!bWasPaused)
                //    hr = Play(false);
                hr = Play(true);
                hr = SetCurrentTime(((Slider)sender).Value);
            }
        }

        private void Slider_ChannelPressed(object sender, PointerRoutedEventArgs e)
        {
            HRESULT hr = HRESULT.S_OK;
            var properties = e.GetCurrentPoint((UIElement)sender).Properties;
            if (properties.IsLeftButtonPressed)
            {
                bClicked = true;
                bWasPaused = m_pMediaEngine.IsPaused();
                if (bWasPaused)
                    hr = Play(true);
                hr = SetCurrentTime(((Slider)sender).Value);
                //if (bWasPaused)
                //    hr = Pause(false);
            }
        }

        private void Slider_ChannelMoved(object sender, PointerRoutedEventArgs e)
        {
            HRESULT hr = HRESULT.S_OK;
            if (bClicked)
            {
                if (!m_pMediaEngine.IsPaused())
                {
                    hr = Pause(true);
                }
                double nTime = ((Slider)sender).Value;
                //hr = SetCurrentTime(nTime);
                TimeSpan ts1 = TimeSpan.FromSeconds(nTime);
                string sCurrentTimeText = ts1.ToString("hh\\:mm\\:ss");
                ts1 = TimeSpan.FromSeconds(m_SliderTime.Maximum);
                string sTotalTimeText = ts1.ToString("hh\\:mm\\:ss");
                m_TextBlockElapsedTime.Text = sCurrentTimeText + "/" + sTotalTimeText;
            }
        }

        // Tasks to avoid freezes on Get...
        private void Notify(MF_MEDIA_ENGINE_EVENT nEvent)
        {
            switch (nEvent)
            {
                case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_LOADEDMETADATA:
                    LaunchTaskGetDuration();
                    break;
                case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_TIMEUPDATE:
                    LaunchTaskGetTime();
                    break;
                case MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_ENDED:
                    LaunchTaskEnd();
                    break;
            }
        }

        private async void LaunchTaskTimer(int nMilliSeconds)
        {
            await TaskTimer(nMilliSeconds);
        }

        System.Threading.Tasks.Task TaskTimer(int nMilliSeconds)
        {
            bool isQueued = m_VideoContainer.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                StartTimer(nMilliSeconds);
            });
            return Task.CompletedTask;
        }

        private async void LaunchTaskEnd()
        {
            await TaskEnd();
        }

        System.Threading.Tasks.Task TaskEnd()
        {
            bool isQueued = m_VideoContainer.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                buttonPlayPause.SetSource("ms-appx:///Assets/Button_Play_Blue.png");
                if (!m_pMediaEngine.GetLoop())
                    Stop(true);
            });
            return Task.CompletedTask;
        }

        System.Threading.Tasks.Task TaskGetDuration()
        {
            bool isQueued = m_VideoContainer.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                double nDuration = m_pMediaEngine.GetDuration();
                if (nDuration != double.PositiveInfinity && nDuration != double.NegativeInfinity)
                    m_SliderTime.Maximum = nDuration;
            });
            return Task.CompletedTask;
        }

        private async void LaunchTaskGetDuration()
        {
            await TaskGetDuration();
        }

        System.Threading.Tasks.Task TaskGetTime()
        {
            bool isQueued = m_VideoContainer.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                //if (!bClicked)
                if (m_pMediaEngine != null)
                {
                    if (!m_pMediaEngine.IsSeeking())
                    {
                        double nTime = GetCurrentTime();
                        m_SliderTime.Value = nTime;
                        TimeSpan ts1 = TimeSpan.FromSeconds(nTime);
                        string sCurrentTimeText = ts1.ToString("hh\\:mm\\:ss");
                        ts1 = TimeSpan.FromSeconds(m_SliderTime.Maximum);
                        string sTotalTimeText = ts1.ToString("hh\\:mm\\:ss");
                        m_TextBlockElapsedTime.Text = sCurrentTimeText + "/" + sTotalTimeText;
                    }
                }
            });
            return Task.CompletedTask;
        }

        private async void LaunchTaskGetTime()
        {
            await TaskGetTime();
        }

        System.Threading.Tasks.Task TaskSetTime(double nTime)
        {
            bool isQueued = m_VideoContainer.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                //await System.Threading.Tasks.Task.Delay(300);
                m_pMediaEngineEx.SetCurrentTimeEx(nTime, MF_MEDIA_ENGINE_SEEK_MODE.MF_MEDIA_ENGINE_SEEK_MODE_NORMAL);
                //if (m_pMediaEngineEx.IsPaused())
                //    m_pMediaEngineEx.FrameStep(true);
            });
            return Task.CompletedTask;
        }

        private async void LaunchTaskSetTime(double nTime)
        {
            await TaskSetTime(nTime);
        }

        private void M_VideoContainer_SizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
        {
            Resize(e.NewSize);
        }

        // for x:Bind
        //public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        //private float _Duration = 10;
        //public double Duration
        //{
        //    get => _Duration;
        //    set
        //    {
        //        _Duration = (float)value;
        //         PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Duration)));
        //    }
        //}
        //public double GetDuration(double x) => (double)_Duration;
        //public double SetDuration(double x) => Duration = (double)x;

        //private float _Time = 0;
        //public double Time
        //{
        //    get => _Time;
        //    set
        //    {
        //        _Time = (float)value;
        //        //bool bLeftButton = (GetKeyState((int)VK_LBUTTON) & 0x8000) == 0x8000;
        //        //if (bLeftButton)
        //        //    SetCurrentTime(value);
        //        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Time)));
        //    }
        //}
        //public double GetTime(double x) => (double)_Time;
        //public double SetTime(double x) => Time = (double)x;    

        HRESULT Resize(Windows.Foundation.Size sz)
        {
            HRESULT hr = HRESULT.S_OK;

            if (m_pDXGISwapChain1 != null)
            {
                if (IsFullScreen())
                {
                    // 0x887a0001 DXGI_ERROR_INVALID_CALL
                    hr = m_pDXGISwapChain1.ResizeBuffers(
                        2,
                        0,
                        0,
                        DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM,
                        //0
                        (uint)DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_GDI_COMPATIBLE
                        );
                }
                else
                {
                    uint nDPI = GetDpiForWindow(m_hWnd);
                    uint nWidth = (uint)(sz.Width * nDPI / 96.0f);
                    uint nHeight = (uint)(sz.Height * nDPI / 96.0f);
                    hr = m_pDXGISwapChain1.ResizeBuffers(
                        2,
                        nWidth,
                        nHeight,
                        DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM,
                        //0
                        (uint)DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_GDI_COMPATIBLE
                        );

                    ////if (hr != HRESULT.S_OK)
                    ////{
                    ////    Console.Beep(500, 10);
                    ////}

                    // In case of Alt + Tab
                    if (m_bFullScreen)
                    {
                        Microsoft.UI.WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(m_hWnd);
                        Microsoft.UI.Windowing.AppWindow apw = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(myWndId);
                        Microsoft.UI.Windowing.OverlappedPresenter presenter = apw.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
                        presenter.SetBorderAndTitleBar(m_bFullScreen, m_bFullScreen);
                        presenter.IsResizable = m_bFullScreen;
                        m_bFullScreen = false;
                        if (m_hHook != 0)
                        {
                            UnhookWindowsHookEx(m_hHook);
                            m_hHook = 0;
                        }
                    }
                }
            }
            return hr;
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            HRESULT hr = HRESULT.S_OK;
            Render();
            if ((GetKeyState(VK_ESCAPE) & 0x8000) == 0x8000)
            {
                //Console.Beep(5000, 10);
                bool bFulllScreen = false;
                IDXGIOutput pDXGIOutput = null;
                hr = m_pDXGISwapChain1.GetFullscreenState(out bFulllScreen, out pDXGIOutput);
                if (hr == HRESULT.S_OK)
                {
                    if (bFulllScreen)
                        SetFullScreen(false);
                    SafeRelease(ref pDXGIOutput);
                }
            }
        }

        private byte Gray(byte nRed, byte nGreen, byte nBlue)
        {
            return (byte)((nRed * 299 + nGreen * 587 + nBlue * 114) / 1000);
        }

        private static int MakePixelGray(int pixel)
        {
            byte blue = (byte)pixel;
            byte green = (byte)(pixel >> 8);
            byte red = (byte)(pixel >> 16);
            byte alpha = (byte)(pixel >> 24);
            byte gray = (byte)(((red * 77) + (green * 150) + (blue * 29) + 128) / 256);
            return (int)(alpha << 24 | gray << 16 | gray << 8 | gray);
        }

        private const byte HLSMAX = 255;
        private const byte RGBMAX = 255;

        // Too slow...
        private Windows.UI.Color TintColor(Windows.UI.Color color, double nTint)
        {
            if (nTint == 0)
                return color;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte h = 0, l = 0, s = 0;
            RGBtoHLS(r, g, b, ref h, ref l, ref s);

            double lum = l;
            if (nTint < 0)
                lum = lum * (1.0 + nTint);
            else if (nTint > 0)
            {
                lum = lum * (1.0 - nTint) + (HLSMAX - HLSMAX * (1.0 - nTint));
                l = (byte)Math.Min(255, Math.Round(lum));
            }

            HLStoRGB(h, l, s, ref r, ref g, ref b);

            return Microsoft.UI.ColorHelper.FromArgb(0, r, g, b);
        }

        // From KB29240
        private void RGBtoHLS(byte r, byte g, byte b, ref byte h, ref byte l, ref byte s)
        {
            byte cMax, cMin;      /* max and min RGB values */
            short Rdelta, Gdelta, Bdelta; /* intermediate value: % of spread from max  */

            /* calculate lightness */
            cMax = Math.Max(Math.Max(r, g), b);
            cMin = Math.Min(Math.Min(r, g), b);
            l = (byte)((((cMax + cMin) * HLSMAX) + RGBMAX) / (2 * RGBMAX));

            if (cMax == cMin)
            {           /* r=g=b --> achromatic case */
                s = 0;                     /* saturation */
                h = 0;// UNDEFINED;             /* hue */
            }
            else
            {                        /* chromatic case */
                /* saturation */
                if (l <= (HLSMAX / 2))
                    s = (byte)((((cMax - cMin) * HLSMAX) + ((cMax + cMin) / 2)) / (cMax + cMin));
                else
                    s = (byte)((((cMax - cMin) * HLSMAX) + ((2 * RGBMAX - cMax - cMin) / 2))
                       / (2 * RGBMAX - cMax - cMin));

                /* hue */
                Rdelta = (short)((((cMax - r) * (HLSMAX / 6)) + ((cMax - cMin) / 2)) / (cMax - cMin));
                Gdelta = (short)((((cMax - g) * (HLSMAX / 6)) + ((cMax - cMin) / 2)) / (cMax - cMin));
                Bdelta = (short)((((cMax - b) * (HLSMAX / 6)) + ((cMax - cMin) / 2)) / (cMax - cMin));

                if (r == cMax)
                    h = (byte)(Bdelta - Gdelta);
                else if (g == cMax)
                    h = (byte)((HLSMAX / 3) + Rdelta - Bdelta);
                else /* B == cMax */
                    h = (byte)(((2 * HLSMAX) / 3) + Gdelta - Rdelta);

                if (h < 0)
                    h += HLSMAX;
                if (h > HLSMAX)
                    h -= HLSMAX;
            }
        }

        /* utility routine for HLStoRGB */
        private short HueToRGB(short n1, short n2, short hue)
        {
            /* range check: note values passed add/subtract thirds of range */
            if (hue < 0)
                hue += HLSMAX;
            if (hue > HLSMAX)
                hue -= HLSMAX;
            /* return r,g, or b value from this tridrant */
            if (hue < (HLSMAX / 6))
                return ((short)(n1 + (((n2 - n1) * hue + (HLSMAX / 12)) / (HLSMAX / 6))));
            if (hue < (HLSMAX / 2))
                return (n2);
            if (hue < ((HLSMAX * 2) / 3))
                return ((short)(n1 + (((n2 - n1) * (((HLSMAX * 2) / 3) - hue) + (HLSMAX / 12)) / (HLSMAX / 6)))
          );
            else
                return (n1);
        }

        private void HLStoRGB(short hue, short lum, short sat, ref byte r, ref byte g, ref byte b)
        {
            short Magic1, Magic2;       /* calculated magic numbers (really!) */
            if (sat == 0)
            {            /* achromatic case */
                r = g = b = (byte)((lum * RGBMAX) / HLSMAX);
                if (hue != 0 /*UNDEFINED*/)
                {
                    /* ERROR */
                }
            }
            else
            {            /* chromatic case */
                /* set up magic numbers */
                if (lum <= (HLSMAX / 2))
                    Magic2 = (short)((lum * (HLSMAX + sat) + (HLSMAX / 2)) / HLSMAX);
                else
                    Magic2 = (short)(lum + sat - ((lum * sat) + (HLSMAX / 2)) / HLSMAX);
                Magic1 = (short)(2 * lum - Magic2);

                /* get RGB, change units from HLSMAX to RGBMAX */
                r = (byte)((HueToRGB(Magic1, Magic2, (short)(hue + (HLSMAX / 3))) * RGBMAX + (HLSMAX / 2)) / HLSMAX);
                g = (byte)((HueToRGB(Magic1, Magic2, hue) * RGBMAX + (HLSMAX / 2)) / HLSMAX);
                b = (byte)((HueToRGB(Magic1, Magic2, (short)(hue - (HLSMAX / 3))) * RGBMAX + (HLSMAX / 2)) / HLSMAX);
            }
        }

        private void Effect(IntPtr pTexture, int nEffect, ref RECT rect)
        {
            HRESULT hr = HRESULT.S_OK;
            if (m_bEffects || m_bCapture)
            {
                EFFECT nCurrentEffect = GetEffects();
                ID3D11Texture2D pD3D11Texture2D = Marshal.GetObjectForIUnknown(pTexture) as ID3D11Texture2D;
                D3D11_TEXTURE2D_DESC textureDesc = new D3D11_TEXTURE2D_DESC();
                pD3D11Texture2D.GetDesc(out textureDesc);
                SafeRelease(ref pD3D11Texture2D);

                IMFMediaBuffer pBuffer = null;
                hr = MFCreateDXGISurfaceBuffer(typeof(ID3D11Texture2D).GUID, pTexture, 0, false, out pBuffer);
                if (hr == HRESULT.S_OK)
                {
                    IMF2DBuffer p2DBuffer = (IMF2DBuffer)pBuffer;
                    IntPtr pData = IntPtr.Zero;
                    hr = p2DBuffer.Lock2D(out pData, out uint nPitch);
                    if (hr == HRESULT.S_OK)
                    {
                        //int nPitch = (int)(textureDesc.Width << 2);
                        int nSize = (int)(nPitch * textureDesc.Height);
                        byte[] pBytesArray = new byte[nSize];
                        Marshal.Copy(pData, pBytesArray, 0, nSize);

                        if (m_bEffects)
                        {
                            for (int nY = 0; nY < textureDesc.Height; nY++)
                            {
                                for (int nX = 0; nX < textureDesc.Width; nX++)
                                {
                                    int nPos = (int)(nY * nPitch + nX * 4);
                                    byte b = pBytesArray[nPos], g = pBytesArray[nPos + 1], r = pBytesArray[nPos + 2], a = pBytesArray[nPos + 3];

                                    //POINT pt = new POINT(nX, nY);
                                    //bool bInRect = PtInRect(ref rect, pt);
                                    //if (bInRect)
                                    if ((nX > rect.left && nX < rect.right) &&
                                        (nY > rect.top && nY < rect.bottom))
                                    {
                                        if ((nCurrentEffect & EFFECT.INVERT) != 0)
                                        {
                                            b ^= unchecked((byte)0x00FFFFFF);
                                            g ^= unchecked((byte)0x00FFFFFF);
                                            r ^= unchecked((byte)0x00FFFFFF);
                                            pBytesArray[nPos] = b;
                                            pBytesArray[nPos + 1] = g;
                                            pBytesArray[nPos + 2] = r;
                                        }
                                        if ((nCurrentEffect & EFFECT.GRAYSCALE) != 0)
                                        {
                                            byte nGray = Gray(r, g, b);
                                            b = nGray;
                                            g = nGray;
                                            r = nGray;
                                            pBytesArray[nPos] = b;
                                            pBytesArray[nPos + 1] = g;
                                            pBytesArray[nPos + 2] = r;
                                        }
                                        if ((nCurrentEffect & EFFECT.RGB) != 0)
                                        {
                                            int nColor = r + EFFECT_RGB_INTENSITY_RED;
                                            nColor = Math.Max(nColor, 0);
                                            r = (byte)Math.Min(255, nColor);

                                            nColor = g + EFFECT_RGB_INTENSITY_GREEN;
                                            nColor = Math.Max(nColor, 0);
                                            g = (byte)Math.Min(255, nColor);

                                            nColor = b + EFFECT_RGB_INTENSITY_BLUE;
                                            nColor = Math.Max(nColor, 0);
                                            b = (byte)Math.Min(255, nColor);

                                            pBytesArray[nPos] = b;
                                            pBytesArray[nPos + 1] = g;
                                            pBytesArray[nPos + 2] = r;
                                        }
                                        if ((nCurrentEffect & EFFECT.LIGHTEN) != 0)
                                        {
                                            if (EFFECT_LIGHTEN_INTENSITY > 0.0 && EFFECT_LIGHTEN_INTENSITY <= 1.0)
                                            {
                                                byte nLightRed, nLightGreen, nLightBlue;
                                                nLightRed = (byte)((EFFECT_LIGHTEN_INTENSITY * (255 - r)) + r);
                                                nLightGreen = (byte)((EFFECT_LIGHTEN_INTENSITY * (255 - g)) + g);
                                                nLightBlue = (byte)((EFFECT_LIGHTEN_INTENSITY * (255 - b)) + b);
                                                b = nLightBlue;
                                                g = nLightGreen;
                                                r = nLightRed;
                                                pBytesArray[nPos] = b;
                                                pBytesArray[nPos + 1] = g;
                                                pBytesArray[nPos + 2] = r;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //Test negative image from array
                        //for (uint i = 0; i < pBytesArray.Length; ++i)
                        //{
                        //    unchecked
                        //    {
                        //        uint nX = i % nPitch / 4;
                        //        uint nY = (i - nX) / nPitch;
                        //        if ((nX > rect.left && nX < rect.right) &&
                        //            (nY > rect.top && nY < rect.bottom))
                        //        {
                        //            pBytesArray[i] ^= (byte)(0x00FFFFFF);
                        //        }
                        //    }
                        //}
                        Marshal.Copy(pBytesArray, 0, pData, pBytesArray.Length);


                        // GDI+ & GDI instead of directly WriteableBitmap, to extract the right size
                        if (m_bCapture)
                        {
                            m_bCapture = false;
                            if (m_hBitmapCapture != IntPtr.Zero)
                                DeleteObject(m_hBitmapCapture);
                            if (m_hGDIPlusBitmapCapture != IntPtr.Zero)
                                GdipDisposeImage(m_hGDIPlusBitmapCapture);
                            GpStatus nStatus = GdipCreateBitmapFromScan0((int)textureDesc.Width, (int)textureDesc.Height, (int)nPitch,
                                PixelFormat.PixelFormat32bppARGB, pData, out m_hGDIPlusBitmapCapture);
                            if (nStatus == GpStatus.Ok)
                            {
                                CreateHBITMAPFromBitmap(m_hGDIPlusBitmapCapture, Microsoft.UI.Colors.Black, out m_hBitmapCapture, ref rect);
                                DisplayTimedText("Frame captured", false, 3000);
                            }
                        }

                        hr = p2DBuffer.Unlock2D();
                    }
                    SafeRelease(ref p2DBuffer);
                    SafeRelease(ref pBuffer);
                }
            }
        }

        // GdipCreateHBITMAPFromBitmap crashes randomly...
        public void CreateHBITMAPFromBitmap(IntPtr pBitmap, Windows.UI.Color backgroundColor, out IntPtr phBitmap, ref RECT rect)
        {
            //float nWidthBitmap = 0, nHeightBitmap = 0;
            //GpStatus nStatus = GdipGetImageDimension(pBitmap, ref nWidthBitmap, ref nHeightBitmap);
            int nWidth = rect.right - rect.left, nHeight = rect.bottom - rect.top;
            BITMAPV5HEADER bi = new BITMAPV5HEADER();
            bi.bV5Size = Marshal.SizeOf(typeof(BITMAPV5HEADER));
            bi.bV5Width = (int)nWidth;
            bi.bV5Height = (int)nHeight;
            bi.bV5Planes = 1;
            bi.bV5BitCount = 32;
            bi.bV5Compression = BI_BITFIELDS;
            bi.bV5AlphaMask = unchecked((int)0xFF000000);
            bi.bV5RedMask = 0x00FF0000;
            bi.bV5GreenMask = 0x0000FF00;
            bi.bV5BlueMask = 0x000000FF;
            IntPtr hDC = CreateCompatibleDC(IntPtr.Zero);
            IntPtr pBits = IntPtr.Zero;
            IntPtr hBitmapDIBSection = CreateDIBSection(hDC, ref bi, DIB_RGB_COLORS, ref pBits, IntPtr.Zero, 0);
            IntPtr hBitmapOld = SelectObject(hDC, hBitmapDIBSection);
            IntPtr hBrush = CreateSolidBrush(RGB(backgroundColor.R, backgroundColor.G, backgroundColor.B));
            IntPtr hBrushOld = SelectObject(hDC, hBrush);
            PatBlt(hDC, 0, 0, (int)nWidth, (int)nHeight, PATCOPY);          

            IntPtr pGraphics = IntPtr.Zero;
            GdipCreateFromHDC(hDC, out pGraphics);
            GdipDrawImagePointRect(pGraphics, pBitmap, 0, 0, rect.left, rect.top, (int)nWidth, (int)nHeight, GpUnit.UnitPixel);
            phBitmap = hBitmapDIBSection;
            hBitmapDIBSection = IntPtr.Zero;
         
            GdipDeleteGraphics(pGraphics);
            SelectObject(hDC, hBitmapOld);
            DeleteObject(hBitmapDIBSection);
            SelectObject(hDC, hBrushOld);
            DeleteObject(hBrush);           
            DeleteDC(hDC);
        }

        public bool CanCapture()
        {
            return ((m_pMediaEngine.IsPaused() || m_bPlaying) && m_pMediaEngine.HasVideo());
        }

        public void Capture()
        {
            if (CanCapture())
                m_bCapture = true;
        }

        public IntPtr GetCaptureGDIImage()
        {
            return m_hBitmapCapture;
        }

        public IntPtr GetCaptureGDIPlusImage()
        {
            return m_hGDIPlusBitmapCapture;
        }

        public WriteableBitmap GetCaptureWriteableBitmapImage()
        {
            GetCaptureWriteableBitmap();
            return m_hWriteableBitmapCapture;
        }

        public async void GetCaptureWriteableBitmap()
        {
            if (m_hBitmapCapture != IntPtr.Zero)
            {
                BITMAP bm;
                GetObject(m_hBitmapCapture, Marshal.SizeOf(typeof(BITMAP)), out bm);
                int nWidth = bm.bmWidth;
                int nHeight = bm.bmHeight;
                BITMAPV5HEADER bi = new BITMAPV5HEADER();
                bi.bV5Size = Marshal.SizeOf(typeof(BITMAPV5HEADER));
                bi.bV5Width = nWidth;
                bi.bV5Height = -nHeight;
                bi.bV5Planes = 1;
                bi.bV5BitCount = 32;
                bi.bV5Compression = BI_BITFIELDS;
                bi.bV5AlphaMask = unchecked((int)0xFF000000);
                bi.bV5RedMask = 0x00FF0000;
                bi.bV5GreenMask = 0x0000FF00;
                bi.bV5BlueMask = 0x000000FF;

                IntPtr hDC = CreateCompatibleDC(IntPtr.Zero);
                IntPtr hBitmapOld = SelectObject(hDC, m_hBitmapCapture);
                int nNumBytes = (int)(nWidth * 4 * nHeight);
                byte[] pPixels = new byte[nNumBytes];
                int nScanLines = GetDIBits(hDC, m_hBitmapCapture, 0, (uint)nHeight, pPixels, ref bi, DIB_RGB_COLORS);
                if (m_hWriteableBitmapCapture != null)
                    m_hWriteableBitmapCapture.Invalidate();
                m_hWriteableBitmapCapture = new WriteableBitmap(nWidth, nHeight);
                await m_hWriteableBitmapCapture.PixelBuffer.AsStream().WriteAsync(pPixels, 0, pPixels.Length);
                SelectObject(hDC, hBitmapOld);
                DeleteDC(hDC);              
            }
        }       

        public async void SaveCapture(Windows.Storage.StorageFile file)
        {
            if (m_hWriteableBitmapCapture != null)
            {
                Guid guidCodec = Guid.Empty;
                switch (file.FileType)
                {
                    case ".jpg":
                        guidCodec = Windows.Graphics.Imaging.BitmapEncoder.JpegEncoderId;
                        break;
                    case ".png":
                        guidCodec = Windows.Graphics.Imaging.BitmapEncoder.PngEncoderId;
                        break;
                    case ".gif":
                        guidCodec = Windows.Graphics.Imaging.BitmapEncoder.GifEncoderId;
                        break;
                    case ".bmp":
                        guidCodec = Windows.Graphics.Imaging.BitmapEncoder.BmpEncoderId;
                        break;
                    case ".tif":
                        guidCodec = Windows.Graphics.Imaging.BitmapEncoder.TiffEncoderId;
                        break;                 
                }
                using (var ras = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite, Windows.Storage.StorageOpenOptions.None))
                {
                    var stream = m_hWriteableBitmapCapture.PixelBuffer.AsStream();
                    byte[] pBytes = new byte[stream.Length];
                    await stream.ReadAsync(pBytes, 0, pBytes.Length);
                    Windows.Graphics.Imaging.BitmapEncoder encoder = await Windows.Graphics.Imaging.BitmapEncoder.CreateAsync(guidCodec, ras);
                    encoder.SetPixelData(Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8, Windows.Graphics.Imaging.BitmapAlphaMode.Straight, (uint)m_hWriteableBitmapCapture.PixelWidth, (uint)m_hWriteableBitmapCapture.PixelHeight, 96.0, 96.0, pBytes);
                    await encoder.FlushAsync();
                }
            }
        }

        private void Render()
        {
            HRESULT hr = HRESULT.S_OK;
            long nPts;
            if (m_pMediaEngine != null)
            {
                hr = m_pMediaEngine.OnVideoStreamTick(out nPts);
                // MF_E_SHUTDOWN = 0xC00D3E85
                if (hr == HRESULT.S_OK)
                {
                    if (m_pDXGISwapChain1 != null)
                    {
                        IntPtr pTextureDst;
                        hr = m_pDXGISwapChain1.GetBuffer(0, ref IID_ID3D11Texture2D, out pTextureDst);
                        if (hr == HRESULT.S_OK)
                        {
                            MFVideoNormalizedRect vnRect = new MFVideoNormalizedRect(0.0f, 1.0f, 0.0f, 1.0f);
                            var offset = m_VideoContainer.ActualOffset;
                            var sz = m_VideoContainer.ActualSize;
                            sz.Y -= (float)m_nControlsHeight;

                            uint nDPI = GetDpiForWindow(m_hWnd);
                            //sz.Y -= (float)(m_nControlsHeight * (nDPI / 96.0f));
                            offset.X *= (nDPI / 96.0f);
                            offset.Y *= (nDPI / 96.0f);
                            sz.X *= (nDPI / 96.0f);
                            sz.Y *= (nDPI / 96.0f);

                            RECT rectTarget = new RECT((int)offset.X, (int)offset.Y, (int)offset.X + (int)sz.X, (int)offset.Y + (int)sz.Y);
                            if (IsFullScreen())
                            {
                                rectTarget.left = 0;
                                rectTarget.top = 0;
                                rectTarget.right = GetSystemMetrics(SM_CXSCREEN);
                                rectTarget.bottom = GetSystemMetrics(SM_CYSCREEN);
                            }

                            hr = m_pMediaEngine.TransferVideoFrame(pTextureDst, ref vnRect, ref rectTarget, m_BorderColor);
                            //  MF_E_NO_VIDEO_SAMPLE_AVAILABLE = 0xC00D4E26
                            if (hr == HRESULT.S_OK)
                            {
                                Effect(pTextureDst, 0, ref rectTarget);                               

                                if ((m_bOverLay && m_hBitmapOverlay != IntPtr.Zero) || m_bSubtitles || m_bError || m_bMessage)
                                {
                                    IDXGISurface pDXGISurface = Marshal.GetObjectForIUnknown(pTextureDst) as IDXGISurface;
                                    if (pDXGISurface != null)
                                    {
                                        IDXGISurface1 pDXGISurface1 = Marshal.GetObjectForIUnknown(pTextureDst) as IDXGISurface1;
                                        if (pDXGISurface1 != null)
                                        {
                                            IntPtr hDC = IntPtr.Zero;
                                            hr = pDXGISurface1.GetDC(false, out hDC);
                                            if (hr == HRESULT.S_OK)
                                            {
                                                BITMAP bm;
                                                GetObject(m_hBitmapOverlay, Marshal.SizeOf(typeof(BITMAP)), out bm);
                                                int nLeft = (int)(rectTarget.left + (sz.X - offset.X - bm.bmWidth) / 2);
                                                int nTop = (int)(rectTarget.top + (sz.Y - offset.Y - bm.bmHeight) / 2);
                                                int nWidth = bm.bmWidth;
                                                int nHeight = bm.bmHeight;
                                                if (nLeft <= offset.X)
                                                {
                                                    nLeft = (int)offset.X;
                                                    nWidth = (int)sz.X - nLeft;
                                                }
                                                if (nLeft + nWidth >= sz.X)
                                                {
                                                    nWidth = (int)(sz.X - offset.X);
                                                }
                                                if (nTop <= offset.Y)
                                                {
                                                    nTop = (int)offset.Y;
                                                    nHeight = (int)sz.Y - nTop;
                                                }
                                                if (nTop + nHeight >= sz.Y)
                                                {
                                                    nHeight = (int)(sz.Y - offset.Y);
                                                }
                                                RECT rectImage = new RECT(nLeft, nTop, nLeft + nWidth, nTop + nHeight);

                                                if (m_bOverLay)
                                                    DisplayImageAlpha(m_hBitmapOverlay, hDC, ref rectImage, 0.5f);                                                                                        

                                                if (m_bSubtitles)
                                                     DisplayTextAlpha(m_sSubtitleText, "Arial", hDC, ref rectTarget, 30, Microsoft.UI.Colors.White, Microsoft.UI.Colors.Black, 0.8, DT_VERTICAL.BOTTOM);

                                                // Error event or message
                                                if (m_bError || m_bMessage)
                                                {
                                                    if (m_bError)
                                                        DisplayTextAlpha(m_sErrorText, "Arial", hDC, ref rectTarget, 48, Microsoft.UI.Colors.Red, Microsoft.UI.Colors.Transparent, nAlpha, DT_VERTICAL.CENTER);
                                                    if (m_bMessage)
                                                        DisplayTextAlpha(m_sMessageText, "Arial", hDC, ref rectTarget, 48, Microsoft.UI.Colors.Yellow, Microsoft.UI.Colors.Transparent, nAlpha, DT_VERTICAL.CENTER);
                                                }

                                                pDXGISurface1.ReleaseDC(IntPtr.Zero);
                                            }
                                            SafeRelease(ref pDXGISurface1);
                                        }
                                        SafeRelease(ref pDXGISurface);
                                    }
                                }
                                hr = m_pDXGISwapChain1.Present(1, 0);
                            }
                            Marshal.Release(pTextureDst);
                        }
                    }
                }
                else
                {
                    if (m_pDXGISwapChain1 != null)
                    {
                        if (!m_bPlaying)
                        {
                            IntPtr pTextureDst;
                            hr = m_pDXGISwapChain1.GetBuffer(0, ref IID_ID3D11Texture2D, out pTextureDst);
                            if (hr == HRESULT.S_OK)
                            {
                                IDXGISurface pDXGISurface = Marshal.GetObjectForIUnknown(pTextureDst) as IDXGISurface;
                                if (pDXGISurface != null)
                                {
                                    IDXGISurface1 pDXGISurface1 = Marshal.GetObjectForIUnknown(pTextureDst) as IDXGISurface1;
                                    if (pDXGISurface1 != null)
                                    {
                                        IntPtr hDC = IntPtr.Zero;
                                        hr = pDXGISurface1.GetDC(false, out hDC);
                                        // 0x887a0001 DXGI_ERROR_INVALID_CALL
                                        // ID3D11Texture2D surface not D3D11_RESOURCE_MISC_GDI_COMPATIBLE 

                                        //ID3D11Texture2D pD3D11Texture2D = Marshal.GetObjectForIUnknown(pTextureDst) as ID3D11Texture2D;
                                        //D3D11_TEXTURE2D_DESC textureDesc = new D3D11_TEXTURE2D_DESC();
                                        //pD3D11Texture2D.GetDesc(out textureDesc);
                                        //SafeRelease(ref pD3D11Texture2D);

                                        // D3D11_TEXTURE2D_DESC desc = { };
                                        // desc.Width = 1274
                                        // desc.Height = 700
                                        // desc.ArraySize = 1;
                                        // desc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
                                        // desc.Usage = D3D11_USAGE_DEFAULT;
                                        // desc.BindFlags = D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_RENDER_TARGET;
                                        // desc.MipLevels = 1;
                                        // desc.SampleDesc.Count = 1;
                                        // desc.MiscFlags = D3D11_RESOURCE_MISC_GDI_COMPATIBLE;

                                        // ID3D11Texture2D* texture2D;
                                        // m_d3dDevice->CreateTexture2D(&desc, nullptr, &texture2D));
                                        // IDXGISurface1* dxgiSurface1 = texture2D;                                    

                                        if (hr == HRESULT.S_OK)
                                        {
                                            var offset = m_VideoContainer.ActualOffset;
                                            var sz = m_VideoContainer.ActualSize;
                                            sz.Y -= (float)m_nControlsHeight;

                                            uint nDPI = GetDpiForWindow(m_hWnd);
                                            //sz.Y -= (float)(m_nControlsHeight * (nDPI / 96.0f));
                                            offset.X *=(nDPI / 96.0f);
                                            offset.Y *= (nDPI / 96.0f);
                                            sz.X *= (nDPI / 96.0f);
                                            sz.Y *= (nDPI / 96.0f);

                                            RECT rectTarget = new RECT((int)offset.X, (int)offset.Y, (int)offset.X + (int)sz.X, (int)offset.Y +(int)sz.Y);
                                            if (IsFullScreen())
                                            {
                                                rectTarget.left = 0;
                                                rectTarget.top = 0;
                                                rectTarget.right = GetSystemMetrics(SM_CXSCREEN);
                                                rectTarget.bottom = GetSystemMetrics(SM_CYSCREEN);
                                                offset.X = 0;
                                                offset.Y = 0;
                                                sz.X = GetSystemMetrics(SM_CXSCREEN);
                                                sz.Y = GetSystemMetrics(SM_CYSCREEN);
                                            }
                                            //IntPtr hBrush = CreateSolidBrush(System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.Red));
                                            int nWin32Color = RGB(Microsoft.UI.Colors.Black.R, Microsoft.UI.Colors.Black.G, Microsoft.UI.Colors.Black.B);
                                            IntPtr hBrush = CreateSolidBrush(nWin32Color);
                                            FillRect(hDC, ref rectTarget, hBrush);
                                            DeleteObject(hBrush);

                                            if (m_hBitmapLogo != IntPtr.Zero)
                                            {
                                                IntPtr hDCMem = CreateCompatibleDC(hDC);
                                                IntPtr hBitmapOld = SelectObject(hDCMem, m_hBitmapLogo);
                                                BITMAP bm;
                                                GetObject(m_hBitmapLogo, Marshal.SizeOf(typeof(BITMAP)), out bm);

                                                //float nNewWidth = (float)bm.bmWidth * (nDPI / 96.0f);
                                                //float nNewHeight = (float)bm.bmHeight * (nDPI / 96.0f);
                                                float nNewWidth = bm.bmWidth;
                                                float nNewHeight = bm.bmHeight;
                                                bm.bmWidth = (int)nNewWidth;
                                                bm.bmHeight = (int)nNewHeight;

                                                int nLeft = (int)(rectTarget.left + (sz.X - offset.X - bm.bmWidth) / 2);
                                                int nTop = (int)(rectTarget.top + (sz.Y - offset.Y - bm.bmHeight) / 2);
                                                int nWidth = bm.bmWidth;
                                                int nHeight = bm.bmHeight;
                                                if (nLeft <= offset.X)
                                                {
                                                    nLeft = (int)offset.X;
                                                    nWidth = (int)sz.X - nLeft;
                                                }
                                                if (nLeft + nWidth >= sz.X)
                                                {
                                                    nWidth = (int)(sz.X - offset.X);
                                                }
                                                if (nTop <= offset.Y)
                                                {
                                                    nTop = (int)offset.Y;
                                                    nHeight = (int)sz.Y - nTop;
                                                }
                                                if (nTop + nHeight >= sz.Y)
                                                {
                                                    nHeight = (int)(sz.Y - offset.Y);
                                                }                                             

                                                int nOldMode = SetStretchBltMode(hDC, HALFTONE);
                                                StretchBlt(hDC, nLeft, nTop, nWidth, nHeight, hDCMem, 0, 0, bm.bmWidth, bm.bmHeight, SRCCOPY);
                                                SetStretchBltMode(hDC, nOldMode);
                                                SelectObject(hDCMem, hBitmapOld);
                                                DeleteDC(hDCMem);
                                            }

                                            // Error event or message
                                            if (m_bError || m_bMessage)
                                            {
                                                if (m_bError)
                                                    DisplayTextAlpha(m_sErrorText, "Arial", hDC, ref rectTarget, 48, Microsoft.UI.Colors.Red, Microsoft.UI.Colors.Transparent, nAlpha, DT_VERTICAL.CENTER);
                                                if (m_bMessage)
                                                    DisplayTextAlpha(m_sMessageText, "Arial", hDC, ref rectTarget, 48, Microsoft.UI.Colors.Yellow, Microsoft.UI.Colors.Transparent, nAlpha, DT_VERTICAL.CENTER);
                                            }
                                          
                                            pDXGISurface1.ReleaseDC(IntPtr.Zero);

                                            // hr = 0x887a0001 DXGI_ERROR_INVALID_CALL
                                            hr = m_pDXGISwapChain1.Present(1, 0);
                                        }
                                        SafeRelease(ref pDXGISurface1);
                                    }
                                    SafeRelease(ref pDXGISurface);
                                }
                                Marshal.Release(pTextureDst);
                            }
                        }
                        else if (m_bPause == true)
                        {
                            IntPtr pTextureDst;
                            hr = m_pDXGISwapChain1.GetBuffer(0, ref IID_ID3D11Texture2D, out pTextureDst);
                            if (hr == HRESULT.S_OK)
                            {
                                MFVideoNormalizedRect vnRect = new MFVideoNormalizedRect(0.0f, 1.0f, 0.0f, 1.0f);
                                var offset = m_VideoContainer.ActualOffset;
                                var sz = m_VideoContainer.ActualSize;
                                sz.Y -= (float)m_nControlsHeight;

                                uint nDPI = GetDpiForWindow(m_hWnd);
                                //sz.Y -= (float)(m_nControlsHeight * (nDPI / 96.0f));
                                offset.X *= (nDPI / 96.0f);
                                offset.Y *= (nDPI / 96.0f);
                                sz.X *= (nDPI / 96.0f);
                                sz.Y *= (nDPI / 96.0f);

                                RECT rectTarget = new RECT((int)offset.X, (int)offset.Y, (int)offset.X + (int)sz.X, (int)offset.Y + (int)sz.Y);
                                if (IsFullScreen())
                                {
                                    rectTarget.left = 0;
                                    rectTarget.top = 0;
                                    rectTarget.right = GetSystemMetrics(SM_CXSCREEN);
                                    rectTarget.bottom = GetSystemMetrics(SM_CYSCREEN);
                                    offset.X = 0;
                                    offset.Y = 0;
                                    sz.X = GetSystemMetrics(SM_CXSCREEN);
                                    sz.Y = GetSystemMetrics(SM_CYSCREEN);
                                }
                                hr = m_pMediaEngine.TransferVideoFrame(pTextureDst, ref vnRect, ref rectTarget, m_BorderColor);
                                if (hr == HRESULT.S_OK)
                                {
                                    Effect(pTextureDst, 0, ref rectTarget);
    
                                    if ((m_bOverLay && m_hBitmapOverlay != IntPtr.Zero) || m_bSubtitles || m_bError || m_bMessage)
                                    {
                                        IDXGISurface pDXGISurface = Marshal.GetObjectForIUnknown(pTextureDst) as IDXGISurface;
                                        if (pDXGISurface != null)
                                        {
                                            IDXGISurface1 pDXGISurface1 = Marshal.GetObjectForIUnknown(pTextureDst) as IDXGISurface1;
                                            if (pDXGISurface1 != null)
                                            {
                                                IntPtr hDC = IntPtr.Zero;
                                                hr = pDXGISurface1.GetDC(false, out hDC);
                                                if (hr == HRESULT.S_OK)
                                                {
                                                    BITMAP bm;
                                                    GetObject(m_hBitmapOverlay, Marshal.SizeOf(typeof(BITMAP)), out bm);
                                                    int nLeft = (int)(rectTarget.left + (sz.X - offset.X - bm.bmWidth) / 2);
                                                    int nTop = (int)(rectTarget.top + (sz.Y - offset.Y - bm.bmHeight) / 2);
                                                    int nWidth = bm.bmWidth;
                                                    int nHeight = bm.bmHeight;
                                                    if (nLeft <= offset.X)
                                                    {
                                                        nLeft = (int)offset.X;
                                                        nWidth = (int)sz.X - nLeft;
                                                    }
                                                    if (nLeft + nWidth >= sz.X)
                                                    {
                                                        nWidth = (int)(sz.X - offset.X);
                                                    }
                                                    if (nTop <= offset.Y)
                                                    {
                                                        nTop = (int)offset.Y;
                                                        nHeight = (int)sz.Y - nTop;
                                                    }
                                                    if (nTop + nHeight >= sz.Y)
                                                    {
                                                        nHeight = (int)(sz.Y - offset.Y);
                                                    }
                                                    RECT rectImage = new RECT(nLeft, nTop, nLeft + nWidth, nTop + nHeight);

                                                    if (m_bOverLay)
                                                        DisplayImageAlpha(m_hBitmapOverlay, hDC, ref rectImage, 0.5f);

                                                    if (m_bSubtitles)
                                                        DisplayTextAlpha(m_sSubtitleText, "Arial", hDC, ref rectTarget, 30, Microsoft.UI.Colors.White, Microsoft.UI.Colors.Black, 0.8, DT_VERTICAL.BOTTOM);

                                                    // Error event or message
                                                    if (m_bError || m_bMessage)
                                                    {
                                                        if (m_bError)
                                                            DisplayTextAlpha(m_sErrorText, "Arial", hDC, ref rectTarget, 48, Microsoft.UI.Colors.Red, Microsoft.UI.Colors.Transparent, nAlpha, DT_VERTICAL.CENTER);
                                                        if (m_bMessage)
                                                            DisplayTextAlpha(m_sMessageText, "Arial", hDC, ref rectTarget, 48, Microsoft.UI.Colors.Yellow, Microsoft.UI.Colors.Transparent, nAlpha, DT_VERTICAL.CENTER);
                                                    }

                                                    pDXGISurface1.ReleaseDC(IntPtr.Zero);
                                                }
                                                SafeRelease(ref pDXGISurface1);
                                            }
                                            SafeRelease(ref pDXGISurface);
                                        }
                                    }
                                    hr = m_pDXGISwapChain1.Present(1, 0);
                                }
                                else
                                {
                                    // 0xC00D4E26 MF_E_NO_VIDEO_SAMPLE_AVAILABLE
                                    if (!HasVideo())
                                    {
                                        if (m_hBitmapNotes != IntPtr.Zero)
                                        {
                                            DisplayBitmapSurface(pTextureDst, m_hBitmapNotes, Microsoft.UI.Colors.White, ref rectTarget);
                                        }
                                        else
                                            FillRectangleSurface(pTextureDst, Microsoft.UI.Colors.LightBlue);
                                        hr = m_pDXGISwapChain1.Present(1, 0);
                                    }
                                }
                                Marshal.Release(pTextureDst);
                            }
                        }
                        else
                        {
                            if (!HasVideo())
                            {
                                IntPtr pTextureDst;
                                hr = m_pDXGISwapChain1.GetBuffer(0, ref IID_ID3D11Texture2D, out pTextureDst);
                                if (hr == HRESULT.S_OK)
                                {
                                    if (m_hBitmapNotes != IntPtr.Zero)
                                    {
                                        var offset = m_VideoContainer.ActualOffset;
                                        var sz = m_VideoContainer.ActualSize;
                                        sz.Y -= (float)m_nControlsHeight;

                                        uint nDPI = GetDpiForWindow(m_hWnd);
                                        //sz.Y -= (float)(m_nControlsHeight * (nDPI / 96.0f));
                                        offset.X *= (nDPI / 96.0f);
                                        offset.Y *= (nDPI / 96.0f);
                                        sz.X *= (nDPI / 96.0f);
                                        sz.Y *= (nDPI / 96.0f);

                                        RECT rectTarget = new RECT((int)offset.X, (int)offset.Y, (int)offset.X + (int)sz.X, (int)offset.Y + (int)sz.Y);
                                        if (IsFullScreen())
                                        {
                                            rectTarget.left = 0;
                                            rectTarget.top = 0;
                                            rectTarget.right = GetSystemMetrics(SM_CXSCREEN);
                                            rectTarget.bottom = GetSystemMetrics(SM_CYSCREEN);
                                            offset.X = 0;
                                            offset.Y = 0;
                                            sz.X = GetSystemMetrics(SM_CXSCREEN);
                                            sz.Y = GetSystemMetrics(SM_CYSCREEN);
                                        }
                                        DisplayBitmapSurface(pTextureDst, m_hBitmapNotes, Microsoft.UI.Colors.White, ref rectTarget);
                                    }
                                    else
                                        FillRectangleSurface(pTextureDst, Microsoft.UI.Colors.Blue);
                                    hr = m_pDXGISwapChain1.Present(1, 0);
                                    Marshal.Release(pTextureDst);
                                }
                            }
                            else
                            {
                                //IntPtr pTextureDst;
                                //hr = m_pDXGISwapChain1.GetBuffer(0, ref IID_ID3D11Texture2D, out pTextureDst);
                                //if (hr == HRESULT.S_OK)
                                //{
                                //    FillRectangleSurface(pTextureDst, Microsoft.UI.Colors.Black);
                                //    hr = m_pDXGISwapChain1.Present(1, 0);
                                //    Marshal.Release(pTextureDst);
                                //}
                            }
                        }
                    }
                }
            }
        }
        
        // Timed text
        private DispatcherTimer dTimer;
        private TimeSpan tsDuration;
        private DateTime tsEnd;
        private bool m_bError = false;
        private bool m_bMessage = false;
        private string m_sErrorText = string.Empty;
        private string m_sMessageText = string.Empty;        
        double nAlpha = 0;

        private void DisplayTimedText(string sText, bool bError, int nMilliSeconds)
        {
            if (bError)
            {
                m_bError = true;
                m_sErrorText = sText;
            }
            else
            {
                m_bMessage = true;
                m_sMessageText = sText;
            }
            LaunchTaskTimer(nMilliSeconds);
        }

        private void StartTimer(int nMilliSeconds)
        {
            dTimer = new DispatcherTimer();
            dTimer.Interval = TimeSpan.FromMilliseconds(100);
            tsDuration = TimeSpan.FromMilliseconds(nMilliSeconds);
            tsEnd = DateTime.UtcNow + tsDuration;
            dTimer.Tick += Dt_Tick;
            dTimer.Start();
        }

        private void Dt_Tick(object sender, object e)
        {
            DateTime dtNow = DateTime.UtcNow;
            if (dtNow >= tsEnd)
            {
                if (dTimer != null)
                {
                    dTimer.Stop();
                    dTimer = null;
                    nAlpha = 0;
                }
                if (m_bError)
                {
                    m_bError = false;
                    m_sErrorText = string.Empty;
                }
            }
            else
            {
                nAlpha = (tsEnd - dtNow).TotalMilliseconds / tsDuration.TotalMilliseconds;
            }
        }          

        private void DisplayTextAlpha(string sText, string sFont, IntPtr hDC, ref RECT rectTarget, int nHeight, Windows.UI.Color colorForeground, Windows.UI.Color colorBackground, double nAlphaBlend, DT_VERTICAL nVerticalMode = DT_VERTICAL.TOP)
        {
            StringBuilder sb = new StringBuilder(sText);
            int nWin32ColorForeground = RGB(colorForeground.R, colorForeground.G, colorForeground.B);
            int nWin32ColorBackground = RGB(colorBackground.R, colorBackground.G, colorBackground.B);

            IntPtr hDCMem = CreateCompatibleDC(hDC);
            BITMAPINFO bi = new BITMAPINFO();
            bi.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            bi.bmiHeader.biWidth = rectTarget.right - rectTarget.left;
            bi.bmiHeader.biHeight = rectTarget.bottom - rectTarget.top;
            bi.bmiHeader.biPlanes = 1;
            bi.bmiHeader.biBitCount = 32;
            bi.bmiHeader.biCompression = BI_RGB;
            IntPtr pBits = IntPtr.Zero;
            IntPtr hBitmapText = CreateDIBSection(hDCMem, ref bi, DIB_RGB_COLORS, ref pBits, IntPtr.Zero, 0);
            if (hBitmapText != IntPtr.Zero)
            {
                IntPtr hBitmapOld = SelectObject(hDCMem, hBitmapText);

                //if (colorBackground == Microsoft.UI.Colors.Transparent)
                //    nWin32ColorBackground = RGB(0, 0, 0);
                if (colorBackground == Microsoft.UI.Colors.Transparent)
                    SetBkMode(hDCMem, TRANSPARENT);
                SetTextColor(hDCMem, nWin32ColorForeground);
                SetBkColor(hDCMem, nWin32ColorBackground);

                LOGFONT lf = new LOGFONT();
                lf.lfHeight = -MulDiv(nHeight, GetDeviceCaps(hDC, LOGPIXELSY), 72);
                lf.lfWeight = FW_NORMAL;
                lf.lfCharSet = DEFAULT_CHARSET;
                lf.lfOutPrecision = OUT_TT_ONLY_PRECIS;// OUT_DEFAULT_PRECIS;
                lf.lfClipPrecision = CLIP_DEFAULT_PRECIS;
                lf.lfQuality = ANTIALIASED_QUALITY;
                lf.lfPitchAndFamily = DEFAULT_PITCH | FF_DONTCARE;
                //lf.lfFaceName = "Times New Roman";
                lf.lfFaceName = sFont;
                IntPtr hFont = CreateFontIndirect(lf);
                IntPtr hFontOld = SelectObject(hDCMem, hFont);

                //DrawText(hDCMem, sb, -1, ref rectTarget, DT_SINGLELINE | DT_CENTER | DT_VCENTER | DT_WORDBREAK);

                RECT rectText;
                if (nVerticalMode == DT_VERTICAL.CENTER)
                {
                    RECT newRect = new RECT(0, 0, rectTarget.right - rectTarget.left, rectTarget.bottom - rectTarget.top);
                    DrawText(hDCMem, sb, -1, ref newRect, DT_WORDBREAK | DT_CALCRECT);
                    int nTextWidth = newRect.right - newRect.left;
                    int nTextHeight = newRect.bottom - newRect.top;
                    newRect.top += ((rectTarget.bottom - rectTarget.top) - nTextHeight) / 2;
                    newRect.bottom = newRect.top + nTextHeight;
                    //newRect.left += ((rectTarget.right - rectTarget.left) - nTextWidth) / 2;
                    newRect.left = 0;
                    //newRect.right = newRect.left + nTextWidth;
                    newRect.right = rectTarget.right;

                    //IntPtr hBrush = CreateSolidBrush(nWin32ColorBackground);
                    //RECT newRect1 = new RECT(newRect.left, newRect.top, newRect.right, newRect.bottom);
                    //FillRect(hDCMem, ref newRect1, hBrush);
                    //DeleteObject(hBrush);

                    DrawText(hDCMem, sb, -1, ref newRect, DT_CENTER | DT_WORDBREAK);

                    BITMAPINFO bi2 = new BITMAPINFO();
                    bi2.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                    bi2.bmiHeader.biWidth = 1;
                    bi2.bmiHeader.biHeight = 1;
                    bi2.bmiHeader.biPlanes = 1;
                    bi2.bmiHeader.biBitCount = 32;
                    bi2.bmiHeader.biCompression = BI_RGB;

                    byte[] pixelsAlpha = new byte[4];
                    pixelsAlpha[0] = 0;
                    pixelsAlpha[1] = 0;
                    pixelsAlpha[2] = 0;
                    if (colorBackground == Microsoft.UI.Colors.Transparent)
                        pixelsAlpha[3] = 0;
                    else
                        pixelsAlpha[3] = (byte)(nAlphaBlend * 255);
                    int nScanLines = StretchDIBits(hDCMem, newRect.left, newRect.top, newRect.right - newRect.left, newRect.bottom - newRect.top,
                        0, 0, 1, 1, pixelsAlpha, ref bi2, DIB_RGB_COLORS, SRCPAINT);
                   
                    rectText = newRect;
                }
                else if (nVerticalMode == DT_VERTICAL.BOTTOM)
                {
                    int nBottomSpace = 0;
                    StringBuilder sbTest = new StringBuilder("y");
                    RECT newRectTest = new RECT();
                    DrawText(hDCMem, sbTest, -1, ref newRectTest, DT_CALCRECT);
                    nBottomSpace = newRectTest.bottom - newRectTest.top;
                    nBottomSpace /= 2;

                    RECT newRect = new RECT(0, 0, rectTarget.right - rectTarget.left, rectTarget.bottom - rectTarget.top);
                    DrawText(hDCMem, sb, -1, ref newRect, DT_WORDBREAK | DT_CALCRECT);
                    int nTextWidth = newRect.right - newRect.left;
                    int nTextHeight = newRect.bottom - newRect.top;
                    newRect.left = 0;
                    newRect.top = rectTarget.bottom - nTextHeight - nBottomSpace;
                    newRect.right = rectTarget.right;
                    newRect.bottom = rectTarget.bottom - nBottomSpace;

                    //IntPtr hBrush = CreateSolidBrush(nWin32ColorBackground);                    
                    //RECT newRect1 = new RECT(newRect.left, newRect.top, newRect.right, newRect.bottom);
                    //FillRect(hDCMem, ref newRect1, hBrush);
                    //DeleteObject(hBrush);

                    DrawText(hDCMem, sb, -1, ref newRect, DT_CENTER | DT_WORDBREAK);

                    BITMAPINFO bi2 = new BITMAPINFO();
                    bi2.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                    bi2.bmiHeader.biWidth = 1;
                    bi2.bmiHeader.biHeight = 1;
                    bi2.bmiHeader.biPlanes = 1;
                    bi2.bmiHeader.biBitCount = 32;
                    bi2.bmiHeader.biCompression = BI_RGB;
 
                    byte[] pixelsAlpha = new byte[4];
                    pixelsAlpha[0] = 0;
                    pixelsAlpha[1] = 0;
                    pixelsAlpha[2] = 0;
                    if (colorBackground == Microsoft.UI.Colors.Transparent)
                        pixelsAlpha[3] = 0;
                    else
                        pixelsAlpha[3] = (byte)(nAlphaBlend * 255);
                    int nScanLines = StretchDIBits(hDCMem, newRect.left, newRect.top, newRect.right - newRect.left, newRect.bottom - newRect.top,
                        0, 0, 1, 1, pixelsAlpha, ref bi2, DIB_RGB_COLORS, SRCPAINT);
                    
                    rectText = newRect;
                }
                else
                {
                    RECT newRect = new RECT(0, 0, rectTarget.right - rectTarget.left, rectTarget.bottom - rectTarget.top);
                    DrawText(hDCMem, sb, -1, ref newRect, DT_WORDBREAK | DT_CALCRECT);
                    int nTextWidth = newRect.right - newRect.left;
                    int nTextHeight = newRect.bottom - newRect.top;
                    newRect.left = 0;
                    newRect.top = rectTarget.top;
                    newRect.right = rectTarget.right;
                    newRect.bottom = newRect.top + nTextHeight;

                    //IntPtr hBrush = CreateSolidBrush(nWin32ColorBackground);
                    //RECT newRect1 = new RECT(newRect.left, newRect.top, newRect.right, newRect.bottom);
                    //FillRect(hDCMem, ref newRect1, hBrush);
                    //DeleteObject(hBrush);

                    DrawText(hDCMem, sb, -1, ref rectTarget, DT_CENTER | DT_WORDBREAK);

                    BITMAPINFO bi2 = new BITMAPINFO();
                    bi2.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                    bi2.bmiHeader.biWidth = 1;
                    bi2.bmiHeader.biHeight = 1;
                    bi2.bmiHeader.biPlanes = 1;
                    bi2.bmiHeader.biBitCount = 32;
                    bi2.bmiHeader.biCompression = BI_RGB;

                    byte[] pixelsAlpha = new byte[4];
                    pixelsAlpha[0] = 0;
                    pixelsAlpha[1] = 0;
                    pixelsAlpha[2] = 0;
                    if (colorBackground == Microsoft.UI.Colors.Transparent)
                        pixelsAlpha[3] = 0;
                    else
                        pixelsAlpha[3] = (byte)(255 * nAlphaBlend);
                    int nScanLines = StretchDIBits(hDCMem, newRect.left, newRect.top, newRect.right - newRect.left, newRect.bottom - newRect.top,
                        0, 0, 1, 1, pixelsAlpha, ref bi2, DIB_RGB_COLORS, SRCPAINT);
                    
                    rectText = rectTarget;
                }

                BLENDFUNCTION bf;
                bf = new BLENDFUNCTION();
                bf.BlendOp = AC.SRC_OVER;
                bf.BlendFlags = 0;
                bf.SourceConstantAlpha = (byte)(255 * nAlphaBlend);
                if (!IsFullScreen())
                    bf.AlphaFormat = AC.SRC_ALPHA;
                bool bRet = AlphaBlend(hDC, rectTarget.left, rectTarget.top, rectTarget.right - rectTarget.left, rectTarget.bottom - rectTarget.top, hDCMem,
                    0, 0, rectTarget.right - rectTarget.left, rectTarget.bottom - rectTarget.top, bf);

                SelectObject(hDCMem, hFontOld);
                DeleteObject(hFont);
                SelectObject(hDCMem, hBitmapOld);
                DeleteObject(hBitmapText);
                DeleteDC(hDCMem);
            }
        }

        public void SetImageOverlay(bool bOverlay, string sImageFile)
        {
            HRESULT hr = HRESULT.S_OK;
            m_bOverLay = bOverlay;
            if (sImageFile != String.Empty && sImageFile != null)
            {
                IntPtr pBitmap = IntPtr.Zero;
                System.Runtime.InteropServices.ComTypes.IStream pstm;
                hr = SHCreateStreamOnFile(sImageFile, (int)STGM_READ, out pstm);
                if (hr == HRESULT.S_OK)
                {
                    GpStatus nStatus = GdipCreateBitmapFromStream(pstm, out pBitmap);
                    if (nStatus == GpStatus.Ok)
                    {
                        GdipCreateHBITMAPFromBitmap(pBitmap, out m_hBitmapOverlay, RGB(Microsoft.UI.Colors.Black.R, Microsoft.UI.Colors.Black.G, Microsoft.UI.Colors.Black.B));
                        Marshal.Release(pBitmap);
                    }
                    SafeRelease(ref pstm);
                }
            }
            if (!bOverlay)
            {
                DeleteObject(m_hBitmapOverlay);
                m_hBitmapOverlay = IntPtr.Zero;
            }            
        }

        public bool GetImageOverlay()
        {
            return m_bOverLay;
        }

        private void DisplayImageAlpha(IntPtr hBitmap, IntPtr hDC, ref RECT rectTarget, float nAlphaBlend)
        {   
            IntPtr hDCMem = CreateCompatibleDC(hDC);
            IntPtr hBitmapOld = SelectObject(hDCMem, hBitmap);
            BITMAP bm;
            GetObject(hBitmap, Marshal.SizeOf(typeof(BITMAP)), out bm);           

            BLENDFUNCTION bf;
            bf = new BLENDFUNCTION();
            bf.BlendOp = AC.SRC_OVER;
            bf.BlendFlags = 0;
            bf.SourceConstantAlpha = (byte)(255 * nAlphaBlend);
            bf.AlphaFormat = AC.SRC_ALPHA;
            AlphaBlend(hDC, rectTarget.left, rectTarget.top, rectTarget.right - rectTarget.left, rectTarget.bottom - rectTarget.top, hDCMem,
                0, 0, bm.bmWidth, bm.bmHeight, bf);
            
            SelectObject(hDCMem, hBitmapOld);
            DeleteDC(hDCMem);
        }

        private void DisplayBitmapSurface(IntPtr pTextureDst, IntPtr hBitmap, Windows.UI.Color color, ref RECT rectTarget)
        {
            HRESULT hr = HRESULT.S_OK;
            IDXGISurface pDXGISurface = Marshal.GetObjectForIUnknown(pTextureDst) as IDXGISurface;
            if (pDXGISurface != null)
            {
                IDXGISurface1 pDXGISurface1 = Marshal.GetObjectForIUnknown(pTextureDst) as IDXGISurface1;
                if (pDXGISurface1 != null)
                {
                    IntPtr hDC = IntPtr.Zero;
                    hr = pDXGISurface1.GetDC(false, out hDC);
                    if (hr == HRESULT.S_OK)
                    {                      
                        IntPtr hDCMem = CreateCompatibleDC(hDC);
                        IntPtr hBitmapOld = SelectObject(hDCMem, hBitmap);
                        int nWin32Color = RGB(color.R, color.G, color.B);
                        IntPtr hBrush = CreateSolidBrush(nWin32Color);
                        FillRect(hDC, ref rectTarget, hBrush);
                        DeleteObject(hBrush);

                        var offset = m_VideoContainer.ActualOffset;
                        var sz = m_VideoContainer.ActualSize;
                        sz.Y -= (float)m_nControlsHeight;

                        uint nDPI = GetDpiForWindow(m_hWnd);
                        //sz.Y -= (float)(m_nControlsHeight * (nDPI / 96.0f));
                        offset.X *= (nDPI / 96.0f);
                        offset.Y *= (nDPI / 96.0f);
                        sz.X *= (nDPI / 96.0f);
                        sz.Y *= (nDPI / 96.0f);

                        BITMAP bm;
                        GetObject(hBitmap, Marshal.SizeOf(typeof(BITMAP)), out bm);
                        int nLeft = (int)(rectTarget.left + (sz.X - offset.X - bm.bmWidth) / 2);
                        int nTop = (int)(rectTarget.top + (sz.Y - offset.Y - bm.bmHeight) / 2);
                        int nWidth = bm.bmWidth;
                        int nHeight = bm.bmHeight;
                        if (nLeft <= offset.X)
                        {
                            nLeft = (int)offset.X;
                            nWidth = (int)sz.X - nLeft;
                        }
                        if (nLeft + nWidth >= sz.X)
                        {
                            nWidth = (int)(sz.X - offset.X);
                        }
                        if (nTop <= offset.Y)
                        {
                            nTop = (int)offset.Y;
                            nHeight = (int)sz.Y - nTop;
                        }
                        if (nTop + nHeight >= sz.Y)
                        {
                            nHeight = (int)(sz.Y - offset.Y);
                        }

                        int nOldMode = SetStretchBltMode(hDC, HALFTONE);
                        StretchBlt(hDC, nLeft, nTop, nWidth, nHeight, hDCMem, 0, 0, bm.bmWidth, bm.bmHeight, SRCCOPY);
                        SetStretchBltMode(hDC, nOldMode);
                        SelectObject(hDCMem, hBitmapOld);
                        DeleteDC(hDCMem);
                        pDXGISurface1.ReleaseDC(IntPtr.Zero);
                    }
                    SafeRelease(ref pDXGISurface1);
                }
                SafeRelease(ref pDXGISurface);
            }
        }

        private void FillRectangleSurface(IntPtr pTextureDst, Windows.UI.Color color)
        {
            HRESULT hr = HRESULT.S_OK;
            IDXGISurface pDXGISurface = Marshal.GetObjectForIUnknown(pTextureDst) as IDXGISurface;
            if (pDXGISurface != null)
            {
                IDXGISurface1 pDXGISurface1 = Marshal.GetObjectForIUnknown(pTextureDst) as IDXGISurface1;
                if (pDXGISurface1 != null)
                {
                    IntPtr hDC = IntPtr.Zero;
                    hr = pDXGISurface1.GetDC(false, out hDC);
                    if (hr == HRESULT.S_OK)
                    {
                        var offset = m_VideoContainer.ActualOffset;
                        var sz = m_VideoContainer.ActualSize;
                        sz.Y -= (float)m_nControlsHeight;

                        uint nDPI = GetDpiForWindow(m_hWnd);
                        //sz.Y -= (float)(m_nControlsHeight * (nDPI / 96.0f));
                        offset.X *= (nDPI / 96.0f);
                        offset.Y *= (nDPI / 96.0f);
                        sz.X *= (nDPI / 96.0f);
                        sz.Y *= (nDPI / 96.0f);

                        RECT rectTarget = new RECT((int)offset.X, (int)offset.Y, (int)sz.X, (int)sz.Y);
                        if (IsFullScreen())
                        {
                            rectTarget.left = 0;
                            rectTarget.top = 0;
                            rectTarget.right = GetSystemMetrics(SM_CXSCREEN);
                            rectTarget.bottom = GetSystemMetrics(SM_CYSCREEN);
                            offset.X = 0;
                            offset.Y = 0;
                            sz.X = GetSystemMetrics(SM_CXSCREEN);
                            sz.Y = GetSystemMetrics(SM_CYSCREEN);
                        }

                        int nWin32Color = RGB(color.R, color.G, color.B);
                        IntPtr hBrush = CreateSolidBrush(nWin32Color);
                        FillRect(hDC, ref rectTarget, hBrush);
                        DeleteObject(hBrush);

                        pDXGISurface1.ReleaseDC(IntPtr.Zero);
                      }
                    SafeRelease(ref pDXGISurface1);
                }
                SafeRelease(ref pDXGISurface);
            }
        }

        private HRESULT CreateD3D11Device()
        {
            HRESULT hr = HRESULT.S_OK;

            int[] aD3D_FEATURE_LEVEL = new int[] { (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1, (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0,
                (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_1, (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_0,  (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_3,
                (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_2, (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_1};
            uint creationFlags = (uint)D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_VIDEO_SUPPORT | (uint)D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT;

            //creationFlags |= (uint)D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG;

            D3D_FEATURE_LEVEL featureLevel;
          
            hr = D3D11CreateDevice(null,    // specify null to use the default adapter
                D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,
                IntPtr.Zero,
                creationFlags,              // optionally set debug and Direct2D compatibility flags
                aD3D_FEATURE_LEVEL, // list of feature levels this app can support
                (uint)aD3D_FEATURE_LEVEL.Length,
                D3D11_SDK_VERSION,
                out m_pD3D11DevicePtr,        // returns the Direct3D device created
                out featureLevel,            // returns feature level of device created                                             
                out m_pD3D11DeviceContextPtr   // returns the device immediate context
            );
            if (hr == HRESULT.S_OK)
            {
                ID3D10Multithread pMultithread = Marshal.GetObjectForIUnknown(m_pD3D11DevicePtr) as ID3D10Multithread;
                pMultithread.SetMultithreadProtected(true);
            }
            return hr;
        }       

        private HRESULT CreateSwapChain(IntPtr hWnd)
        {
            HRESULT hr = HRESULT.S_OK;

            IDXGIFactory2 pDXGIFactory2 = null;
            hr = CreateDXGIFactory2(DXGI_CREATE_FACTORY_DEBUG, typeof(IDXGIFactory2).GUID, out pDXGIFactory2);
            if (hr == HRESULT.S_OK)
            {
                DXGI_SWAP_CHAIN_DESC1 swapChainDesc = new DXGI_SWAP_CHAIN_DESC1();
                swapChainDesc.Width = 1;
                swapChainDesc.Height = 1;

                swapChainDesc.Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM; // this is the most common swapchain format
                swapChainDesc.Stereo = false;
                swapChainDesc.SampleDesc.Count = 1;                // don't use multi-sampling
                swapChainDesc.SampleDesc.Quality = 0;
                swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
                swapChainDesc.BufferCount = 2;                     // use double buffering to enable flip
                //swapChainDesc.Scaling = DXGI_SCALING.DXGI_SCALING_STRETCH;
                swapChainDesc.Scaling = DXGI_SCALING.DXGI_SCALING_NONE;
                swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL; // all apps must use this SwapEffect       
                //swapChainDesc.Flags = 0;
                swapChainDesc.Flags = DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_GDI_COMPATIBLE;
                //swapChainDesc.AlphaMode = DXGI_ALPHA_MODE.DXGI_ALPHA_MODE_PREMULTIPLIED;

                hr = pDXGIFactory2.CreateSwapChainForHwnd(m_pD3D11DevicePtr, hWnd, ref swapChainDesc, IntPtr.Zero, null, out m_pDXGISwapChain1);
                // hr = 0x887a0001
                if (hr == HRESULT.S_OK)
                {
                    IDXGIDevice1 pDXGIDevice1 = Marshal.GetObjectForIUnknown(m_pD3D11DevicePtr) as IDXGIDevice1;
                    hr = pDXGIDevice1.SetMaximumFrameLatency(1);

                    SafeRelease(ref pDXGIDevice1);
                    SafeRelease(ref pDXGIFactory2);
                }
            }
            //Marshal.Release(pD3D11DevicePtr);
            //Marshal.Release(pD3D11DeviceContextPtr);

            return hr;
        }

        public HRESULT LoadURL(string sUrl)
        {
            HRESULT hr = HRESULT.S_OK;
            if (m_pMediaEngine != null)
            {
                if (!m_pMediaEngine.GetAutoPlay())
                    hr = m_pMediaEngine.SetAutoPlay(true);

                m_SliderTime.Maximum = 0;
                m_SliderTime.Value = 0;
                hr = m_pMediaEngine.SetSource(sUrl);
                hr = m_pMediaEngine.Load();
                if (hr == HRESULT.S_OK)
                {
                    if (m_bSubtitles && m_sSubtitlesURL != string.Empty)
                    {
                        m_sSubtitleText = "";
                        AddSubtitles(m_sSubtitlesURL);
                    }
                    buttonPlayPause.SetSource("ms-appx:///Assets/Button_Pause_Blue.png");
                }
            }
            return hr;
        }

        public HRESULT Play(bool bUpdateImageButton)
        {
            HRESULT hr = HRESULT.S_OK;
            if (m_pMediaEngine != null)
            {
                string sSourceURL = "";
                hr = m_pMediaEngine.GetCurrentSource(out sSourceURL);
                if (sSourceURL != null)
                {
                    if (bUpdateImageButton)
                        buttonPlayPause.SetSource("ms-appx:///Assets/Button_Pause_Blue.png");
                    hr = m_pMediaEngine.Play();
                    //hr = m_pMediaEngine.SetPlaybackRate(-1);
                }
            }
            return hr;
        }

        public HRESULT Pause(bool bUpdateImageButton)
        {            
            HRESULT hr = HRESULT.S_OK;
            if (m_pMediaEngine != null)
            {
                string sSourceURL = "";
                hr = m_pMediaEngine.GetCurrentSource(out sSourceURL);
                if (sSourceURL != null)
                {
                    if (bUpdateImageButton)
                        buttonPlayPause.SetSource("ms-appx:///Assets/Button_Play_Blue.png");
                    hr = m_pMediaEngine.Pause();
                    //hr = m_pMediaEngineEx.FrameStep(true);
                }
            }
            return hr;
        }

        public HRESULT Stop(bool bUpdateImageButton)
        {
            HRESULT hr = HRESULT.S_OK;
             hr = m_pMediaEngine.Shutdown();
            if (hr == HRESULT.S_OK)
            {
                SafeRelease(ref m_pMediaEngine);
                SafeRelease(ref m_pTimedText);
                m_bPlaying = false;
                hr = Initialize(IntPtr.Zero, 0, null, Microsoft.UI.Colors.Black);
                m_SliderTime.Maximum = 0;
                m_SliderTime.Value = 0;
                TimeSpan ts1 = TimeSpan.FromSeconds(0);
                string sCurrentTimeText = ts1.ToString("hh\\:mm\\:ss");
                string sTotalTimeText = ts1.ToString("hh\\:mm\\:ss");
                m_TextBlockElapsedTime.Text = sCurrentTimeText + "/" + sTotalTimeText;
                //SafeRelease(ref m_pDXGISwapChain1);
                //hr = CreateSwapChain(m_hWnd); // needs to force resizing
                if (bUpdateImageButton)
                    buttonPlayPause.SetSource("ms-appx:///Assets/Button_Play_Blue.png");
            }
            return hr;
        }

        double m_nControlsHeightOld = 0;
        public void ShowControls(bool bShow)
        {
            if (bShow)
            {
                m_nControlsHeight = m_nControlsHeightOld;
                gridContainer.Visibility = Visibility.Visible;                
            }
            else
            {
                m_nControlsHeightOld = m_nControlsHeight;
                m_nControlsHeight = 0;
                gridContainer.Visibility = Visibility.Collapsed;               
            }
            var sz = m_VideoContainer.ActualSize;
            Windows.Foundation.Size szVideoContainer = new Windows.Foundation.Size(sz.X, sz.Y);
            Resize(szVideoContainer);
        }

        public double GetCurrentTime()
        {
            double nCurrentTime = 0;
            if (m_pMediaEngine != null)
            {
                nCurrentTime = m_pMediaEngine.GetCurrentTime();
            }
            return nCurrentTime;
        }

        public HRESULT SetCurrentTime(double nSeekTime)
        {
            HRESULT hr = HRESULT.S_OK;
            if (m_pMediaEngine != null)
            {
                string sSourceURL = "";
                hr = m_pMediaEngine.GetCurrentSource(out sSourceURL);
                if (sSourceURL != null)
                {
                    if (!m_pMediaEngine.IsSeeking())
                    {
                        IMFMediaTimeRange pMediaTimeRange = null;
                        hr = m_pMediaEngine.GetSeekable(out pMediaTimeRange);
                        if (hr == HRESULT.S_OK)
                        {
                            bool bSeekable = pMediaTimeRange.ContainsTime(nSeekTime);
                            double nStart = 0, nEnd = 0;
                            hr = pMediaTimeRange.GetStart(0, out nStart);
                            hr = pMediaTimeRange.GetEnd(0, out nEnd);
                            if (bSeekable)
                                //hr = m_pMediaEngineEx.SetCurrentTimeEx(nSeekTime, MF_MEDIA_ENGINE_SEEK_MODE.MF_MEDIA_ENGINE_SEEK_MODE_NORMAL);
                                hr = m_pMediaEngine.SetCurrentTime(nSeekTime);
                            //LaunchTaskSetTime(nSeekTime);
                            SafeRelease(ref pMediaTimeRange);
                        }
                    }
                }
            }
            return hr;
        }

        public HRESULT SetFullScreen(bool bFulllScreen)
        {
            HRESULT hr = HRESULT.S_OK;
            Microsoft.UI.WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(m_hWnd);
            Microsoft.UI.Windowing.AppWindow apw = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(myWndId);
            Microsoft.UI.Windowing.OverlappedPresenter presenter = apw.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;                         
            presenter.SetBorderAndTitleBar(!bFulllScreen, !bFulllScreen);
            presenter.IsResizable = !bFulllScreen;           
            hr = m_pDXGISwapChain1.SetFullscreenState(bFulllScreen, null);
            m_bFullScreen = bFulllScreen;
            if (bFulllScreen)
            {
                if (m_hHook == 0)
                {
                    MouseProcedure = new HookProc(MouseProc);
                    //hHook = SetWindowsHookEx(WH_MOUSE, MouseProcedure, (IntPtr)0, (int)GetCurrentThreadId());
                    m_hHook = SetWindowsHookEx(WH_MOUSE_LL, MouseProcedure, (IntPtr)0, 0);
                }
            }
            else
            {
                if (m_hHook != 0)
                {
                    UnhookWindowsHookEx(m_hHook);
                    m_hHook = 0;
                }
            }
            return hr;
        }

        public bool IsFullScreen()
        {
            HRESULT hr = HRESULT.S_OK;
            bool bFulllScreen = false;
            IDXGIOutput pDXGIOutput = null;
            hr = m_pDXGISwapChain1.GetFullscreenState(out bFulllScreen, out pDXGIOutput);
            if (hr == HRESULT.S_OK)
            {
                SafeRelease(ref pDXGIOutput);
            }
            return bFulllScreen;
        }

        public bool IsPlaying()
        {
            return ((m_pMediaEngine.IsPaused() || m_bPlaying));
        }

        // After MF_MEDIA_ENGINE_EVENT_LOADEDDATA 
        public bool HasVideo()
        {
            return m_pMediaEngine.HasVideo();
        }

        public void EnableHorizontalMirrorMode(bool bMirror)
        {
            // For testing styles in cues
            // GetCueList();

            // Always false ?
            //if (m_pMediaEngineEx.IsStereo3D())
            //{
            //    m_pMediaEngineEx.SetStereo3DRenderMode(MF3DVideoOutputType.MF3DVideoOutputType_Stereo);
            //    m_pMediaEngineEx.SetStereo3DFramePackingMode(MF_MEDIA_ENGINE_S3D_PACKING_MODE.MF_MEDIA_ENGINE_S3D_PACKING_MODE_SIDE_BY_SIDE);
            //}

            m_pMediaEngineEx.EnableHorizontalMirrorMode(bMirror);
        }

        public void RegisterSubtitles()
        {
            HRESULT hr = HRESULT.S_OK;
            IntPtr pMediaEnginePtr = Marshal.GetComInterfaceForObject(m_pMediaEngine, typeof(IMFMediaEngine));
            IntPtr pTimedTextPtr = IntPtr.Zero;
            hr = MFGetService(pMediaEnginePtr, MF_MEDIA_ENGINE_TIMEDTEXT, typeof(IMFTimedText).GUID, out pTimedTextPtr);
            if (hr == HRESULT.S_OK)
            {
                m_pTimedText = Marshal.GetObjectForIUnknown(pTimedTextPtr) as IMFTimedText;
                hr = m_pTimedText.RegisterNotifications(this);
                Marshal.Release(pTimedTextPtr);
                pTimedTextPtr = IntPtr.Zero;
            }
            Marshal.Release(pMediaEnginePtr);
        }

        // Only after Load (subtitles file loaded)
        public void GetCueList()
        {
            HRESULT hr = HRESULT.S_OK;
            IMFTimedTextTrackList pTracks = null;
            hr = m_pTimedText.GetTracks(out pTracks);
            if (hr == HRESULT.S_OK)
            {
                uint nTracksLength = pTracks.GetLength();
                for (uint i = 0; i < nTracksLength; i++)
                {
                    IMFTimedTextTrack pTimedTextTrack = null;
                    hr = pTracks.GetTrack(i, out pTimedTextTrack);
                    if (hr == HRESULT.S_OK && pTimedTextTrack != null)
                    {
                        IMFTimedTextCueList pCueList = null;
                        hr = pTimedTextTrack.GetCueList(out pCueList);
                        if (hr == HRESULT.S_OK && pCueList != null)
                        {
                            uint nCount = pCueList.GetLength();
                            for (uint c = 0; c < nCount; c++)
                            {
                                IMFTimedTextCue pCue = null;
                                hr = pCueList.GetCueByIndex(c, out pCue);
                                if (hr == HRESULT.S_OK && pCueList != null)
                                {
                                    uint nLineCount = pCue.GetLineCount();
                                    for (uint l = 0; l < nLineCount; l++)
                                    {
                                        IMFTimedTextFormattedText pTimedTextFormattedText = null;
                                        hr = pCue.GetLine(l, out pTimedTextFormattedText);
                                        if (hr == HRESULT.S_OK && pTimedTextFormattedText != null)
                                        {
                                            string sText = string.Empty;
                                            hr = pTimedTextFormattedText.GetText(out sText);                                           

                                            // Always 0...
                                            uint nsfCount = pTimedTextFormattedText.GetSubformattingCount();
                                            for (uint j = 0; j < nsfCount; j++)
                                            {
                                                uint nFirstChar = 0;
                                                uint nCharLength = 0;
                                                IMFTimedTextStyle pTimedTextStyleLine = null;
                                                hr = pTimedTextFormattedText.GetSubformatting(j, out nFirstChar, out nCharLength, out pTimedTextStyleLine);
                                                if (hr == HRESULT.S_OK)
                                                {
                                                    // Code to get styles should be added here
                                                    SafeRelease(ref pTimedTextStyleLine);
                                                }
                                            }
                                            SafeRelease(ref pTimedTextFormattedText);
                                        }
                                    }

                                    MF_TIMED_TEXT_TRACK_KIND nCueKind = pCue.GetCueKind();

                                    IMFTimedTextBinary pTimedTextBinary = null;
                                    hr = pCue.GetData(out pTimedTextBinary);
                                    if (hr == HRESULT.S_OK && pTimedTextBinary != null)
                                    {
                                        SafeRelease(ref pTimedTextBinary);
                                    }

                                    IMFTimedTextStyle pStyle = null;
                                    hr = pCue.GetStyle(out pStyle);
                                    // Always null...
                                    if (hr == HRESULT.S_OK && pStyle != null)
                                    {
                                        SafeRelease(ref pStyle);
                                    }
                                    SafeRelease(ref pCue);
                                }
                            }
                            SafeRelease(ref pCueList);
                        }
                        SafeRelease(ref pTimedTextTrack);
                    }
                }
                SafeRelease(ref pTracks);
            }
        }

        public int FindTrackFromLabel(string sLabel)
        {
            HRESULT hr = HRESULT.S_OK;
            int nTrackId = -1;
            IMFTimedTextTrackList pTracks = null;
            hr = m_pTimedText.GetTracks(out pTracks);
            if (hr == HRESULT.S_OK)
            {
                uint nTracksLength = pTracks.GetLength();
                for (uint i = 0; i < nTracksLength; i++)
                {
                    IMFTimedTextTrack pTimedTextTrack = null;
                    hr = pTracks.GetTrack(i, out pTimedTextTrack);
                    if (hr == HRESULT.S_OK && pTimedTextTrack != null)
                    {
                        string sCurrentLabel = string.Empty;
                        hr = pTimedTextTrack.GetLabel(out sCurrentLabel);
                        if (hr == HRESULT.S_OK)
                        {
                            if (sCurrentLabel == sLabel)
                            {
                                int nCurrentTrackId = (int)pTimedTextTrack.GetId();
                                nTrackId = nCurrentTrackId;
                                break;
                            }
                        }
                        SafeRelease(ref pTimedTextTrack);
                    }
                }
                SafeRelease(ref pTracks);
            }
            return nTrackId;
        }

        public void UpdateTracksFromCollection(System.Collections.ObjectModel.ObservableCollection<string> collection)
        {
            // Cannot remove and loop tracks at same time...
            List<IMFTimedTextTrack> listTracksToRemove = new List<IMFTimedTextTrack>();
             HRESULT hr = HRESULT.S_OK;
            IMFTimedTextTrackList pTracks = null;
            hr = m_pTimedText.GetTracks(out pTracks);
            if (hr == HRESULT.S_OK && pTracks != null)
            {
                uint nTracksLength = pTracks.GetLength();
                for (uint i = 0; i < nTracksLength; i++)
                {
                    IMFTimedTextTrack pTimedTextTrack = null;
                    hr = pTracks.GetTrack(i, out pTimedTextTrack);
                    if (hr == HRESULT.S_OK && pTimedTextTrack != null)
                    {
                        string sCurrentLabel = string.Empty;
                        hr = pTimedTextTrack.GetLabel(out sCurrentLabel);
                        if (hr == HRESULT.S_OK)
                        {
                            if (!collection.Contains(sCurrentLabel))
                            {
                                listTracksToRemove.Add(pTimedTextTrack);
                            }
                            else
                                SafeRelease(ref pTimedTextTrack);
                        }                        
                    }
                }
                SafeRelease(ref pTracks);
            }
            foreach (var TrackToRemove in listTracksToRemove)
            {
                hr = m_pTimedText.RemoveTrack(TrackToRemove);
            }  
        }

        public int AddSubtitles(string sFile)
        {
            HRESULT hr = HRESULT.S_OK;
            hr = m_pTimedText.AddDataSourceFromUrl(sFile, null, null,  
            MF_TIMED_TEXT_TRACK_KIND.MF_TIMED_TEXT_TRACK_KIND_SUBTITLES, true, out uint nTrackId);
            if (hr == HRESULT.S_OK)
            {                            
                return (int)nTrackId;
            }
            return -1;
        }

        public void SetSubtitles(bool bSet, string sURL)
        {
            m_bSubtitles = bSet;
            m_sSubtitlesURL = sURL;
        }

        public void SetEffects(bool bSet)
        {
            m_bEffects = bSet;
        }

        public void UpdateEffects(EFFECT nEffect)
        {            
            m_nEffects = nEffect;
        }

        public EFFECT GetEffects()
        {
            return m_nEffects;
        }

        private int MouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if ((nCode >= 0))
            {
                //MOUSEHOOKSTRUCT pMOUSEHOOKSTRUCT = new MOUSEHOOKSTRUCT();
                //pMOUSEHOOKSTRUCT = (MOUSEHOOKSTRUCT)Marshal.PtrToStructure(lParam, pMOUSEHOOKSTRUCT.GetType());
                //System.Diagnostics.Debug.WriteLine("GetForegroundWindow = {0:X} - wParam = {1}", GetForegroundWindow(), wParam);
                if (wParam == (IntPtr)WM_LBUTTONDOWN)
                {
                    if (GetForegroundWindow() == m_hWnd)
                    {
                        if (IsFullScreen())
                        {
                            Console.Beep(10000, 10);
                            return 1;
                        }
                     }
                }               
            }
            return nCode < 0 ? CallNextHookEx(m_hHook, nCode, wParam, lParam) : 0;
        }

        private static void SafeRelease<T>(ref T comObject) where T : class
        {
            T t = comObject;
            comObject = default(T);
            if (null != t)
            {
                if (Marshal.IsComObject(t))
                    Marshal.ReleaseComObject(t);
            }
        }

        private void Clean()
        {
            MFShutdown();
            HRESULT hr = HRESULT.S_OK;
            hr = m_pMediaEngine.Shutdown();
            if (hr == HRESULT.S_OK)
            {
                SafeRelease(ref m_pMediaEngine);
            }

            SafeRelease(ref m_pDXGISwapChain1);

            SafeRelease(ref m_pDXGIDeviceManager);
            SafeRelease(ref m_pAttributes);
            if (m_pD3D11DevicePtr != IntPtr.Zero)
                Marshal.Release(m_pD3D11DevicePtr);
            if (m_pD3D11DeviceContextPtr != IntPtr.Zero)
                Marshal.Release(m_pD3D11DeviceContextPtr);
            if (m_pUnknownMediaEngineNotify != IntPtr.Zero)
                Marshal.Release(m_pUnknownMediaEngineNotify);            
            SafeRelease(ref m_pMediaEngineClassFactory);
            SafeRelease(ref m_pTimedText);

            if (m_hBitmapLogo != IntPtr.Zero)
                DeleteObject(m_hBitmapLogo);
            if (m_hBitmapOverlay != IntPtr.Zero)
                DeleteObject(m_hBitmapOverlay);
            if (m_hBitmapNotes != IntPtr.Zero)
                DeleteObject(m_hBitmapNotes);
            if (m_hBitmapCapture != IntPtr.Zero)
                DeleteObject(m_hBitmapCapture);
            if (m_hGDIPlusBitmapCapture != IntPtr.Zero)
                GdipDisposeImage(m_hGDIPlusBitmapCapture); 

            GdiplusShutdown(m_initToken);
            if (m_hHook != 0)
                UnhookWindowsHookEx(m_hHook);
        }

        public void Dispose()
        {
            Clean();
        }

        public void TrackAdded(uint trackId)
        {
            return;
        }

        public void TrackRemoved(uint trackId)
        {
            return;
        }

        public void TrackSelected(uint trackId, bool selected)
        {
            return;
        }

        public void TrackReadyStateChanged(MF_TIMED_TEXT_TRACK_READY_STATE trackId)
        {
            switch (trackId)
            {
                case MF_TIMED_TEXT_TRACK_READY_STATE.MF_TIMED_TEXT_TRACK_READY_STATE_NONE:                    
                    break;
                case MF_TIMED_TEXT_TRACK_READY_STATE.MF_TIMED_TEXT_TRACK_READY_STATE_LOADING:                    
                    break;
                case MF_TIMED_TEXT_TRACK_READY_STATE.MF_TIMED_TEXT_TRACK_READY_STATE_LOADED:                    
                    break;
                case MF_TIMED_TEXT_TRACK_READY_STATE.MF_TIMED_TEXT_TRACK_READY_STATE_ERROR:
                    break;
            }
            return;
        }

        public void Error(MF_TIMED_TEXT_ERROR_CODE errorCode, HRESULT extendedErrorCode, uint sourceTrackId)
        {
            return;
        }

        public void Cue(MF_TIMED_TEXT_CUE_EVENT cueEvent, double currentTime, IMFTimedTextCue cue)
        {
            HRESULT hr = HRESULT.S_OK;
            switch (cueEvent)
            {
                case MF_TIMED_TEXT_CUE_EVENT.MF_TIMED_TEXT_CUE_EVENT_CLEAR:
                    m_sSubtitleText = "";
                    break;
                case MF_TIMED_TEXT_CUE_EVENT.MF_TIMED_TEXT_CUE_EVENT_ACTIVE:
                    {
                        MF_TIMED_TEXT_TRACK_KIND tk = cue.GetCueKind();
                        if (tk == MF_TIMED_TEXT_TRACK_KIND.MF_TIMED_TEXT_TRACK_KIND_SUBTITLES)
                        {
                            string sTextCue = "";
                            uint nLineCount = cue.GetLineCount();
                            for (uint i = 0; i < nLineCount; i++)
                            {
                                IMFTimedTextFormattedText pTimedTextFormattedText = null;
                                hr = cue.GetLine(i, out pTimedTextFormattedText);
                                if (hr == HRESULT.S_OK && pTimedTextFormattedText != null)
                                {   
                                    string sText = string.Empty;
                                    hr = pTimedTextFormattedText.GetText(out sText);
                                    if (sTextCue == "")
                                        sTextCue = sText;
                                    else
                                    {
                                        sTextCue += Environment.NewLine;
                                        sTextCue += sText;
                                    }

                                    // Always 0...
                                    uint nsfCount = pTimedTextFormattedText.GetSubformattingCount();
                                    for (uint j = 0; j < nsfCount; j++)
                                    {
                                        uint nFirstChar = 0;
                                        uint nCharLength = 0;
                                        IMFTimedTextStyle pTimedTextStyleLine = null;
                                        hr = pTimedTextFormattedText.GetSubformatting(j, out nFirstChar, out nCharLength, out pTimedTextStyleLine);
                                        if (hr == HRESULT.S_OK)
                                        {
                                            // Code to get styles should be added here
                                            SafeRelease(ref pTimedTextStyleLine);
                                        }
                                    }
                                    SafeRelease(ref pTimedTextFormattedText);
                                }
                            }
                            m_sSubtitleText = sTextCue;

                            // Always null...
                            IMFTimedTextStyle pTimedTextStyle = null;
                            hr = cue.GetStyle(out pTimedTextStyle);
                            if (hr == HRESULT.S_OK && pTimedTextStyle != null)
                            {
                                bool bBold = false;
                                hr = pTimedTextStyle.GetBold(out bBold);
                                SafeRelease(ref pTimedTextStyle);
                            }

                            // Always null...
                            IMFTimedTextRegion pTimedTextRegion = null;
                            hr = cue.GetRegion(out pTimedTextRegion);
                            if (hr == HRESULT.S_OK && pTimedTextRegion != null)
                            {                               
                                string sRegionName = string.Empty;
                                hr = pTimedTextRegion.GetName(out sRegionName);
                                SafeRelease(ref pTimedTextRegion);
                            }

                            double nStartTime = cue.GetStartTime();
                            double nDuration = cue.GetDuration();

                            string sOriginalId = string.Empty;
                            uint nId = cue.GetId();                        
                            
                            //IntPtr pString = IntPtr.Zero;
                            //hr = cue.GetOriginalId(out pString);                           
                            //string sCue = Marshal.PtrToStringUni(pString);

                            // Empty...
                            //hr = cue.GetOriginalId(out sOriginalId);
                            uint nTrackId = cue.GetTrackId();

                            //IMFTimedTextBinary pTimedTextBinary = null;
                            //hr = cue.GetData(out pTimedTextBinary);
                        }
                        // to do...
                        else if(tk == MF_TIMED_TEXT_TRACK_KIND.MF_TIMED_TEXT_TRACK_KIND_CAPTIONS)
                        {
                            //...
                        }
                        else if (tk == MF_TIMED_TEXT_TRACK_KIND.MF_TIMED_TEXT_TRACK_KIND_METADATA)
                        {
                            //...
                        }
                        else if (tk == MF_TIMED_TEXT_TRACK_KIND.MF_TIMED_TEXT_TRACK_KIND_UNKNOWN)
                        {
                            //...
                        }
                    }
                    break;
                case MF_TIMED_TEXT_CUE_EVENT.MF_TIMED_TEXT_CUE_EVENT_INACTIVE:
                    m_sSubtitleText = "";
                    break;               
            }
            return;
        }

        public void Reset()
        {
            return;
        }
    }

    // Not used, seems useless...
    public class CMediaEngineSrcElements : IMFMediaEngineSrcElements, IDisposable
    {
        private System.Collections.ObjectModel.ObservableCollection<CSrcElements> listSrcElements = new System.Collections.ObjectModel.ObservableCollection<CSrcElements>();

        public class CSrcElements
        {
            public String pURL { get; set; }
            public String pType { get; set; }
            public String pMedia { get; set; }

            public CSrcElements(String pURL, String pType, String pMedia)
            {
                this.pURL = pURL;
                this.pType = pType;
                this.pMedia = pMedia;
            }
        }

        public uint GetLength()
        {
            return (uint)listSrcElements.Count;
        }

        public HRESULT GetURL(uint index, out string pURL)
        {
            HRESULT hr = HRESULT.S_OK;
            string sURL = listSrcElements[(int)index].pURL;
            pURL = sURL;
            return hr;
        }

        public HRESULT GetType(uint index, out string pType)
        {
            HRESULT hr = HRESULT.S_OK;
            string sType = listSrcElements[(int)index].pType;
            pType = sType;
            return hr;
        }

        public HRESULT GetMedia(uint index, out string pMedia)
        {
            HRESULT hr = HRESULT.S_OK;
            string sMedia = listSrcElements[(int)index].pMedia;
            pMedia = sMedia;
            return hr;
        }

        public HRESULT AddElement(string pURL, string pType, string pMedia)
        {
            HRESULT hr = HRESULT.S_OK;
            listSrcElements.Add(new CSrcElements(pURL, pType, pMedia));
            return hr;
        }

        public HRESULT RemoveAllElements()
        {
            HRESULT hr = HRESULT.S_OK;
            listSrcElements.Clear();
            return hr;
        }

        public void Dispose()
        {
            RemoveAllElements();
            listSrcElements = null;
        }
    }
}
