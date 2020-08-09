namespace SequelNet.Migrations
{
    public abstract class Migration
    {
        /// <summary>
        /// What to do when migrating up to this version?
        /// </summary>
        public virtual void Up()
        {
            UpAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// What to do when migrating down from this version?
        /// </summary>
        public virtual void Down()
        {
            DownAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// What to do when migrating up to this version?
        /// </summary>
        public abstract System.Threading.Tasks.Task UpAsync();

        /// <summary>
        /// What to do when migrating down from this version?
        /// </summary>
        public abstract System.Threading.Tasks.Task DownAsync();
    }
}
