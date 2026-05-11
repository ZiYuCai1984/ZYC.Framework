using System.Globalization;
using System.Text;
using ZYC.Framework.Abstractions.Workspace;

namespace ZYC.Framework.Workspace.CustomLayouts;

internal static class CustomWorkspaceLayoutThumbnailBuilder
{
    private static readonly string[] ThumbnailPalette =
    [
        "#2563EB",
        "#059669",
        "#D97706",
        "#DC2626",
        "#7C3AED",
        "#0891B2"
    ];

    public static string Build(WorkspaceNode root)
    {
        const double width = 96;
        const double height = 64;
        const double padding = 4;

        var sb = new StringBuilder();
        sb.Append("""
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 96 64">
  <rect x="1" y="1" width="94" height="62" rx="8" fill="#F8FAFC" stroke="#CBD5E1" stroke-width="2"/>
""");

        AppendWorkspaceNodeSvg(sb, root, padding, padding, width - padding * 2, height - padding * 2);
        sb.Append("</svg>");

        var svgBytes = Encoding.UTF8.GetBytes(sb.ToString());
        return $"data:image/svg+xml;base64,{Convert.ToBase64String(svgBytes)}";
    }

    private static void AppendWorkspaceNodeSvg(
        StringBuilder sb,
        WorkspaceNode node,
        double x,
        double y,
        double width,
        double height)
    {
        if (node.Left == null || node.Right == null)
        {
            var color = ThumbnailPalette[Math.Abs(node.Index) % ThumbnailPalette.Length];
            sb.Append(
                $"""
  <rect x="{Format(x)}" y="{Format(y)}" width="{Format(width)}" height="{Format(height)}" rx="4" fill="{color}" fill-opacity="0.88" stroke="#0F172A" stroke-opacity="0.18" stroke-width="1"/>
""");
            return;
        }

        const double gap = 2;
        var ratio = Math.Clamp(node.Ratio, 0.15, 0.85);

        if (node.IsHorizontal)
        {
            var leftWidth = Math.Max(0, width * ratio - gap / 2);
            var rightWidth = Math.Max(0, width - leftWidth - gap);

            AppendWorkspaceNodeSvg(sb, node.Left, x, y, leftWidth, height);
            AppendWorkspaceNodeSvg(sb, node.Right, x + leftWidth + gap, y, rightWidth, height);
            return;
        }

        var topHeight = Math.Max(0, height * ratio - gap / 2);
        var bottomHeight = Math.Max(0, height - topHeight - gap);

        AppendWorkspaceNodeSvg(sb, node.Left, x, y, width, topHeight);
        AppendWorkspaceNodeSvg(sb, node.Right, x, y + topHeight + gap, width, bottomHeight);
    }

    private static string Format(double value)
    {
        return value.ToString("0.##", CultureInfo.InvariantCulture);
    }
}
