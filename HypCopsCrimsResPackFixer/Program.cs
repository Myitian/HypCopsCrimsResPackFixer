using BigGustave;
using System.Drawing;
using System.IO.Compression;

namespace HypCopsCrimsResPackFixer;
class Program
{
    static readonly string DefaultPackName = "HypCopsCrims_Patch.zip";
    static readonly DirectoryInfo DefaultMCPath = Environment.OSVersion.Platform switch
    {
        PlatformID.Win32NT => new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft")),
        PlatformID.Unix => new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".minecraft")),
        PlatformID.MacOSX => new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library/Application Support/minecraft")),
        _ => throw new NotImplementedException()
    };
    static readonly string[] X1Y1Names = ["kebab", "aztec", "alban", "aztec2", "bomb", "plant", "wasteland"];
    static readonly string[] X2Y1Names = ["pool", "courbet", "sea", "sunset", "creebet"];
    static readonly string[] X1Y2Names = ["wanderer", "graham"];
    static readonly string[] X4Y2Names = ["fighters"];
    static readonly string[] X2Y2Names = ["match", "bust", "stage", "void", "skull_and_roses", "wither"];
    static readonly string[] X4Y4Names = ["pointer", "pigscene", "burning_skull"];
    static readonly string[] X4Y3Names = ["skeleton", "donkey_kong"];
    static void Main()
    {
        Console.Error.WriteLine("Enter .minecraft folder:");
        Console.Error.WriteLine($"(Leave space to use \"{DefaultMCPath.FullName}\")");
        string? read = Console.In.ReadLine()?.Trim().Trim('"');
        DirectoryInfo mc = new(string.IsNullOrEmpty(read) ? DefaultMCPath.FullName : read);
        DirectoryInfo mcDownloads = new(Path.Combine(mc.FullName, "downloads"));
        if (!mcDownloads.Exists)
        {
            Console.Error.WriteLine("Folder not exist!");
            return;
        }
        foreach (FileInfo file in mcDownloads.EnumerateFiles("*.*", SearchOption.AllDirectories))
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
            ZipArchiveEntry? mcmeta = null, painting = null, font = null;
            foreach (ZipArchiveEntry entry in zip.Entries)
            {
                switch (entry.FullName)
                {
                    case "pack.mcmeta":
                        mcmeta = entry;
                        break;
                    case "assets/minecraft/textures/painting/paintings_kristoffer_zetterstrand.png":
                        painting = entry;
                        break;
                    case "assets/minecraft/mcpatcher/font/unicode_page_92.png":
                        font = entry;
                        break;
                }
            }
            if (mcmeta is null)
                continue;
            using (StreamReader sr = new(mcmeta.Open()))
                if (!sr.ReadToEnd().Contains("Cops and Crims\\n©2018 Hypixel, Inc."))
                    continue;
            Console.Error.WriteLine("Enter pack name to save:");
            Console.Error.WriteLine($"(Leave space to use \"{DefaultPackName}\")");
            read = Console.In.ReadLine()?.Trim().Trim('"');
            DirectoryInfo mcResourcepacks = new(Path.Combine(mc.FullName, "resourcepacks"));
            string path = Path.Combine(mcResourcepacks.FullName, string.IsNullOrEmpty(read) ? DefaultPackName : read);
            using FileStream fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read);
            using ZipArchive patcher = new(fs, ZipArchiveMode.Create);
            ZipArchiveEntry targetMcmeta = patcher.CreateEntry("pack.mcmeta");
            using (Stream s = targetMcmeta.Open())
                s.Write(@"{
    ""pack"": {
        ""pack_format"": 32,
        ""supported_formats"": [
            4,
            32
        ],
        ""description"": ""Hypixel Cops and Crims\nResource Patch Pack""
    }
}"u8);
            ZipArchiveEntry targetShader = patcher.CreateEntry("assets/minecraft/shaders/core/rendertype_entity_solid.fsh");
            using (Stream s = targetShader.Open())
                s.Write(@"#version 150

#moj_import <fog.glsl>

uniform sampler2D Sampler0;

uniform vec4 ColorModulator;
uniform float FogStart;
uniform float FogEnd;
uniform vec4 FogColor;

in float vertexDistance;
in vec4 vertexColor;
in vec4 lightMapColor;
in vec4 overlayColor;
in vec2 texCoord0;
in vec4 normal;

out vec4 fragColor;

void main() {
    vec4 color = texture(Sampler0, texCoord0) * vertexColor * ColorModulator;
    color.rgb = mix(overlayColor.rgb, color.rgb, overlayColor.a);
    if ( color.a == 0.0 ) discard;
    color *= lightMapColor;
    fragColor = linear_fog(color, vertexDistance, FogStart, FogEnd, FogColor);
}

// Download from https://bugs.mojang.com/browse/MC-164001"u8);
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
                ZipArchiveEntry back = patcher.CreateEntry("assets/minecraft/textures/painting/back.png");
                x1y1.Save(back.Open());
                for (int i = 0; i < X1Y1Names.Length; i++)
                {
                    bmp.CopyTo(x1y1, CreateRect16(i, 0, 1, 1, wUnit, hUnit), Point.Empty);
                    ZipArchiveEntry e = patcher.CreateEntry($"assets/minecraft/textures/painting/{X1Y1Names[i]}.png");
                    x1y1.Save(e.Open());
                }
                Bitmap x2y1 = new(wUnit * 2, hUnit);
                for (int i = 0; i < X2Y1Names.Length; i++)
                {
                    bmp.CopyTo(x2y1, CreateRect16(i * 2, 2, 2, 1, wUnit, hUnit), Point.Empty);
                    ZipArchiveEntry e = patcher.CreateEntry($"assets/minecraft/textures/painting/{X2Y1Names[i]}.png");
                    x2y1.Save(e.Open());
                }
                Bitmap x1y2 = new(wUnit, hUnit * 2);
                for (int i = 0; i < X1Y2Names.Length; i++)
                {
                    bmp.CopyTo(x1y2, CreateRect16(i, 4, 1, 2, wUnit, hUnit), Point.Empty);
                    ZipArchiveEntry e = patcher.CreateEntry($"assets/minecraft/textures/painting/{X1Y2Names[i]}.png");
                    x1y2.Save(e.Open());
                }
                Bitmap x4y2 = new(wUnit * 4, hUnit * 2);
                for (int i = 0; i < X4Y2Names.Length; i++)
                {
                    bmp.CopyTo(x4y2, CreateRect16(i * 4, 6, 4, 2, wUnit, hUnit), Point.Empty);
                    ZipArchiveEntry e = patcher.CreateEntry($"assets/minecraft/textures/painting/{X4Y2Names[i]}.png");
                    x4y2.Save(e.Open());
                }
                Bitmap x2y2 = new(wUnit * 2, hUnit * 2);
                for (int i = 0; i < X2Y2Names.Length; i++)
                {
                    bmp.CopyTo(x2y2, CreateRect16(i * 2, 8, 2, 2, wUnit, hUnit), Point.Empty);
                    ZipArchiveEntry e = patcher.CreateEntry($"assets/minecraft/textures/painting/{X2Y2Names[i]}.png");
                    x2y2.Save(e.Open());
                }
                Bitmap x4y4 = new(wUnit * 4, hUnit * 4);
                for (int i = 0; i < X4Y4Names.Length; i++)
                {
                    bmp.CopyTo(x4y4, CreateRect16(i * 4, 12, 4, 4, wUnit, hUnit), Point.Empty);
                    ZipArchiveEntry e = patcher.CreateEntry($"assets/minecraft/textures/painting/{X4Y4Names[i]}.png");
                    x4y4.Save(e.Open());
                }
                Bitmap x4y3 = new(wUnit * 4, hUnit * 3);
                for (int i = 0; i < X4Y3Names.Length; i++)
                {
                    bmp.CopyTo(x4y3, CreateRect16(12, 4 + i * 3, 4, 3, wUnit, hUnit), Point.Empty);
                    ZipArchiveEntry e = patcher.CreateEntry($"assets/minecraft/textures/painting/{X4Y3Names[i]}.png");
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
                ZipArchiveEntry targetPng = patcher.CreateEntry("assets/minecraft/textures/mcpatcher.png");
                target.Save(targetPng.Open());
                ZipArchiveEntry targetDefaultJson = patcher.CreateEntry("assets/minecraft/font/default.json");
                using (Stream s = targetDefaultJson.Open())
                    s.Write(@"{
    ""providers"": [
        {
            ""type"": ""reference"",
            ""id"": ""minecraft:include/space""
        },
        {
            ""type"": ""bitmap"",
            ""file"": ""minecraft:mcpatcher.png"",
            ""ascent"": 7,
            ""chars"": [
                ""\u9260\u9261\u9262\u9263\u9264\u9265\u9266\u9267\u9268\u9269\u926a\u926b\u926c\u926d\u926e\u926f"",
                ""\u9270\u9271\u9272\u9273\u9274\u9275\u9276\u9277\u9278\u9279\u927a\u927b\u927c\u927d\u927e\u927f"",
                ""\u9280\u9281\u9282\u9283\u9284\u9285\u9286\u9287\u9288\u9289\u928a\u928b\u928c\u928d\u928e\u928f"",
                ""\u9290\u9291\u9292\u9293\u9294\u9295\u9296\u9297\u9298\u9299\u929a\u929b\u929c\u929d\u929e\u929f""
            ]
        },
        {
            ""type"": ""reference"",
            ""id"": ""minecraft:include/default"",
            ""filter"": {
                ""uniform"": false
            }
        },
        {
            ""type"": ""reference"",
            ""id"": ""minecraft:include/unifont""
        }
    ]
}"u8);
                ZipArchiveEntry targetUniformJson = patcher.CreateEntry("assets/minecraft/font/uniform.json");
                using (Stream s = targetUniformJson.Open())
                    s.Write(@"{
    ""providers"": [
        {
            ""type"": ""reference"",
            ""id"": ""minecraft:include/space""
        },
        {
            ""type"": ""bitmap"",
            ""file"": ""minecraft:mcpatcher.png"",
            ""ascent"": 7,
            ""chars"": [
                ""\u9260\u9261\u9262\u9263\u9264\u9265\u9266\u9267\u9268\u9269\u926a\u926b\u926c\u926d\u926e\u926f"",
                ""\u9270\u9271\u9272\u9273\u9274\u9275\u9276\u9277\u9278\u9279\u927a\u927b\u927c\u927d\u927e\u927f"",
                ""\u9280\u9281\u9282\u9283\u9284\u9285\u9286\u9287\u9288\u9289\u928a\u928b\u928c\u928d\u928e\u928f"",
                ""\u9290\u9291\u9292\u9293\u9294\u9295\u9296\u9297\u9298\u9299\u929a\u929b\u929c\u929d\u929e\u929f""
            ]
        },
        {
            ""type"": ""reference"",
            ""id"": ""minecraft:include/unifont""
        }
    ]
}"u8);
            }
            zip?.Dispose();
            Console.Error.WriteLine("Done!");
            return;
        }
    }
    static Rectangle CreateRect16(int x, int y, int width, int height, int wUnit, int hUnit)
    {
        return new(x * wUnit, y * hUnit, width * wUnit, height * hUnit);
    }
}