namespace FinanceService.Domain.Entities;

public class Currency
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public decimal Rate { get; private set; }

    public Currency(int id, string name, decimal rate)
    {
        Id = id;
        Name = name;
        Rate = rate;
    }
}
