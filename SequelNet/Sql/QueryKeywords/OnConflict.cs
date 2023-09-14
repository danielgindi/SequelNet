namespace SequelNet;

public class OnConflict
{
    public OnConflict()
    {
    }

    public string OnColumn;
    public string OnConstraint;
    public AssignmentColumnList Updates = new AssignmentColumnList();

    #region Updates

    public OnConflict Update(string columnName, object value)
    {
        Updates.Add(new AssignmentColumn(null, columnName, null, value, ValueObjectType.Value));
        return this;
    }

    #endregion
}
