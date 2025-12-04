namespace TuiDui
{
    public class NoteEventManager
    {
        private readonly JsonNoteRepository _repository;
        private List<NoteItem> _notes;

        public NoteEventManager()
        {
            _repository = new JsonNoteRepository("notes.json");
            LoadNotes();
        }

        public void LoadNotes()
        {
            _notes = _repository.LoadNotes().ToList();
        }

        public List<NoteItem> GetNoteList()
        {
            if (_notes == null) LoadNotes();
            return _notes;
        }

        public List<string> GetNoteDisplayList()
        {
            if (_notes == null) LoadNotes();
            return _notes.Select(n => n.Title).ToList();
        }

        public string FormatNoteForDisplay(NoteItem note)
        {
            return note.Title;
        }

        public void SaveNotes()
        {
            if (_notes != null)
                _repository.SaveNotes(_notes);
        }

        public void AddNote(string title, string note)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(note))
                throw new ArgumentException("Title or note cannot be empty.", nameof(title));

            var newNote = new NoteItem
            {
                Title = title,
                Note = note,
            };

            _notes.Add(newNote);
            SaveNotes();
        }

        public void EditNote(int index, string newTitle, string newNote)
        {
            if (index < 0 || index >= _notes.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Invalid todo index.");

            if (string.IsNullOrWhiteSpace(newTitle) || string.IsNullOrWhiteSpace(newNote))
                throw new ArgumentException("Title cannot be empty.", nameof(newTitle));

            var note = _notes[index];
            note.Title = newTitle;
            note.Note = newNote;

            SaveNotes();
        }


        public void DeleteNoteAt(int index)
        {
            if (_notes != null && index >= 0 && index < _notes.Count)
                _notes.RemoveAt(index);
        }

    }
}

