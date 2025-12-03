namespace TuiDui
{
    public class TodoItem : ISelectableItem
    {
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public bool IsComplete { get; set; }
    }
}
