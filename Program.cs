using ShaNext.ShaNext;

namespace ShaNext
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            string hashedPassword = ShaNextHashing.Hash("blaze");
            Console.WriteLine(hashedPassword);

            bool compare = ShaNextCompare.Compare("blaze", "88a5c49d8cb7f295df955627440db159fecd962f91d56017b451fc9d6facd79f");
            Console.WriteLine(compare);

            string salt = ShaNextSalt.NewSalt();
            Console.WriteLine(salt);

            string hashedWithSalt = ShaNextHashing.HashWithSalt("blaze", "gfeDi3DEEAUzJ9rnzyBdlIxqHGYr7B1Huy9VijHe5ta+RZCsfdPJr0fXsIwBT6q99vp+OYFQEhM3kgWNossiiA==");
            Console.WriteLine(hashedWithSalt);

             bool compareSalt = ShaNextCompare.CompareWithSalt("blaze", "gfeDi3DEEAUzJ9rnzyBdlIxqHGYr7B1Huy9VijHe5ta+RZCsfdPJr0fXsIwBT6q99vp+OYFQEhM3kgWNossiiA==", "6cb8ef8747b3c610e734b42d317cf6e246ecf6d109b7a6623a5303e0d2ac7e7d");
            Console.WriteLine(compareSalt);
            */

            string hashedSalt = ShaNextHashing.GenerateSaltedHash("test");
            Console.WriteLine(hashedSalt);

            string filePath = "C:\\Users\\47488\\source\\repos\\ShaNext\\e.png";
            string fileHash = ShaNextHashing.HashFile(filePath);
            Console.WriteLine(fileHash);

            bool test2 = ShaNextCompare.VerifyFileHash(filePath, "6caaa4f1ffa8ab5893e4accba66358c48afa7f7de67830f5e5cb99278ea837a8");
            Console.WriteLine(test2);

        }

    }
}
