using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vivelin.Hosts
{
    public class HostsFile
    {
        private static readonly Encoding s_encoding = new UTF8Encoding(false);

        internal HostsFile(IEnumerable<HostsFileEntry> entries)
        {
            Entries = new ObservableCollection<HostsFileEntry>(entries);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<HostsFileEntry> Entries { get; }

        public static async Task<HostsFile> LoadAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new ArgumentException("The stream cannot be read.", nameof(stream));

            var entries = new List<HostsFileEntry>();
            var reader = new StreamReader(stream, s_encoding);
            var line = await reader.ReadLineAsync().ConfigureAwait(false);
            while (line != null && !cancellationToken.IsCancellationRequested)
            {
                var readTask = reader.ReadLineAsync();
                var entry = HostsFileEntry.Parse(line);
                entries.Add(entry);
                line = await readTask.ConfigureAwait(false);
            }

            return new HostsFile(entries);
        }

        public async Task SaveAsync(Stream destination, CancellationToken cancellationToken = default)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            if (!destination.CanWrite)
                throw new ArgumentException("The stream cannot be written to.", nameof(destination));

            var writer = new StreamWriter(destination, s_encoding);
            foreach (var entry in Entries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var line = entry.ToString();
                await writer.WriteLineAsync(line).ConfigureAwait(false);
            }
            await writer.FlushAsync();
        }
    }
}
