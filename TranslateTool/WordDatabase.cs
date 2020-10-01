using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Windows.Media.Animation;

namespace TranslateTool
{
    public abstract class Word
    {
        public static Dictionary<int, Word> WordPointers = new Dictionary<int, Word>();

        public static int ReadInt(FileStream inFile, byte[] buffer)
        {
            inFile.Read(buffer, 0, 4);

            return BitConverter.ToInt32(buffer, 0);
        }

        public static Word Deserialise(FileStream inFile, FileStream inFileValues, byte[] buffer)
        {
            int type;

            inFile.Read(buffer, 0, 1);
            type = buffer[0];

            if (type == 1 || type == 3)
            {
                return Kanji.Deserialise(inFile, inFileValues, type, buffer);
            }
            else if (type == 2 || type == 4)
            {
                return Kana.Deserialise(inFile, inFileValues, type, buffer);
            }
            else
            {
                throw new Exception("Invalid type.");
            }
        }

        public int Pointer = -1;
        public bool Defined = false;
        public SerialisedString Value;
        public List<SerialisedString> Meanings = new List<SerialisedString>();

        public Word()
        {
        }

        public Word(string value, FileStream outFileValues)
        {
            Value = new SerialisedString(value, outFileValues);
        }

        public abstract void Serialise(FileStream outFile);

        public override string ToString()
        {
            return Value.Text;
        }
    }
    public class Kanji : Word
    {

        public static Kanji Deserialise(FileStream inFile, FileStream inFileValues, int type, byte[] buffer)
        {
            var pointer = Word.ReadInt(inFile, buffer);
            Kanji result;

            if (Word.WordPointers.TryGetValue(pointer, out Word word))
            {
                result = word as Kanji;

                if (result.Defined)
                {
                    return result;
                }
            }
            else
            {
                result = new Kanji(pointer);
            }
            if (type == 1)
            {
                int meaningLength;
                int readingLength;

                result.Value = SerialisedString.Deserialise(inFile, inFileValues, buffer);
                inFile.Read(buffer, 0, 2);
                meaningLength = BitConverter.ToInt16(buffer, 0);

                for (var i = 0; i < meaningLength; i++)
                {
                    result.Meanings.Add(SerialisedString.Deserialise(inFile, inFileValues, buffer));
                }
                inFile.Read(buffer, 0, 2);
                readingLength = BitConverter.ToInt16(buffer, 0);

                for (var i = 0; i < readingLength; i++)
                {
                    result.ReadingList.Add(Word.Deserialise(inFile, inFileValues, buffer) as Kana);
                }
                result.Defined = true;

                return result;
            }
            return result;
        }

        public Dictionary<string, Kana> Readings;
        public List<Kana> ReadingList;

        public Kanji(string value, FileStream outFileValues) : base(value, outFileValues)
        {
            Readings = new Dictionary<string, Kana>();
        }

        public Kanji(int pointer)
        {
            ReadingList = new List<Kana>();
            WordPointers[pointer] = this;
            Pointer = pointer;
        }

        public override void Serialise(FileStream outFile)
        {
            if (Pointer == -1)
            {
                Pointer = (int)outFile.Length;

                var pointerBytes = BitConverter.GetBytes(Pointer);
                var typeByte = new byte[] { 1 };
                var meaningLengthBytes = BitConverter.GetBytes((short)Meanings.Count);
                var readingLengthBytes = BitConverter.GetBytes((short)Readings.Count);

                outFile.Write(typeByte, 0, 1);
                outFile.Write(pointerBytes, 0, pointerBytes.Length);
                Value.Serialise(outFile);
                outFile.Write(meaningLengthBytes, 0, meaningLengthBytes.Length);

                foreach (var meaning in Meanings)
                {
                    meaning.Serialise(outFile);
                }
                outFile.Write(readingLengthBytes, 0, readingLengthBytes.Length);

                foreach (var reading in Readings.Values)
                {
                    reading.Serialise(outFile);
                }
            }
            else
            {
                var typeByte = new byte[] { 3 };
                var pointerBytes = BitConverter.GetBytes(Pointer);

                outFile.Write(typeByte, 0, 1);
                outFile.Write(pointerBytes, 0, 4);
            }
        }
    }
    public class Kana : Word
    {
        public static Kana Deserialise(FileStream inFile, FileStream inFileValues, int type, byte[] buffer)
        {
            var pointer = Word.ReadInt(inFile, buffer);
            Kana result;

            if (Word.WordPointers.TryGetValue(pointer, out Word word))
            {
                result = word as Kana;

                if (result.Defined)
                {
                    return result;
                }
            }
            else
            {
                result = new Kana(pointer);
            }
            if (type == 2)
            {
                int meaningLength;

                result.Value = SerialisedString.Deserialise(inFile, inFileValues, buffer);
                inFile.Read(buffer, 0, 2);
                meaningLength = BitConverter.ToInt16(buffer, 0);

                for (var i = 0; i < meaningLength; i++)
                {
                    result.Meanings.Add(SerialisedString.Deserialise(inFile, inFileValues, buffer));
                }
                result.Defined = true;

                return result;
            }
            return result;
        }

