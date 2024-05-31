namespace ShaNext.ShaNext
{
    public interface IHashAlgorithm
    {
        byte[] ComputeHash(string input);
    }
}
