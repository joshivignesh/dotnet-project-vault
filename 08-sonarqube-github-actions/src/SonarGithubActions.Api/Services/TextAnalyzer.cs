using System.Text.RegularExpressions;
using SonarGithubActions.Api.Models;

namespace SonarGithubActions.Api.Services;

/// <summary>
/// Analyzes text for readability metrics including Flesch Reading Ease.
/// Demonstrated for SonarCloud code quality analysis.
/// </summary>
public partial class TextAnalyzer
{
    [GeneratedRegex(@"\b\w+\b", RegexOptions.Compiled)]
    private static partial Regex WordPattern();

    [GeneratedRegex(@"[.!?]+", RegexOptions.Compiled)]
    private static partial Regex SentenceEndPattern();

    [GeneratedRegex(@"\n\s*\n", RegexOptions.Compiled)]
    private static partial Regex ParagraphPattern();

    [GeneratedRegex(@"[aeiouAEIOU]", RegexOptions.Compiled)]
    private static partial Regex VowelPattern();

    public TextAnalysisResult Analyze(string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        var words = WordPattern().Matches(content);
        var sentences = SentenceEndPattern().Matches(content);
        var paragraphs = ParagraphPattern().Matches(content);

        var wordCount = words.Count;
        var sentenceCount = Math.Max(1, sentences.Count); // at least 1 sentence
        var paragraphCount = Math.Max(1, paragraphs.Count + 1);

        var avgWordsPerSentence = wordCount == 0 ? 0.0 : Math.Round((double)wordCount / sentenceCount, 2);
        var syllableCount = CountSyllables(words.Select(m => m.Value));

        var flesch = CalculateFleschScore(wordCount, sentenceCount, syllableCount);
        var level = DetermineReadabilityLevel(flesch);

        return new TextAnalysisResult(
            CharacterCount: content.Length,
            WordCount: wordCount,
            SentenceCount: sentenceCount,
            ParagraphCount: paragraphCount,
            AverageWordsPerSentence: avgWordsPerSentence,
            FleschReadingEase: flesch,
            ReadabilityLevel: level
        );
    }

    private static int CountSyllables(IEnumerable<string> words)
    {
        var total = 0;
        foreach (var word in words)
        {
            total += EstimateSyllables(word);
        }
        return Math.Max(1, total);
    }

    public static int EstimateSyllables(string word)
    {
        if (string.IsNullOrEmpty(word)) return 0;

        var lower = word.ToLowerInvariant();
        var count = VowelPattern().Matches(lower).Count;

        // Subtract silent e at end
        if (lower.EndsWith('e') && count > 1)
            count--;

        // Each diphthong/triphthong counts as one
        count -= CountConsecutiveVowelGroups(lower);

        return Math.Max(1, count);
    }

    private static int CountConsecutiveVowelGroups(string word)
    {
        var reductions = 0;
        var inVowelRun = false;
        foreach (var ch in word)
        {
            var isVowel = "aeiou".Contains(ch);
            if (isVowel && inVowelRun) reductions++;
            inVowelRun = isVowel;
        }
        return reductions;
    }

    private static double CalculateFleschScore(int words, int sentences, int syllables)
    {
        if (words == 0) return 0;
        var score = 206.835
            - 1.015 * ((double)words / sentences)
            - 84.6 * ((double)syllables / words);
        return Math.Round(Math.Clamp(score, 0, 100), 1);
    }

    private static string DetermineReadabilityLevel(double score) => score switch
    {
        >= 90 => "Very Easy",
        >= 80 => "Easy",
        >= 70 => "Fairly Easy",
        >= 60 => "Standard",
        >= 50 => "Fairly Difficult",
        >= 30 => "Difficult",
        _ => "Very Difficult"
    };
}
