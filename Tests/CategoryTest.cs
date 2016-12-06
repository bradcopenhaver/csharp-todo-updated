using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace ToDoList.Objects
{
  public class CategoryTest : IDisposable
  {
    public CategoryTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=todo_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_CategoriesEmptyAtFirst()
    {
      //Arrange, Act
      int result = Category.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_Equal_ReturnsTrueForSame()
    {
      Category firstCategory = new Category("Household chores");
      Category secondCategory = new Category("Household chores");
      Assert.Equal(firstCategory, secondCategory);
    }

    [Fact]
    public void Test_Save_SavesCategoryToDatabase()
    {
      Category testCategory = new Category("Household chores");

      testCategory.Save();
      List<Category> result = Category.GetAll();
      List<Category> testList = new List<Category>{testCategory};
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Save_AssignsIdToObject()
    {
      Category testCategory = new Category("Household chores");
      testCategory.Save();
      Category savedCategory = Category.GetAll()[0];
      int result = savedCategory.GetId();
      int testId = testCategory.GetId();
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_Find_FindsCategoryInDatabase()
    {
      Category testCategory = new Category("Household chores");
      testCategory.Save();
      Category foundCategory = Category.Find(testCategory.GetId());
      Assert.Equal(testCategory, foundCategory);
    }

    [Fact]
    public void Test_GetTasks_RetrievesAllTasksWithCategory()
    {
      Category testCategory = new Category("Household chores");
      testCategory.Save();

      Task firstTask = new Task("Mow the lawn", testCategory.GetId(), new DateTime(2016, 11, 30));
      firstTask.Save();
      Task secondTask = new Task("Do the dishes", testCategory.GetId(), new DateTime(2016, 11, 30));
      secondTask.Save();

      List<Task> testTaskList = new List<Task> {firstTask, secondTask};
      List<Task> resultTaskList = testCategory.GetTasks();

      Assert.Equal(testTaskList, resultTaskList);
    }

    public void Dispose()
    {
      Category.DeleteAll();
    }
  }
}
