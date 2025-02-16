﻿using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SimpleTodo.Api
{
    public static class TodoEndpointsExtensions
    {
        public static RouteGroupBuilder MapTodoApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetLists);
            group.MapPost("/", CreateList);
            group.MapGet("/{listId:guid}", GetList);
            group.MapPut("/{listId:guid}", UpdateList);
            group.MapDelete("/{listId:guid}", DeleteList);
            group.MapGet("/{listId:guid}/items", GetListItems);
            group.MapPost("/{listId:guid}/items", CreateListItem);
            group.MapGet("/{listId:guid}/items/{itemId:guid}", GetListItem);
            group.MapPut("/{listId:guid}/items/{itemId:guid}", UpdateListItem);
            group.MapDelete("/{listId:guid}/items/{itemId:guid}", DeleteListItem);
            group.MapGet("/{listId:guid}/state/{state}", GetListItemsByState);
            return group;
        }

        public static RouteGroupBuilder MapVersionApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAppVersion);
            return group;
        }

        public static async Task<Ok<IEnumerable<TodoList>>> GetLists(ListsRepository repository, int? skip = null, int? batchSize = null)
        {
            return TypedResults.Ok(await repository.GetListsAsync(skip, batchSize));
        }

        public static async Task<IResult> CreateList(ListsRepository repository, CreateUpdateTodoList list)
        {
            var todoList = new TodoList(list.name)
            {
                Description = list.description
            };

            await repository.AddListAsync(todoList);

            return TypedResults.Created($"/lists/{todoList.Id}", todoList);
        }

        public static async Task<IResult> GetList(ListsRepository repository, Guid listId)
        {
            var list = await repository.GetListAsync(listId);

            return list == null ? TypedResults.NotFound() : TypedResults.Ok(list);
        }

        public static async Task<IResult> UpdateList(ListsRepository repository, Guid listId, CreateUpdateTodoList list)
        {
            var existingList = await repository.GetListAsync(listId);
            if (existingList == null)
            {
                return TypedResults.NotFound();
            }

            existingList.Name = list.name;
            existingList.Description = list.description;
            existingList.UpdatedDate = DateTimeOffset.UtcNow;

            await repository.SaveChangesAsync();

            return TypedResults.Ok(existingList);
        }

        public static async Task<IResult> DeleteList(ListsRepository repository, Guid listId)
        {
            if (await repository.GetListAsync(listId) == null)
            {
                return TypedResults.NotFound();
            }

            await repository.DeleteListAsync(listId);

            return TypedResults.NoContent();
        }

        public static async Task<IResult> GetListItems(ListsRepository repository, Guid listId, int? skip = null, int? batchSize = null)
        {
            if (await repository.GetListAsync(listId) == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(await repository.GetListItemsAsync(listId, skip, batchSize));
        }

        public static async Task<IResult> CreateListItem(ListsRepository repository, Guid listId, CreateUpdateTodoItem item)
        {
            if (await repository.GetListAsync(listId) == null)
            {
                return TypedResults.NotFound();
            }

            var newItem = new TodoItem(listId, item.name)
            {
                Name = item.name,
                Description = item.description,
                State = item.state,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await repository.AddListItemAsync(newItem);

            return TypedResults.Created($"/lists/{listId}/items{newItem.Id}", newItem);
        }

        public static async Task<IResult> GetListItem(ListsRepository repository, Guid listId, Guid itemId)
        {
            if (await repository.GetListAsync(listId) == null)
            {
                return TypedResults.NotFound();
            }

            var item = await repository.GetListItemAsync(listId, itemId);

            return item == null ? TypedResults.NotFound() : TypedResults.Ok(item);
        }

        public static async Task<IResult> UpdateListItem(ListsRepository repository, Guid listId, Guid itemId, CreateUpdateTodoItem item)
        {
            var existingItem = await repository.GetListItemAsync(listId, itemId);
            if (existingItem == null)
            {
                return TypedResults.NotFound();
            }

            existingItem.Name = item.name;
            existingItem.Description = item.description;
            existingItem.CompletedDate = item.completedDate;
            existingItem.DueDate = item.dueDate;
            existingItem.State = item.state;
            existingItem.UpdatedDate = DateTimeOffset.UtcNow;

            await repository.SaveChangesAsync();

            return TypedResults.Ok(existingItem);
        }

        public static async Task<IResult> DeleteListItem(ListsRepository repository, Guid listId, Guid itemId)
        {
            if (await repository.GetListItemAsync(listId, itemId) == null)
            {
                return TypedResults.NotFound();
            }

            await repository.DeleteListItemAsync(listId, itemId);

            return TypedResults.NoContent();
        }

        public static async Task<IResult> GetListItemsByState(ListsRepository repository, Guid listId, string state, int? skip = null, int? batchSize = null)
        {
            if (await repository.GetListAsync(listId) == null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(await repository.GetListItemsByStateAsync(listId, state, skip, batchSize));
        }

        public static IResult GetAppVersion(IConfiguration configuration)
        {
            var appVersionRaw = configuration["MY_APP_VERSION"] ?? "v0.0.1-rc";
            var appVersion = Regex.Match(appVersionRaw, @"v[^:]+$").Value;

            Console.WriteLine($"Framework Version: {appVersion}");

            return TypedResults.Ok(new AppVersion(appVersion));
        }
    }

    public record CreateUpdateTodoList(string name, string? description = null);
    public record CreateUpdateTodoItem(string name, string state, DateTimeOffset? dueDate, DateTimeOffset? completedDate, string? description = null);
    public record AppVersion(string Value);
}
