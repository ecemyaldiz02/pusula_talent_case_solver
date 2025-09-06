using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class LongestVowelSubsequenceSolver
{
    private static readonly HashSet<char> Vowels = new HashSet<char>(new[] { 'a', 'e', 'i', 'o', 'u' });

    private record OutputRow(
      [property: JsonPropertyName("word")] string Word,
      [property: JsonPropertyName("sequence")] string Sequence,
      [property: JsonPropertyName("length")] int Length
    );

    // Her kelime için ARDIŞIK seslilerden en uzun alt diziyi bul ve JSON döndür
    public static string LongestVowelSubsequenceAsJson(List<string> words)
    {
        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        if (words?.Any() != true)
        {
            return JsonSerializer.Serialize(new List<OutputRow>(), jsonOptions);
        }

        var records = words.Select(word =>
        {
            if (string.IsNullOrEmpty(word))
            {
                return new OutputRow(word ?? string.Empty, string.Empty, 0);
            }

            int? bestStart = null; int bestLen = 0;
            int? currStart = null; int currLen = 0;
            for (int i = 0; i < word.Length; i++)
            {
                char ch = char.ToLowerInvariant(word[i]);
                if (Vowels.Contains(ch))
                {
                    if (!currStart.HasValue) currStart = i;
                    currLen++;
                }
                else
                {
                    UpdateBestSegment(ref bestStart, ref bestLen, currStart, currLen);
                    currStart = null; currLen = 0;
                }
            }
            UpdateBestSegment(ref bestStart, ref bestLen, currStart, currLen);

            string seq = bestLen > 0 && bestStart.HasValue ? word.Substring(bestStart.Value, bestLen) : string.Empty;
            return new OutputRow(word, seq, bestLen);
        }).ToList();

        return JsonSerializer.Serialize(records, jsonOptions);
    }

    private static void UpdateBestSegment(ref int? bestStart, ref int bestLen, int? currStart, int currLen)
    {
        if (currLen > bestLen)
        {
            bestLen = currLen;
            bestStart = currStart;
        }
    }
}


