using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Vivelin.Hosts
{
    public class HostsFileManager : INotifyPropertyChanged
    {
        private HostsFile _hostsFile;

        public HostsFileManager()
            : this(SystemHostsPath, Path.GetTempFileName())
        {
#if DEBUG
            HostsFile = new HostsFile(new List<HostsFileEntry>
            {
                new HostsFileEntry("# This is a comment", "This is a comment", null, null, false),
                new HostsFileEntry("# 127.0.0.1 localhost loopback", null, new IPAddress(new byte[]{127,0,0,1 }), new List<string>() { "localhost", "loopback" }, false),
                new HostsFileEntry("127.0.0.1 localhost loopback", null, new IPAddress(new byte[]{127,0,0,1 }), new List<string>() { "localhost", "loopback" }, true)
            });
#endif
        }

        public HostsFileManager(string sourcePath, string tempPath)
        {
            SourcePath = sourcePath;
            TempPath = tempPath;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static string SystemHostsPath
        {
            get
            {
                var systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
                return Path.Combine(systemFolder, "drivers", "etc", "hosts");
            }
        }

        public HostsFile HostsFile
        {
            get { return _hostsFile; }
            protected set
            {
                if (value != _hostsFile)
                {
                    _hostsFile = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SourcePath { get; }

        public string TempPath { get; }

        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            File.Copy(SourcePath, TempPath, overwrite: true);

            using var stream = new FileStream(TempPath, FileMode.Open, FileAccess.Read);
            HostsFile = await HostsFile.LoadAsync(stream, cancellationToken).ConfigureAwait(false);
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            using var stream = new FileStream(TempPath, FileMode.Create, FileAccess.Write);
            await HostsFile.SaveAsync(stream, cancellationToken).ConfigureAwait(false);

            File.Copy(TempPath, SourcePath);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName is null)
                throw new ArgumentNullException(nameof(propertyName));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}