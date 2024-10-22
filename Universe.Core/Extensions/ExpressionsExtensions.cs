using System.Linq.Expressions;

namespace Universe.Core.Extensions;

public static class ExpressionsExtensions
{
	public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> exp, Expression<Func<T, bool>> newExp) =>
		Combine(exp, newExp, Expression.Or);

	public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> exp, Expression<Func<T, bool>> newExp) =>
		Combine(exp, newExp, Expression.And);


	private static Expression<Func<T, bool>> Combine<T>(
		Expression<Func<T, bool>> exp,
		Expression<Func<T, bool>> newExp,
		Func<Expression, Expression, BinaryExpression> combiner)
	{
		var visitor = new ParameterUpdateVisitor(newExp.Parameters.First(), exp.Parameters.First());
		var newExpWithSameParamName = visitor.Visit(newExp) as Expression<Func<T, bool>>;

		if (newExpWithSameParamName is null)
			throw new InvalidOperationException("В процессе объединения двух выражений произошла ошибка после приведения параметров к одному имени.");

		var binExp = combiner(exp.Body, newExpWithSameParamName.Body);
		return Expression.Lambda<Func<T, bool>>(binExp, newExp.Parameters);
	}

	class ParameterUpdateVisitor : ExpressionVisitor
	{
		private readonly ParameterExpression _oldParameter;
		private readonly ParameterExpression _newParameter;

		public ParameterUpdateVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
		{
			_oldParameter = oldParameter;
			_newParameter = newParameter;
		}

		protected override Expression VisitParameter(ParameterExpression node) =>
			ReferenceEquals(node, _oldParameter) ? _newParameter : base.VisitParameter(node);
	}
}
