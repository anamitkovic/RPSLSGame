namespace RPSLSGame.Application.Interfaces;

public interface IRandomNumberService
{
    Task<int> GetRandomNumberAsync();
}
