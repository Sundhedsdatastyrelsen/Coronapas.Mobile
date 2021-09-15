namespace SSICPAS.Core.Interfaces
{
    public interface IRandomService
    {
        double GenerateRandomDouble();
        string GenerateRandomString(int length);
    }
}
