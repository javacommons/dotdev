using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
//using SevenZipExtractor;
//using SevenZip;
enum ProjectType
{
    cui, gui, wpf, lib
}
class Options
{
    [Option('t', "type", HelpText = "cui, gui, wpf, lib", Required = true)]
    public ProjectType ProjectType
    {
        get;
        set;
    }
}
class Program
{
    static void Main(string[] args)
    {
        Assembly myAssembly = Assembly.GetEntryAssembly();
        string path = myAssembly.Location;
        Console.WriteLine(path.ToUpper());
        var parseResult = Parser.Default.ParseArguments<Options>(args);
        //Options opt = null;
        switch (parseResult.Tag)
        {
        case ParserResultType.Parsed:
        {
            var parsed = parseResult as Parsed<Options>;
            Options opt = parsed.Value;
            Console.WriteLine($"opt.ProjectType = {opt.ProjectType}");
            ExtractZip(opt.ProjectType);
        }
        break;
        case ParserResultType.NotParsed:
        {
            var notParsed = parseResult as NotParsed<Options>;
            //Console.Error.WriteLine(CommandLine.Text.HelpText.AutoBuild(parseResult));
        }
        break;
        }
    }
    protected static void ExtractZip(ProjectType type)
    {
        byte[] bytes = null;
        string dir = null;
        switch (type)
        {
        case ProjectType.cui:
        {
            bytes = ResourceAsBytes(Assembly.GetExecutingAssembly(), "dotdev.CUI.zip");
            dir = "CUI";
        }
        break;
        case ProjectType.gui:
        {
            bytes = ResourceAsBytes(Assembly.GetExecutingAssembly(), "dotdev.GUI.zip");
            dir = "GUI";
        }
        break;
        case ProjectType.wpf:
        {
            bytes = ResourceAsBytes(Assembly.GetExecutingAssembly(), "dotdev.WPF.zip");
            dir = "WPF";
        }
        break;
        case ProjectType.lib:
        {
            bytes = ResourceAsBytes(Assembly.GetExecutingAssembly(), "dotdev.LIB.zip");
            dir = "LIB";
        }
        break;
        default:
            break;
        }
        if (dir == null)
        {
            Console.WriteLine("dir == null");
            return;
        }
        string path = Path.GetTempFileName();
        using (FileStream output = new FileStream(path, FileMode.CreateNew))
        {
            output.Write(bytes, 0, bytes.Length);
        }
        ZipFile.ExtractToDirectory(path, dir);
        Console.WriteLine($"{dir} created.");
    }
    /*
    public static Stream ResourceAsStream(Assembly assembly, string name)
    {
        return assembly.GetManifestResourceStream(name);
    }
    */
    public static byte[] ResourceAsBytes(Assembly assembly, string name)
    {
        Stream stream = assembly.GetManifestResourceStream(name);
        if (stream == null) return new byte[] { };
        byte[] bytes = new byte[(int)stream.Length];
        stream.Read(bytes, 0, (int)stream.Length);
        return bytes;
    }
}
