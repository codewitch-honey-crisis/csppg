using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace csppg
{
    static class Program
    {
		static readonly string CodeBase = _GetCodeBase();
		static readonly string Filename = Path.GetFileName(CodeBase);
		static readonly string Name = _GetName();
		static readonly Version Version = _GetVersion();
		static readonly string Description = _GetDescription();
		static int Main(string[] args) => Run(args, Console.In, Console.Out, Console.Error);
		/// <summary>
		/// Runs the process
		/// </summary>
		/// <param name="args">The arguments</param>
		/// <param name="stdin">The input stream</param>
		/// <param name="stdout">The output stream</param>
		/// <param name="stderr">The error stream</param>
		/// <returns>The error code</returns>
		public static int Run(string[] args, TextReader stdin, TextWriter stdout, TextWriter stderr)
		{
			string inputfile=null;
			bool @internal = false;
			string outputfile=null;
			string codeclass = null;
			string codemethod = null;
			string codenamespace=null;
			bool ifstale = false;
			// our working variables
			var result = 0;
			TextReader input = null;
			TextWriter output = null;
			try
			{
				if (0 == args.Length)
				{
					result = -1;
					_PrintUsage(stderr);

				}
				else if (args[0].StartsWith("/"))
				{
					throw new ArgumentException("Missing input file.");
				}
				else
				{
					// process the command line args
					inputfile = args[0];
					for (var i = 1; i < args.Length; ++i)
					{
						switch (args[i].ToLowerInvariant())
						{
							case "/output":
								if (args.Length - 1 == i) // check if we're at the end
									throw new ArgumentException(string.Format("The parameter \"{0}\" is missing an argument", args[i].Substring(1)));
								++i; // advance 
								outputfile = args[i];
								break;
						case "/method":
							if (args.Length - 1 == i) // check if we're at the end
								throw new ArgumentException(string.Format("The parameter \"{0}\" is missing an argument", args[i].Substring(1)));
							++i; // advance 
							codemethod = args[i];
							break;
						case "/class":
								if (args.Length - 1 == i) // check if we're at the end
									throw new ArgumentException(string.Format("The parameter \"{0}\" is missing an argument", args[i].Substring(1)));
								++i; // advance 
								codeclass = args[i];
								break;
							case "/namespace":
								if (args.Length - 1 == i) // check if we're at the end
									throw new ArgumentException(string.Format("The parameter \"{0}\" is missing an argument", args[i].Substring(1)));
								++i; // advance 
								codenamespace = args[i];
								break;
							case "/internal":
								@internal = true;
								break;
							case "/ifstale":
								ifstale = true;
								break;
							default:
								throw new ArgumentException(string.Format("Unknown switch {0}", args[i]));
						}
					}


					if (string.IsNullOrWhiteSpace(inputfile))
						throw new ArgumentException("inputfile");
					var cwd = Environment.CurrentDirectory;
					var iSearch = (string.IsNullOrWhiteSpace(outputfile)||!outputfile.Contains("*"))?-1:inputfile.IndexOfAny(new char[] { '*', '?' });
					string srch = null;
					var repl = outputfile;
					if(iSearch>-1) {
						var li = inputfile.LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
						if (li > iSearch) throw new ArgumentException("<inputfile> cannot use a directory search. is your path valid?"); 
						if(li>-1) {
							srch = inputfile.Substring(li + 1);
							inputfile = inputfile.Substring(0, li);
                        } else {
							srch = inputfile;
							inputfile = cwd;
							
                        }
                    }
					foreach (var ifile in (-1 < iSearch) ? Directory.GetFiles(inputfile, srch) : new string[] { inputfile }) {
						if(-1<iSearch) {
							outputfile = Path.Combine(inputfile,repl.Replace("*", Path.GetFileNameWithoutExtension(Path.GetFileName(ifile))));
                        }
						if (!ifstale || _IsStale(ifile, outputfile)) {
							if (null != outputfile) {
								stderr.WriteLine("{0} is building file: {1}", Name, outputfile);
								cwd = Path.GetDirectoryName(outputfile);
								output = new StreamWriter(outputfile);
							} else {
								stderr.WriteLine("{0} is building preprocessor.", Name);
								output = stdout;
							}
							var cls = codeclass;
							if (string.IsNullOrEmpty(cls)) {
								// default we want it to be named after the code file
								// otherwise we'll use inputfile
								if (null != outputfile)
									cls= Path.GetFileNameWithoutExtension(outputfile);
								else if(0>iSearch)
									cls = Path.GetFileNameWithoutExtension(ifile);
							} else {
								if (-1<iSearch && cls.Contains("*")) {
									cls = cls.Replace("*", Path.GetFileNameWithoutExtension(Path.GetFileName(ifile)));
                                }
                            }
							var mth = codemethod;
							if(!string.IsNullOrEmpty(mth)) {
								if(-1<iSearch && !string.IsNullOrEmpty(codeclass)) {
									mth= mth.Replace("*", Path.GetFileNameWithoutExtension(Path.GetFileName(ifile)));
								}
                            }
							input = new StreamReader(ifile);
							Preprocessor.Run(input, output, null, mth,cls, codenamespace, null, true, @internal);
						} else {
							stderr.WriteLine("{0} skipped building of {1} because it was not stale.", Name, outputfile);
						}
					}

				}
			}
			// we don't like to catch in debug mode
#if !DEBUG
			catch (Exception ex)
			{
				result = _ReportError(ex, stderr);
			}
#endif
			finally
			{
				// close the input file if necessary
				if (null != input)
					input.Close();
				// close the output file if necessary
				if (null != outputfile && null != output)
					output.Close();
			}
			
			return result;
		}
		static void _PrintUsage(TextWriter w)
		{
			w.Write("Usage: " + Filename + " ");
			w.WriteLine("<inputfile> [/output <outputfile>] [/class <codeclass>]");
			w.WriteLine("   [/namespace <codenamespace>] [/internal] [/ifstale]");
			w.WriteLine();

			w.Write(Name);
			w.Write(" ");
			w.Write(Version.ToString());
			if (!string.IsNullOrWhiteSpace(Description))
			{
				w.Write(" - ");
				w.WriteLine(Description);
			}
			else
			{
				w.WriteLine(" - <No description>");
			}
			w.WriteLine();
			w.WriteLine("   <inputfile>     The input template");
			w.WriteLine("   <outputfile>    The preprocessor source file - defaults to STDOUT");
			w.WriteLine("   <codemethod>    The name of the method to generate - defaults to Run");
			w.WriteLine("   <codeclass>     The name of the main class to generate - default derived from <outputfile>");
			w.WriteLine("   <codenamespace> The namespace to generate the code under - defaults to none");
			w.WriteLine("   <internal>      Mark the generated class as internal - defaults to public");
			w.WriteLine("   <ifstale>       Only generate if the input is newer than the output");
			w.WriteLine();
		}
		static bool _IsStale(string inputfile, string outputfile)
		{
			if (string.IsNullOrEmpty(outputfile) || string.IsNullOrEmpty(inputfile))
				return true;
			var result = true;
			// File.Exists doesn't always work right
			try
			{
				if (File.GetLastWriteTimeUtc(outputfile) >= File.GetLastWriteTimeUtc(inputfile))
					result = false;
			}
			catch { }
			return result;
		}
		static string _GetCodeBase()
		{
			try { return Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName; }
			catch { return Path.Combine(Environment.CurrentDirectory, typeof(Program).Namespace+".exe"); }
		}
		static string _GetName()
		{
			try
			{
				foreach (var attr in Assembly.GetExecutingAssembly().CustomAttributes)
				{
					if (typeof(AssemblyTitleAttribute) == attr.AttributeType)
					{
						return attr.ConstructorArguments[0].Value as string;
					}
				}
			}
			catch { }
			return Path.GetFileNameWithoutExtension(Filename);
		}
		static Version _GetVersion()
		{
			return Assembly.GetExecutingAssembly().GetName().Version;
		}
		static string _GetDescription()
		{
			string result = null;
			foreach (Attribute ca in Assembly.GetExecutingAssembly().GetCustomAttributes())
			{
				var ada = ca as AssemblyDescriptionAttribute;
				if (null != ada && !string.IsNullOrWhiteSpace(ada.Description))
				{
					result = ada.Description;
					break;
				}
			}
			return result;
		}

		// do our error handling here (release builds)
		static int _ReportError(Exception ex, TextWriter stderr)
		{
			_PrintUsage(stderr);
			stderr.WriteLine("Error: {0}", ex.Message);
			return -1;
		}
	}
}
