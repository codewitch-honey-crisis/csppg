using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
internal partial class Foo {
    public static void testRun(TextWriter Response, IDictionary<string, object> Arguments) {
        Response.Write("<");
//%%
        Response.Write("\r\nvar rules = (IList<LexRule>)Arguments[\"rules\"];\r\nvar ignoreCase = (bool)Arguments[\"ignorecase\"];\r\nvar inputFile = (string)Arguments[\"inputfile\"];\r\nvar outputFile = (string)Arguments[\"outputfile\"];\r\nvar stderr = (TextWriter)Arguments[\"stderr\"];\r\nvar dot = (bool)Arguments[\"dot\"];\r\nvar jpg = (bool)Arguments[\"jpg\"];\r\nvar cwd = Path.GetDirectoryName(outputFile!=null?outputFile:inputFile);\r\nvar blockEnds = BuildBlockEnds(rules,inputFile,ignoreCase);\r\n%>\r\nTRUNCATE TABLE [dbo].[");
        Response.Write(codeclass);
        Response.Write("SymbolData]\r\nTRUNCATE TABLE [dbo].[");
        Response.Write(codeclass);
        Response.Write("StateTransition]\r\nTRUNCATE TABLE [dbo].[");
        Response.Write(codeclass);
        Response.Write("State]\r\nGO");

Run("SqlTableMatcherFillerGenerator",arguments,Response);

        Response.Write("\r\n");
        Response.Flush();
    }
}
