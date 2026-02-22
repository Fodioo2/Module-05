using System;
using System.Collections.Generic;

public class Product : ICloneable
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public object Clone()
    {
        return new Product
        {
            Name = this.Name,
            Price = this.Price,
            Quantity = this.Quantity
        };
    }

    public override string ToString()
        => $"{Name} x{Quantity} ({Price} each)";
}


public class Discount : ICloneable
{
    public string Title { get; set; }
    public decimal Amount { get; set; } 

    public object Clone()
    {
        return new Discount
        {
            Title = this.Title,
            Amount = this.Amount
        };
    }

    public override string ToString()
        => $"{Title}: -{Amount}";
}


public class Order : ICloneable
{
    public List<Product> Products { get; set; } = new List<Product>();
    public decimal ShippingCost { get; set; }
    public List<Discount> Discounts { get; set; } = new List<Discount>();
    public string PaymentMethod { get; set; }


    public object Clone()
    {
        Order copy = new Order();
        copy.ShippingCost = this.ShippingCost;
        copy.PaymentMethod = this.PaymentMethod;

        foreach (var p in this.Products)
            copy.Products.Add((Product)p.Clone());

        foreach (var d in this.Discounts)
            copy.Discounts.Add((Discount)d.Clone());

        return copy;
    }

    public decimal GetTotal()
    {
        decimal sum = 0;

        foreach (var p in Products)
            sum += p.Price * p.Quantity;

        sum += ShippingCost;

        foreach (var d in Discounts)
            sum -= d.Amount;

        return sum;
    }

    public void Print(string title)
    {
        Console.WriteLine($"===== {title} =====");
        Console.WriteLine($"Payment: {PaymentMethod}");
        Console.WriteLine($"Shipping: {ShippingCost}");
        Console.WriteLine("Products:");
        foreach (var p in Products)
            Console.WriteLine(" - " + p);

        Console.WriteLine("Discounts:");
        if (Discounts.Count == 0) Console.WriteLine(" - (none)");
        foreach (var d in Discounts)
            Console.WriteLine(" - " + d);

        Console.WriteLine($"TOTAL = {GetTotal()}");
        Console.WriteLine();
    }
}

class Program
{
    static void Main()
    {
        Order baseOrder = new Order();
        baseOrder.Products.Add(new Product { Name = "Laptop", Price = 1000m, Quantity = 1 });
        baseOrder.Products.Add(new Product { Name = "Mouse", Price = 20m, Quantity = 2 });

        baseOrder.ShippingCost = 15m;
        baseOrder.Discounts.Add(new Discount { Title = "Welcome discount", Amount = 30m });
        baseOrder.PaymentMethod = "Card";

        baseOrder.Print("BASE ORDER");
        Order order2 = (Order)baseOrder.Clone();
        order2.PaymentMethod = "Cash";
        order2.Discounts.Add(new Discount { Title = "Extra discount", Amount = 10m });

        order2.Products[0].Quantity = 2; 

        order2.Print("CLONED ORDER (modified)");

        baseOrder.Print("BASE ORDER (after clone changes)");
    }
}