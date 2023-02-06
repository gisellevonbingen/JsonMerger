using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangFileDiff
{
    public struct LineInfo
    {
        public int LineNumber { get; set; }
        public string Content { get; set; }

        public override string ToString()
        {
            return $"Line: {this.LineNumber + 1} - {this.Content}";
        }

        public string ToStringWithOnlyValue()
        {
            return $"Line: {this.LineNumber + 1} - {this.Content.ExtractValue()}";
        }

    }

}
