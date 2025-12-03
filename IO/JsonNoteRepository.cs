using System.Text.Json;

namespace TuiDui
{
    public class JsonNoteRepository
    {
        private readonly string _filePath;

        public JsonNoteRepository(string filePath)
        {
            _filePath = filePath;
        }

        public List<NoteItem> LoadNotes()
        {
            if (!File.Exists(_filePath))
                return new List<NoteItem>();

            var json = File.ReadAllText(_filePath);

            var items = JsonSerializer.Deserialize<List<NoteItem>>(json);

            return items ?? new List<NoteItem>();
        }

        public void SaveNotes(List<NoteItem> items)
        {
            var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
