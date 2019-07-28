namespace SequelNet
{
    public enum QueryHint
    {
        None,

        /// <summary>
        /// Will issue a SELECT ... FOR UPDATE query.
        /// This will ensure that the DB will lock the rows involved in the query.
        /// * Applies only when ever there's no auto-commit, i.e inside a transaction.
        /// * Supported in both MySql and MSSql
        /// </summary>
        ForUpdate,

        /// <summary>
        /// Will issue a SELECT ... LOCK IN SHARED MODE query.
        /// Sets a shared mode lock on the rows read in the query. A shared mode lock enables other sessions to read the rows but not to modify them.
        /// The rows read are the latest available, so if they belong to another transaction that has not yet committed, the read blocks until that transaction ends.
        /// * Applies only when ever there's no auto-commit, i.e inside a transaction.
        /// * Supported in MySql
        /// </summary>
        LockInSharedMode
    }
}
