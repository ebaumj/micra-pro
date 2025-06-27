using System.Diagnostics;
using MicraPro.Shared.Domain;

namespace MicraPro.Shared.Infrastructure;

public class SystemService : ISystemService
{
    private static async Task<string> Bash(string file, string cmd)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = file,
                Arguments = cmd.Replace("\"", "\\\""),
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };
        process.Start();
        var result = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();
        return result;
    }

    public async Task<bool> ShutdownAsync(CancellationToken ct) =>
        (await Bash("/sbin/shutdown", "now")).Contains("The system will power off now!");

    public async Task<bool> RebootAsync(CancellationToken ct) =>
        (await Bash("/sbin/reboot", "now")).Contains("The system will reboot now!");
}
