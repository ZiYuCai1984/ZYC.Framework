using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework;

[RegisterSingleInstanceAs(typeof(IProduct))]
internal class Product : IProduct
{
    public string PackageId => ProductInfo.PackageId;

    public string Version => ProductInfo.Version;

    public string Description => ProductInfo.Description;

    public string Copyright => ProductInfo.Copyright;

    public string Author => ProductInfo.Author;
}