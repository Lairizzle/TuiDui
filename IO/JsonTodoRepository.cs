using System.Text.Json;

namespace TuiDui
{
    public class JsonTodoRepository
    {
        private readonly string _filePath;

        public JsonTodoRepository(string filePath)
        {
            _filePath = filePath;
        }

        public List<TodoItem> LoadTodos()
        {
            if (!File.Exists(_filePath))
                return new List<TodoItem>();

            var json = File.ReadAllText(_filePath);

            var items = JsonSerializer.Deserialize<List<TodoItem>>(json);

            return items ?? new List<TodoItem>();
        }

        public void SaveTodos(List<TodoItem> items)
        {
            var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}

