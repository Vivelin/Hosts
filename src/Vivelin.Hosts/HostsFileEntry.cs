using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;

namespace Vivelin.Hosts
{
    public class HostsFileEntry
    {
        private HostsFileEntry(string line, string comment, IPAddress address, List<string> hostNames, bool enabled)
        {
            Line = line;
            Comment = comment;
            Address = address;
            HostNames = hostNames;
            Enabled = enabled;
        }

        public string Line { get; }

        public IPAddress Address { get; set; }

        public List<string> HostNames { get; }
            = new List<string>();

        public string Comment { get; set; }

        public bool Enabled { get; set; }

        public bool IsValid => Address != null && HostNames.Count > 0;

        public override string ToString()
        {
            var builder = new StringBuilder();
            if (!Enabled && (IsValid || Comment == null))
            {
                builder.Append("# ");
            }

            if (Address != null)
            {
                builder.Append(Address.ToString());
                builder.Append(' ');
                builder.Append(string.Join(' ', HostNames));
            }

            if (Comment != null)
            {
                if (Address != null)
                    builder.Append(' ');
                builder.Append("# ");
                builder.Append(Comment);
            }

            return builder.ToString().Trim();
        }

        public static HostsFileEntry Parse(string value)
        {
            var tokens = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var enabled = true;
            string comment = null;
            IPAddress ipAddress = null;
            var hostNames = new List<string>();

            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i].Trim();
                if (token == "#")
                {
                    comment = string.Join(' ', tokens.Skip(i + 1));
                    if (!enabled || tokens.Length <= i || !IPAddress.TryParse(tokens[i + 1], out _))
                    {
                        // Stop parsing when we encounter a comment in an already commented-out line
                        // Stop parsing when the next token is not an IP address (so a regular comment)
                        break;
                    }

                    enabled = false;
                    comment = null;
                }
                else if (token.StartsWith('#'))
                {
                    comment = token.Substring(1) + ' ' + string.Join(' ', tokens.Skip(i + 1));
                    if (!enabled || !IPAddress.TryParse(token.Substring(1), out var address))
                    {
                        // Stop parsing when we encounter a comment in an already commented-out line
                        // Stop parsing when the commented token is an IP address (so a regular comment)
                        break;
                    }

                    enabled = false;
                    ipAddress = address;
                    comment = null;
                }
                else if (ipAddress == null && IPAddress.TryParse(token, out var address))
                {
                    // IP addresses are easy to recognize, but we can only have one
                    ipAddress = address;
                }
                else
                {
                    // Anything that isn't a comment or an IP address has to be a hostname
                    hostNames.Add(token);
                }
            }

            return new HostsFileEntry(value, comment, ipAddress, hostNames, enabled);
        }
    }
}
