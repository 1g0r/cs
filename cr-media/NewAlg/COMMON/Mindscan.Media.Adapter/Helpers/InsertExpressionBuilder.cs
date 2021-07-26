using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Mindscan.Media.Adapter.Helpers
{
	internal class InsertExpressionBuilder<TEntity> where TEntity : class
	{
		private readonly TEntity _entity;
		private string _tableName;
		private readonly StringBuilder _columns;
		private readonly StringBuilder _values;
		private readonly List<MemberExpression> _expressions = new List<MemberExpression>();
		public InsertExpressionBuilder(TEntity entity)
		{
			_columns = new StringBuilder();
			_values = new StringBuilder();
			_entity = entity;
		}

		public InsertExpressionBuilder<TEntity> Insert(string table)
		{
			_tableName = table;
			return this;
		}

		public InsertExpressionBuilder<TEntity> Value(Expression<Func<TEntity, object>> expression)
		{
			var prop = expression.Body as MemberExpression;
			if (prop != null)
			{
				var func = expression.Compile();
				if (!IsDefault(func(_entity)))
				{
					_expressions.Add(prop);
				}
			}
			return this;
		}

		public InsertExpressionBuilder<TEntity> Values(params Expression<Func<TEntity, object>>[] expressions)
		{
			if (expressions != null)
			{
				foreach (var expression in expressions)
				{
					Value(expression);
				}
			}
			return this;
		}

		public string Build()
		{
			_columns.Append("(");
			_values.Append("(");
			foreach (var expression in _expressions)
			{
				var name = expression.Member.Name;
				_columns.Append($"\"{name}\", ");
				_values.Append($"@{name}, ");
			}

			if (_columns.Length > 2)
			{
				_columns.Remove(_columns.Length - 2, 2);
			}

			if (_values.Length > 2)
			{
				_values.Remove(_values.Length - 2, 2);
			}
			_columns.Append(")");
			_values.Append(")");
			return _columns
				.Insert(0, $"INSERT INTO {_tableName} ")
				.Append(" VALUES ")
				.Append(_values)
				.Append(" returning *;")
				.ToString();
		}

		private bool IsDefault(object value)
		{
			if (value != null)
			{
				var type = value.GetType();
				if (type.IsValueType)
				{
					return value.Equals(Activator.CreateInstance(type));
				}

				return false;
			}
			return true;
		}
	}
}