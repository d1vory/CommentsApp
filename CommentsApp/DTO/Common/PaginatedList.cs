using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace CommentsApp.DTO.Common;

public class PaginatedList<T>
{
    public List<T> Items { get; private set; }
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }
    
    public int TotalItems { get; private set; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalItems = count;
        Items = items;
    }
    
    public static async Task<PaginatedList<T>> CreateAsync<TD>(IQueryable<TD> source, int pageIndex, int pageSize, IMapper mapper)
    {
        var count = await source.CountAsync();
        var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        var projectedItems = await mapper.ProjectTo<T>(items).ToListAsync();
        return new PaginatedList<T>(projectedItems, count, pageIndex, pageSize);
    }
}