        public Kana(string value, FileStream outFileValues) : base(value, outFileValues) { }

        public Kana(int pointer)
        {
            WordPointers[pointer] = this;
        }

        public override void Serialise(FileStream outFile)
        {
            if (Pointer == -1)
            {
                Pointer = (int)outFile.Length;

                var pointerBytes = BitConverter.GetBytes(Pointer);
                var typeByte = new byte[] { 2 };
                var meaningLengthBytes = BitConverter.GetBytes((short)Meanings.Count);

                outFile.Write(typeByte, 0, 1);
                outFile.Write(pointerBytes, 0, pointerBytes.Length);
                Value.Serialise(outFile);
                outFile.Write(meaningLengthBytes, 0, meaningLengthBytes.Length);

                foreach (var meaning in Meanings)
                {
                    meaning.Serialise(outFile);
                }
            }
            else
            {
                var typeByte = new byte[] { 4 };
                var pointerBytes = BitConverter.GetBytes(Pointer);

                outFile.Write(typeByte, 0, 1);
                outFile.Write(pointerBytes, 0, 4);
            }
        }
    }
    public class SerialisedString
    {
        public static Dictionary<int, SerialisedString> LoadedPointers = new Dictionary<int, SerialisedString>();
        public static List<string> Values = new List<string>();

        public static SerialisedString Deserialise(FileStream inFile, FileStream inFileValues, byte[] buffer)
        {
            inFile.Read(buffer, 0, 4);

            var pointer = BitConverter.ToInt32(buffer, 0);

            if (LoadedPointers.TryGetValue(pointer, out SerialisedString serialisedString))
            {
                return serialisedString;
            }
            inFileValues.Seek(pointer, SeekOrigin.Begin);
            inFileValues.Read(buffer, 0, 2);

            var length = BitConverter.ToInt16(buffer, 0);
            var result = new SerialisedString(pointer);
            var stringBuffer = new byte[length];

            inFileValues.Read(stringBuffer, 0, length);
            result.ID = SerialisedString.Values.Count;
            result.Pointer = pointer;
            SerialisedString.Values.Add(Encoding.UTF8.GetString(stringBuffer));

            return result;
        }

        public int ID;
        public int Pointer;

        public SerialisedString(int pointer)
        {
            Pointer = pointer;
        }

        public SerialisedString(string content, FileStream outFileValues)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var bytesLength = (short)Math.Min(bytes.Length, 400);
            var bytesLengthBytes = BitConverter.GetBytes(bytesLength);

            Pointer = (int)outFileValues.Length;
            outFileValues.Write(bytesLengthBytes, 0, 2);
            outFileValues.Write(bytes, 0, bytesLength);
        }

        public string Text
        {
            get
            {
                return SerialisedString.Values[ID];
            }
        }

        public void Serialise(FileStream outFile)
        {
            var pointerBytes = BitConverter.GetBytes(Pointer);

            outFile.Write(pointerBytes, 0, 4);
        }

        public override string ToString()
        {
            return Text;
        }
    }
    public static class WordDatabase
    {

        public static List<Word> Deserialise()
        {
            var inFile = new FileStream("WordRelations.bindb", FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 8);
            var inFileValues = new FileStream("WordContent.bindb", FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 8, FileOptions.RandomAccess);
            var inFileSortedPointers = new FileStream("WordStructure.bindb", FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 8);
            var result = new Dictionary<int, List<Word>>();
            var buffer = new byte[400];

            while(inFile.Position < inFile.Length)
            {
                Word.Deserialise(inFile, inFileValues, buffer);
            }
            for (var i = 0; i < 21; i++)
            {
                int length;

                result[i] = new List<Word>();
                inFileSortedPointers.Read(buffer, 0, 4);
                length = BitConverter.ToInt32(buffer, 0);

                for(var j = 0; j < length; j++)
                {
                    result[i].Add(Word.Deserialise(inFileSortedPointers, inFileValues, buffer));
                }
            }
            var plainDatabase = new List<Word>();

            for(var i = 0; i< 21; i++)
            {
                foreach(var w in result[i])
                {
                    plainDatabase.Add(w);
                }
            }

            return plainDatabase;
        }
    }
}
