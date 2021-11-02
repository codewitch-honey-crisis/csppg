using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
internal partial class Foo {
    public static void test2Run(TextWriter Response, IDictionary<string, object> Arguments) {

var rules = (IList<LexRule>)Arguments["rules"];
var ignoreCase = (bool)Arguments["ignorecase"];
var inputFile = (string)Arguments["inputfile"];
var outputFile = (string)Arguments["outputfile"];
var stderr = (TextWriter)Arguments["stderr"];
var dot = (bool)Arguments["dot"];
var jpg = (bool)Arguments["jpg"];
var cwd = Path.GetDirectoryName(outputFile!=null?outputFile:inputFile);
var blockEnds = BuildBlockEnds(rules,inputFile,ignoreCase);

        Response.Write("\r\nTRUNCATE TABLE [dbo].[");
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
