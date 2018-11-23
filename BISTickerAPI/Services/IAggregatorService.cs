namespace BISTickerAPI.Services
{
    public interface IAggregatorService
    {
        object GetAveragedOuput(string mainCoin, string baseCoin);
        object GetPerExchangeOutput(string mainCoin, string baseCoin);
        void UpdateTickers();
    }
}