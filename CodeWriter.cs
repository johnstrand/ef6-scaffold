using System;
using System.IO;

namespace ScaffoldEF
{
    class CodeWriter : IDisposable
    {
        readonly StreamWriter writer;
        int indent;
        internal CodeWriter(string filename)
        {
            writer = new StreamWriter(filename);
        }
        public void WriteText(string text)
        {
            WriteIndent();
            writer.WriteLine(text);
        }

        public void BeginBlock(string preamble)
        {
            WriteIndent();
            writer.WriteLine(preamble);
            WriteIndent();
            writer.WriteLine("{");
            indent++;
        }

        public void EndBlock()
        {
            indent--;
            WriteIndent();
            writer.WriteLine("}");
        }

        public void WriteIndent()
        {
            writer.Write(new string('\t', indent));
        }

        public void Indent()
        {
            indent++;
        }

        public void Deindent()
        {
            indent--;
        }

        public void WriteLine(string text = "")
        {
            writer.WriteLine(text);
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
}
