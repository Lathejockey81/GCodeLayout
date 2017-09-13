using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GCodeLayout
{
    class ProgramLine
    {
        public ProgramLine() {}

        public ProgramLine(String Content, int Number)
        {
            LineContent = Content;
            LineNumber = Number;
        }

        private String _lineContent;
        public String LineContent 
        { 
            get
            {
                return _lineContent;
            }
            set
            {
                _lineContent = value;
                ExtractComments(_lineContent);
            }
        }

        private void ExtractComments(string value)
        {
            Comments = String.Empty;
            var comments = new List<String>();

            // Find T Commands
            var r = new Regex(@"^.*(T[0-9]+).*$");
            if (r.IsMatch(value))
            {
                Match m = r.Match(value);
                comments.Add(m.Groups[1].Value.ToString());
            }

            // Find Z Positions
            r = new Regex(@"^.*(Z[0-9\.\-]+).*$");
            if (r.IsMatch(value))
            {
                Match m = r.Match(value);
                comments.Add(m.Groups[1].Value.ToString());
                ZDim = m.Groups[1].Value.ToString();
            }

            // Find Comments
            r = new Regex(@"^.*(\(.+\)).*$");
            if (r.IsMatch(value))
            {
                Match m = r.Match(value);
                comments.Add(m.Groups[1].Value.ToString());
            }

            if (comments.Count > 0)
            {
                Comments = String.Join(" ", comments);
            }
        }

        public int LineNumber { get; set; }

        public String Comments { get; set; }

        public String ToolNum { get; set; }

        public String XDim { get; set; }

        public String YDim { get; set; }

        public String ZDim { get; set; }

        public void ShiftZ(Decimal Distance, Boolean ShiftAboveZero = false)
        {
            if (ZDim == null) return;

            var r = new Regex(@"^.*Z([0-9\.\-]+).*$");

            if (r.IsMatch(_lineContent))
            {
                Match m = r.Match(_lineContent);
                var dim = Convert.ToDecimal(m.Groups[1].Value);
                if (dim <= 0 || ShiftAboveZero)
                {
                    dim += Distance;
                }

                _lineContent = Regex.Replace(_lineContent, @"Z[0-9\.\-]+", "Z" + dim.ToString("0.####"));
                ExtractComments(_lineContent);
            }            
        }
    }
}
