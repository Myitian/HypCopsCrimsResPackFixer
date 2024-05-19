using BigGustave;
using System.Drawing;
using System.IO.Compression;

namespace HypCopsCrimsResPackFixer;
class Program
{
    static void Main()
    {
        Resources res = new();
        Console.Error.WriteLine(Resources.MessageEnterMinecraftFolder);
        Console.Error.WriteLine(string.Format(Resources.MessageEnterMinecraftFolderHint, Resources.DefaultMCPath.FullName));
        string? read = Console.In.ReadLine()?.Trim().Trim('"');
        DirectoryInfo mc = new(string.IsNullOrEmpty(read) ? Resources.DefaultMCPath.FullName : read);
        DirectoryInfo mcDownloads = new(Path.Combine(mc.FullName, Resources.Downloads));
        if (!mcDownloads.Exists)
        {
            Console.Error.WriteLine(Resources.MessageFolderNotExist);
            return;
        }
        foreach (FileInfo file in mcDownloads.EnumerateFiles(Resources.Pattern, SearchOption.AllDirectories))
        {
            ZipArchive zip;
            try
            {
                zip = ZipFile.OpenRead(file.FullName);
            }
            catch
            {
                continue;
            }
            ZipArchiveEntry? mcmeta = null, painting = null, font = null, clownfish = null;
            foreach (ZipArchiveEntry entry in zip.Entries)
            {
                switch (entry.FullName)
                {
                    case Resources.McmetaPath:
                        mcmeta = entry;
                        break;
                    case Resources.LegacyPaintingPath:
                        painting = entry;
                        break;
                    case Resources.LegacyFontPath:
                        font = entry;
                        break;
                    case Resources.ClownfishTexturePath:
                        clownfish = entry;
                        break;
                }
            }
            if (mcmeta is null)
                continue;
            using (StreamReader sr = new(mcmeta.Open()))
                if (!sr.ReadToEnd().Contains(Resources.Keyword))
                    continue;
            Console.Error.WriteLine(Resources.MessageEnterPackName);
            Console.Error.WriteLine(string.Format(Resources.MessageEnterPackNameHint, Resources.DefaultPackName));
            read = Console.In.ReadLine()?.Trim().Trim('"');
            DirectoryInfo mcResourcepacks = new(Path.Combine(mc.FullName, Resources.Resourcepacks));
            string path = Path.Combine(mcResourcepacks.FullName, string.IsNullOrEmpty(read) ? Resources.DefaultPackName : read);
            using FileStream fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read);
            using ZipArchive patcher = new(fs, ZipArchiveMode.Create);
            ZipArchiveEntry targetMcmeta = patcher.CreateEntry(Resources.McmetaPath);
            using (Stream s = targetMcmeta.Open())
                s.Write(res.Mcmeta);
            ZipArchiveEntry targetShader = patcher.CreateEntry(Resources.ShaderPath);
            using (Stream s = targetShader.Open())
                s.Write(res.Shader);
            MemoryStream ms;
            if (painting is not null)
            {
                ms = new();
                using (Stream sp = painting.Open())
                    sp.CopyTo(ms);
                ms.Position = 0;
                Bitmap bmp = new(ms);
                ms.Close();
                int wUnit = bmp.Width / 16;
                int hUnit = bmp.Height / 16;
                Bitmap x1y1 = new(wUnit, hUnit);
                bmp.CopyTo(x1y1, CreateRect16(15, 0, 1, 1, wUnit, hUnit), Point.Empty);
                Pixel p = x1y1.Raw[^1];
                x1y1.Raw[^1] = new Pixel(p.R, p.G, p.B, 1, p.IsGrayscale);
                ZipArchiveEntry back = patcher.CreateEntry(Resources.PaintingBackPath);
                x1y1.Save(back.Open());
                for (int i = 0; i < Resources.X1Y1Names.Length; i++)
                {
                    bmp.CopyTo(x1y1, CreateRect16(i, 0, 1, 1, wUnit, hUnit), Point.Empty);
                    ZipArchiveEntry e = patcher.CreateEntry(string.Format(Resources.PaintingPath, Resources.X1Y1Names[i]));
                    x1y1.Save(e.Open());
                }
                Bitmap x2y1 = new(wUnit * 2, hUnit);
                for (int i = 0; i < Resources.X2Y1Names.Length; i++)
                {
                    bmp.CopyTo(x2y1, CreateRect16(i * 2, 2, 2, 1, wUnit, hUnit), Point.Empty);
                    ZipArchiveEntry e = patcher.CreateEntry(string.Format(Resources.PaintingPath, Resources.X2Y1Names[i]));
                    x2y1.Save(e.Open());
                }
                Bitmap x1y2 = new(wUnit, hUnit * 2);
                for (int i = 0; i < Resources.X1Y2Names.Length; i++)
                {
                    bmp.CopyTo(x1y2, CreateRect16(i, 4, 1, 2, wUnit, hUnit), Point.Empty);
                    ZipArchiveEntry e = patcher.CreateEntry(string.Format(Resources.PaintingPath, Resources.X1Y2Names[i]));
                    x1y2.Save(e.Open());
                }
                Bitmap x4y2 = new(wUnit * 4, hUnit * 2);
                for (int i = 0; i < Resources.X4Y2Names.Length; i++)
                {
                    bmp.CopyTo(x4y2, CreateRect16(i * 4, 6, 4, 2, wUnit, hUnit), Point.Empty);
                    ZipArchiveEntry e = patcher.CreateEntry(string.Format(Resources.PaintingPath, Resources.X4Y2Names[i]));
                    x4y2.Save(e.Open());
                }
                Bitmap x2y2 = new(wUnit * 2, hUnit * 2);
                for (int i = 0; i < Resources.X2Y2Names.Length; i++)
                {
                    bmp.CopyTo(x2y2, CreateRect16(i * 2, 8, 2, 2, wUnit, hUnit), Point.Empty);
                    ZipArchiveEntry e = patcher.CreateEntry(string.Format(Resources.PaintingPath, Resources.X2Y2Names[i]));
                    x2y2.Save(e.Open());
                }
                Bitmap x4y4 = new(wUnit * 4, hUnit * 4);
                for (int i = 0; i < Resources.X4Y4Names.Length; i++)
                {
                    bmp.CopyTo(x4y4, CreateRect16(i * 4, 12, 4, 4, wUnit, hUnit), Point.Empty);
                    ZipArchiveEntry e = patcher.CreateEntry(string.Format(Resources.PaintingPath, Resources.X4Y4Names[i]));
                    x4y4.Save(e.Open());
                }
                Bitmap x4y3 = new(wUnit * 4, hUnit * 3);
                for (int i = 0; i < Resources.X4Y3Names.Length; i++)
                {
                    bmp.CopyTo(x4y3, CreateRect16(12, 4 + i * 3, 4, 3, wUnit, hUnit), Point.Empty);
                    ZipArchiveEntry e = patcher.CreateEntry(string.Format(Resources.PaintingPath, Resources.X4Y3Names[i]));
                    x4y3.Save(e.Open());
                }
            }
            if (font is not null)
            {
                ms = new();
                using (Stream sf = font.Open())
                    sf.CopyTo(ms);
                ms.Position = 0;
                Bitmap bmp = new(ms);
                ms.Close();
                int wUnit = bmp.Width / 16;
                int hUnit = bmp.Height / 16;
                Bitmap target = new(bmp.Width, hUnit * 4, bmp.HasAlphaChannel);
                bmp.CopyTo(target, CreateRect16(0, 6, 16, 4, wUnit, hUnit), Point.Empty);
                ZipArchiveEntry targetPng = patcher.CreateEntry(Resources.FontPath);
                target.Save(targetPng.Open());
                ZipArchiveEntry targetDefaultJson = patcher.CreateEntry(Resources.FontDefaultJsonPath);
                using (Stream s = targetDefaultJson.Open())
                    s.Write(res.FontDefaultJson);
                ZipArchiveEntry targetUniformJson = patcher.CreateEntry(Resources.FontUniformJsonPath);
                using (Stream s = targetUniformJson.Open())
                    s.Write(res.FontUniformJson);
            }
            if (clownfish is not null)
            {
                ZipArchiveEntry e = patcher.CreateEntry(Resources.TropicalFishTexturePath);
                using Stream s = clownfish.Open();
                using Stream t = e.Open();
                s.CopyTo(t);
            }
            zip?.Dispose();
            Console.Error.WriteLine(Resources.MessageDone);
            return;
        }
    }
    static Rectangle CreateRect16(int x, int y, int width, int height, int wUnit, int hUnit)
    {
        return new(x * wUnit, y * hUnit, width * wUnit, height * hUnit);
    }
}