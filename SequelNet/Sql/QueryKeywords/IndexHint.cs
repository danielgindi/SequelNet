using System.Collections.Generic;

namespace SequelNet
{
    public class IndexHintList : List<IndexHint> { }
    
    public class IndexHint
    {
        public string[] IndexNames;
        public IndexHintMode Hint;

        public IndexHint()
        {
        }
        
        public IndexHint(string name, IndexHintMode hint)
        {
            IndexNames = new string[] { name };
            Hint = hint;
        }

        public IndexHint(string[] names, IndexHintMode hint)
        {
            IndexNames = names;
            Hint = hint;
        }
    }

    public enum IndexHintMode
    {
        Use,
        Ignore,
        Force,
    }    
}
