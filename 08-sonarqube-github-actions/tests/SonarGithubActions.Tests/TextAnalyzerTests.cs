using SonarGithubActions.Api.Services;

namespace SonarGithubActions.Tests;

public class TextAnalyzerTests
{
    private readonly TextAnalyzer _sut = new();

    [Fact]
    public void Analyze_SimpleText_ReturnsWordCount()
    {
        var result = _sut.Analyze("Hello world. How are you?");

        Assert.Equal(5, result.WordCount);
    }

    [Fact]
    public void Analyze_SentenceCount_CountsPunctuation()
    {
        var result = _sut.Analyze("First sentence. Second one! Third?");

        Assert.Equal(3, result.SentenceCount);
    }

    [Fact]
    public void Analyze_CharacterCount_IncludesSpaces()
    {
        var text = "Hello world";
        var result = _sut.Analyze(text);

        Assert.Equal(11, result.CharacterCount);
    }

    [Fact]
    public void Analyze_MultiParagraph_CountsParagraphs()
    {
        var text = "Para one.\n\nPara two.\n\nPara three.";
        var result = _sut.Analyze(text);

        Assert.Equal(3, result.ParagraphCount);
    }

    [Fact]
    public void Analyze_AverageWords_CalculatesCorrectly()
    {
        var result = _sut.Analyze("One two three. Four five.");

        Assert.Equal(2.5, result.AverageWordsPerSentence);
    }

    [Fact]
    public void Analyze_SingleWord_DoesNotThrow()
    {
        var result = _sut.Analyze("Hello.");

        Assert.Equal(1, result.WordCount);
        Assert.True(result.FleschReadingEase >= 0 && result.FleschReadingEase <= 100);
    }

    [Fact]
    public void Analyze_EmptyWhitespace_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _sut.Analyze("   "));
    }

    [Fact]
    public void Analyze_FleschScore_WithinValidRange()
    {
        var text = "The quick brown fox jumps over the lazy dog. Simple sentences are easy.";
        var result = _sut.Analyze(text);

        Assert.InRange(result.FleschReadingEase, 0, 100);
    }

    [Fact]
    public void Analyze_ReadabilityLevel_NotEmpty()
    {
        var result = _sut.Analyze("The cat sat on the mat.");

        Assert.False(string.IsNullOrEmpty(result.ReadabilityLevel));
    }

    // ── Syllable estimation unit tests ───────────────────────────────────────
    [Theory]
    [InlineData("cat", 1)]
    [InlineData("hello", 2)]
    [InlineData("beautiful", 3)]
    [InlineData("a", 1)]
    public void EstimateSyllables_KnownWords_ReturnsExpected(string word, int expected)
    {
        var actual = TextAnalyzer.EstimateSyllables(word);
        Assert.Equal(expected, actual);
    }
}
