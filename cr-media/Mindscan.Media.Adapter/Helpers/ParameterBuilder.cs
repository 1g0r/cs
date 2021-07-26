using System;
using System.Linq.Expressions;
using System.Text;

namespace Mindscan.Media.Adapter.Helpers
{
	public class ParameterBuilder<TEntity> where TEntity : class
	{
		private string _value;

		public ParameterBuilder<TEntity> Name(string name)
		{
			if (!string.IsNullOrWhiteSpace(name))
			{
				_value = name + " => ";
			}

			return this;
		}
		public ParameterBuilder<TEntity> Set(Expression<Func<TEntity, object>> expression)
		{
			_value += Parse((dynamic)expression.Body);
			return this;
		}

		public ParameterBuilder<TEntity> SetComplex(Expression<Func<TEntity, object>> expression)
		{
			_value += $"({Parse((dynamic)expression.Body)})";
			return this;
		}

		public ParameterBuilder<TEntity> OfType(string name)
		{
			_value += "::" + name;
			return this;
		}

		public string Build()
		{
			return _value;
		}

		private string Parse(NewExpression member)
		{
			if (member.Members == null || member.Members.Count == 0)
				return "";

			var result = new StringBuilder("");
			result.Append($"@{member.Members[0].Name}");
			for (var i = 1; i < member.Members.Count; ++i)
			{
				result.Append($", @{member.Members[i].Name}");
			}
			return result + "";
		}

		private string Parse(UnaryExpression member)
		{
			return Parse((dynamic)member.Operand);
		}
		private string Parse(MemberExpression member)
		{
			return "@" + member.Member.Name;
		}
		private string Parse(dynamic member)
		{
			return "";
		}
	}
}