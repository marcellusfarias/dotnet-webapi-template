﻿using MyBuyingList.Application.Contracts.BuyingListDtos;
using MyBuyingList.Application.DTOs;

namespace MyBuyingList.Application.Common.Interfaces.Services;

public interface IBuyingListService
{
    GetBuyingListDto? GetById(int id);
    /// <summary>
    /// Creates new buying list.
    /// </summary>
    /// <param name="buyingListDto"></param>
    /// <param name="currentUserId"></param>
    /// <returns>The new list Id.</returns>
    int Create(CreateBuyingListDto buyingListDto, int currentUserId);
    void ChangeName(UpdateBuyingListNameDto dto);
    /// <summary>
    /// Delete buying list and items associated.
    /// </summary>
    /// <param name="buyingListId"></param>
    /// <exception cref="">Throws ResourceNotFound</exception>
    void Delete(int buyingListId);
}