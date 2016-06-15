using ModestTree.Util;
using System;

namespace Zenject
{
    public enum InjectSources
    {
        Any,
        Local,
        Parent,
        AnyParent,
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public abstract class InjectAttributeBase : PreserveAttribute
    {
        public bool IsOptional
        {
            get;
            protected set;
        }

        public string Identifier
        {
            get;
            protected set;
        }

        public InjectSources SourceType
        {
            get;
            protected set;
        }
    }

    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class InjectAttribute : InjectAttributeBase
    {
        public InjectAttribute(string identifier)
        {
            Identifier = identifier;
        }

        public InjectAttribute(InjectSources sourceType)
        {
            SourceType = sourceType;
        }

        public InjectAttribute(string identifier, InjectSources sourceType)
        {
            Identifier = identifier;
            SourceType = sourceType;
        }

        public InjectAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class InjectOptionalAttribute : InjectAttributeBase
    {
        public InjectOptionalAttribute(string identifier)
        {
            Identifier = identifier;
            IsOptional = true;
        }

        public InjectOptionalAttribute(InjectSources sourceType)
        {
            SourceType = sourceType;
            IsOptional = true;
        }

        public InjectOptionalAttribute(string identifier, InjectSources sourceType)
        {
            Identifier = identifier;
            SourceType = sourceType;
            IsOptional = true;
        }

        public InjectOptionalAttribute()
        {
            IsOptional = true;
        }
    }
}
