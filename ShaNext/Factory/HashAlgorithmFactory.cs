using System;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace ShaNext.ShaNext
{
    public class HashAlgorithmFactory
    {
        private static string configFilePath = "hash_config.json";
        private static string defaultConfig = "{\"default_algorithm\": \"SHA_256\"}";

        static HashAlgorithmFactory()
        {
            EnsureConfigFileExists();
        }

        public static IHashAlgorithm Create()
        {
            Config config;
            if (File.Exists(configFilePath))
            {
                var nullcheck = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFilePath));
                if(nullcheck == null)
                {
                    config = new Config();
                }
                else
                {
                    config = nullcheck;
                }
            }
            else
            {
                using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ShaNext.hash_config.json")!;
                using StreamReader reader = new StreamReader(stream);

                var nullcheck = JsonConvert.DeserializeObject<Config>(reader.ReadToEnd());
                if (nullcheck == null)
                {
                    config = new Config();
                }
                else
                {
                    config = nullcheck;
                }
            }

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
                "SCRYPT" => new Scrypt(),
                _ => throw new InvalidOperationException("Unknown hashing algorithm, read the Default Configuration section of the README for all available hashing algorithms."),
            };
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
            public string? DefaultAlgorithm { get; set; }
        }
    }
}
