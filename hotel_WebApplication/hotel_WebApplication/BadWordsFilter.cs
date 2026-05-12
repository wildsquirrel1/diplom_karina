using System.Text.Json;

namespace hotel_WebApplication
{
    public class BadWordItem
    {
        public string id { get; set; }
        public string word { get; set; }
    }

    public class BadWordsFilter
    {
        private readonly HashSet<string> _badWords;
        private readonly ILogger<BadWordsFilter> _logger;
        private readonly IWebHostEnvironment _env;

        public BadWordsFilter(IWebHostEnvironment env, ILogger<BadWordsFilter> logger)
        {
            _env = env;
            _logger = logger;
            _badWords = LoadBadWords();

            _logger.LogInformation($"Loaded {_badWords.Count} bad words");
        }

        private HashSet<string> LoadBadWords()
        {
            var path = Path.Combine(_env.ContentRootPath, "badwords.json");
            _logger.LogInformation($"Looking for badwords.json at: {path}");

            if (!File.Exists(path))
            {
                _logger.LogError($"badwords.json NOT FOUND at {path}");
                return new HashSet<string>();
            }

            try
            {
                var json = File.ReadAllText(path);
                var items = JsonSerializer.Deserialize<List<BadWordItem>>(json);

                var words = new HashSet<string>(
                    items.Select(w => w.word.ToLower()),
                    StringComparer.OrdinalIgnoreCase
                );

                _logger.LogInformation($"Successfully loaded {words.Count} words");
                return words;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading badwords.json");
                return new HashSet<string>();
            }
        }

        public bool ContainsBadWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            var lowerText = text.ToLower();
            foreach (var badWord in _badWords)
            {
                if (lowerText.Contains(badWord))
                {
                    _logger.LogWarning($"Bad word '{badWord}' found in text");
                    return true;
                }
            }
            return false;
        }
    }
}