namespace TuiDui
{
    using Terminal.Gui;

    public partial class MainDialog
    {
        private readonly TodoEventManager _todoManager;
        private readonly NoteEventManager _noteManager;
        private int _selectedTodoIndex = -1;
        private int _selectedNoteIndex = -1;

        private enum ActiveList
        {
            Todo,
            Note,
            None
        }

        private enum ActiveFrame
        {
            TodoFrame,
            NoteFrame,
            None
        }

        private ActiveList _activeList = ActiveList.None;
        private ActiveFrame _activeFrame = ActiveFrame.None;

        public MainDialog()
        {
            InitializeComponent();

            _todoManager = new TodoEventManager();
            _noteManager = new NoteEventManager();
            LoadData();
            LoadHandlers();
        }

        private void LoadData()
        {
            todoListEvents.SetSource(_todoManager.GetTodoDisplayList());
            noteListEvents.SetSource(_noteManager.GetNoteDisplayList());
        }

        private void LoadHandlers()
        {
            //List Handlers
            todoListEvents.SelectedItemChanged += OnSelected;
            noteListEvents.SelectedItemChanged += OnSelected;
            todoListEvents.OpenSelectedItem += OnOpen;
            noteListEvents.OpenSelectedItem += OnOpen;
            todoListEvents.KeyPress += OnListKeyPressed;
            noteListEvents.KeyPress += OnListKeyPressed;

            //Buttons Handlers
            addTodoEvent.Clicked += () => OnAddClicked("todo");
            saveNote.Clicked += () => OnAddClicked("note");
            updateTodoEvent.Clicked += () => OnUpdateEventClicked("todo");
            updateNote.Clicked += () => OnUpdateEventClicked("note");

            //Formatting
            todoDateText.Enter += e => { todoDateText.Text = ""; };

            todoListEvents.Enter += (_) => _activeList = ActiveList.Todo;
            noteListEvents.Enter += (_) => _activeList = ActiveList.Note;
            toDoEntry.Enter += (_) => _activeFrame = ActiveFrame.TodoFrame;
            noteEntryFrame.Enter += (_) => _activeFrame = ActiveFrame.NoteFrame;

        }

        private void OnSelected(ListViewItemEventArgs args)
        {
            int index = args.Item;
            var item = GetItem(index);
            if (item == null) return;

            switch (_activeList)
            {
                case ActiveList.Todo:
                    _selectedTodoIndex = index;
                    if (item is TodoItem todo)
                    {
                        todoEventText.Text = todo.Title;
                        todoDateText.Text = todo.Date?.ToString("MM/dd/yyyy") ?? string.Empty;
                    }
                    break;

                case ActiveList.Note:
                    _selectedNoteIndex = index;
                    if (item is NoteItem note)
                    {
                        noteTextView.Text = note.Note;
                    }
                    break;
            }
        }

        private void OnListKeyPressed(View.KeyEventEventArgs e)
        {
            if (e.KeyEvent.Key == Key.DeleteChar)
            {
                int index = GetIndex();
                var item = GetItem(index);

                if (item == null)
                {
                    e.Handled = true;
                    return;
                }

                var n = MessageBox.Query("Delete?", $"Delete '{item.Title}'?", "Yes", "No");
                if (n == 0)
                {
                    if (item is TodoItem)
                    {
                        _todoManager.DeleteTodoAt(index);
                        _todoManager.SaveTodos();
                    }

                    if (item is NoteItem)
                    {
                        _noteManager.DeleteNoteAt(index);
                        _noteManager.SaveNotes();
                        noteTextView.Text = "";
                    }

                    RefreshList(index);
                }

                e.Handled = true;
            }
        }

        private void OnOpen(ListViewItemEventArgs args)
        {
            int index = GetIndex();
            var item = GetItem(index);
            if (todoListEvents.HasFocus)
            {
                if (item is TodoItem)
                {
                    _todoManager.ToggleTodoCompletion(index);
                    _todoManager.SaveTodos();
                }
            }
            else if (noteListEvents.HasFocus)
            {
                if (item is NoteItem)
                    return;
            }

            RefreshList(index);
        }

        private void OnAddClicked(string type)
        {
            switch (type)
            {
                case "todo":
                    var todoResult = ValidateTodoInputs();
                    if (todoResult == null) return;

                    var (title, date) = todoResult.Value;
                    _todoManager.AddTodo(title, date);
                    RefreshList(_todoManager.GetTodoList().Count - 1);
                    break;

                case "note":
                    var noteResult = ValidateNoteInputs();
                    if (noteResult == null) return;

                    var (noteTitle, body) = noteResult.Value;
                    _noteManager.AddNote(noteTitle, body);
                    RefreshList(_noteManager.GetNoteList().Count - 1);
                    break;
            }
        }

