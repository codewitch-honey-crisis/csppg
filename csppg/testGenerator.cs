using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
namespace Testing {
    internal partial class Generator {
        public static void test(TextWriter Response, IDictionary<string, object> Arguments, string text) {
            Response.Write("// ");
            Response.Write(text);
            Response.Write("\r\n");
            Response.Flush();
        }
    }
}
