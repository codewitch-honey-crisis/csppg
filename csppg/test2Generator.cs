using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
namespace Testing {
    internal partial class Generator {
        public static void test2(TextWriter Response, IDictionary<string, object> Arguments) {
            Response.Write("// reads the next UTF32 codepoint off an enumerator\r\nconst string _UnicodeSurrogateError = \"Invalid surrogate found in Unicode stream\";\r\nstatic int _ReadUtf32(System.Collections.Generic.IEnumerator<char> cursor, out int adv) {\r\n    adv = 0;\r\n    if(!cursor.MoveNext()) return -1;\r\n    ++adv;\r\n    var chh = cursor.Current;\r\n    int result = chh;\r\n    if(char.IsHighSurrogate(chh)) {\r\n        if(!cursor.MoveNext()) throw new System.IO.IOException(_UnicodeSurrogateError);\r\n        ++adv;\r\n        var chl = cursor.Current;\r\n        if(!char.IsLowSurrogate(chl)) throw new System.IO.IOException(_UnicodeSurrogateError);\r\n        result = char.ConvertToUtf32(chh,chl);\r\n    }\r\n    return result;\r\n}\r\n// reads the next UTF32 codepoint off a text reader\r\nstatic int _ReadUtf32(System.IO.TextReader reader, out int adv) {\r\n    adv=0;\r\n    var result = reader.Read();\r\n    if (-1 != result) {\r\n        ++adv;\r\n        if (char.IsHighSurrogate(unchecked((char)result))) {\r\n            var chl = reader.Read();\r\n            if (-1 =");
            Response.Write("= chl) throw new System.IO.IOException(_UnicodeSurrogateError);\r\n            ++adv;\r\n            if (!char.IsLowSurrogate(unchecked((char)chl))) throw new System.IO.IOException(_UnicodeSurrogateError);\r\n            result = char.ConvertToUtf32(unchecked((char)result), unchecked((char)chl));\r\n        }\r\n    }\r\n    return result;\r\n}\r\n");
            Response.Flush();
        }
    }
}
