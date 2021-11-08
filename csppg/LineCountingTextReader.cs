using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csppg {
    class LineCountingTextReader : TextReader {
        int _line;
        TextReader _inner;
        public LineCountingTextReader(TextReader reader, int line = 1) {
            if (reader == null) throw new ArgumentNullException("reader");
            _inner = reader;
            _line = line;
        }
        public int Line { get { return _line;  } }
        public TextReader BaseReader {
            get {
                return _inner;
            }
        }
        public override int Read() {
            var i = _inner.Read();
            if(i=='\n') {
                ++_line;
            }
            return i; 
            
        }
    }
}
