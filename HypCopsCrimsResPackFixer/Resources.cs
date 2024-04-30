using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypCopsCrimsResPackFixer
{
    public readonly ref struct Resources
    {
        public Resources() { }
        public const string Pattern = "*.*";
        public const string Keyword = "Cops and Crims\\n©2018 Hypixel, Inc.";
        public const string MessageEnterMinecraftFolder = "Enter .minecraft folder";
        public const string MessageEnterMinecraftFolderHint = "(Leave space to use \"{0}\")";
        public const string MessageFolderNotExist = "Folder not exist";
        public const string MessageEnterPackName = "Enter pack name to save:";
        public const string MessageEnterPackNameHint = "(Leave space to use \"{0}\")";
        public const string MessageDone = "Done!";
        public const string Downloads = "downloads";
        public const string Resourcepacks = "resourcepacks";
        public const string McmetaPath = "pack.mcmeta";
        public const string LegacyPaintingPath = "assets/minecraft/textures/painting/paintings_kristoffer_zetterstrand.png";
        public const string LegacyFontPath = "assets/minecraft/mcpatcher/font/unicode_page_92.png";
        public const string DefaultPackName = "HypCopsCrims_Patch.zip";
        public const string ShaderPath = "assets/minecraft/shaders/core/rendertype_entity_solid.fsh";
        public const string PaintingBackPath = "assets/minecraft/textures/painting/back.png";
        public const string PaintingPath = "assets/minecraft/textures/painting/{0}.png";
        public const string FontPath = "assets/minecraft/textures/mcpatcher.png";
        public const string FontDefaultJsonPath = "assets/minecraft/font/default.json";
        public const string FontUniformJsonPath = "assets/minecraft/font/uniform.json";

        public static readonly DirectoryInfo DefaultMCPath = Environment.OSVersion.Platform switch
        {
            PlatformID.Win32NT => new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft")),
            PlatformID.Unix => new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".minecraft")),
            PlatformID.MacOSX => new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library/Application Support/minecraft")),
            _ => throw new NotImplementedException()
        };
        public static readonly ImmutableArray<string> X1Y1Names = ["kebab", "aztec", "alban", "aztec2", "bomb", "plant", "wasteland"];
        public static readonly ImmutableArray<string> X2Y1Names = ["pool", "courbet", "sea", "sunset", "creebet"];
        public static readonly ImmutableArray<string> X1Y2Names = ["wanderer", "graham"];
        public static readonly ImmutableArray<string> X4Y2Names = ["fighters"];
        public static readonly ImmutableArray<string> X2Y2Names = ["match", "bust", "stage", "void", "skull_and_roses", "wither"];
        public static readonly ImmutableArray<string> X4Y4Names = ["pointer", "pigscene", "burning_skull"];
        public static readonly ImmutableArray<string> X4Y3Names = ["skeleton", "donkey_kong"];
        public readonly ReadOnlySpan<byte> Mcmeta = @"{
    ""pack"": {
        ""pack_format"": 32,
        ""supported_formats"": [
            4,
            32
        ],
        ""description"": ""Hypixel Cops and Crims\nResource Patch Pack""
    }
}"u8;
        public readonly ReadOnlySpan<byte> Shader = @"#version 150

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

// Download from https://bugs.mojang.com/browse/MC-164001"u8;
        public readonly ReadOnlySpan<byte> FontDefaultJson = @"{
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
}"u8;
        public readonly ReadOnlySpan<byte> FontUniformJson = @"{
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
}"u8;
    }
}
