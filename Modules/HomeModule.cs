using Nancy;
using System.Collections.Generic;
using System;
using ToDoList.Objects;
using Nancy.ViewEngines.Razor;

namespace ToDoList
{
	public class HomeModule : NancyModule
	{
		public HomeModule()
		{
			Get["/"] = _ =>
			{
        List<Category> allCategories = Category.GetAll();
				return View["index.cshtml", allCategories];
			};
			Get["/categories"] = _ =>
			{
        List<Category> allCategories = Category.GetAll();
				return View["categories.cshtml", allCategories];
			};
      Get["categories/new"] = _ =>
      {
        return View["categories_form.cshtml"];
      };
      Post["categories/new"] = _ =>
      {
        Category newCategory = new Category(Request.Form["category-name"]);
        newCategory.Save();
        return View["success.cshtml"];
      };
			Get["/tasks"] = _ =>
			{
        List<Task> allTasks = Task.GetAll();
				return View["tasks.cshtml", allTasks];
			};
			Get["/tasks/delete/{id}"] = parameters =>
			{
				Task.DeleteTask(parameters.id);
				return View["cleared.cshtml"];
			};
      Get["/tasks/new"] = _ => {
	      List<Category> AllCategories = Category.GetAll();
	      return View["tasks_form.cshtml", AllCategories];
      };
      Post["/tasks/new"] = _ => {
        Task newTask = new Task(Request.Form["task-description"]);
        newTask.Save();
				newTask.AddCategory(Category.Find(Request.Form["category-id"]));
        return View["success.cshtml"];
      };
      Post["/tasks/delete"] = _ =>
      {
        Task.DeleteAll();
        return View["cleared.cshtml"];
      };
      Get["/categories/{id}"] = parameters =>
      {
        Dictionary<string, object> model = new Dictionary<string, object>();
        Category SelectedCategory = Category.Find(parameters.id);
        List<Task> CategoryTasks = SelectedCategory.GetTasks();
				List<Task> AllTasks = Task.GetAll();
        model.Add("category", SelectedCategory);
        model.Add("tasks", CategoryTasks);
				model.Add("allTasks", AllTasks);
        return View["category.cshtml", model];
      };
			Get["/tasks/{id}"] = parameters =>
      {
        Dictionary<string, object> model = new Dictionary<string, object>();
        Task SelectedTask = Task.Find(parameters.id);
        List<Category> TaskCategories = SelectedTask.GetCategories();
				List<Category> AllCategories = Category.GetAll();
        model.Add("task", SelectedTask);
        model.Add("categories", TaskCategories);
				model.Add("allCategories", AllCategories);
        return View["task.cshtml", model];
      };
			Post["task/add_category"] = _ => {
			  Category category = Category.Find(Request.Form["category-id"]);
			  Task task = Task.Find(Request.Form["task-id"]);
			  task.AddCategory(category);
			  return View["success.cshtml"];
			};
			Post["category/add_task"] = _ => {
			  Category category = Category.Find(Request.Form["category-id"]);
			  Task task = Task.Find(Request.Form["task-id"]);
			  category.AddTask(task);
			  return View["success.cshtml"];
			};
		}
	}
}
