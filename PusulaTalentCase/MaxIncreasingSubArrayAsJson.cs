using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public static class MaxIncreasingSubArray
{
    // Ardışık ve sıkı artan alt diziler içinde toplamı en yüksek olanı JSON olarak döndürür.s
    public static string MaxIncreasingSubArrayAsJson(List<int> numbers)
    {
        if (numbers?.Any() != true)
        {
            return JsonSerializer.Serialize(Array.Empty<int>());
        }

        int bestSubarrayStart = 0;
        int bestSubarrayLength = 1;
        long bestSubarraySum = numbers[0];

        int currentStartIndex = 0;
        int currentLength = 1;
        long currentSum = numbers[0];

        for (int i = 1; i < numbers.Count; i++)
        {
            if (numbers[i] > numbers[i - 1])
            {
                currentLength++;
                currentSum += numbers[i];
            }
            else
            {
                UpdateBestIfNeeded(ref bestSubarrayStart, ref bestSubarrayLength, ref bestSubarraySum, currentStartIndex, currentLength, currentSum);
                currentStartIndex = i;
                currentLength = 1;
                currentSum = numbers[i];
            }
        }

        UpdateBestIfNeeded(ref bestSubarrayStart, ref bestSubarrayLength, ref bestSubarraySum, currentStartIndex, currentLength, currentSum);

        var bestSubArray = numbers.GetRange(bestSubarrayStart, bestSubarrayLength);
        return JsonSerializer.Serialize(bestSubArray);
    }

    private static void UpdateBestIfNeeded(ref int bestStartIndex, ref int bestLength, ref long bestSum,
                                           int candidateStart, int candidateLength, long candidateSum)
    {
        bool isBetter = candidateSum > bestSum || (candidateSum == bestSum && candidateLength > bestLength);
        if (isBetter)
        {
            bestStartIndex = candidateStart;
            bestLength = candidateLength;
            bestSum = candidateSum;
        }
    }
}

