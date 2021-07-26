namespace Mindscan.Media.HtmlParser
{
	public interface IExpression
	{
		object Evaluate(ExpressionContext context, object data);
	}
}