using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.Globalization;

namespace ToDoList.Objects
{
  public class Task
  {
    private int _id;
    private string _description;

    public Task(string Description, int Id = 0)
    {
      _id = Id;
      _description = Description;
    }

		public override bool Equals(Object otherTask)
		{
			if (!(otherTask is Task))
			{
				return false;
			}
			else
			{
				Task newTask = (Task) otherTask;
				bool idEquality = (this.GetId() == newTask.GetId());
				bool descriptionEquality = (this.GetDescription() == newTask.GetDescription());
				return (idEquality && descriptionEquality);
			}
		}
    public override int GetHashCode()
    {
      return this.GetDescription().GetHashCode();
    }

    public void AddCategory(Category newCategory)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO categories_tasks (category_id, task_id) VALUES (@CategoryId, @TaskId);", conn);

      cmd.Parameters.AddWithValue("@CategoryId", newCategory.GetId());
      cmd.Parameters.AddWithValue("@TaskId", _id);
      cmd.ExecuteNonQuery();

      if (conn != null) {conn.Close();}
    }

    public List<Category> GetCategories()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("SELECT category_id FROM categories_tasks WHERE task_id = @TaskId;", conn);

      cmd.Parameters.AddWithValue("@TaskId", _id);
      SqlDataReader rdr = cmd.ExecuteReader();

      List<int> categoryIds = new List<int> {};

      while (rdr.Read())
      {
        int categoryId = rdr.GetInt32(0);
        categoryIds.Add(categoryId);
      }

      if (rdr != null) {rdr.Close();}

      List<Category> categories = new List<Category> {};

      foreach (int categoryId in categoryIds)
      {
        SqlCommand categoryQuery = new SqlCommand("SELECT * FROM categories WHERE id = @CategoryId;", conn);

        categoryQuery.Parameters.AddWithValue("@CategoryId", categoryId);
        SqlDataReader queryReader = categoryQuery.ExecuteReader();
        while (queryReader.Read())
        {
          int thisCategoryId = queryReader.GetInt32(0);
          string categoryName = queryReader.GetString(1);
          Category foundCategory = new Category(categoryName, thisCategoryId);
          categories.Add(foundCategory);
        }
        if (queryReader != null) {queryReader.Close();}
      }
      if (conn != null) {conn.Close();}
      return categories;
    }

    public int GetId()
    {
      return _id;
    }
    public string GetDescription()
    {
      return _description;
    }
    public void SetDescription(string newDescription)
    {
      _description = newDescription;
    }
    public static List<Task> GetAll()
    {
      List<Task> allTasks = new List<Task>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM tasks;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int taskId = rdr.GetInt32(0);
        string taskDescription = rdr.GetString(1);
        Task newTask = new Task(taskDescription, taskId);
        allTasks.Add(newTask);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allTasks;
    }
		public void Save()
		{
			SqlConnection conn = DB.Connection();
			conn.Open();
			SqlCommand cmd = new SqlCommand("INSERT INTO tasks (description) OUTPUT INSERTED.id VALUES (@TaskDescription);", conn);

			cmd.Parameters.AddWithValue("@TaskDescription", _description);
			SqlDataReader rdr = cmd.ExecuteReader();

			while(rdr.Read())
			{
				this._id = rdr.GetInt32(0);
			}
			if (rdr != null) {rdr.Close();}
			if (conn != null) {conn.Close();}
		}

		public static Task Find(int id)
		{
			SqlConnection conn = DB.Connection();
			conn.Open();

			SqlCommand cmd = new SqlCommand("SELECT * FROM tasks WHERE id = @TaskId;", conn);
			SqlParameter taskIdParameter = new SqlParameter();
			taskIdParameter.ParameterName = "@TaskId";
			taskIdParameter.Value = id.ToString();
			cmd.Parameters.Add(taskIdParameter);
			SqlDataReader rdr = cmd.ExecuteReader();

			int foundTaskId = 0;
			string foundTaskDescription = null;

			while(rdr.Read())
			{
				foundTaskId = rdr.GetInt32(0);
				foundTaskDescription = rdr.GetString(1);
			}
			Task foundTask = new Task(foundTaskDescription, foundTaskId);

			if (rdr != null)
			{
				rdr.Close();
			}
			if (conn != null)
			{
				conn.Close();
			}

			return foundTask;
		}

    public static void DeleteTask(int id)
		{
			SqlConnection conn = DB.Connection();
			conn.Open();

			SqlCommand cmd = new SqlCommand("DELETE FROM tasks WHERE id = @TaskId;", conn);
			SqlParameter taskIdParameter = new SqlParameter();
			taskIdParameter.ParameterName = "@TaskId";
			taskIdParameter.Value = id.ToString();
			cmd.Parameters.Add(taskIdParameter);
			SqlDataReader rdr = cmd.ExecuteReader();
			conn.Close();
		}

		public static void DeleteAll()
		{
		  SqlConnection conn = DB.Connection();
		  conn.Open();
		  SqlCommand cmd = new SqlCommand("DELETE FROM tasks;", conn);
		  cmd.ExecuteNonQuery();
		  conn.Close();
		}
  }
}
