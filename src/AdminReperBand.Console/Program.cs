using AdminReperBand.Domain;

SongCatalog catalog = new SongCatalog();
bool running = true;

Console.WriteLine("ADMIN REPER BAND");
Console.WriteLine(
    "Versión en memoria: los datos se pierden al cerrar.");

while (running)
{
    Console.WriteLine();
    Console.WriteLine("1. Agregar canción");
    Console.WriteLine("2. Listar canciones");
    Console.WriteLine("3. Consultar una canción");
    Console.WriteLine("4. Buscar canciones");
    Console.WriteLine("5. Modificar canción");
    Console.WriteLine("6. Eliminar canción");
    Console.WriteLine("0. Salir");

    try
    {
        int option = ReadInteger("Selecciona una opción: ", 0, 6);

        switch (option)
        {
            case 1:
                AddSong();
                break;

            case 2:
                ShowSongs(catalog.Songs);
                break;

            case 3:
                ViewSong();
                break;

            case 4:
                SearchSongs();
                break;

            case 5:
                UpdateSong();
                break;

            case 6:
                RemoveSong();
                break;

            case 0:
                running = false;
                break;
        }
    }
    catch (ArgumentException exception)
    {
        Console.WriteLine($"Datos inválidos: {exception.Message}");
    }
    catch (InvalidOperationException exception)
    {
        Console.WriteLine($"No se realizó la operación: {exception.Message}");
    }
    catch (KeyNotFoundException exception)
    {
        Console.WriteLine(exception.Message);
    }
    catch (EndOfStreamException)
    {
        Console.WriteLine();
        Console.WriteLine("Se cerró la entrada. Terminando.");
        running = false;
    }
}

Console.WriteLine("Hasta luego.");

void AddSong()
{
    string title = ReadRequiredText("Título: ");
    string artist = ReadRequiredText("Artista: ");
    int durationSeconds = ReadDuration();

    Song song = catalog.Add(title, artist, durationSeconds);

    Console.WriteLine($"Canción agregada: {song.Title}.");
}

void ViewSong()
{
    Song? song = SelectSong();

    if (song is null)
    {
        return;
    }

    Console.WriteLine();
    Console.WriteLine($"Id: {song.Id}");
    Console.WriteLine($"Título: {song.Title}");
    Console.WriteLine($"Artista: {song.Artist}");
    Console.WriteLine(
        $"Duración: {FormatDuration(song.DurationSeconds)}");
}

void SearchSongs()
{
    string query = ReadRequiredText(
        "Busca por título o artista: ");

    IReadOnlyList<Song> results = catalog.Search(query);

    if (results.Count == 0)
    {
        Console.WriteLine("No se encontraron coincidencias.");
        return;
    }

    ShowSongs(results);
}

void UpdateSong()
{
    Song? song = SelectSong();

    if (song is null)
    {
        return;
    }

    Console.WriteLine($"Modificando: {song.Title} — {song.Artist}");
    Console.WriteLine(
        "Introduce los datos completos; aún no se guardarán cambios.");

    string title = ReadRequiredText("Nuevo título: ");
    string artist = ReadRequiredText("Nuevo artista: ");
    int durationSeconds = ReadDuration();

    catalog.Update(song.Id, title, artist, durationSeconds);

    Console.WriteLine("Canción modificada.");
}

void RemoveSong()
{
    Song? song = SelectSong();

    if (song is null)
    {
        return;
    }

    string confirmation = ReadLine(
        $"¿Eliminar {song.Title} — {song.Artist}? [s/N]: ");

    if (!string.Equals(
        confirmation.Trim(),
        "s",
        StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Eliminación cancelada.");
        return;
    }

    catalog.Remove(song.Id);
    Console.WriteLine("Canción eliminada.");
}

Song? SelectSong()
{
    if (catalog.Songs.Count == 0)
    {
        Console.WriteLine("Todavía no hay canciones.");
        return null;
    }

    ShowSongs(catalog.Songs);

    int selection = ReadInteger(
        "Número de canción, o 0 para cancelar: ",
        0,
        catalog.Songs.Count);

    if (selection == 0)
    {
        return null;
    }

    // El número es solo una selección temporal de la interfaz.
    return catalog.Songs[selection - 1];
}

void ShowSongs(IReadOnlyList<Song> songs)
{
    if (songs.Count == 0)
    {
        Console.WriteLine("Todavía no hay canciones.");
        return;
    }

    Console.WriteLine();

    for (int index = 0; index < songs.Count; index++)
    {
        Song song = songs[index];

        Console.WriteLine(
            $"{index + 1}. {song.Title} — {song.Artist}"
            + $" | {FormatDuration(song.DurationSeconds)}");
    }
}

string ReadLine(string prompt)
{
    Console.Write(prompt);

    return Console.ReadLine()
        ?? throw new EndOfStreamException();
}

string ReadRequiredText(string prompt)
{
    while (true)
    {
        string value = ReadLine(prompt);

        if (!string.IsNullOrWhiteSpace(value))
        {
            return value.Trim();
        }

        Console.WriteLine("Este campo no puede quedar vacío.");
    }
}

int ReadInteger(string prompt, int minimum, int maximum)
{
    while (true)
    {
        string input = ReadLine(prompt);

        if (int.TryParse(input, out int value)
            && value >= minimum
            && value <= maximum)
        {
            return value;
        }

        Console.WriteLine(
            $"Escribe un entero entre {minimum} y {maximum}.");
    }
}

int ReadDuration()
{
    while (true)
    {
        int minutes = ReadInteger(
            "Duración — minutos: ",
            0,
            int.MaxValue / 60);

        int seconds = ReadInteger(
            "Duración — segundos: ",
            0,
            59);

        // long evita desbordar al comprobar el total.
        long totalSeconds = (long)minutes * 60 + seconds;

        if (totalSeconds > 0 && totalSeconds <= int.MaxValue)
        {
            return (int)totalSeconds;
        }

        Console.WriteLine(
            "La duración debe ser positiva y caber en segundos enteros.");
    }
}

string FormatDuration(int totalSeconds)
{
    int minutes = totalSeconds / 60;
    int seconds = totalSeconds % 60;

    return $"{minutes}:{seconds:D2}";
}