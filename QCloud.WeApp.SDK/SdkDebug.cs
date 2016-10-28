using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    internal static class SdkDebug
    {
#if DEBUG
        internal class IndentContext : IDisposable
        {
            public IndentContext()
            {
                Debug.Indent();
            }
            public void Dispose()
            {
                Debug.Unindent();
            }
        }
#else
        internal class IndentContext : IDisposable {
            public IndentContext() { }
            public void Dispose() { }
        }
#endif

        public static IndentContext Indent()
        {
            return new IndentContext();
        }

        public static void WriteLine(object content, bool outputOrderMark = false)
        {
#if DEBUG
            if (content == null)
            {
                Debug.WriteLine(null);
            }            
            if (content is string) {
                foreach (var line in (content as string).Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)) {
                    Debug.WriteLine(line);
                };
            }
            else
            {
                Debug.WriteLine(content);
            }
#endif
        }

        public static IndentContext WriteLineAndIndent(string content)
        {
            WriteLine(content);
            return Indent();
        }
    }
}
