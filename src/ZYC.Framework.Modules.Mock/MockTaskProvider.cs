using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Modules.TaskManager.Abstractions;

namespace ZYC.Framework.Modules.Mock;

[RegisterSingleInstanceAs(typeof(ITaskProvider), PreserveExistingDefaults = true)]
internal sealed class MockTaskProvider : ITaskProvider
{
    public MockTaskProvider(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;

        UninitializedTaskDefinitions.Add(
            ObjectTools.CreateUninitializedObject<CountTaskDefinition>());
    }

    private IList<IManagedTaskDefinition> UninitializedTaskDefinitions { get; } = new List<IManagedTaskDefinition>();

    private ILifetimeScope LifetimeScope { get; }

    public string ProviderId => "mock";

    public IEnumerable<TaskDefinitionDescriptor> GetDefinitions()
    {
        var taskDefinitionDescriptors = new List<TaskDefinitionDescriptor>();

        foreach (var t in UninitializedTaskDefinitions)
        {
            taskDefinitionDescriptors.Add(
                new TaskDefinitionDescriptor(
                    t.GetType(),
                    $"{ProviderId}/{t.TaskType}",
                    1,
                    t.DisplayName,
                    t.Description));
        }

        return taskDefinitionDescriptors;
    }

    public IManagedTaskDefinition Create(TaskDefinitionCreateContext createContext)
    {
        foreach (var definition in GetDefinitions())
        {
            if (definition.DefinitionId == createContext.DefinitionId)
            {
                return (IManagedTaskDefinition)LifetimeScope.Resolve(
                    definition.DefinitionType,
                    new TypedParameter(typeof(TaskDefinitionCreateContext), createContext));
            }
        }

        throw new InvalidOperationException(
            $"Unknown definition: {createContext.DefinitionId}@{createContext.Version}");
    }
}