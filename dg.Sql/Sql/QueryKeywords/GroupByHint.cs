namespace dg.Sql
{
    public enum GroupByHint
    {
        None,

        /// <summary>
        /// Will add a "rollup" row, which summarizes the data.
        /// * Supported in both MySql and MSSql
        /// </summary>
        RollUp
    }
}
