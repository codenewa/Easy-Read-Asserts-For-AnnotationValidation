using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    public static class EvaluationValidator<TObj>
    {
        public static bool StaticValidation(TObj obj)
        {
            var brokenRules = new List<ValidationResult>();
            var context = new ValidationContext(obj, null, null);
            var result = Validator.TryValidateObject(obj, context, brokenRules, true);

            if (brokenRules.Count > 0)
                return false;
            else
                return true;
        }

        public static List<ValidationResult> StaticValidationBrokenRulesList(TObj obj)
        {
            var brokenRules = new List<ValidationResult>();
            var context = new ValidationContext(obj, null, null);
            var result = Validator.TryValidateObject(obj, context, brokenRules, true);

            return brokenRules;
        }
    }

    public static class ModelAssertionExtensions
    {
        public static Tuple<T, Expression<Func<T, object>>> When<T>(this T model, Expression<Func<T, object>> property)
        {
            return new Tuple<T, Expression<Func<T, object>>>(model, property);
        }

        public static Tuple<T, Expression<Func<T, object>>> HasValue<T>(this Tuple<T, Expression<Func<T, object>>> targetModel, object value)
        {
            ParameterExpression valueParameterExpression = Expression.Parameter(typeof(object));
            var expression = targetModel.Item2;
            Expression targetExpression = expression.Body is UnaryExpression ? ((UnaryExpression)expression.Body).Operand : expression.Body;

            var newValue = Expression.Parameter(targetModel.Item2.Body.Type);
            var assign = Expression.Lambda<Action<T, object>>
                        (
                            Expression.Assign(targetExpression, Expression.Convert(valueParameterExpression, targetExpression.Type)),
                            expression.Parameters.Single(), valueParameterExpression
                        );

            assign.Compile().Invoke(targetModel.Item1, value);
            return new Tuple<T, Expression<Func<T, object>>>(targetModel.Item1, expression);
        }

        public static Tuple<T, Expression<Func<T, object>>, bool> InvokeValidation<T>(this Tuple<T, Expression<Func<T, object>>> tuple)
        {
            var sut = tuple.Item1;
            var result = EvaluationValidator<T>.StaticValidation(sut);
            return new Tuple<T, Expression<Func<T, object>>, bool>(sut, tuple.Item2, result);
        }

        private static string GetPropertyName<T>(this Expression<Func<T, object>> expr)
        {
            return (expr.Body is UnaryExpression)
                ? ((expr.Body as UnaryExpression).Operand as MemberExpression).Member.Name
                : (expr.Body as MemberExpression).Member.Name;
        }

        public static void ShouldPassBecause<T>(this Tuple<T, Expression<Func<T, object>>, bool> result, string reason)
        {
            var sut = result.Item1;
            var property = result.Item2;

            Assert.IsTrue(result.Item3, string.Format("{0} should fail validation because {1}",property.GetPropertyName(), reason));
        }

        public static void ShouldFailBecause<T>(this Tuple<T, Expression<Func<T, object>>, bool> result, string reason)
        {
            var sut = result.Item1;
            var property = result.Item2;


            
            Assert.IsFalse(result.Item3, string.Format("{0} should fail validation because {1}", property.GetPropertyName(), reason));
        }
    }
}
