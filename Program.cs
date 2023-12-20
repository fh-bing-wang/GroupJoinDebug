using Microsoft.EntityFrameworkCore;
using Trial;

var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseNpgsql("Host=jp-gateway-preprocessing-db-1.localhost.localdomain;Port=443;Username=postgres;Password=password")
    .Options;

using var context = new AppDbContext(contextOptions);
var query = context.Parents
    .GroupJoin(
        context.Children,
        p => p.Id,
        children => children.ParentId,
        (parent, children) => new PClass{ Parent=parent, Children = children.DefaultIfEmpty() }
    )
    .SelectMany(entity => entity.Children.Select(c => new { entity.Parent, c }))
    .GroupJoin(
        context.Children,
        p => p.Parent.Id,
        children => children.ParentId,
        (parent, children) => new { parent, children = children.DefaultIfEmpty() }
    )
    .SelectMany(entity => entity.children.Select(c => new { entity.parent, c }))
    .ToQueryString();

Console.WriteLine(query);