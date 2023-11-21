namespace ApiCos.Services
{
    public interface IGasStation 
    {
        Task<string> UpdateGasStation();
        void UpdateGasStationPrice();

    }
}
