using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Mindscan.Media.Adapter.Helpers
{
	public class FunctionExpressionBuilder<TEntity> where TEntity : class
	{
		private readonly TEntity _entity;
		private readonly List<ParameterBuilder<TEntity>> _parameters = new List<ParameterBuilder<TEntity>>();
		private string _name;
		public FunctionExpressionBuilder(TEntity entity)
		{
			_entity = entity;
		}

		public FunctionExpressionBuilder<TEntity> Name(string name)
		{
			_name = name;
			return this;
		}

		public FunctionExpressionBuilder<TEntity> Parameters(params Func<ParameterBuilder<TEntity>, ParameterBuilder<TEntity>>[] builders)
		{
			foreach (var builder in builders)
			{
				_parameters.Add(builder.Invoke(new ParameterBuilder<TEntity>()));
			}
			return this;
		}

		public FunctionExpressionBuilder<TEntity> Parameters(Expression<Func<TEntity, object>> expression)
		{
			if (expression != null)
			{
				_parameters.Add(new ParameterBuilder<TEntity>().Set(expression));
			}

			return this;
		}

		public string Build()
		{
			if (string.IsNullOrWhiteSpace(_name))
				throw new InvalidOperationException("Specify function name");
			var builder = new StringBuilder("SELECT * FROM ").Append(_name).Append(" (");
			if (_parameters.Count > 0)
			{
				builder.Append($"{_parameters[0].Build()}");
				for (int i = 1; i < _parameters.Count; i++)
				{
					builder.Append($", {_parameters[i].Build()}");
				}
			}

			builder.Append(");");
			return builder.ToString();
		}
	}
}