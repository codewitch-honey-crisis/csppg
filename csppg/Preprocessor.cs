using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;

namespace csppg
{
    class Preprocessor
    {
		public static void Run(TextReader input, TextWriter output, IDictionary<string, object> args = null, string codemethod = "Run", string codeclass="Preprocessor", string codenamespace=null, TextReader codebehind = null, bool generatePreprocessor=false,bool @internal = false)
        {
			if (string.IsNullOrWhiteSpace(codeclass))
				codeclass = "Preprocessor";
			if (string.IsNullOrWhiteSpace(codemethod))
				codeclass = "Run";
			if (string.IsNullOrWhiteSpace(codenamespace))
				codenamespace = null;
			var frameworkPath = RuntimeEnvironment.GetRuntimeDirectory();
			var cwd = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			try
			{
				Directory.CreateDirectory(Path.Combine(cwd, "temp"));
			}
			catch
            {

            }
			if (!generatePreprocessor)
			{
				cwd = Path.Combine(cwd, "temp");
				foreach (var file in Directory.GetFiles(cwd))
				{
					try
					{
						File.Delete(file);
					}
					catch { }
				}
			}
			var sb = new StringBuilder();
			var sw = (!generatePreprocessor)?new StringWriter(sb):output;
			sw.WriteLine("using System;");
			sw.WriteLine("using System.IO;");
			sw.WriteLine("using System.Text;");
			sw.WriteLine("using System.Collections.Generic;");
			if(codenamespace!=null)
            {
				sw.Write("namespace {0} ", codenamespace);
				sw.WriteLine("{");
            }
			sw.Write("{0}{2} partial class {1} ",codenamespace!=null?"    ":"", codeclass,@internal && generatePreprocessor?"internal":"public");
			sw.WriteLine("{");
			if (codenamespace != null) sw.Write("    ");
			sw.Write("    public static void {0}(TextWriter Response, IDictionary<string, object> Arguments) ",codemethod);
			sw.WriteLine("{");
			int cur;
			var more = true;
			var cch = '\0'; 
			while (more)
			{
				char ccch;
				var text = _ReadUntilStartContext(input,out ccch);
				if (cch == 0) cch = ccch; else if (ccch != cch && 0!=ccch) throw new InvalidOperationException("Invalid mixing and matching of context switches");
				if (0 < text.Length)
				{
					var srl = new StringReader(text);
					char[] buf = new char[1024];
					var r = srl.Read(buf, 0, buf.Length);
					while (0!=r)
					{
						if (codenamespace != null) sw.Write("    ");
						sw.Write("        Response.Write(");
						var s = _ToStringLiteral(buf,r);
						sw.Write(s); 
						sw.WriteLine(");");
						r = srl.Read(buf, 0, buf.Length);
					}
				}
				cur = input.Read();
				if (-1 == cur)
					more = false;
				else if ('=' == cur)
				{
					if (codenamespace != null) sw.Write("    ");
					sw.Write("        Response.Write(");
					sw.Write(_ReadUntilEndContext(-1, input, cch));
					sw.WriteLine(");");
				}
				else
					sw.WriteLine(_ReadUntilEndContext(cur, input,cch));
			}
			if (codenamespace != null) sw.Write("    ");
			sw.WriteLine("        Response.Flush();");
			if (codenamespace != null) sw.Write("    ");
			sw.WriteLine("    }");
			if (codenamespace != null) sw.Write("    ");
			sw.WriteLine("}");
			if(codenamespace!=null) sw.WriteLine("}");
			sw.Flush();
			if (!generatePreprocessor)
			{
				var cscPath = Path.Combine(frameworkPath, "csc.exe");
				var psi = new ProcessStartInfo(cscPath);
				psi.UseShellExecute = false;
				psi.CreateNoWindow = true;
				psi.RedirectStandardError = true;
				psi.RedirectStandardOutput = true;
				var dllext = Path.GetExtension(typeof(int).Assembly.GetModules()[0].FullyQualifiedName);
				var bfn = "Preprocess";
				var ufn = bfn + dllext;
				int ufi = 1;
				while (File.Exists(Path.Combine(cwd, ufn)))
					ufn = bfn + (++ufi).ToString() + dllext;
				var csufn = bfn + ".cs";
				ufi = 1;
				while (File.Exists(Path.Combine(cwd, csufn)))
					csufn = bfn + (++ufi).ToString() + ".cs";
				var cbufn = "Codebehind.cs";
				if (null != codebehind)
				{
					ufi = 1;
					while (File.Exists(Path.Combine(cwd, cbufn)))
						cbufn = "Codebehind" + (++ufi).ToString() + ".cs";
					using (var fw = new StreamWriter(Path.Combine(cwd, cbufn), false, Encoding.UTF8))
					{
						char[] buf = new char[1024];
						var r = codebehind.Read(buf, 0, buf.Length);
						while (0 != r)
						{
							fw.Write(buf, 0, r);
							r = codebehind.Read(buf, 0, buf.Length);
						}
						fw.Flush();
					}
				}
				using (var fw = new StreamWriter(Path.Combine(cwd, csufn), false, Encoding.UTF8))
				{
					fw.BaseStream.SetLength(0);
					fw.Write(sb.ToString());
				}

				psi.Arguments = "-target:library -out:\"" + Path.Combine(cwd, ufn).Replace("\"", "\"\"") + "\" \"" + Path.Combine(cwd, csufn).Replace("\"", "\"\"") + "\"";
				if(codebehind!=null)
                {
					psi.Arguments += " \""+Path.Combine(cwd,cbufn).Replace("\"", "\"\"") + "\"";
                }
				var proc = Process.Start(psi);
				proc.WaitForExit();
				Assembly asm = null;
				Exception ex = null;
				if (File.Exists(Path.Combine(cwd, csufn)))
				{
					try
					{
						asm = Assembly.LoadFrom(Path.Combine(cwd, ufn));
					}
					catch (Exception e) { ex = new TargetInvocationException(proc.StandardOutput.ReadToEnd() + Environment.NewLine + proc.StandardError.ReadToEnd(), e); }

				}
				else
				{
					ex = new TargetInvocationException(proc.StandardOutput.ReadToEnd() + Environment.NewLine + proc.StandardError.ReadToEnd(), new Exception("Error compiling assembly."));
				}
				if (null != ex)
				{
					throw ex;
				}
				if (null == args) args = new Dictionary<string, object>();
				if (null != asm)
				{
					var s = codeclass;
					if (codenamespace != null) s = codenamespace + "." + codeclass;
					var t = asm.GetType(s);
					var m = t.GetMethod(codemethod);
					if (null != m)
					{
						try
						{
							m.Invoke(null, new object[] { output, args });
						}
						catch (TargetInvocationException tex)
						{
							throw tex.InnerException;
						}
					}
				}
				File.Delete(csufn);
				try
				{
					File.Delete(ufn);
				}
				catch { }
			}
			
		}
		static string _ReadUntilStartContext(TextReader input,out char ctxChar)
		{
			int cur = input.Read();
			var sb = new StringBuilder();
			while (true)
			{
				if ('<' == cur)
				{
					cur = input.Read();
					if (-1 == cur)
					{
						sb.Append('<');
						ctxChar = '\0';
						return sb.ToString();
					}
					else if ('#' == cur || '%' == cur)
					{
						ctxChar = unchecked((char)cur);
						return sb.ToString();
					}
					sb.Append('<');
					continue;
				}
				else if (-1 == cur)
				{
					ctxChar = '\0';
					return sb.ToString();
				}
				sb.Append(unchecked((char)cur));
				cur = input.Read();
			}
		}
		static string _ReadUntilEndContext(int firstChar, TextReader input,char ctxChar)
		{
			int cur;
			cur = firstChar;
			if (-1 == firstChar)
				cur = input.Read();
			var sb = new StringBuilder();
			while (true)
			{
				if(ctxChar == cur)
				{
					cur = input.Read();
					if (-1 == cur)
					{
						sb.Append(ctxChar);
						return sb.ToString();
					}
					else if ('>' == cur)
						return sb.ToString();
					sb.Append(ctxChar);
				}
				else if (-1 == cur)
					return sb.ToString();
				sb.Append(unchecked((char)cur));
				cur = input.Read();
			}
		}
		static string _ToStringLiteral(IEnumerable<char> text, int r = -1)
		{
			var result = new StringBuilder();
			result.Append("\"");
			var i = 0;
			foreach (var ch in text)
			{
				if (r > -1 && i == r)
					break;
				switch (ch)
				{
					case '\0':
						result.Append("\0");
						break;
					case '\a':
						result.Append("\a");
						break;
					case '\b':
						result.Append("\\b");
						break;
					case '\f':
						result.Append("\\f");
						break;
					case '\n':
						result.Append("\\n");
						break;
					case '\r':
						result.Append("\\r");
						break;
					case '\t':
						result.Append("\\t");
						break;
					case '\v':
						result.Append("\\v");
						break;
					case '\"':
						result.Append("\\\"");
						break;
					case '\'':
						result.Append("\\\'");
						break;
					case '\\':
						result.Append("\\\\");
						break;
					default:
						result.Append(ch);
						break;
				}
				++i;
			}
			result.Append("\"");
			return result.ToString();
		}

	}
}
