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
    private bool _completed;
    private DateTime _dueDate;

    public Task(string Description, DateTime dueDate, bool completed = false, int Id = 0)
    {
      _id = Id;
      _description = Description;
      _completed = completed;
      _dueDate = dueDate;
    }

    public Task(string Description, bool completed = false, DateTime? dueDate = null, int Id = 0)
    {
      _id = Id;
      _description = Description;
      _completed = completed;
      if (dueDate == null)
      {
        _dueDate = (DateTime) DateTime.Today;
      }
      else
      {
        _dueDate = (DateTime) dueDate;
      }
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
        bool completedEquality = (this.GetCompleted() == newTask.GetCompleted());
        bool dueDateEquality = (_dueDate == newTask.GetDueDate());
				return (idEquality && descriptionEquality && completedEquality && dueDateEquality);
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
    public bool GetCompleted()
    {
      return _completed;
    }
    public void ChangeCompleted()
    {
      _completed = _completed ? false : true;
    }
    public string GetDescription()
    {
      return _description;
    }
    public void SetDescription(string newDescription)
    {
      _description = newDescription;
    }
    public DateTime GetDueDate()
    {
      return _dueDate;
    }

    public void ToggleCompleted()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT completed FROM tasks WHERE id = @id;", conn);
      cmd.Parameters.AddWithValue("@id", _id);
      SqlDataReader rdr = cmd.ExecuteReader();

      bool completed = false;
      while(rdr.Read())
      {
        completed = rdr.GetBoolean(0);
      }
      if (rdr != null) rdr.Close();
      if (completed) {
        SqlCommand setCompletedCmd = new SqlCommand("UPDATE tasks SET completed = FALSE WHERE id = @id;", conn);
        setCompletedCmd.Parameters.AddWithValue("@id", _id);
        setCompletedCmd.ExecuteNonQuery();
        _completed = false;
      }
      else
      {
        SqlCommand setCompletedCmd = new SqlCommand("UPDATE tasks SET completed = 'TRUE' WHERE id = @id;", conn);
        setCompletedCmd.Parameters.AddWithValue("@id", _id);
        setCompletedCmd.ExecuteNonQuery();
        _completed = true;
      }
      if (conn != null) conn.Close();
    }

    public static List<Task> GetAll()
    {
      List<Task> allTasks = new List<Task>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM tasks ORDER BY due_date;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int taskId = rdr.GetInt32(0);
        string taskDescription = rdr.GetString(1);
        bool taskCompleted = rdr.GetBoolean(2);
        DateTime taskDueDate = rdr.GetDateTime(3);
        Task newTask = new Task(taskDescription, taskCompleted, taskDueDate, taskId);
        allTasks.Add(newTask);
      }

      if (rdr != null) rdr.Close();
      if (conn != null) conn.Close();

      return allTasks;
    }
		public void Save()
		{
			SqlConnection conn = DB.Connection();
			conn.Open();
			SqlCommand cmd = new SqlCommand("INSERT INTO tasks (description, completed, due_date) OUTPUT INSERTED.id VALUES (@TaskDescription, @Completed, @DueDate);", conn);

			cmd.Parameters.AddWithValue("@TaskDescription", _description);
      cmd.Parameters.AddWithValue("@Completed", _completed);
      cmd.Parameters.AddWithValue("@DueDate", _dueDate);
			SqlDataReader rdr = cmd.ExecuteReader();

			while(rdr.Read()) this._id = rdr.GetInt32(0);

			if (rdr != null) {rdr.Close();}
			if (conn != null) {conn.Close();}
		}

		public static Task Find(int id)
		{
			SqlConnection conn = DB.Connection();
			conn.Open();

			SqlCommand cmd = new SqlCommand("SELECT description, completed, due_date FROM tasks WHERE id = @TaskId;", conn);
			SqlParameter taskIdParameter = new SqlParameter();
			taskIdParameter.ParameterName = "@TaskId";
			taskIdParameter.Value = id.ToString();
			cmd.Parameters.Add(taskIdParameter);
			SqlDataReader rdr = cmd.ExecuteReader();

			int foundTaskId = 0;
			string foundTaskDescription = null;
      bool foundTaskCompleted = false;
      DateTime foundTaskDueDate = DateTime.Today;
			while(rdr.Read())
			{
				foundTaskDescription = rdr.GetString(0);
        foundTaskCompleted = rdr.GetBoolean(1);
        foundTaskDueDate = rdr.GetDateTime(2);
			}
			Task foundTask = new Task(foundTaskDescription, foundTaskCompleted, foundTaskDueDate, id);

			if (rdr != null) rdr.Close();
			if (conn != null) conn.Close();

			return foundTask;
		}

    public static void DeleteTask(int id)
		{
			SqlConnection conn = DB.Connection();
			conn.Open();

			SqlCommand cmd = new SqlCommand("DELETE FROM tasks WHERE id = @TaskId; DELETE FROM categories_tasks WHERE task_id = @TaskId;", conn);
			cmd.Parameters.AddWithValue("@TaskId", id.ToString());
			SqlDataReader rdr = cmd.ExecuteReader();
			if (conn != null) conn.Close();
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
