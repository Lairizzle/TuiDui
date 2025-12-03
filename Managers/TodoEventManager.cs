namespace TuiDui
{
    public class TodoEventManager
    {
        private readonly JsonTodoRepository _repository;
        private List<TodoItem> _todos;

        public TodoEventManager()
        {
            _repository = new JsonTodoRepository("todos.json");
            LoadTodos();
        }

        public void LoadTodos()
        {
            _todos = _repository.LoadTodos().ToList();
        }

        public List<TodoItem> GetTodoList()
        {
            if (_todos == null) LoadTodos();
            return _todos;
        }

        public List<string> GetTodoDisplayList()
        {
            if (_todos == null) LoadTodos();
            return _todos.Select(t => t.IsComplete
                ? $"[x] (Due: {t.Date:MM/dd}) {t.Title}"
                : $"[ ] (Due: {t.Date:MM/dd}) {t.Title}").ToList();
        }

        public string FormatTodoForDisplay(TodoItem todo)
        {
            return todo.IsComplete
                ? $"[x] (Due: {todo.Date:MM/dd}) {todo.Title}"
                : $"[ ] (Due: {todo.Date:MM/dd}) {todo.Title}";
        }

        public void SaveTodos()
        {
            if (_todos != null)
                _repository.SaveTodos(_todos);
        }

        public void AddTodo(string title, DateTime? date = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));

            var newTodo = new TodoItem
            {
                Title = title,
                Date = date,
                IsComplete = false
            };

            _todos.Add(newTodo);
            SaveTodos();
        }

        public void EditTodo(int index, string newTitle, DateTime? newDate = null)
        {
            if (index < 0 || index >= _todos.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Invalid todo index.");

            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("Title cannot be empty.", nameof(newTitle));

            var todo = _todos[index];
            todo.Title = newTitle;
            todo.Date = newDate;

            SaveTodos();
        }


        public void DeleteTodoAt(int index)
        {
            if (_todos != null && index >= 0 && index < _todos.Count)
                _todos.RemoveAt(index);
        }

        public void ToggleTodoCompletion(int index)
        {
            if (_todos != null && index >= 0 && index < _todos.Count)
                _todos[index].IsComplete = !_todos[index].IsComplete;
        }
    }
}

