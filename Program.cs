using System;
using System.Diagnostics;
using System.IO;
using ImageMagick;
using NetVips;
using SkiaSharp;

class Program
{
    static void Main()
    {
        string inputPath = "./file_example_TIFF_10MB.tiff";
        string outputDir = "output";
        Directory.CreateDirectory(outputDir);

        int newWidth = 800;
        int newHeight = 600;

        Console.WriteLine("Starting image resizing benchmark..." + File.Exists(inputPath));

        Benchmark("NetVips", () => ResizeWithNetVips(inputPath, Path.Combine(outputDir, "output_netvips2.jpg"), newWidth, newHeight));
        Benchmark("ImageMagick", () => ResizeWithImageMagick(inputPath, Path.Combine(outputDir, "output_imagemagick2.jpg"), newWidth, newHeight));
        Benchmark("SkiaSharp", () => ResizeWithSkia(inputPath, Path.Combine(outputDir, "output_skia2.jpg"), newWidth, newHeight));
    }

    static void Benchmark(string libraryName, Action resizeAction)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        resizeAction();
        stopwatch.Stop();
        Console.WriteLine($"{libraryName}: {stopwatch.ElapsedMilliseconds} ms");
    }

    static void ResizeWithNetVips(string inputPath, string outputPath, int width, int height)
    {
        using var image = NetVips.Image.NewFromFile(inputPath);
        var resized = image.Resize((double)width / image.Width);
        resized.WriteToFile(outputPath);
    }

    static void ResizeWithImageMagick(string inputPath, string outputPath, int width, int height)
    {
        using var image = new MagickImage(inputPath);
        image.Resize(width, height);
        image.Write(outputPath);
    }

    static void ResizeWithSkia(string inputPath, string outputPath, int width, int height)
    {
        Console.WriteLine("ResizeWithSkia..." + File.Exists(inputPath));

        using var inputStream = File.OpenRead(inputPath);
        using var bitmap = SKBitmap.Decode(inputStream);
        using var resizedBitmap = bitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.High);
        using var image = SKImage.FromBitmap(resizedBitmap);
        using var outputStream = File.OpenWrite(outputPath);
        image.Encode(SKEncodedImageFormat.Jpeg, 90).SaveTo(outputStream);
    }
}
