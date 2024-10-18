using System;

public interface ICostCalculationStrategy
{
    decimal CalculateCost(TripDetails details);
}

public class TripDetails
{
    public decimal Distance { get; set; }
    public int Passengers { get; set; }
    public string ServiceClass { get; set; }
    public bool HasChildDiscount { get; set; }
    public bool HasSeniorDiscount { get; set; }
    public bool HasLuggage { get; set; }
    public decimal BasePricePerKm { get; set; }
}

public class AirplaneCostStrategy : ICostCalculationStrategy
{
    public decimal CalculateCost(TripDetails details)
    {
        decimal baseCost = details.Distance * details.BasePricePerKm * 1.5m;
        baseCost = ApplyServiceClassMultiplier(details, baseCost);
        baseCost = ApplyDiscounts(details, baseCost);

        if (details.HasLuggage) baseCost += 50m;
        return baseCost * details.Passengers;
    }

    private decimal ApplyServiceClassMultiplier(TripDetails details, decimal baseCost)
    {
        if (details.ServiceClass == "Бизнес") return baseCost * 1.5m;
        return baseCost;
    }

    private decimal ApplyDiscounts(TripDetails details, decimal baseCost)
    {
        if (details.HasChildDiscount) baseCost *= 0.8m;
        if (details.HasSeniorDiscount) baseCost *= 0.85m;
        return baseCost;
    }
}



public class TrainCostStrategy : ICostCalculationStrategy
{
    public decimal CalculateCost(TripDetails details)
    {
        decimal baseCost = details.Distance * details.BasePricePerKm;
        baseCost = ApplyServiceClassMultiplier(details, baseCost);
        baseCost = ApplyDiscounts(details, baseCost);
        return baseCost * details.Passengers;
    }

    private decimal ApplyServiceClassMultiplier(TripDetails details, decimal baseCost)
    {
        if (details.ServiceClass == "Бизнес") return baseCost * 1.3m;
        return baseCost;
    }

    private decimal ApplyDiscounts(TripDetails details, decimal baseCost)
    {
        if (details.HasChildDiscount) baseCost *= 0.9m;
        if (details.HasSeniorDiscount) baseCost *= 0.9m;
        return baseCost;
    }
}

public class BusCostStrategy : ICostCalculationStrategy
{
    public decimal CalculateCost(TripDetails details)
    {
        decimal baseCost = details.Distance * details.BasePricePerKm * 0.8m;
        baseCost = ApplyDiscounts(details, baseCost);
        return baseCost * details.Passengers;
    }

    private decimal ApplyDiscounts(TripDetails details, decimal baseCost)
    {
        if (details.HasChildDiscount) baseCost *= 0.85m;
        if (details.HasSeniorDiscount) baseCost *= 0.9m;
        return baseCost;
    }
}

public class TravelBookingContext
{
    private ICostCalculationStrategy _costStrategy;

    public void SetCostStrategy(ICostCalculationStrategy strategy)
    {
        _costStrategy = strategy;
    }

    public decimal CalculateTripCost(TripDetails details)
    {
        if (_costStrategy == null)
            throw new InvalidOperationException("Стратегия расчета стоимости не установлена");

        return _costStrategy.CalculateCost(details);
    }
}

