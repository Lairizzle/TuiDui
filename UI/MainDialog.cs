namespace TuiDui
{
    using Terminal.Gui;

    public partial class MainDialog
    {
        private readonly TodoEventManager _todoManager;

        public MainDialog()
        {
            InitializeComponent();
            _todoManager = new TodoEventManager();
            LoadTodoEvents();
            LoadTodoHandlers();
        }

        private void LoadTodoEvents()
        {
            todoListEvents.SetSource(_todoManager.GetTodoDisplayList());
        }

        private void LoadTodoHandlers()
        {
            //List Handlers
            todoListEvents.SelectedItemChanged += OnSelected;
            noteListEvents.SelectedItemChanged += OnSelected;
            todoListEvents.OpenSelectedItem += OnOpen;
            noteListEvents.OpenSelectedItem += OnOpen;
            todoListEvents.KeyPress += OnListKeyPressed;
            noteListEvents.KeyPress += OnListKeyPressed;

            //Buttons Handlers
            addTodoEvent.Clicked += OnAddClicked;
            updateTodoEvent.Clicked += OnUpdateTodoEventClicked;

            //Formatting
            todoDateText.Enter += e => { todoDateText.Text = ""; };
        }

        private void OnSelected(ListViewItemEventArgs args)
        {
            int index = GetIndex();
            var item = GetItem(index);

            if (todoListEvents.HasFocus)
            {
                if (item is TodoItem todo)
                {
                    todoEventText.Text = todo.Title;

                    if (todo.Date.HasValue)
                        todoDateText.Text = todo.Date.Value.ToString("MM/dd/yyyy");
                    else
                        todoDateText.Text = string.Empty;
                }
                return;
            }

            if (noteListEvents.HasFocus)
            {
                if (item is NoteItem note)
                {
                    // Fill in your note UI here, example:
                    //noteTitleText.Text = note.Title;
                    //noteBodyText.Text = note.Body ?? "";
                }
                return;
            }
        }

        private void OnListKeyPressed(View.KeyEventEventArgs e)
        {
            int index = GetIndex();
            var item = GetItem(index);

            if (e.KeyEvent.Key == Key.DeleteChar)
            {
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

            if (item is TodoItem)
            {
                _todoManager.ToggleTodoCompletion(index);
                _todoManager.SaveTodos();
            }

            if (item is NoteItem)
            { }

            RefreshList(index);
        }

        private void OnAddClicked()
        {
            var result = ValidateInputs();
            if (result == null)
                return;

            var (title, date) = result.Value;

            _todoManager.AddTodo(title, date);
            RefreshList(_todoManager.GetTodoList().Count - 1);
        }

        private void OnUpdateTodoEventClicked()
        {
            var result = ValidateInputs();
            int index = GetIndex();

            if (index == -1)
                return;

            if (result == null)
                return;

            var (title, date) = result.Value;

            _todoManager.EditTodo(index, title, date);
            RefreshList(index);
        }

        private int GetIndex()
        {
            int index = 0;

            if (todoListEvents.HasFocus)
            {
                index = todoListEvents.SelectedItem;
            }

            if (noteListEvents.HasFocus)
            {
                index = noteListEvents.SelectedItem;
            }

            if (index < 0)
                return -1;

            return index;
        }

        private ISelectableItem GetItem(int index)
        {
            var item = _todoManager.GetTodoList()[index];
            return item;
        }

        private void RefreshList(int preserveIndex = 0)
        {
            int selected = todoListEvents.SelectedItem;
            int top = todoListEvents.TopItem;

            var todos = _todoManager.GetTodoList();
            todoListEvents.SetSource(_todoManager.GetTodoDisplayList());

            if (todos.Count > 0)
                todoListEvents.SelectedItem = Math.Min(preserveIndex, todos.Count - 1);

            todoListEvents.TopItem = Math.Min(top, Math.Max(0, todos.Count - 1));
        }

        private (string Title, DateTime? Date)? ValidateInputs()
        {
            string title = todoEventText.Text?.ToString().Trim();
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.ErrorQuery("Error", "Title cannot be empty.", "Ok");
                return null; // return null on invalid input
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
                    return null; // return null on invalid input
                }
            }

            return (title, date);
        }
    }
}
