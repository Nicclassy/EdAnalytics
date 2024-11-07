namespace Ed.Analytics.Models;

public readonly record struct ThreadNumber(int Value)
{
    public static implicit operator ThreadNumber(int value) => Parse(value);
    
    public static ThreadNumber Parse(int? value) 
    {
        if (value is not int number)
            throw new ArgumentException("value cannot be null");
    
        if (int.IsNegative(number))
            throw new ArgumentException("thread numbers must be positive");

        return new(number);
    }
}