using System;

namespace Vivelin.Hosts
{
    public class HostsFileEntry
    {
        private string _line;
        private bool _isEnabled;

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

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        private void Parse(ReadOnlySpan<char> value)
        {
            var trimmed = value.Trim();

            if (trimmed[0] == '#')
            {
                _isEnabled = false;
            }
        }
    }
}
