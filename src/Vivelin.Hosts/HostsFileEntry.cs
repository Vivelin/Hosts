using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace Vivelin.Hosts
{
    public class HostsFileEntry
    {
        private string _line;
        private string _comment;
        private IPAddress _address;
        private List<string> _hostNames = new List<string>();
        private bool _enabled;

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

        public ReadOnlyCollection<string> HostNames
        {
            get { return _hostNames.AsReadOnly(); }
        }

        public string Comment
        {
            get { return _comment; }
        }

        public bool Enabled
        {
            get { return _enabled; }
        }

        private void Parse(string value)
        {
            var tokens = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            _enabled = true;
            _comment = null;
            _address = null;
            _hostNames.Clear();

            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i].Trim();
                if (token == "#")
                {
                    _comment = string.Join(' ', tokens.Skip(i + 1));
                    if (!_enabled || tokens.Length <= i || !IPAddress.TryParse(tokens[i + 1], out _))
                    {
                        break;
                    }

                    _enabled = false;
                    _comment = null;
                }
                else if (token.StartsWith('#'))
                {
                    _comment = token.Substring(1) + ' ' + string.Join(' ', tokens.Skip(i + 1));
                    if (!_enabled || !IPAddress.TryParse(token.Substring(1), out var address))
                    {
                        break;
                    }

                    _enabled = false;
                    _address = address;
                    _comment = null;
                }
                else if (_address == null && IPAddress.TryParse(token, out var address))
                {
                    _address = address;
                }
                else
                {
                    _hostNames.Add(token);
                }
            }
        }
    }
}
