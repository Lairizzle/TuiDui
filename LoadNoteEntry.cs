namespace TuiDui
{
    public class LoadNoteEntry
    {
        private readonly JsonNoteRepository _repository;

        public LoadNoteEntry(JsonNoteRepository repository)
        {
            _repository = repository;
        }

        // Returns display-friendly strings for ListView
        public List<string> GetNoteDisplayList()
        {
            var items = _repository.LoadNotes();

            return items.Select(i => $"[ ] {i.Title} - {i.Date}").ToList();
        }
    }
}
