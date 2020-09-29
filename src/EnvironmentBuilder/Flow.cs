using EnvironmentBuilder.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnvironmentBuilder
{
    public class Flow : IFlow
    {
        private readonly IEnvironmentBuilder _builder;
        private readonly IFlow _parent;

        public static IFlow Create(Action<Abstractions.IEnvironmentConfiguration> configuration = null)
        {
            var builder = EnvironmentManager.Create(configuration);

            return new Flow(builder);
        }

        private Flow(IEnvironmentBuilder builder, IFlow parent = null)
        {
            _builder = builder;
            _parent = parent;
        }
        public IFlow Case(Func<IEnvironmentBuilder, IEnvironmentBundle> sources)
        {
            var bundle = sources?.Invoke(_builder);
            throw new NotImplementedException();
        }

        public IFlow OneOf(params Action<IFlow>[] cases)
        {
            throw new NotImplementedException();
        }

        public IFlow AllOf(params Action<IFlow>[] cases)
        {
            throw new NotImplementedException();
        }

        public IFlow AnyOf(params Action<IFlow>[] cases)
        {
            throw new NotImplementedException();
        }

        public IFlow And => _parent ?? throw new InvalidOperationException();
        public IFlow Or => _parent ?? throw new InvalidOperationException();

        public void Validate()
        {

        }
    }

    public interface IFlow
    {
        IFlow Case(Func<IEnvironmentBuilder, IEnvironmentBundle> sources);
        IFlow OneOf(params Action<IFlow>[] cases);
        IFlow AllOf(params Action<IFlow>[] cases);
        IFlow AnyOf(params Action<IFlow>[] cases);
        IFlow And { get; }
        IFlow Or { get; }
    }

    public interface IFlowCaseBuilder : IFlow
    {
        
    }
}
