//Name: Jordan Sukhnandan
//Activity 4

using System;
public interface IHouse
{
    float Price { get; set; }
    string Location { get; }
    int NumberOfRooms { get; set; }

    void RentRoom(int numRenters, int numRoomsToRent);

    void ReceiveRent(Money rent);

    //Event
    event EventHandler RentReceived;
}

public class Money
{
    public float Amount;
};

class Rentee<T>
{
    private T[] arr = new T[100];

    public T this[int i]
    {
        get { return arr[i]; }
        set { arr[i] = value; }
    }
}

class AHouse: IHouse
{
    private float price; //price of the house
    private float rentOwed = 700;
    private readonly string location;
    private int numberOfRooms;
    private int numberOfPeople;
    private int numAvailableRooms;
    public float Price 
    {
        get { return price; }
        set { price = value; }
    }

    private float RentOwed
    {
        get { return rentOwed; }
        set { rentOwed = value; }
    }

    public string Location
    {
        get { return location; }
    }

    public int NumberOfRooms
    {
        get { return numberOfRooms; }
        set { numberOfRooms = value;}
    }

    public int NumberOfPeople
    {
        get { return numberOfPeople; }
        set { numberOfPeople = value;}
    }

    public int NumAvailableRooms
    {
        get { return numAvailableRooms; }
        set 
        { 
            if(value <= numberOfRooms -1 && value >= 0) // at least 1 room belongs to the house owner
                numAvailableRooms = value;
            else
            {
                Console.WriteLine("At least one room must belong to the house owner.");
            }
        }
    }

    public void RentRoom(int numRenters, int numRoomsToRent)
    {
        if(numAvailableRooms >= numRoomsToRent)
        {
            numberOfPeople += numRenters;
            numAvailableRooms -= numRoomsToRent;
        }
        else
        {
            Console.WriteLine("There are not enough rooms to rent this house.");
        }
    }

    //Events
    event EventHandler PreRentReceived;
    event EventHandler PostRentReceived;
    object objectLock = new Object();

    event EventHandler IHouse.RentReceived
    {
        add
        {
            lock (objectLock)
            {
                PreRentReceived += value;
            }
        }
        remove
        {
            lock(objectLock)
            {
                PreRentReceived -= value;
            }
        }
    }

    public void ReceiveRent(Money rent)
    {
        if(rent.Amount == rentOwed)
        {
            PreRentReceived?.Invoke(this, EventArgs.Empty);
            Console.WriteLine($"$'{rent.Amount}' is being transferred, please wait...");
            Thread.Sleep(4000);
            rentOwed = 0;    
            PostRentReceived?.Invoke(this, EventArgs.Empty);
        }
        else if(rent.Amount < rentOwed)
        {
            rentOwed -= rent.Amount;
            Console.WriteLine($"You still owe $'{rent.Amount}' for this month.");
        }
    }
}

class Renter
{
    public Renter(AHouse house)
    {
        IHouse h = (IHouse)house;
        h.RentReceived += OnRentReceived;
    }
    public void OnRentReceived(object source, EventArgs eventArgs)
    {
        Console.WriteLine("Your rent money has been received!");
    }
}

class Program
{
    static void Main(string[] args)
    {
        var rent = new Money { Amount = 700 };
        var house = new AHouse();
        var renter = new Renter(house);
        var rentee = new Rentee<string>();

        rentee[0] = "John";
        rentee[1] = "Mary";
        rentee[2] = "Smith";

        house.ReceiveRent(rent);
        Console.ReadKey();
    }
}

