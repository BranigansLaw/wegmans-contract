using System.Runtime.CompilerServices;
using Wegmans.PharmacyLibrary.InterfaceEngine;

public class EmptyJob : RxInterfaceBase
{
    public static string FilePath => GetFilePath();

    public override void Execute()
    {
    }

    protected override void Dispose(bool disposing)
    {
    }

    private static string GetFilePath([CallerFilePath] string caller = "")
    {
        return caller;
    }
}