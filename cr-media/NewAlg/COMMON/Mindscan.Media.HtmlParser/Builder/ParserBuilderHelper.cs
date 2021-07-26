using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Mindscan.Media.HtmlParser.Core;
using Mindscan.Media.HtmlParser.Core.Commands;
using Mindscan.Media.HtmlParser.Core.Expression;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Builder
{
	internal static class ParserBuilderHelper
	{
		private static readonly Dictionary<string, Type> SupportedCommands;
		private static readonly Dictionary<string, Type> SupportedSchemas;
		private static readonly Dictionary<string, Type> SupportedParsers;
		private static readonly Dictionary<string, Type> SupportedExpressions;

		private static readonly Regex DelimiterRegex = @"\)\.\b".CreateRegex();
		private static readonly Regex NameRegex = "((?<name>\\w*)\\s*\\()".CreateRegex();
		static ParserBuilderHelper()
		{
			SupportedCommands = GetDerivedTypes<IPipelineCommand>();
			SupportedSchemas = GetDerivedTypes<ISchema>();
			SupportedParsers = GetDerivedTypes<IPageParser>();
			SupportedExpressions = GetDerivedTypes<IExpression>();
		}

		private static Dictionary<string, Type> GetDerivedTypes<T>()
		{
			var type = typeof(T);

			return type.Assembly
				.GetTypes()
				.Where(p => type.IsAssignableFrom(p) && !p.IsAbstract)
				.ToDictionary(
					x => (x.GetCustomAttribute(typeof(CustomJsonObjectAttribute), true) as CustomJsonObjectAttribute)?.FullName.ToLower(),
					x => x);
		}

		public static IEnumerable<IPipelineCommand> ParsePipelineCode(string code)
		{
			if (string.IsNullOrWhiteSpace(code))
			{
				yield break;
			}
			foreach (var funcExpression in DelimiterRegex.Split(code))
			{
				var name = GetFunctionName(funcExpression);
				if (!string.IsNullOrEmpty(name))
				{
					var fullName = $"{SupportedNames.Commands.Namespace}.{name}".ToLower();

					Type targetType;
					if (TryGetType<IPipelineCommand>(fullName, out targetType))
					{
						var result = Activator.CreateInstance(targetType) as CommandBase;
						if (result != null)
						{
							result.FromExpression(GetParametersJson(name, funcExpression));
							yield return result;
						}
					}
				}
			}
		}

		public static IExpression BuildExpressionCode(IExpression expression)
		{
			var pipe = expression as PipelineExpression;
			if (pipe != null)
			{
				pipe.Code = BuildPipelineCode(pipe.Commands);
				pipe.Commands.Clear();
				return pipe;
			}
			var baseExpression = expression as ExpressionBase;
			if (baseExpression != null && baseExpression.Expressions.Count > 0)
			{
				for (var i=0; i < baseExpression.Expressions.Count; ++i)
				{
					baseExpression.Expressions[i] = BuildExpressionCode(baseExpression.Expressions[i]);
				}
			}
			return expression;
		}

		public static string BuildPipelineCode(IEnumerable<IPipelineCommand> commands)
		{
			var expressions = new List<string>();
			foreach (var command in commands)
			{
				var expression = (command as CommandBase)?.ToExpression();
				if (!string.IsNullOrWhiteSpace(expression))
				{
					expressions.Add(expression);
				}
			}
			return string.Join(".", expressions);
		}

		public static bool TryGetType<T>(string name, out Type type) where T : class
		{
			Dictionary<string, Type> temp = null;
			var tType = typeof(T);
			if (typeof(IPageParser).IsAssignableFrom(tType))
			{
				temp = SupportedParsers;
			}
			if (typeof(IPipelineCommand).IsAssignableFrom(tType))
			{
				temp = SupportedCommands;
			}
			if (typeof(ISchema).IsAssignableFrom(tType))
			{
				temp = SupportedSchemas;
			}
			if (typeof(IExpression).IsAssignableFrom(tType))
			{
				temp = SupportedExpressions;
			}
			if (temp != null && !string.IsNullOrWhiteSpace(name) && temp.TryGetValue(name.ToLower(), out type))
			{
				return true;
			}
			type = null;
			return false;
		}

		public static string ToJsonParameters(this int value)
		{
			return ToJsonParameters(new[] { value.ToString() });
		}

		public static string ToJsonParameters(this string value)
		{
			return ToJsonParameters(new[] { value });
		}

		public static string ToJsonParameters(this IEnumerable<string> values)
		{
			var list = values?.Where(x => !string.IsNullOrEmpty(x)).ToList();
			if (list != null && list.Count > 0)
				return JsonConvert.SerializeObject(values).Trim('[', ']');
			return string.Empty;
		}

		public static string ToJsonParameters(this List<List<string>> values)
		{
			var result = new List<string>();
			foreach (var list in values)
			{
				if (list != null && list.Count > 0)
				{
					result.Add(JsonConvert.SerializeObject(list));
				}
			}
			return string.Join(",", result);
		}

		private static string GetFunctionName(string expression)
		{
			var match = NameRegex.Match(expression);
			if (match.Success)
			{
				return match.Groups["name"].Value;
			}
			return null;
		}

		private static string GetParametersJson(string funcName, string funcExpression)
		{
			return $"[{funcExpression.Replace(funcName, "").Trim(' ', '(', ')')}]";
		}
	}
}
