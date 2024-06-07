using Newtonsoft.Json;
using ShaNext.ShaNext;

public class HashAlgorithmFactory
{
    private static readonly string configFilePath = "hash_config.json";
    private static readonly string defaultConfig = "{\"default_algorithm\": \"SHA_256\"}";

    static HashAlgorithmFactory()
    {
        EnsureConfigFileExists();
    }

    public static IHashAlgorithm Create()
    {
        Config config = LoadConfig();
        return config.DefaultAlgorithm!.ToUpper() switch
        {
            "SHA_256" => new SHA_256(),
            "SHA_1" => new SHA_1(),
            "SHA_224" => new SHA_224(),
            "SHA_384" => new SHA_384(),
            "SHA_512" => new SHA_512(),
            "SHA_512_224" => new SHA_512_224(),
            "SHA_512_256" => new SHA_512_256(),
            "SHA_3" => new SHA_3(),
            "SHAKE128" => new SHAKE128(),
            "MD5" => new MD5(),
            "WHIRLPOOL" => new Whirlpool(),
            "RIPEMD_160" => new RIPEMD_160(),
            "ARGON2" => new Argon2(),
            _ => throw new InvalidOperationException("Unknown hashing algorithm. Check the configuration."),
        };
    }

    private static Config LoadConfig()
    {
        if (!File.Exists(configFilePath))
        {
            return JsonConvert.DeserializeObject<Config>(defaultConfig) ?? new Config();
        }

        var configContent = File.ReadAllText(configFilePath);
        return JsonConvert.DeserializeObject<Config>(configContent) ?? new Config();
    }

    private static void EnsureConfigFileExists()
    {
        if (!File.Exists(configFilePath))
        {
            File.WriteAllText(configFilePath, defaultConfig);
        }
    }

    private class Config
    {
        [JsonProperty("default_algorithm")]
        public string? DefaultAlgorithm { get; set; } = "SHA_256";
    }
}
