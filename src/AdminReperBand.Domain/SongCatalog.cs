namespace AdminReperBand.Domain;

public class SongCatalog
{
    private readonly List<Song> songs = new();

    public IReadOnlyList<Song> Songs => songs.AsReadOnly();

    public Song Add(
        string title,
        string artist,
        int durationSeconds)
    {
        Song song = new Song(title, artist, durationSeconds);

        EnsureNoDuplicate(song.Title, song.Artist);

        songs.Add(song);

        return song;
    }

    public Song GetById(Guid id)
    {
        Song? song = songs.Find(song => song.Id == id);

        if (song is null)
        {
            throw new KeyNotFoundException(
                "La canción no existe.");
        }

        return song;
    }

    public IReadOnlyList<Song> Search(string query)
    {
        string normalizedQuery =
            Song.NormalizeText(query, nameof(query));

        return songs.FindAll(song =>
            song.Title.Contains(
                normalizedQuery,
                StringComparison.OrdinalIgnoreCase)
            || song.Artist.Contains(
                normalizedQuery,
                StringComparison.OrdinalIgnoreCase));
    }

    public void Update(
        Guid id,
        string title,
        string artist,
        int durationSeconds)
    {
        Song song = GetById(id);

        string normalizedTitle =
                Song.NormalizeText(title, nameof(title));

        string normalizedArtist =
                Song.NormalizeText(artist, nameof(artist));

        EnsureNoDuplicate(normalizedTitle, normalizedArtist, id);

        song.Update(
            normalizedTitle,
            normalizedArtist,
            durationSeconds);
    }

    public void Remove(Guid id)
    {
        Song song = GetById(id);
        songs.Remove(song);
    }

    private void EnsureNoDuplicate(
        string title,
        string artist,
        Guid? excludedId = null)
    {
        bool duplicateExists = songs.Exists(song =>
            song.Id != excludedId
            && string.Equals(
                song.Title,
                title,
                StringComparison.OrdinalIgnoreCase)
            && string.Equals(
                song.Artist,
                artist,
                StringComparison.OrdinalIgnoreCase));

        if (duplicateExists)
        {
            throw new InvalidOperationException(
                "Ya existe una canción con ese título y artista.");
        }
    }
}