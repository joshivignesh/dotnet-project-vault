namespace SonarGithubActions.Api.Models;

public record TextRequest(string Content);

public record TextAnalysisResult(
    int CharacterCount,
    int WordCount,
    int SentenceCount,
    int ParagraphCount,
    double AverageWordsPerSentence,
    double FleschReadingEase,
    string ReadabilityLevel
);
