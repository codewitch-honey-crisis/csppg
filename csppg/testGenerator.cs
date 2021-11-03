using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Reggie;
using F;
using LC;
namespace Testing {
    internal partial class testGenerator {
        public static void Run(TextWriter Response, IDictionary<string, object> Arguments, string foo, int bar) {
            Response.Write("\r\n");

var rules = (IList<LexRule>)Arguments["rules"];
var ignoreCase = (bool)Arguments["ignorecase"];
var inputFile = (string)Arguments["inputfile"];
var outputFile = (string)Arguments["outputfile"];
var stderr = (TextWriter)Arguments["stderr"];
var dot = (bool)Arguments["dot"];
var jpg = (bool)Arguments["jpg"];
var cwd = Path.GetDirectoryName(outputFile!=null?outputFile:inputFile);
var blockEnds = BuildBlockEnds(rules,inputFile,ignoreCase);

            Response.Write("TRUNCATE TABLE [dbo].[");
            Response.Write(codeclass);
            Response.Write("SymbolData]\r\nTRUNCATE TABLE [dbo].[");
            Response.Write(codeclass);
            Response.Write("StateTransition]\r\nTRUNCATE TABLE [dbo].[");
            Response.Write(codeclass);
            Response.Write("State]\r\nGO");
            Response.Write("\r\n");

Run("SqlTableMatcherFillerGenerator",arguments,Response);

            Response.Write("\r\n");
            Response.Flush();
        }
    }
}
