﻿using FitNoteIT.Modules.Users.Core.Abstractions;
using FitNoteIT.Modules.Users.Core.Entities;
using FitNoteIT.Modules.Users.Core.Exceptions;
using FitNoteIT.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FitNoteIT.Modules.Users.Core.Persistence.Repositories;
internal sealed class UserRepository : IUserRepository
{
	private readonly UsersDbContext _dbContext;

	public UserRepository(UsersDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<PaginatedList<User>> PaginatedGetAllAsync(int pageNumber, int pageSize)
	{
		var query = await _dbContext.Users.Include(x => x.Roles).PaginatedListAsync(pageNumber, pageSize);
		
		return query;
	}
	
	public async Task<User> GetByIdAsync(Guid id)
	{
		var query = await _dbContext.Users.Include(x => x.Roles).SingleOrDefaultAsync(x => x.Id == id) ?? throw new UserNotFoundException(id);
		
		return query;
	}

	public async Task<User> GetByEmailAsync(string email)
	{
		var query = await _dbContext.Users.Include(x => x.Roles).SingleOrDefaultAsync(x => x.Email == email) ?? throw new UserNotFoundException(email, "email");
		
		return query;
	}

	public async Task<User> GetByUserNameAsync(string userName)
	{
		var query = await _dbContext.Users.Include(x => x.Roles).SingleOrDefaultAsync(x => x.UserName == userName) ?? throw new UserNotFoundException(userName, "username");
		return query;
	}

	public async Task AddAsync(User user)
	{
		await _dbContext.Users.AddAsync(user);
		await _dbContext.SaveChangesAsync();
	}

	public async Task UpdateAsync(User user)
	{
		_dbContext.Users.Update(user);
		await _dbContext.SaveChangesAsync();
	}

	public async Task DeleteAsync(User user)
	{
		_dbContext.Users.Remove(user);
		await _dbContext.SaveChangesAsync();
	}

	public async Task<bool> CredentialsAvailableForUser(string? email, string? userName)
	{
		var query = await _dbContext.Users.AnyAsync(x => x.Email == email || x.UserName == userName);
		return query;
	}
}
