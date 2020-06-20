using System;
using System.Linq;
using System.Net;

namespace Vivelin.Hosts
{
    public class HostsFileEntry
    {
        private string _line;
        private string _comment;
        private IPAddress _address;

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

        public IPAddress Address
        {
            get { return _address; }
        }

        public string Comment
        {
            get { return _comment; }
        }

        private void Parse(string value)
        {
            var tokens = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var inComment = false;
            _comment = null;
            _address = null;

            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i].Trim();
                if (token == "#")
                {
                    _comment = string.Join(' ', tokens.Skip(i + 1));
                    if (inComment || tokens.Length <= i || !IPAddress.TryParse(tokens[i + 1], out _))
                    {
                        break;
                    }

                    inComment = true;
                    _comment = null;
                }
                else if (token.StartsWith('#'))
                {
                    _comment = token.Substring(1) + ' ' + string.Join(' ', tokens.Skip(i + 1));
                    if (inComment || !IPAddress.TryParse(token.Substring(1), out var address))
                    {
                        break;
                    }

                    inComment = true;
                    _address = address;
                    _comment = null;
                }
                else if (_address == null && IPAddress.TryParse(token, out var address))
                {
                    _address = address;
                }
                else
                {

                }
            }
        }
    }
}
