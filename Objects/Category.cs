using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.Data;

namespace ToDoList.Objects
{
  public class Category
  {
    private int _id;
    private string _name;

    public Category(string name, int id = 0)
    {
      _name = name;
      _id = id;
    }

    public override bool Equals(Object otherCategory)
    {
      if (!(otherCategory is Category))
      {
        return false;
      }
      else
      {
        Category newCategory = (Category) otherCategory;
        bool idEquality = (this._id == newCategory.GetId());
        bool nameEquality = (this._name == newCategory.GetName());
        return (idEquality && nameEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetName().GetHashCode();
    }

    public static List<Category> GetAll()
    {
      List<Category> allCategories = new List<Category>{};
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("SELECT * FROM categories;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();
      while (rdr.Read())
      {
        int categoryId = rdr.GetInt32(0);
        string categoryName = rdr.GetString(1);
        Category newCategory = new Category(categoryName, categoryId);
        allCategories.Add(newCategory);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allCategories;
    }

    public string GetName()
    {
      return _name;
    }

    public int GetId()
    {
      return _id;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM categories WHERE id = @CategoryId; DELETE FROM categories_tasks WHERE category_id = @CategoryId;", conn);

      cmd.Parameters.AddWithValue("@CategoryId", _id.ToString());
      cmd.ExecuteNonQuery();

      if (conn != null) conn.Close();
    }

    public static Category Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("SELECT * FROM categories WHERE id = @CategoryId;", conn);

      SqlParameter idParam = new SqlParameter();
      idParam.ParameterName = "@CategoryId";
      idParam.Value = id.ToString();
      cmd.Parameters.Add(idParam);
      SqlDataReader rdr = cmd.ExecuteReader();

      string CategoryName = null;
      while (rdr.Read())
      {
        CategoryName = rdr.GetString(1);
      }
      Category foundCategory = new Category(CategoryName, id);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundCategory;
    }

    public void Save()
		{
			SqlConnection conn = DB.Connection();
			conn.Open();
			SqlCommand cmd = new SqlCommand("INSERT INTO categories (description) OUTPUT INSERTED.id VALUES (@CategoryName);", conn);

			cmd.Parameters.AddWithValue("@CategoryName", _name);
			SqlDataReader rdr = cmd.ExecuteReader();

			while(rdr.Read())
			{
				this._id = rdr.GetInt32(0);
			}
			if (rdr != null)
			{
				rdr.Close();
			}
			if (conn != null)
			{
				conn.Close();
			}
		}

    public void AddTask(Task newTask)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO categories_tasks (category_id, task_id) VALUES (@CategoryId, @TaskId);", conn);

      cmd.Parameters.AddWithValue("@CategoryId", _id);
      cmd.Parameters.AddWithValue("@TaskId", newTask.GetId());
      cmd.ExecuteNonQuery();

      if (conn != null) {conn.Close();}
    }

    public List<Task> GetTasks()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT task_id FROM categories_tasks WHERE category_id = @CategoryId;", conn);
      cmd.Parameters.AddWithValue("@CategoryId", _id);
      SqlDataReader rdr = cmd.ExecuteReader();

      List<int> taskIds = new List<int> {};
      while(rdr.Read())
      {
        int taskId = rdr.GetInt32(0);
        taskIds.Add(taskId);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      List<Task> tasks = new List<Task> {};
      foreach (int taskId in taskIds)
      {
        SqlCommand taskQuery = new SqlCommand("SELECT description, completed, due_date FROM tasks WHERE id = @TaskId;", conn);
        taskQuery.Parameters.AddWithValue("@TaskId", taskId);

        SqlDataReader queryReader = taskQuery.ExecuteReader();
        while(queryReader.Read())
        {
          string taskDescription = queryReader.GetString(0);
          bool taskCompleted = queryReader.GetBoolean(1);
          DateTime taskDueDate = queryReader.GetDateTime(2);
          Task foundTask = new Task(taskDescription, taskCompleted, taskDueDate, taskId);
          tasks.Add(foundTask);
        }
        if (queryReader != null) {queryReader.Close();}
      }
      if (conn != null) {conn.Close();}
      return tasks;
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM categories", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }
  }
}
