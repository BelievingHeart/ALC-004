﻿using System;
using System.Linq.Expressions;
using System.Reflection;
 using System.Threading.Tasks;

 namespace WPFCommon.Helpers
{
    public static class ExpressionHelper
    {
        /// <summary>
        /// Gets the property that the expression is wrapping
        /// </summary>
        /// <param name="expression">The expression to compile and invoke</param>
        /// <typeparam name="T">The type of the wrapped property</typeparam>
        /// <returns></returns>
        public static T GetPropValue<T>(this Expression<Func<T>> expression)
        {
            return expression.Compile().Invoke();
        }

        public static void SetPropValue<T>(this Expression<Func<T>> expression, T value)
        {
            // Convert ()=> owner.Property to owner.Property
            var ownerDotProperty = (MemberExpression) expression.Body;
            // Get the property
            var propertyInfo = (PropertyInfo) ownerDotProperty.Member;
            // Get the owner of the property
            var owner = Expression.Lambda(ownerDotProperty.Expression).Compile().DynamicInvoke();
            // Set the property value of the owner
            propertyInfo.SetValue(owner, value);
        }
        
        /// <summary>
        /// Run async action that is only allowed to run one instance each time
        /// This workaround exists because property can not be passed by ref
        /// For example, to avoid multiple button clicks before a task is finished
        /// The action is allowed to run if the wrapped property is false, i.e. not busy
        /// If the action is finished, the action is allowed to run again
        /// </summary>
        /// <param name="isBusyExpression">The expression that wraps a boolean property</param>
        /// <param name="action">The task to run</param>
        /// <returns></returns>
        public static async Task RunOnlySingleFireIsAllowedEachTimeCommand(Expression<Func<bool>> isBusyExpression,
            Func<Task> action)
        {
            // Check if the system is busy
            if (isBusyExpression.GetPropValue()) return;
            // Flag the system busy before task is run
            isBusyExpression.SetPropValue(true);
            // Execute task if the system is not busy
            try
            {
                await action();
            }
            finally
            {
                // Flag the system not-busy again after task is finished
                isBusyExpression.SetPropValue(false);
            }
        }
    }
}