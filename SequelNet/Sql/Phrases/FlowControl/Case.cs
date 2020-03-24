using System.Collections.Generic;
using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Case : IPhrase
    {
        public class WhenClause
        {
            public ValueWrapper? When;
            public ValueWrapper? Then;
        }

        public ValueWrapper? Value;
        public List<WhenClause> Conditions = new List<WhenClause>();
        public ValueWrapper? ElseValue;

        private WhenClause _CurrentWhen;

        #region Constructors

        public Case()
        {
        }

        public Case(ValueWrapper value)
        {
            this.Value = value;
        }

        public Case(Where condition)
        {
            this.Value = new ValueWrapper(condition, ValueObjectType.Value);
        }

        public Case(WhereList condition)
        {
            this.Value = new ValueWrapper(condition, ValueObjectType.Value);
        }

        public Case(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public Case(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        #endregion

        #region Builder

        public Case When(ValueWrapper value)
        {
            this._CurrentWhen = new WhenClause
            {
                When = value,
                Then = null
            };
            return this;
        }

        public Case When(Where condition)
        {
            return When(new ValueWrapper(condition, ValueObjectType.Value));
        }

        public Case When(WhereList condition)
        {
            return When(new ValueWrapper(condition, ValueObjectType.Value));
        }

        public Case When(string tableName, string columnName)
        {
            return When(new ValueWrapper(tableName, columnName));
        }

        public Case When(object value, ValueObjectType valueType)
        {
            return When(new ValueWrapper(value, valueType));
        }

        public Case When(IPhrase value)
        {
            return When(new ValueWrapper(value));
        }

        public Case Then(ValueWrapper value)
        {
            if (this._CurrentWhen != null)
            {
                this._CurrentWhen.Then = value;
                Conditions.Add(this._CurrentWhen);
                this._CurrentWhen = null;
            }
            else if (Conditions.Count > 0)
            {
                Conditions[Conditions.Count - 1].Then = value;
            }
            return this;
        }

        public Case Then(string tableName, string columnName)
        {
            return Then(new ValueWrapper(tableName, columnName));
        }

        public Case Then(object value, ValueObjectType valueType)
        {
            return Then(new ValueWrapper(value, valueType));
        }

        public Case Then(IPhrase value)
        {
            return Then(new ValueWrapper(value));
        }

        public Case Else(ValueWrapper value)
        {
            this.ElseValue = value;
            return this;
        }

        public Case Else(string tableName, string columnName)
        {
            return Else(new ValueWrapper(tableName, columnName));
        }

        public Case Else(object value, ValueObjectType valueType)
        {
            return Else(new ValueWrapper(value, valueType));
        }

        public Case Else(IPhrase value)
        {
            return Else(new ValueWrapper(value));
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "CASE";

            if (Value != null)
            {
                ret += " " + Value?.Build(conn, relatedQuery);
            }

            foreach (var when in Conditions)
            {
                ret += " WHEN " + (when.When == null ? "NULL" : when.When?.Build(conn, relatedQuery));
                ret += " THEN " + (when.Then == null ? "NULL" : when.Then?.Build(conn, relatedQuery));
            }

            if (ElseValue != null)
            {
                ret += " ELSE " + ElseValue?.Build(conn, relatedQuery);
            }

            ret += " END";

            return ret;
        }
    }
}
