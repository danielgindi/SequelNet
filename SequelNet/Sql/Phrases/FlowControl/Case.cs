using System.Collections.Generic;
using System.Text;
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
            this.Value = ValueWrapper.From(condition);
        }

        public Case(WhereList condition)
        {
            this.Value = ValueWrapper.From(condition);
        }

        public Case(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public Case(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
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
            return When(ValueWrapper.From(condition));
        }

        public Case When(WhereList condition)
        {
            return When(ValueWrapper.From(condition));
        }

        public Case When(string tableName, string columnName)
        {
            return When(ValueWrapper.Column(tableName, columnName));
        }

        public Case When(object value, ValueObjectType valueType)
        {
            return When(ValueWrapper.Make(value, valueType));
        }

        public Case When(IPhrase value)
        {
            return When(ValueWrapper.From(value));
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
            return Then(ValueWrapper.Column(tableName, columnName));
        }

        public Case Then(object value, ValueObjectType valueType)
        {
            return Then(ValueWrapper.Make(value, valueType));
        }

        public Case Then(IPhrase value)
        {
            return Then(ValueWrapper.From(value));
        }

        public Case Else(ValueWrapper value)
        {
            this.ElseValue = value;
            return this;
        }

        public Case Else(string tableName, string columnName)
        {
            return Else(ValueWrapper.Column(tableName, columnName));
        }

        public Case Else(object value, ValueObjectType valueType)
        {
            return Else(ValueWrapper.Make(value, valueType));
        }

        public Case Else(IPhrase value)
        {
            return Else(ValueWrapper.From(value));
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append("CASE");

            if (Value != null)
            {
                sb.Append(" " + Value?.Build(conn, relatedQuery));
            }

            foreach (var when in Conditions)
            {
                sb.Append(" WHEN " + (when.When == null ? "NULL" : when.When?.Build(conn, relatedQuery)));
                sb.Append(" THEN " + (when.Then == null ? "NULL" : when.Then?.Build(conn, relatedQuery)));
            }

            if (ElseValue != null)
            {
                sb.Append(" ELSE " + ElseValue?.Build(conn, relatedQuery));
            }

            sb.Append(" END");
        }
    }
}
