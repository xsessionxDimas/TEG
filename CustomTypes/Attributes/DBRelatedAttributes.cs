using System;

namespace CustomTypes.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ByPassInsertParamAttribute : Attribute
    {
        // empty class
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ByPassUpdateParamAttribute : Attribute
    {
        // empty class
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class UCWordsFieldAttribute : Attribute
    {
        // empty class
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class LowerCaseFieldAttribute : Attribute
    {
        // empty class
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class UpperCaseFieldAttribute : Attribute
    {
        // empty class
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ActiveFieldAttribute : Attribute
    {
        // empty class
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NullFieldAttribute : Attribute
    {
        // empty class
    }
}
