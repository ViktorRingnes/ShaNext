using ShaNext.ShaNext;

namespace ShaNext
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string path = "C:\\Users\\47488\\source\\repos\\ShaNext\\e.png";
            string fileHash = await ShaNextHashing.HashFileAsync(path);
            bool compare = await ShaNextCompare.VerifyFileHashAsync(path, fileHash);
            Console.WriteLine(compare);

            string input = "testInput";
            string hash = await ShaNextHashing.HashAsync(input);
            Console.WriteLine($"Hash: {hash}");

            string salt = ShaNextSalt.NewSalt();
            string saltedHash = await ShaNextHashing.HashWithSaltAsync(input, salt);
            Console.WriteLine($"Salted Hash: {saltedHash}");

            string saltedHashStored = await ShaNextHashing.GenerateSaltedHashAsync(input);
            bool verifySaltedHash = await ShaNextCompare.VerifySaltedHashAsync(input, saltedHashStored);
            Console.WriteLine($"Verify Salted Hash: {verifySaltedHash}");
        }

    }
}
