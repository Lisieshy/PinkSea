using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.Database;
using PinkSea.Database.Models;
using PinkSea.Models.Dto;

namespace PinkSea.Services;

/// <summary>
/// A feed builder.
/// </summary>
/// <param name="dbContext">The database context.</param>
public class FeedBuilder(
    PinkSeaDbContext dbContext,
    IDidResolver didResolver)
{
    /// <summary>
    /// The oekaki query.
    /// </summary>
    private IQueryable<OekakiModel> _query = dbContext
        .Oekaki
        .OrderByDescending(o => o.IndexedAt);

    /// <summary>
    /// Adds a where clause to the feed.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>The feed builder.</returns>
    public FeedBuilder Where(Expression<Func<OekakiModel, bool>> expression)
    {
        _query = _query.Where(expression);
        return this;
    }

    /// <summary>
    /// Adds filtering by a tag.
    /// </summary>
    /// <param name="tag">The tag.</param>
    /// <returns>This feed builder.</returns>
    public FeedBuilder WithTag(string tag)
    {
        _query = _query.Where(o =>
            dbContext.TagOekakiRelations
                .Include(r => r.Tag)
                .Any(r => r.OekakiId == o.Key && r.Tag.Name == tag));

        return this;
    }

    /// <summary>
    /// Sets the query to index since some time frame.
    /// </summary>
    /// <param name="since">Since when we should index.</param>
    /// <returns>The feed builder.</returns>
    public FeedBuilder Since(DateTimeOffset since)
    {
        _query = _query.Where(o => o.IndexedAt < since);
        return this;
    }

    /// <summary>
    /// Sets the limit on how many objects to fetch.
    /// </summary>
    /// <param name="count">The count of objects.</param>
    /// <returns>The feed builder.</returns>
    public FeedBuilder Limit(int count)
    {
        _query = _query.Take(count);
        return this;
    }

    /// <summary>
    /// Gets the feed.
    /// </summary>
    /// <returns>The list of oekaki DTOs.</returns>
    public async Task<List<OekakiDto>> GetFeed()
    {
        var list = await _query.ToListAsync();
        
        var dids = list.Select(o => o.AuthorDid);
        var map = new Dictionary<string, string>();
        foreach (var did in dids)
            map[did] = await didResolver.GetHandleFromDid(did) ?? "Invalid handle";

        var oekakiDtos = list
            .Select(o => OekakiDto.FromOekakiModel(o, map[o.AuthorDid]))
            .ToList();

        return oekakiDtos;
    }
}