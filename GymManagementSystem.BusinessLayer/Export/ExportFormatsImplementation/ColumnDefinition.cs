namespace GymManagementSystem.BusinessLayer.Export.ExportFormatsImplementation;

/// <summary>
/// this generic class represent the definition of column Ex: Header: "Names", and the value selector the method that extract value from object
/// </summary>
/// <typeparam name="T"></typeparam>
public class ColumnDefinition<T>
{
    public string Header { get; private set; }
    public Func<T, object?> ValueSelector { get; private set; }
    
    public ColumnDefinition(string header, Func<T, object?> valueSelector)
    {
        Header = header;
        ValueSelector = valueSelector;
    }
}