class Program
{
    static void Main(string[] args)
    {
        TripDetails trip = new TripDetails
        {
            Distance = 1000m,
            Passengers = 2,
            ServiceClass = "Эконом",
            HasChildDiscount = true,
            HasSeniorDiscount = false,
            HasLuggage = true,
            BasePricePerKm = 5m
        };

        TravelBookingContext context = new TravelBookingContext();

        context.SetCostStrategy(new AirplaneCostStrategy());
        decimal airplaneCost = context.CalculateTripCost(trip);
        Console.WriteLine($"Стоимость поездки на самолете: {airplaneCost} тг.");

        context.SetCostStrategy(new TrainCostStrategy());
        decimal trainCost = context.CalculateTripCost(trip);
        Console.WriteLine($"Стоимость поездки на поезде: {trainCost} тг.");

        context.SetCostStrategy(new BusCostStrategy());
        decimal busCost = context.CalculateTripCost(trip);
        Console.WriteLine($"Стоимость поездки на автобусе: {busCost} тг.");
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IObserver
{
    void Update(string stockSymbol, decimal price);
}

public interface ISubject
{
    void RegisterObserver(IObserver observer, string stockSymbol);
    void RemoveObserver(IObserver observer, string stockSymbol);
    void NotifyObservers(string stockSymbol);
}

public class StockExchange : ISubject
{
    private readonly Dictionary<string, List<IObserver>> _observers;
    private readonly Dictionary<string, decimal> _stockPrices;

    public StockExchange()
    {
        _observers = new Dictionary<string, List<IObserver>>();
        _stockPrices = new Dictionary<string, decimal>();
    }

    public void RegisterObserver(IObserver observer, string stockSymbol)
    {
        if (!_observers.ContainsKey(stockSymbol))
        {
            _observers[stockSymbol] = new List<IObserver>();
        }
        _observers[stockSymbol].Add(observer);
        Console.WriteLine($"Наблюдатель добавлен для акции: {stockSymbol}");
    }

    public void RemoveObserver(IObserver observer, string stockSymbol)
    {
        if (_observers.ContainsKey(stockSymbol))
        {
            _observers[stockSymbol].Remove(observer);
            Console.WriteLine($"Наблюдатель удален для акции: {stockSymbol}");
        }
    }

    public void NotifyObservers(string stockSymbol)
    {
        if (_observers.ContainsKey(stockSymbol))
        {
            foreach (var observer in _observers[stockSymbol])
            {
                observer.Update(stockSymbol, _stockPrices[stockSymbol]);
            }
        }
    }

    public void UpdateStockPrice(string stockSymbol, decimal newPrice)
    {
        if (_stockPrices.ContainsKey(stockSymbol))
        {
            _stockPrices[stockSymbol] = newPrice;
        }
        else
        {
            _stockPrices.Add(stockSymbol, newPrice);
        }
        Console.WriteLine($"Цена акции {stockSymbol} обновлена до {newPrice}");
        NotifyObservers(stockSymbol);
    }
}

public class Trader : IObserver
{
    private readonly string _name;

    public Trader(string name)
    {
        _name = name;
    }

    public void Update(string stockSymbol, decimal price)
    {
        Console.WriteLine($"{_name} получил уведомление: Акция {stockSymbol} теперь стоит {price} тг.");
    }
}

public class TradingBot : IObserver
{
    private readonly string _name;
    private readonly decimal _threshold;

    public TradingBot(string name, decimal threshold)
    {
        _name = name;
        _threshold = threshold;
    }


    public void Update(string stockSymbol, decimal price)
    {
        if (price > _threshold)
        {
            Console.WriteLine($"{_name}: Продажа акции {stockSymbol}, так как цена {price} выше порога {_threshold} тг.");
        }
        else
        {
            Console.WriteLine($"{_name}: Покупка акции {stockSymbol}, так как цена {price} ниже порога {_threshold} тг.");
        }
    }
}




class Program
{
    static async Task Main(string[] args)
    {

        StockExchange stockExchange = new StockExchange();

        Trader trader1 = new Trader("Трейдер Асыл");
        Trader trader2 = new Trader("Трейдер Влад");
        TradingBot bot1 = new TradingBot("Робот Alpha", 150m);

        stockExchange.RegisterObserver(trader1, "AAPL");
        stockExchange.RegisterObserver(trader2, "AAPL");
        stockExchange.RegisterObserver(bot1, "GOOG");

        stockExchange.UpdateStockPrice("AAPL", 145m);
        stockExchange.UpdateStockPrice("GOOG", 160m);

        stockExchange.RemoveObserver(trader2, "AAPL");


        stockExchange.UpdateStockPrice("AAPL", 155m);
        stockExchange.UpdateStockPrice("GOOG", 140m);

        await Task.Delay(2000);
        stockExchange.UpdateStockPrice("AAPL", 165m);
    }
}
