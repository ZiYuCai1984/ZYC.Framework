using ZYC.Framework.Core.Converters;
using ZYC.Framework.Modules.TaskManager.Abstractions;

namespace ZYC.Framework.Modules.TaskManager.Converters;

internal class ProgressMultiValueConverter : MultiValueConverterBase<double?, ManagedTaskState, double>
{
    protected override double Convert(double? progress, ManagedTaskState state)
    {
        if (state == ManagedTaskState.Completed)
        {
            return 1.0;
        }

        if (progress == null)
        {
            return 0.0;
        }

        return progress.Value;
    }
}