using System.Diagnostics;
using System.IO.Compression;

namespace Mega_Subtitles_Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            bool extract = false; // Flag to indicate if we are extracting files
            bool compress = false; // Flag to indicate if we are compressing files
            bool deleteZip = false; // Flag to indicate if we should delete the ZIP file after extraction
            string? input = null; // Input file or directory for compression or extraction
            string? output = null; // Output file or directory for the ZIP archive
            string? runApp = null;  // Application to run after extraction, if specified

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].Trim().ToLower()) // Handle command-line arguments
                {
                    case "-e": extract = true; break; // Option to extract files from a ZIP archive
                    case "-c": compress = true; break; // Option to compress a folder into a ZIP archive
                    case "-d": deleteZip = true; break; // Option to delete the ZIP file after extraction                   
                    case "-i": if (i + 1 < args.Length) input = args[++i]; break; // Input file or directory for compression or extraction
                    case "-o": if (i + 1 < args.Length) output = args[++i]; break; // Output file or directory for the ZIP archive
                    case "-r": if (i + 1 < args.Length) runApp = args[++i]; break; // Application to run after extraction, if specified
                    case "-?": // Show help message
                    case "--help":
                        ShowHelp();
                        Thread.Sleep(500); // Delay
                        return;
                    default:
                        WriteLog($"Unknown argument: {args[i]}");
                        Thread.Sleep(500); // Delay
                        return;
                }
            }

            if (extract) // Check if we need to extract files
            {
                if (File.Exists(input) && !string.IsNullOrEmpty(output))
                {
                    WriteLog("Extracting...");
                    ZipFile.ExtractToDirectory(input, output, true);

                    if (!string.IsNullOrEmpty(runApp) && File.Exists(runApp))
                    {
                        Process.Start(Path.Combine(output, runApp));
                        WriteLog($"Running application: {runApp}");
                    }

                    if (deleteZip && File.Exists(input))
                    {
                        File.Delete(input); // Delete the ZIP file if specified
                        WriteLog($"Deleted ZIP file: {input}");
                    }
                    WriteLog($"Extraction completed to: {output}");
                }
                else
                {
                    WriteLog("Invalid input or output for extraction.");
                    ShowHelp();
                }
            }
            else if (compress) // Check if we need to compress files
            {
                if (Directory.Exists(input) && !string.IsNullOrEmpty(output))
                {
                    WriteLog("Compressing...");
                    ZipFile.CreateFromDirectory(input, output);
                    WriteLog($"Compression completed to: {output}");
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
            Thread.Sleep(1000); // Delay
        }

        static void ShowHelp() // Display help information
        {
            string helpMessage = "Usage: Updater.exe [-e | -c | -d] -i <input> -o <output> [-r <runApp>]\n" +
                              "Options:\n" +
                              "  -e          Extract files from a ZIP archive\n" +
                              "  -c          Compress a folder into a ZIP archive\n" +
                              "  -d          Delete the ZIP file after extraction\n" +
                              "  -i <input>  Specify input file or folder\n" +
                              "  -o <output> Specify output file or folder\n" +
                              "  -r <runApp> Run the specified application after extraction\n" +
                              "  -? or --help Show this help message\n";
            WriteLog(helpMessage);
        }

        static void WriteLog(string message) // Log messages to the console
        {
            string LogPath = AppDomain.CurrentDomain.BaseDirectory + "Logs.txt";

            // Log messages to the console with a timestamp
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}" + Environment.NewLine;
            File.AppendAllText(LogPath, logMessage);
        }
    }

}
// This code is a simple command-line utility for compressing and extracting files using ZIP format.







