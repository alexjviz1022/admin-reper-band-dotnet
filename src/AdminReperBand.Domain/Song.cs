namespace AdminReperBand.Domain;

public class Song
{
    public Guid Id { get; }
    public string Title { get; private set; }
    public string Artist { get; private set; }
    public int DurationSeconds { get; private set; }

    public Song(string title, string artist, int durationSeconds)
    {
        string validTitle = NormalizeText(title, nameof(title));
        string validArtist = NormalizeText(artist, nameof(artist));
        ValidateDuration(durationSeconds);

        Id = Guid.NewGuid();
        Title = validTitle;
        Artist = validArtist;
        DurationSeconds = durationSeconds;
    }

    internal void Update(
        string title,
        string artist,
        int durationSeconds)
    {
        // Validar todo antes de modificar el objeto.
        string validTitle = NormalizeText(title, nameof(title));
        string validArtist = NormalizeText(artist, nameof(artist));
        ValidateDuration(durationSeconds);

        Title = validTitle;
        Artist = validArtist;
        DurationSeconds = durationSeconds;
    }

    internal static string NormalizeText(
        string value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "El texto no puede quedar vacío.",
                parameterName);
        }

        // Quita espacios externos y unifica espacios internos.
        return string.Join(
            " ",
            value.Split(
                (char[]?)null,
                StringSplitOptions.RemoveEmptyEntries));
    }

    private static void ValidateDuration(int durationSeconds)
    {
        if (durationSeconds <= 0)
        {
            throw new ArgumentException(
                "La duración debe ser mayor que cero.",
                nameof(durationSeconds));
        }
    }
}
