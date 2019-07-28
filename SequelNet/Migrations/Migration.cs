namespace SequelNet.Migrations
{
    public abstract class Migration
    {
        /// <summary>
        /// What to do when migrating up to this version?
        /// </summary>
        public abstract void Up();

        /// <summary>
        /// What to do when migrating down from this version?
        /// </summary>
        public abstract void Down();
    }
}
