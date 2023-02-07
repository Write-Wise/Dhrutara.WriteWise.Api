namespace Dhrutara.WriteWise.Providers
{
    public interface IHashProvider
    {
        string ComputeSha256Hash(string text);
    }
}
