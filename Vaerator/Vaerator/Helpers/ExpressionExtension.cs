using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Vaerator.Helpers
{
    public static class ExpressionExtension
    {
        public static string GetPropertyPath<T, P>(Expression<Func<T, P>> expression)
        {
            // Working outside in e.g. given p.Spouse.Name - the first node will be Name, then Spouse, then p
            IList<string> propertyNames = new List<string>();
            var currentNode = expression.Body;
            while (currentNode.NodeType != ExpressionType.Parameter)
            {
                switch (currentNode.NodeType)
                {
                    case ExpressionType.MemberAccess:
                    case ExpressionType.Convert:
                        MemberExpression memberExpression;
                        memberExpression = (currentNode.NodeType == ExpressionType.MemberAccess ? (MemberExpression)currentNode : (MemberExpression)((UnaryExpression)currentNode).Operand);
                        if (!(memberExpression.Member is PropertyInfo ||
                                memberExpression.Member is FieldInfo))
                        {
                            throw new InvalidOperationException("The member '" + memberExpression.Member.Name + "' is not a field or property");
                        }
                        propertyNames.Add(memberExpression.Member.Name);
                        currentNode = memberExpression.Expression;
                        break;
                    case ExpressionType.Call:
                        MethodCallExpression methodCallExpression = (MethodCallExpression)currentNode;
                        if (methodCallExpression.Method.Name == "get_Item")
                        {
                            propertyNames.Add("[" + methodCallExpression.Arguments.First().ToString() + "]");
                            currentNode = methodCallExpression.Object;
                        }
                        else
                        {
                            throw new InvalidOperationException("The member '" + methodCallExpression.Method.Name + "' is a method call but a Property or Field was expected.");
                        }
                        break;

                    // To include method calls, remove the exception and uncomment the following three lines:
                    //propertyNames.Add(methodCallExpression.Method.Name);
                    //currentExpression = methodCallExpression.Object;
                    //break;
                    default:
                        throw new InvalidOperationException("The expression NodeType '" + currentNode.NodeType.ToString() + "' is not supported, expected MemberAccess, Convert, or Call.");
                }
            }
            return string.Join(".", propertyNames.Reverse().ToArray());
        }
    }
}
