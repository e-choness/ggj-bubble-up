
using System.Collections.Generic;

public static class ListExtensions
{
    private static Dictionary<uint, object> previousRandom = new();

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

    public static T Random<T>(this List<T> list)
    {
        if (list.Count == 0) return default;
        System.Random rng = new System.Random();
        return list[rng.Next(list.Count)];
    }

    /// <summary>
    /// Get a random item from the list, making sure it is not the same that was picked just prior from the same ID.
    /// </summary>
    public static T RandomNoRepeat<T>(this List<T> list, uint ID)
    {
        T random = list.Random();
        if (previousRandom.ContainsKey(ID)) // not the first time
        {
            if (list.Count <= 2) throw new System.Exception("Cannot get random without repeats because the given list is not long enough");
            while (random.Equals((T)previousRandom[ID])) random = list.Random();
            previousRandom[ID] = random;
            return random;
        }
        else previousRandom.Add(ID, random);
        return random;
    }
}
