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
            //Todo Handlers
            todoListEvents.SelectedItemChanged += OnSelected;
            todoListEvents.OpenSelectedItem += OnTodoOpen;
            todoListEvents.KeyPress += OnListKeyPressed;
            addTodoEvent.Clicked += OnAddTodoEventClicked;
            updateTodoEvent.Clicked += OnUpdateTodoEventClicked;
            todoDateText.Enter += e => { todoDateText.Text = ""; };
        }

        private void OnSelected(ListViewItemEventArgs args)
        {
            int index = GetTodoIndex();
            var item = GetTodoItem(index);

            todoEventText.Text = item.Title;
            if (item.Date.HasValue)
                todoDateText.Text = item.Date.Value.ToString("MM/dd/yyyy");
            else
                todoDateText.Text = string.Empty;
        }

        private void OnListKeyPressed(View.KeyEventEventArgs e)
        {
            int index = GetTodoIndex();
            var item = GetTodoItem(index);

            if (e.KeyEvent.Key == Key.DeleteChar)
            {
                var n = MessageBox.Query("Delete?", $"Delete '{item.Title}'?", "Yes", "No");
                if (n == 0)
                {
                    _todoManager.DeleteTodoAt(index);
                    _todoManager.SaveTodos();
                    RefreshTodoList(index);
                }
                e.Handled = true;
            }
        }

        private void OnTodoOpen(ListViewItemEventArgs args)
        {
            int index = GetTodoIndex();
            _todoManager.ToggleTodoCompletion(index);
            _todoManager.SaveTodos();
            RefreshTodoList(index);
        }

        private void OnAddTodoEventClicked()
        {
            var result = ValidateTodoInputs();
            if (result == null)
                return;

            var (title, date) = result.Value;

            _todoManager.AddTodo(title, date);
            RefreshTodoList(_todoManager.GetTodoList().Count - 1);
        }

        private void OnUpdateTodoEventClicked()
        {
            var result = ValidateTodoInputs();
            int index = GetTodoIndex();

            if (index == -1)
                return;

            if (result == null)
                return;

            var (title, date) = result.Value;

            _todoManager.EditTodo(index, title, date);
            RefreshTodoList(index);
        }

        private int GetTodoIndex()
        {
            int index = todoListEvents.SelectedItem;
            if (index < 0)
                return -1;

            return index;
        }

        private TodoItem GetTodoItem(int index)
        {
            var item = _todoManager.GetTodoList()[index];
            return item;
        }

        private void RefreshTodoList(int preserveIndex = 0)
        {
            var todos = _todoManager.GetTodoList();
            todoListEvents.SetSource(_todoManager.GetTodoDisplayList());

            if (todos.Count > 0)
                todoListEvents.SelectedItem = Math.Min(preserveIndex, todos.Count - 1);
        }

        private (string Title, DateTime? Date)? ValidateTodoInputs()
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
