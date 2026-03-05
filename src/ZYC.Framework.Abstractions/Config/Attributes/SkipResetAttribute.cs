namespace ZYC.Framework.Abstractions.Config.Attributes;

#pragma warning disable CS1591

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property)]
public class SkipResetAttribute : Attribute
{
}