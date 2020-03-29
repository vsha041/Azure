using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ServerlessFuncs
{
	public static class TodoApi
	{
		public static List<Todo> Items = new List<Todo>();

		[FunctionName("CreateTodo")]
		public static async Task<IActionResult> CreateTodo([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req, ILogger log)
		{
			log.LogInformation("Creating a new todo list item");
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);

			var todo = new Todo
			{
				TaskDescription = input.TaskDescription
			};
			Items.Add(todo);
			return new OkObjectResult(todo);
		}

		[FunctionName("GetTodos")]
		public static IActionResult GetTodos([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req, ILogger log)
		{
			log.LogInformation("Getting todo list items");
			return new OkObjectResult(Items);
		}

		[FunctionName("GetTodoById")]
		public static IActionResult GetTodoById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")]HttpRequest req, ILogger log, string id)
		{
			var todo = Items.FirstOrDefault(t => t.Id == id);
			if (todo == null)
			{
				return new NotFoundResult();
			}
			return new OkObjectResult(todo);
		}

		[FunctionName("UpdateTodo")]
		public static async Task<IActionResult> UpdateTodo([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")]HttpRequest req, ILogger log, string id)
		{
			var todo = Items.FirstOrDefault(t => t.Id == id);
			if (todo == null)
			{
				return new NotFoundResult();
			}

			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			var updated = JsonConvert.DeserializeObject<TodoUpdateModel>(requestBody);

			todo.IsCompleted = updated.IsCompleted;
			if (!string.IsNullOrEmpty(updated.TaskDescription))
			{
				todo.TaskDescription = updated.TaskDescription;
			}

			return new OkObjectResult(todo);
		}

		[FunctionName("DeleteTodo")]
		public static IActionResult DeleteTodo(
			[HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")]HttpRequest req, ILogger log, string id)
		{
			var todo = Items.FirstOrDefault(t => t.Id == id);
			if (todo == null)
			{
				return new NotFoundResult();
			}
			Items.Remove(todo);
			return new OkResult();
		}
	}
}