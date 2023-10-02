﻿using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Features.BuyingLists;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Infrastructure.Repositories;

public class BuyingListRepository : RepositoryBase<BuyingList>, IBuyingListRepository
{
    public BuyingListRepository(ApplicationDbContext context) : base(context) { }

    public async Task DeleteBuyingListAndItemsAsync(BuyingList buyingList, CancellationToken token)
    {
        try
        {
            foreach (var item in buyingList.Items)
                _context.Set<BuyingListItem>().Remove(item);
            
            _context.Set<BuyingList>().Remove(buyingList);
            await _context.SaveChangesAsync(token);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }
}