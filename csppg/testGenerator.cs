using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
#line hidden
namespace Testing {
    internal partial class Generator {
        public static void test(TextWriter Response, IDictionary<string, object> Arguments, string docTemplate, bool @private, string returnTemplate, string methodName, string parametersTemplate) {
            #line 5 "C:\Users\gazto\source\repos\csppg\csppg\test.template"

dynamic a = Arguments;
var indentPart = "    ";
string indent;
if(""!=(string)a.@namespace) {
	indent = indentPart + indentPart;
} else {
	indent = indentPart;
}
Generate(docTemplate,Arguments,Response,(""!=(string)a.@namespace)?2:1);
            #line 14 "C:\Users\gazto\source\repos\csppg\csppg\test.template"
            Response.Write(indent);
            #line 14 "C:\Users\gazto\source\repos\csppg\csppg\test.template"
            Response.Write(!@private?"public ":"");
            #line 14 "C:\Users\gazto\source\repos\csppg\csppg\test.template"

Generate(returnTemplate,Arguments,Response);
            #line 15 "C:\Users\gazto\source\repos\csppg\csppg\test.template"
            Response.Write(" ");
            #line 15 "C:\Users\gazto\source\repos\csppg\csppg\test.template"
            Response.Write(methodName);
            #line 15 "C:\Users\gazto\source\repos\csppg\csppg\test.template"
            Response.Write("(");
            #line 15 "C:\Users\gazto\source\repos\csppg\csppg\test.template"

Generate(parametersTemplate,Arguments,Response);
            #line 16 "C:\Users\gazto\source\repos\csppg\csppg\test.template"
            Response.Write(") {\r\n");
            Response.Flush();
        }
    }
}
