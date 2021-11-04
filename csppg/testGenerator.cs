using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
namespace Testing {
    internal partial class Generator {
        public static void test(TextWriter Response, IDictionary<string, object> Arguments, string docTemplate, bool @private, string returnTemplate, string methodName, string parametersTemplate) {

dynamic a = Arguments;
var indentPart = "    ";
string indent;
if(""!=(string)a.@namespace) {
	indent = indentPart + indentPart;
} else {
	indent = indentPart;
}
Generate(docTemplate,Arguments,Response,(""!=(string)a.@namespace)?2:1);
            Response.Write(indent);
            Response.Write(!@private?"public ":"");
Generate(returnTemplate,Arguments,Response);
            Response.Write(" ");
            Response.Write(methodName);
            Response.Write("(");
Generate(parametersTemplate,Arguments,Response);
            Response.Write(") {\r\n");
            Response.Flush();
        }
    }
}
