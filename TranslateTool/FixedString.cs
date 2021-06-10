using System;
using System.Text;

namespace TranslateTool
{
    public unsafe struct FixedString
    {
        public static implicit operator string(FixedString value)
        {
            return Encoding.Unicode.GetString((byte*)value.value, value.length);
        }
        public static implicit operator FixedString(string value)
        {
            return new FixedString(value);
        }
        private short length;
        private fixed char value[1028];

        public FixedString(string value)
        {
            if(value.Length > 1028)
            {
                throw new OverflowException("Input value is too large.");
            }
            for(var i = 0; i < value.Length; i++)
            {
                this.value[i] = value[i];
            }
            length = (short)(value.Length*sizeof(char));
        }
    }
}
