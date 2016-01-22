using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Hosts
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      try
      {
        var hostsFile = Path.Combine(Environment.SystemDirectory, "drivers", "etc", "hosts");
        var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var tempFile = Path.Combine(tempFolder, "hosts.txt");

        // Make a backup and a copy for writing
        Directory.CreateDirectory(tempFolder);
        File.Copy(hostsFile, hostsFile + ".bak", overwrite: true);
        File.Copy(hostsFile, tempFile, overwrite: true);

        // Open the writable copy in the default .txt editor and wait for the process to exit
        var editor = new ProcessStartInfo
        {
          FileName = tempFile,
          UseShellExecute = true,
          Verb = "open"
        };
        var process = Process.Start(editor);
        if (process != null)
        {
          process.WaitForExit();

          // Assume the file has been saved by now, so copy it back
          // This is where we most likely require elevated permissions
          File.Copy(tempFile, hostsFile, overwrite: true);
        }
        else
        {
          MessageBox.Show(Resources.Strings.ProcessIsNull, 
            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        File.Delete(tempFile);
      }
      catch (UnauthorizedAccessException ex)
      {
        MessageBox.Show(string.Format(Resources.Strings.UnhandledUAException, ex.Message), 
          Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }
}
