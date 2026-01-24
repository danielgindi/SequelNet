namespace SequelNet.SchemaGenerator;

public class DalIndexColumn
{
    public DalIndexColumn()
    {
    }

    public DalIndexColumn(string name, bool literal, string sortDirection)
    {
        this.Name = name;
        this.SortDirection = sortDirection;
        this.Literal = literal;
    }

    public string Name;
    public string SortDirection;
    public bool Literal;
}