using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Mega_Subtitles_Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            bool extract = false;
            bool compress = false;
            bool deleteZip = false;
            string? input = null;
            string? output = null;
            string? runApp = null;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i].Trim().ToLower();

                switch (arg)
                {
                    case "-e": extract = true; break;
                    case "-c": compress = true; break;
                    case "-d": deleteZip = true; break;
                    case "-i":
                        if (i + 1 < args.Length && !args[i + 1].StartsWith('-'))
                            input = args[++i];
                        else
                            WriteLog("Missing or invalid value for -i");
                        break;
                    case "-o":
                        if (i + 1 < args.Length && !args[i + 1].StartsWith('-'))
                            output = args[++i];
                        else
                            WriteLog("Missing or invalid value for -o");
                        break;
                    case "-r":
                        if (i + 1 < args.Length && !args[i + 1].StartsWith('-'))
                            runApp = args[++i];                        
                        break;
                    case "-?":
                    case "--help":
                        ShowHelp();
                        return;
                    default:
                        WriteLog($"Unknown argument: {args[i]}");
                        return;
                }
            }

            // Prevent using both extract and compress
            if (extract && compress)
            {
                WriteLog("Cannot use -e (extract) and -c (compress) at the same time.");
                ShowHelp();
                return;
            }

            if (extract)
            {
                if (File.Exists(input) && !string.IsNullOrEmpty(output))
                {
                    WriteLog("Extracting...");
                    try
                    {
                        ZipFile.ExtractToDirectory(input, output, true);
                        Thread.Sleep(800); // Delay

                        WriteLog($"Extraction completed to: {output}");

                        if (!string.IsNullOrEmpty(runApp))
                        {
                            string exePath = Path.Combine(output, runApp);
                            if (File.Exists(exePath))
                            {
                                WriteLog($"Running application: {exePath}");
                                try
                                {
                                    Process.Start(new ProcessStartInfo(exePath) { UseShellExecute = true });
                                }
                                catch (Exception ex)
                                {
                                    WriteLog($"Error launching application: {ex.Message}");
                                }
                            }
                            else
                            {
                                WriteLog($"Application not found: {exePath}");
                            }
                        }

                        if (deleteZip && File.Exists(input))
                        {
                            File.Delete(input);
                            WriteLog($"Deleted ZIP file: {input}");
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog($"Extraction error: {ex.Message}");
                    }
                }
                else
                {
                    WriteLog("Invalid input or output for extraction.");
                    ShowHelp();
                }
            }
            else if (compress)
            {
                if (Directory.Exists(input) && !string.IsNullOrEmpty(output))
                {
                    WriteLog("Compressing...");
                    try
                    {
                        ZipFile.CreateFromDirectory(input, output);
                        WriteLog($"Compression completed to: {output}");
                    }
                    catch (Exception ex)
                    {
                        WriteLog($"Compression error: {ex.Message}");
                    }
                }
                else
                {
                    WriteLog("Invalid input or output for compression.");
                    ShowHelp();
                }
            }
            else
            {
                WriteLog("No action specified.");
                ShowHelp();
            }

        }

        static void ShowHelp()
        {
            string helpMessage =
@"Usage: Updater.exe [-e | -c] [-d] -i <input> -o <output> [-r <runApp>]
Options:
  -e           Extract files from a ZIP archive
  -c           Compress a folder into a ZIP archive
  -d           Delete the ZIP file after extraction
  -i <input>   Specify input file or folder
  -o <output>  Specify output file or folder
  -r <runApp>  Run the specified application after extraction
  -? --help    Show this help message";

            WriteLog(helpMessage);
        }

        static void WriteLog(string message)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs.txt");
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";

            File.AppendAllText(logPath, logMessage);
        }
    }
}
