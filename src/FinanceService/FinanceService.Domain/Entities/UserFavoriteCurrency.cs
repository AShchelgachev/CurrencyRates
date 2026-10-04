namespace FinanceService.Domain.Entities;

public class UserFavoriteCurrency
{
    public int UserId { get; private set; }

    public int CurrencyId { get; private set; }

    public UserFavoriteCurrency(int userId, int currencyId)
    {
        UserId = userId;
        CurrencyId = currencyId;
    }
}
