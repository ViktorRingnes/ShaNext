using ShaNext.ShaNext;

namespace ShaNext
{
    class Program
    {
        static void Main(string[] args)
        {
            string hashedPassword = ShaNextHashing.Hash("blaze");
            Console.WriteLine(hashedPassword);

            bool compare = ShaNextCompare.Compare("blaz", "a19bd8e875df8cbd502d02406454da64354258b454c1d6f89df8ffb90aeb560d05318c81dfdf52ab1d9d208b8a0abf0c205ae46f3f347742db342a5398f034a8");
            Console.WriteLine(compare);

            string salt = ShaNextSalt.Salt();
            Console.WriteLine(salt);
        }

    }
}
