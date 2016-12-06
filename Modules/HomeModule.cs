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
				return View["task.cshtml", allTasks];
			};
      Get["/tasks/new"] = _ => {
      List<Category> AllCategories = Category.GetAll();
      return View["tasks_form.cshtml", AllCategories];
      };
      Post["/tasks/new"] = _ => {
				DateTime date = new DateTime(Request.Form["year"], Request.Form["month"], Request.Form["day"]);
        Task newTask = new Task(Request.Form["task-description"], Request.Form["category-id"], date);
        newTask.Save();
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
        var SelectedCategory = Category.Find(parameters.id);
        var CategoryTasks = SelectedCategory.GetTasks();
        model.Add("category", SelectedCategory);
        model.Add("tasks", CategoryTasks);
        return View["category.cshtml", model];
      };
		}
	}
}
