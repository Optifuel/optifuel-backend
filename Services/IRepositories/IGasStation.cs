namespace Api.Services
{
    public interface IGasStation 
    {
        Task<string> UpdateGasStation();
        void UpdateGasStationPrice();

    }
}
