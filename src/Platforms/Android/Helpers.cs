using System;
using Java.Lang;
using Console = System.Console;
using Java.IO;
using Microsoft.Maui.ApplicationModel;

namespace Glyphy.Platforms.Android
{
    internal class Helpers
    {
        public static bool CreateRootSubProcess(out Process? subProcess)
        {
            subProcess = null;
            try
            {
                subProcess = Runtime.GetRuntime()?.Exec("su");
                if (subProcess is null)
                    throw new NullReferenceException("Unable to create sub-process.");

                DataOutputStream inputStream = new DataOutputStream(subProcess?.OutputStream);
                DataInputStream outputStream = new DataInputStream(subProcess?.InputStream);

                if (outputStream is null || inputStream is null)
                    throw new NullReferenceException("Unable to open sub-process streams.");

                inputStream.WriteBytes("id\n");
                inputStream.Flush();
#pragma warning disable CS0618 // Type or member is obsolete
                string? result = outputStream.ReadLine();
#pragma warning restore CS0618

                if (string.IsNullOrEmpty(result) || !result!.Contains("uid=0"))
                    throw new PermissionException("Root access denied.");

                outputStream.Dispose();
                inputStream.Dispose();

                return true;
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Root access rejected: " + e.Message);

                if (subProcess != null)
                    subProcess.Dispose();

                return false;
            }
        }
    }
}
