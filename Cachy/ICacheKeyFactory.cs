namespace Cachy;
public interface ICacheKeyFactory<T>
{
    string GenerateKey();
}