        private void OnUpdateEventClicked(string type)
        {
            switch (type)
            {
                case "todo":
                    var todoResult = ValidateTodoInputs();
                    if (todoResult == null || _selectedTodoIndex < 0) return;

                    var (title, date) = todoResult.Value;
                    _todoManager.EditTodo(_selectedTodoIndex, title, date);
                    RefreshList(_selectedTodoIndex);
                    break;

                case "note":
                    var noteResult = ValidateNoteInputs(_noteManager.GetNoteList()[_selectedNoteIndex].Title);
                    if (noteResult == null || _selectedNoteIndex < 0) return;

                    var (noteTitle, body) = noteResult.Value;
                    _noteManager.EditNote(_selectedNoteIndex, noteTitle, body);
                    RefreshList(_selectedNoteIndex);
                    break;
            }
        }


        private string? PromptForNoteTitle(string defaultTitle = "")
        {
            string? result = null;

            var dialog = new Dialog("Enter Note Title", 60, 10);

            var titleField = new TextField(defaultTitle) { X = 1, Y = 1, Width = 40 };
            dialog.Add(titleField);

            var okButton = new Button("OK");
            okButton.Clicked += () => { result = titleField.Text?.ToString().Trim(); Application.RequestStop(); };
            dialog.AddButton(okButton);

            var cancelButton = new Button("Cancel");
            cancelButton.Clicked += () => { result = null; Application.RequestStop(); };
            dialog.AddButton(cancelButton);

            Application.Run(dialog);

            if (string.IsNullOrEmpty(result))
                return null;

            return result;
        }

        private int GetIndex()
        {
            switch (_activeList)
            {
                case ActiveList.Todo:
                    return _todoManager.GetTodoList().Count > 0
                        ? todoListEvents.SelectedItem
                        : -1;

                case ActiveList.Note:
                    return _noteManager.GetNoteList().Count > 0
                        ? noteListEvents.SelectedItem
                        : -1;

                default:
                    return -1;
            }
        }

        private ISelectableItem? GetItem(int index)
        {
            if (index < 0)
                return null;

            switch (_activeList)
            {
                case ActiveList.Todo:
                    {
                        var list = _todoManager.GetTodoList();
                        return index < list.Count ? list[index] : null;
                    }

                case ActiveList.Note:
                    {
                        var list = _noteManager.GetNoteList();
                        return index < list.Count ? list[index] : null;
                    }

                default:
                    return null;
            }
        }


        private void RefreshList(int preserveIndex = 0)
        {
            switch (_activeList)
            {
                case ActiveList.Todo:
                    {
                        int top = todoListEvents.TopItem;

                        var todos = _todoManager.GetTodoList();
                        todoListEvents.SetSource(_todoManager.GetTodoDisplayList());

                        if (todos.Count > 0)
                            todoListEvents.SelectedItem = Math.Min(preserveIndex, todos.Count - 1);

                        todoListEvents.TopItem = Math.Min(top, Math.Max(0, todos.Count - 1));
                        break;
                    }

                case ActiveList.Note:
                    {
                        int top = noteListEvents.TopItem;

                        var notes = _noteManager.GetNoteList();
                        noteListEvents.SetSource(_noteManager.GetNoteDisplayList());

                        if (notes.Count > 0)
                            noteListEvents.SelectedItem = Math.Min(preserveIndex, notes.Count - 1);

                        noteListEvents.TopItem = Math.Min(top, Math.Max(0, notes.Count - 1));
                        break;
                    }
            }
        }

        private (string Title, DateTime? Date)? ValidateTodoInputs()
        {
            string title = todoEventText.Text?.ToString().Trim();
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.ErrorQuery("Error", "Title cannot be empty.", "Ok");
                return null;
            }

            DateTime? date = null;
            string dateText = todoDateText.Text?.ToString().Trim();
            if (!string.IsNullOrEmpty(dateText))
            {
                if (DateTime.TryParse(dateText, out var parsedDate))
                    date = parsedDate;
                else
                {
                    MessageBox.ErrorQuery("Error", "Invalid date format, use MM/dd/YYYY/", "Ok");
                    return null;
                }
            }

            return (title, date);
        }

        private (string Title, string Note)? ValidateNoteInputs(string defaultTitle = "")
        {
            string? title = PromptForNoteTitle(defaultTitle);
            if (title == null) return null;

            string noteBody = noteTextView.Text?.ToString().Trim() ?? "";

            return (title, noteBody);
        }

    }
}
