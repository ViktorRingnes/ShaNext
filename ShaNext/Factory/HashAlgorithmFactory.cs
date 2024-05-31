using System;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

namespace ShaNext.ShaNext
{
    public class HashAlgorithmFactory
    {
        private static string configFilePath = "hash_config.json";

        public static IHashAlgorithm Create()
        {
            Config config;
            if (File.Exists(configFilePath))
            {
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFilePath));
            }
            else
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ShaNext.hash_config.json"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    config = JsonConvert.DeserializeObject<Config>(reader.ReadToEnd());
                }
            }

            switch (config.DefaultAlgorithm.ToUpper())
            {
                case "SHA_256":
                    return new SHA_256();
                case "SHA_1":
                    return new SHA_1();
                case "SHA_224":
                    return new SHA_224();
                case "SHA_384":
                    return new SHA_384();
                case "SHA_512":
                    return new SHA_512();
                case "SHA_512_224":
                    return new SHA_512_224();
                case "SHA_512_256":
                    return new SHA_512_256();
                case "SHA_3":
                    return new SHA_3();
                case "SHAKE128":
                    return new SHAKE128();
                case "MD5":
                    return new MD5();
                case "WHIRLPOOL":
                    return new Whirlpool();
                case "RIPEMD_160":
                    return new RIPEMD_160();
                default:
                    throw new InvalidOperationException("Unknown hashing algorithm");
            }
        }

        private class Config
        {
            [JsonProperty("default_algorithm")]
            public string DefaultAlgorithm { get; set; }
        }
    }
}
