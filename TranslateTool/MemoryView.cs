using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace TranslateTool
{
    public unsafe class MemoryView
    {
        private byte* rawView;
        private int index;
        private int length;

        public MemoryView(int size)
        {
            rawView = (byte*)Marshal.AllocHGlobal(size);
            index = 0;
            length = size;
        }

        public void SetByteIndex(int index)
        {
            this.index = index;
        }

        public byte[] Read(int count)
        {
            var result = new byte[count];

            Marshal.Copy((IntPtr)(rawView + index), result, 0, count);
            index += count;

            return result;
        }

        public byte[] ReadAt(int count, int index)
        {
            var result = new byte[count];

            Marshal.Copy((IntPtr)(rawView + index), result, 0, count);

            return result;
        }

        public void WriteAt(byte[] source, int index)
        {
            Marshal.Copy(source, 0, (IntPtr)(rawView + index), source.Length);
        }

        public void WriteAt(byte[] source, int sourceStart, int count, int index)
        {
            Marshal.Copy(source, sourceStart, (IntPtr)(rawView + index), count);
        }

        public void Write(byte[] source)
        {
            Marshal.Copy(source, 0, (IntPtr)(rawView + index), source.Length);
            index += source.Length;
        }

        public void Write(byte[] source, int sourceStart, int count)
        {
            Marshal.Copy(source, sourceStart, (IntPtr)(rawView + index), count);
            index += source.Length;
        }

        public T ReadValue<T>()
            where T:unmanaged
        {
            var result = *(T*)(rawView + index);

            index += sizeof(T);

            return result;
        }

        public T ReadValueAt<T>(int index)
            where T:unmanaged
        {
            return *(T*)(rawView + index);
        }

        public void WriteValue<T>(T value)
            where T:unmanaged
        {
            ((T*)rawView)[index] = value;
            index += sizeof(T);
        }

        public void WriteValueAt<T>(T value, int index)
            where T : unmanaged
        {
            ((T*)rawView)[index] = value;
        }
    }
}
