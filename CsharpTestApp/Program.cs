using System.Runtime.InteropServices;
using System.Text.Json;

namespace CsharpTestApp;

internal partial class Program
{
    public static void Main()
    {
        IntPtr daemon = monero_daemon_connect("http://localhost:18081", "", "", "", "");
        if (daemon == IntPtr.Zero)
        {
            string? err = monero_get_error();
            throw new Exception($"Failed to connect to daemon: {err}");
        }

        string? infoJson = monero_daemon_get_info(daemon);
        if (infoJson == null)
        {
            string? err = monero_get_error();
            throw new Exception($"get_info failed: {err}");
        }

        Console.WriteLine(infoJson);

        using JsonDocument doc = JsonDocument.Parse(infoJson);
        string? version = doc.RootElement.GetProperty("version").GetString();

        const string expectedVersion = "0.18.5.1-release";
        if (version != expectedVersion)
        {
             throw new Exception($"Unexpected daemon version: expected '{expectedVersion}', got '{version}'");
        }
        Console.WriteLine($"Version assertion passed: {version}");

        monero_daemon_free(daemon);
    }

    [LibraryImport("monero-c")]
    private static partial IntPtr monero_daemon_connect(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string uri,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string username,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string password,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string proxyUri,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string zmqUri);

    [LibraryImport("monero-c")]
    private static partial void monero_daemon_free(IntPtr daemon);

    [LibraryImport("monero-c")]
    [return: MarshalAs(UnmanagedType.LPUTF8Str)]
    private static partial string? monero_daemon_get_info(IntPtr daemon);

    [LibraryImport("monero-c")]
    [return: MarshalAs(UnmanagedType.LPUTF8Str)]
    private static partial string? monero_get_error();
}