namespace TuiDui
{
    public interface ISelectableItem
    {
        string Title { get; }
        DateTime? Date { get; } // Nullable if notes may not have a date
    }
}

