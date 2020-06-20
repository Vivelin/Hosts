using System;
using System.Linq;

namespace Vivelin.Hosts
{
    public class HostsFileEntry
    {
        private string _line;
        private bool _isEnabled;
        private string _comment;

        public string Line
        {
            get { return _line; }
            set
            {
                if (value != _line)
                {
                    _line = value;
                    Parse(value);
                }
            }
        }

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        private void Parse(string value)
        {
            var tokens = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                if (token.Trim() == "#")
                {
                    Comment = string.Join(' ', tokens.Skip(i + 1));
                    break;
                }

                if (token.Trim().StartsWith('#'))
                {
                    Comment = token.Substring(1) + ' ' + string.Join(' ', tokens.Skip(i + 1));
                    break;
                }
            }
        }
    }
}
