using EnvironmentBuilder.Abstractions;
using EnvironmentBuilder.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EnvironmentBuilder.Extensions
{
#if NET35
    public class AggregateException : Exception
    {
        public List<Exception> InnerExceptions { get; set; } = new List<Exception>();
        public AggregateException(string message, params Exception[] innerExceptions): base(message)
        {
            InnerExceptions.AddRange(innerExceptions);
        }

        public AggregateException(string message, List<ArgumentException> innerExceptions) : base(message)
        {
            foreach (var argumentException in innerExceptions)
            {
                InnerExceptions.Add(argumentException);
            }
            
        }
    }
#endif
    public static class FlowExtensions
    {
        public static Resolvable<T> As<T>(this IEnvironmentBuilder bundle)
        {
            return new Resolvable<T>(bundle.Bundle().Build<T>);
        }

        public static Resolvable<T> When<T, TR>(this IEnvironmentBuilder bundle, TR match, Func<T> segment)
        {
            var wrapper = bundle.Bundle();
            return new Resolvable<T>(() =>
            {
                var resolvedValue = wrapper.Build<TR>();
                if ((resolvedValue != null && resolvedValue.Equals(match)) ||
                    (match != null && match.Equals(resolvedValue)))
                {
                    return segment == null
                        ? default
                        : segment.Invoke();
                }

                return default;
            })
            {
                Scope = new FlowScope
                {
                    PreviousState = wrapper
                }
            };
        }

        public static Resolvable<T> When<T, TR>(this Resolvable<T> segment, TR match, Func<T> nextSegment)
        {
            if (segment.Scope?.PreviousState == null)
            {
                throw new ArgumentException("Cannot create a logical or condition. Missing previous state. Start 'Or' conditions with 'As' precondition");
            }

            return new Resolvable<T>(() =>
            {
                var resolvedValue = segment.Scope.PreviousState.Build<TR>();
                if ((resolvedValue != null && resolvedValue.Equals(match)) ||
                    (match != null && match.Equals(resolvedValue)))
                {
                    return nextSegment == null
                        ? segment.Value
                        : nextSegment.Invoke();
                }

                return segment.Value;
            })
            {
                Scope = new FlowScope
                {
                    PreviousState = segment.Scope.PreviousState
                }
            };
        }

        public static Resolvable<T> Required<T>(this Resolvable<T> resolvable)
        {
            return new Resolvable<T>(() =>
                resolvable.Value == null
                    ? throw new ArgumentException("Missing required argument")
                    : resolvable.Value
            );
        }

        public static Resolvable<T> Required<T>(this IEnvironmentBuilder builder)
        {
            return builder.As<T>().Required();
        }

        public static FlowResult<T> Verify<T>(this FlowResult<T> result)
        {
            try
            {
                var value = result.Value;
                var argumentExceptions = new List<ArgumentException>();
                if (value.GetType().IsClass)
                {
                    foreach (var propertyInfo in value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        try
                        {
                            if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Resolvable<>))
                            {
                                var resolvable = propertyInfo.GetValue(value, null);
                                var verifiable =
                                    typeof(FlowExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                        .Where(i => i.Name == nameof(Verify))
                                        .FirstOrDefault(i =>
                                            i.GetParameters().FirstOrDefault()?.ParameterType.Name ==
                                            typeof(Resolvable<>).Name)
                                    ?.MakeGenericMethod(resolvable.GetType().GetGenericArguments());
                                verifiable?.Invoke(null, new[] {resolvable});
                            }
                            else
                            {
                                propertyInfo.GetValue(value, null).Verify();
                            }
                        }
                        catch(TargetInvocationException tix)
                        {
                            if(tix.InnerException is ArgumentException aix)
                            {
                                argumentExceptions.Add(new ArgumentException("Missing argument.", propertyInfo.Name, aix));
                            }
                            else if (tix.InnerException is AggregateException agix)
                            {
                                foreach (var iageInnerException in agix.InnerExceptions.OfType<ArgumentException>())
                                {
                                    argumentExceptions.Add(new ArgumentException("Missing argument.", propertyInfo.Name, iageInnerException));
                                }
                            }
                            else
                            {
                                throw;
                            }
                        }
                        catch (ArgumentException iae)
                        {
                            argumentExceptions.Add(new ArgumentException("Missing argument.", propertyInfo.Name, iae));
                        }
                        catch (AggregateException iage)
                        {
                            foreach (var iageInnerException in iage.InnerExceptions.OfType<ArgumentException>())
                            {
                                argumentExceptions.Add(new ArgumentException("Missing argument.", propertyInfo.Name, iageInnerException));
                            }
                        }
                    }
                }

                if (argumentExceptions.Count > 0)
                {
                    throw new AggregateException(
                        "The flow model is invalid.", 
                        argumentExceptions);
                }
            }
            catch (ArgumentException e)
            {
                var wrapper = new AggregateException(
                    "The flow model is invalid.",
                    e
                );
            }

            return result;
        }

        public static Resolvable<T> Verify<T>(this Resolvable<T> resolvable)
        {
            var value = resolvable.Value;
            value.Verify();
            return resolvable;
        }

        internal static T Verify<T>(this T value)
        {
            var argumentExceptions = new List<ArgumentException>();

            if (value.GetType().IsClass && value.GetType() != typeof(string))
            {
                foreach (var propertyInfo in value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    try
                    {
                        if (propertyInfo.PropertyType == typeof(Resolvable<>))
                        {
                            var resolvableValue = propertyInfo.GetValue(value, null);
                            var typeArg = resolvableValue.GetType().GetGenericArguments()[0];
                            var genericResolvable = resolvableValue.GetType().MakeGenericType(typeArg);
                            var verifyable =
                                typeof(FlowExtensions).GetMethod(nameof(Verify), new[] { genericResolvable });
                            verifyable?.Invoke(null, new[] { genericResolvable });
                        }
                        else
                        {
                            propertyInfo.GetValue(value, null).Verify();
                        }
                    }
                    catch (ArgumentException iae)
                    {
                        argumentExceptions.Add(new ArgumentException("Missing argument.", propertyInfo.Name, iae));
                    }
                    catch (AggregateException iage)
                    {
                        foreach (var iageInnerException in iage.InnerExceptions.OfType<ArgumentException>())
                        {
                            argumentExceptions.Add(new ArgumentException("Missing argument.", propertyInfo.Name, iageInnerException));
                        }
                    }
                }
            }

            if (argumentExceptions.Count > 0)
            {
                throw new AggregateException(
                    "The flow model is invalid.",
                    argumentExceptions);
            }

            return value;
        }
    }
}