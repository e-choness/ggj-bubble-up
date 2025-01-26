
using System.Collections.Generic;

public static class ListExtensions
{
    public static List<T> Shuffle<T>(this List<T> list)
    {
        List<T> result = new List<T>(list);
        System.Random rng = new System.Random();
        int n = result.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            T value = result[k];  
            result[k] = result[n];  
            result[n] = value;  
        }
        return result;
    }
}
