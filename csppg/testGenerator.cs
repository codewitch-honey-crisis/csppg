using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
namespace Testing {
    internal partial class Generator {
        public static void test(TextWriter Response, IDictionary<string, object> Arguments) {

// for easy access
dynamic a = Arguments;

// set our value types (used for parsing)
a.input = "";
a.output = "";
a.@class = "";
a.@namespace = "";
a.token = "";
a.ignorecase = false;
a.dot = false;
a.jpg = false;
a.tables = false;
a.lexer = false;
a.lines = false;
a.target = "";
a.ifstale = false;
a.textreader = false;

var requiredArgs = new HashSet<string>();
requiredArgs.Add("input");
var failed = false;
a._exception = null;
try {
    // parse input arguments
	CrackArguments("input",(string[])a._args,requiredArgs,Arguments);

    // defaults and validation
    if((string)a.output==(string)a.input) throw new ArgumentException("<input> and <output> indicated the same file.");
    if(""==(string)a.output && (bool)a.ifstale) {
            Response.Write("Warning: /ifstale will be ignored because /output was not specified\r\n");

    }
    a.input = Path.GetFullPath((string)a.input);
    if(""!=(string)a.output) {
        a.output = Path.GetFullPath((string)a.output);
    }
    var target = ((string)a.target).ToLowerInvariant();
    if(target == "csharp" || target == "c#")
        target = "cs";
    if(string.IsNullOrEmpty(target)) {
        if(!string.IsNullOrEmpty((string)a.output)) {
            target = Path.GetExtension((string)a.output);
            if(!string.IsNullOrEmpty(target) && target[0]=='.')
                target=target.Substring(1);
        } 
        if(string.IsNullOrEmpty(target)) {
            target = "cs";
        }
        a.target = target;
    }
    if(!IsValidTarget(Arguments))
        throw new NotSupportedException(string.Format("Unsupported target {0} was indicated",a.target));

    if(target=="rgg" && (bool)a.ignorecase) {
            Response.Write("Warning: /ignorecase will be ignored since a Reggie binary is being used.\r\n");

    }
    if(""==(string)a.@class) {
        if(""!=(string)a.output) {
            a.@class = Path.GetFileNameWithoutExtension((string)a.output);
        } else {
            a.@class = Path.GetFileNameWithoutExtension((string)a.input);
        }
    }
    if(""!=(string)a.output) {
        a._cwd=Path.GetDirectoryName((string)a.output);
    }
}
catch(Exception ex) {
    failed = true;
    a._exception = ex;
    goto print_usage;
}
if(!failed) {
            Response.Write(a._name);
            Response.Write(" ");
            Response.Write(a._version);
            Response.Write(" ");

if(!(bool)a.ifstale || IsStale((string)a.input,(string)a.output)) {
            Response.Write("preparing to generate ");
            Response.Write(a.output);
            Response.Write("\r\n");

    var isBinary  = IsBinaryInputFile(Arguments);
    var isLexer = false;
    if(isBinary && "rgl"==(string)a._fourcc)
        isLexer = true;
    if(!(bool)a.lexer && isBinary && isLexer) {
            Response.Write("Warning: The input file is a binary lexer. A lexer will be generated.\r\n");

        a.lexer = true;
    }
    if(!(bool)a.lexer && (bool)a.lines) {
            Response.Write("Warning: /lines will be ignored because /lexer was not specified\r\n");

    }
    if(!isBinary) {
            Response.Write("Parsing input and computing tables...\r\n");

    }
    LoadInputFile(Arguments);
    if(!isBinary) {
            Response.Write("Finished crunching input. Building output...\r\n");

    } else {
            Response.Write("Loaded state tables. Building output...\r\n");

    }

    if((bool)a.dot || (bool)a.jpg) {
        if((bool)a.lexer) {
            var fa = F.FFA.FromDfaTable((int[])a._dfa);
            if((bool)a.dot) {
                var opts = new F.FFA.DotGraphOptions();
                var fn = Path.Combine((string)a._cwd, ((string)a.@class) + ".dot");
            Response.Write("Writing ");
            Response.Write(fn);
            Response.Write("...\r\n");

                using(var sw=new StreamWriter(fn)) {
                    fa.WriteDotTo(sw,opts);
                }        
            }
            if((bool)a.jpg) {
                var opts = new F.FFA.DotGraphOptions();
                var fn = Path.Combine((string)a._cwd, ((string)a.@class) + ".jpg");
            Response.Write("Writing ");
            Response.Write(fn);
            Response.Write("...\r\n");

                fa.RenderToFile(fn,opts);
            }
        } else { // if((bool)a.lexer) ...
            var st = (string[])a._symbolTable;
            for(var i = 0;i<st.Length;++i) {
                var sti = st[i];
                if(sti != null && sti.Length > 0) {
                    var fa = F.FFA.FromDfaTable(((int[][])a._dfas)[i]);
                    var s = st[i];
                    if((bool)a.dot) {
                        var opts = new F.FFA.DotGraphOptions();
                        var fn = Path.Combine((string)a._cwd, s + ".dot");
            Response.Write("Writing ");
            Response.Write(fn);
            Response.Write("...\r\n");

                        using(var sw=new StreamWriter(fn)) {
                            fa.WriteDotTo(sw,opts);
                        }        
                    } // if((bool)a.dot) ...
                    if((bool)a.jpg) {
                        var opts = new F.FFA.DotGraphOptions();
                        var fn = Path.Combine((string)a._cwd, s + ".jpg");
            Response.Write("Writing ");
            Response.Write(fn);
            Response.Write("...\r\n");

                        fa.RenderToFile(fn,opts);
                    } // if((bool)a.jpg) ...
                } // if(sti != null) ...
            } // for(var i ...
        } // if((bool)a.lexer) ...
    } // if((bool)a.dot || (bool)a.jpg)
    TextWriter ost = null;
    try {
        bool isfile;
        ost=(isfile=(""!=(string)a.output))?new StreamWriter(Path.Combine((string)a._cwd,(string)a.output)):(TextWriter)a._stdout;
        var torun = ((string)a.target)+"TargetGenerator";
        if(isfile) {
            // truncate it because reasons
            ((StreamWriter)ost).BaseStream.SetLength(0L);
        }
        Generate(Arguments,ost);
            Response.Write("Generation of ");
            Response.Write(""!=(string)a.output?(string)a.output:"output");
            Response.Write(" complete.\r\n");

    }
    finally {
        if(null!=ost && ((TextWriter)a._stdout)!=ost) {
            ost.Close();
        }
    }
} else {
            Response.Write("skipping ");
            Response.Write(a.output);
            Response.Write(" because it is not stale.");

}
} //if(!failed) ... 
            Response.Write("\r\n");
return;
print_usage:

            Response.Write("Usage: ");
            Response.Write(a._exe);
            Response.Write(" <input> [/output <output>] [/class <class>]\r\n   [/namespace <namespace>] [/token <token>] [/textreader] \r\n   [/tables] [/lexer] [/target <target>] [/lines] [/ignorecase]\r\n   [/dot] [/jpg] [/ifstale]\r\n            \r\n");
            Response.Write(a._name);
            Response.Write(" ");
            Response.Write(a._version);
            Response.Write(" - ");
            Response.Write(a._description);
            Response.Write("\r\n\r\n   <input>      The input specification file\r\n   <output>     The output source file - defaults to STDOUT\r\n   <class>      The name of the main class to generate \r\n       - default derived from <output>\r\n   <namespace>  The name of the namespace or database to use \r\n       - defaults to nothing\r\n   <token>      The fully qualified name of an external token \r\n        - defaults to a tuple\r\n   <textreader> Generate TextReader instead of IEnumerable<char>\r\n                - C#/cs target only\r\n   <tables>     Generate DFA table code - defaults to compiled\r\n   <lexer>      Generate a lexer instead of matcher functions\r\n   <lines>      Generate line counting code\r\n        - defaults to non-line counted, only used with /lexer\r\n   <ignorecase> Generate case insensitive matches by default\r\n        - defaults to case sensitive\r\n   <target>     The output target to generate for\r\n       - default derived from <output> or \"cs\"\r\n       Supported targets: ");
            Response.Write(string.Join(", ",SupportedTargets));
            Response.Write("\r\n   <dot>        Generate .dot files for the state machine(s)\r\n   <jpg>        Generate .jpg files for the state machine(s)\r\n       - requires GraphViz\r\n   <ifstale>    Skip unless <output> is newer than <input>\r\n      \r\n");
if(a._exception!=null) {
            Response.Write(((Exception)a._exception).Message );
}

            Response.Write("\r\n");
            Response.Flush();
        }
    }
}
