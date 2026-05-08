using System.Windows;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Core;

/// <summary>
///     Base class for WPF dialog windows resolved through <see cref="IDialogManager" />.
/// </summary>
public class DialogWindowBase : Window, IDialog;
