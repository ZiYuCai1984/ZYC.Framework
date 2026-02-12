namespace ZYC.Automation.Abstractions.Config.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property)]
public class SkipResetAttribute : Attribute
{
}