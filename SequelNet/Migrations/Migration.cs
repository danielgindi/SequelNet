namespace SequelNet.Migrations
{
    public abstract class Migration : IMigration
    {
        /// <summary>
        /// What to do when migrating up to this version?
        /// </summary>
        public virtual void Up()
        {
        }

        /// <summary>
        /// What to do when migrating down from this version?
        /// </summary>
        public virtual void Down()
        {
        }
    }
}
