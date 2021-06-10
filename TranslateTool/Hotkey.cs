using WinKey = System.Windows.Forms.Keys;

namespace TranslateTool
{
    public struct Hotkey
    {
        public static bool operator ==(Hotkey left, Hotkey right)
        {
            return left.Key == right.Key && left.Modifier == right.Modifier;
        }
        public static bool operator !=(Hotkey left, Hotkey right)
        {
            return left.Key != right.Key && left.Modifier != right.Modifier;
        }
        public KeyModifier Modifier;
        public WinKey Key;

        public Hotkey(KeyModifier modifier, WinKey key)
        {
            Modifier = modifier;
            Key = key;
        }

        public override bool Equals(object obj)
        {
            return obj is Hotkey hotkey &&
                   Modifier == hotkey.Modifier &&
                   Key == hotkey.Key;
        }

        public override int GetHashCode()
        {
            var hashCode = 503722236;
            hashCode = hashCode * -1521134295 + Modifier.GetHashCode();
            hashCode = hashCode * -1521134295 + Key.GetHashCode();
            return hashCode;
        }
    }
}
