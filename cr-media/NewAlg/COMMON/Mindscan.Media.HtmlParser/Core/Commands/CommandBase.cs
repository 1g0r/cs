using System.Diagnostics;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[DebuggerStepThrough]
	internal abstract class CommandBase : CustomJsonObject, IPipelineCommand
	{
		public virtual object Run(ParserContext context, object data)
		{
			if (data != null)
			{
				return Execute(context, data);
			}
			return null;
		}
		internal string ToExpression()
		{
			return $"{Name}({BuildParametersJson()})";
		}

		protected internal abstract void FromExpression(string json);

		protected internal abstract string BuildParametersJson();

		protected virtual object Execute(ParserContext context, object data)
		{
			//By default return null.
			return null;
		}
	}

	[DebuggerStepThrough]
	internal abstract class CommandBase<T1> : CommandBase
	{
		//Override to be able to see overloads of Execute method
		protected override object Execute(ParserContext context, object data)
		{
			if (data is T1)
			{
				return Execute(context, (T1)data);
			}
			return base.Execute(context, data);
		}
		protected abstract object Execute(ParserContext context, T1 data);
	}

	[DebuggerStepThrough]
	internal abstract class CommandBase<T1, T2> : CommandBase<T1>
	{
		//Override to be able to see overloads of Execute method
		protected override object Execute(ParserContext context, object data)
		{
			if (data is T2)
			{
				return Execute(context, (T2)data);
			}
			return base.Execute(context, data);
		}
		protected abstract object Execute(ParserContext context, T2 data);
	}

	[DebuggerStepThrough]
	internal abstract class CommandBase<T1, T2, T3> : CommandBase<T1, T2> 
	{
		//Override to be able to see overloads of Execute method
		protected override object Execute(ParserContext context, object data)
		{
			if (data is T3)
			{
				return Execute(context, (T3)data);
			}
			return base.Execute(context, data);
		}
		protected abstract object Execute(ParserContext context, T3 data);
	}

	[DebuggerStepThrough]
	internal abstract class CommandBase<T1, T2, T3, T4> : CommandBase<T1, T2, T3> 
	{
		//Override to be able to see overloads of Execute method
		protected override object Execute(ParserContext context, object data)
		{
			if (data is T4)
			{
				return Execute(context, (T4)data);
			}
			return base.Execute(context, data);
		}
		protected abstract object Execute(ParserContext context, T4 data);
	}

	[DebuggerStepThrough]
	internal abstract class CommandBase<T1, T2, T3, T4, T5> : CommandBase<T1, T2, T3, T4> 
	{
		//Override to be able to see overloads of Execute method
		protected override object Execute(ParserContext context, object data)
		{
			if (data is T5)
			{
				return Execute(context, (T5)data);
			}
			return base.Execute(context, data);
		}
		protected abstract object Execute(ParserContext context, T5 data);
	}

	[DebuggerStepThrough]
	internal abstract class CommandBase<T1, T2, T3, T4, T5, T6> : CommandBase<T1, T2, T3, T4, T5> 
	{
		//Override to be able to see overloads of Execute method
		protected override object Execute(ParserContext context, object data)
		{
			if (data is T6)
			{
				return Execute(context, (T6)data);
			}
			return base.Execute(context, data);
		}
		protected abstract object Execute(ParserContext context, T6 data);
	}

	[DebuggerStepThrough]
	internal abstract class CommandBase<T1, T2, T3, T4, T5, T6, T7> : CommandBase<T1, T2, T3, T4, T5, T6>
	{
		//Override to be able to see overloads of Execute method
		protected override object Execute(ParserContext context, object data)
		{
			if (data is T7)
			{
				return Execute(context, (T7)data);
			}
			return base.Execute(context, data);
		}
		protected abstract object Execute(ParserContext context, T7 data);
	}

	[DebuggerStepThrough]
	internal abstract class CommandBase<T1, T2, T3, T4, T5, T6, T7, T8> : CommandBase<T1, T2, T3, T4, T5, T6, T7>
	{
		//Override to be able to see overloads of Execute method
		protected override object Execute(ParserContext context, object data)
		{
			if (data is T8)
			{
				return Execute(context, (T8)data);
			}
			return base.Execute(context, data);
		}
		protected abstract object Execute(ParserContext context, T8 data);
	}
}
