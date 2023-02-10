using System.Collections.ObjectModel;

namespace Scrips.Core.Shared.Authorization;

public static class ScripsAction
{
    public const string View = nameof(View);
    public const string Search = nameof(Search);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
}

public static class ScripsResource
{
    public const string Tenants = nameof(Tenants);
}

public static class ScripsPermissions
{
    private static readonly ScripsPermission[] _all = new ScripsPermission[]
    {
        new("View Tenants", ScripsAction.View, ScripsResource.Tenants, IsRoot: true),
        new("Create Tenants", ScripsAction.Create, ScripsResource.Tenants, IsRoot: true),
        new("Update Tenants", ScripsAction.Update, ScripsResource.Tenants, IsRoot: true),
    };

    public static IReadOnlyList<ScripsPermission> All { get; } = new ReadOnlyCollection<ScripsPermission>(_all);
    public static IReadOnlyList<ScripsPermission> Root { get; } = new ReadOnlyCollection<ScripsPermission>(_all.Where(p => p.IsRoot).ToArray());
    public static IReadOnlyList<ScripsPermission> Admin { get; } = new ReadOnlyCollection<ScripsPermission>(_all.Where(p => !p.IsRoot).ToArray());
    public static IReadOnlyList<ScripsPermission> Basic { get; } = new ReadOnlyCollection<ScripsPermission>(_all.Where(p => p.IsBasic).ToArray());
}

public record ScripsPermission(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false)
{
    public string Name => NameFor(Action, Resource);
    public static string NameFor(string action, string resource) => $"Permissions.{resource}.{action}";
}
