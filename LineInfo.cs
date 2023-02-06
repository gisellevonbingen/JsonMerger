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

        public bool EqualsKeyAndValue(string otherContent)
        {
            return this.Content.ExtractKey().Equals(otherContent.ExtractKey()) && this.Content.ExtractValue().Equals(otherContent.ExtractValue());
        }

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
