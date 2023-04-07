using System;
using System.IO;
// using System.Windows.Forms;

namespace DTPKutil
{
    static class DTPKUtilCLI
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        // [STAThread]

        static void Help(){
            Console.WriteLine("  -help These instructions");
            Console.WriteLine("  -decompress <inputFile> <outputFile>");
            Console.WriteLine("      Decompress a .SND file normally.");
            Console.WriteLine("  -decompress32 <inputFile> <outputFile>");
            Console.WriteLine("      Decompress a .SND file and expand samples for PC release.");
            Console.WriteLine("  -extract <inputFile> <outputFolder>");
            Console.WriteLine("      Extracts .WAV files from a .SND file normally.");
            return;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("DTPKUtil Command Line");
            if (args.Length <= 0)
            {
                Help();
                return;
            }
            //Note: This console output won't actually print to the console because it's a desktop app.
            //It can be fixed, but this is a bonus feature anyway so just use it correctly.
            if(args[0].Equals("/?") || args[0].Equals("-help"))
            {
                Help();
            }
            if(args.Length >= 3)
            {
                switch(args[0].ToLower()){
                    case "-decompress":
                        if (File.Exists(args[1]))
                        {
                            DtpkFile file = new DtpkFile(File.ReadAllBytes(args[1]));
                            file = file.Decompress(false);
                            File.WriteAllBytes(args[2], file.FileData);
                            Console.WriteLine("File decompressed successfully");
                            return;
                        }
                        break;
                    case "-decompress32":
                        if (File.Exists(args[1]))
                        {
                            DtpkFile file = new DtpkFile(File.ReadAllBytes(args[1]));
                            file = file.Decompress(true);
                            File.WriteAllBytes(args[2], file.FileData);
                            Console.WriteLine("File decompressed successfully");
                            return;
                        }
                        break;
                    case "-extract":
                        if (!Directory.Exists(args[2])){
                            Directory.CreateDirectory(args[2]);
                        }
                        if (File.Exists(args[1]))
                        {
                            DtpkFile file = new DtpkFile(File.ReadAllBytes(args[1]));
                            for (int i = 0; i < file.Samples.Count; i++)
                            {
                                string fileOut = Path.GetFileNameWithoutExtension(args[1]) + "_" + (i + 1) + ".wav";
                                fileOut = Path.Combine(args[2], fileOut);
                                File.WriteAllBytes(fileOut, WavUtil.AddWavHeader(file.GetSampleData(file.Samples[i], true, file.Is2018Format), file.Is2018Format ? (byte)32 : (byte)16));
                            }
                            Console.WriteLine("Samples extracted successfully");
                            return;
                        }
                        break;
                    default:
                        Help();
                        break;
                }
            }
            // Application.EnableVisualStyles();
            // Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new MainView());
        }

    }
}
