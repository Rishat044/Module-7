using System;

public interface IPaymentStrategy
{
    void Pay(decimal amount);
}

public class CreditCardPayment : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Оплата банковской картой на сумму {amount} тенге выполнена.");
    }
}

public class PayPalPayment : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Оплата через PayPal на сумму {amount} тенге выполнена.");
    }
}

public class CryptoPayment : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Оплата криптовалютой на сумму {amount} тенге выполнена.");
    }
}

public class PaymentContext
{
    private IPaymentStrategy _paymentStrategy;

    public void SetPaymentStrategy(IPaymentStrategy paymentStrategy)
    {
        _paymentStrategy = paymentStrategy;
    }

    public void Pay(decimal amount)
    {
        if (_paymentStrategy == null)
        {
            Console.WriteLine("Стратегия оплаты не выбрана.");
        }
        else
        {
            _paymentStrategy.Pay(amount);
        }
    }
}





class Program
{
    static void Main(string[] args)
    {
        PaymentContext paymentContext = new PaymentContext();
        
        Console.WriteLine("Выберите способ оплаты: ");
        Console.WriteLine("1. Банковская карта");
        Console.WriteLine("2. PayPal");
        Console.WriteLine("3. Криптовалюта");

        string choice = Console.ReadLine();
        decimal amount = 1000.00m;  

        switch (choice)
        {
            case "1":
                paymentContext.SetPaymentStrategy(new CreditCardPayment());
                break;
            case "2":
                paymentContext.SetPaymentStrategy(new PayPalPayment());
                break;
            case "3":
                paymentContext.SetPaymentStrategy(new CryptoPayment());
                break;
            default:
                Console.WriteLine("Неверный выбор.");
                return;
        }

        paymentContext.Pay(amount);
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// наблюдатель

using System;
using System.Collections.Generic;
using System.IO;


public interface IObserver
{
    void Update(string currency, decimal rate);
}

public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer); 
    void Notify();
}

public class CurrencyExchange : ISubject
{
    private Dictionary<string, decimal> _currencyRates = new Dictionary<string, decimal>();
    private List<IObserver> _observers = new List<IObserver>();

    public void Attach(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify()
    {
        foreach (var observer in _observers)
        {
            foreach (var currency in _currencyRates)
            {
                observer.Update(currency.Key, currency.Value);
            }
        }
    }

    public void UpdateCurrencyRate(string currency, decimal rate)
    {
        _currencyRates[currency] = rate;
        Console.WriteLine($"Курс валюты {currency} обновлен до {rate}");
        Notify();
    }
}

public class ConsoleObserver : IObserver
{
    public void Update(string currency, decimal rate)
    {
        Console.WriteLine($"[ConsoleObserver] Курс {currency} теперь {rate}");
    }
}

public class FileObserver : IObserver
{
    private string _filePath;

    public FileObserver(string filePath)
    {
        _filePath = filePath;
    }

    public void Update(string currency, decimal rate)
    {
        File.AppendAllText(_filePath, $"Курс {currency}: {rate}\n");
        Console.WriteLine($"[FileObserver] Курс {currency} записан в файл.");
    }
}

public class EmailObserver : IObserver
{
    private string _email;

    public EmailObserver(string email)
    {
        _email = email;
    }

    public void Update(string currency, decimal rate)
    {
        Console.WriteLine($"[EmailObserver] Отправлено уведомление на {_email}: Курс {currency} изменился на {rate}");
    }
}