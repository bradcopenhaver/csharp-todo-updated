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
      Category.DeleteAll();
      //Arrange, Act
      int result = Category.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_Equal_ReturnsTrueForSame()
    {
      Category.DeleteAll();
      Category firstCategory = new Category("Household chores");
      Category secondCategory = new Category("Household chores");
      Assert.Equal(firstCategory, secondCategory);
    }

    [Fact]
    public void Test_Save_SavesCategoryToDatabase()
    {
      Category.DeleteAll();
      Category testCategory = new Category("Household chores");

      testCategory.Save();
      List<Category> result = Category.GetAll();
      List<Category> testList = new List<Category>{testCategory};
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Save_AssignsIdToObject()
    {
      Category.DeleteAll();
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
      Category.DeleteAll();
      Category testCategory = new Category("Household chores");
      testCategory.Save();
      Category foundCategory = Category.Find(testCategory.GetId());
      Assert.Equal(testCategory, foundCategory);
    }

    [Fact]
    public void Test_Delete_DeletesCategoryFromDatabase()
    {
      Category.DeleteAll();
      //Arrange
      string name1 = "Home stuff";
      Category testCategory1 = new Category(name1);
      testCategory1.Save();

      string name2 = "Work stuff";
      Category testCategory2 = new Category(name2);
      testCategory2.Save();

      //Act
      testCategory1.Delete();
      List<Category> resultCategories = Category.GetAll();
      List<Category> testCategoryList = new List<Category> {testCategory2};

      //Assert
      Assert.Equal(testCategoryList, resultCategories);
    }

    [Fact]
    public void Test_GetTasks_returnsAllCategoryTasks()
    {
      Category.DeleteAll();
      //Arrange
      Category testCategory = new Category("Household chores");
      testCategory.Save();

      Task testTask1 = new Task("Mow the lawn");
      testTask1.Save();
      Task testTask2 = new Task("Buy plane ticket");
      testTask2.Save();

      //Act
      testCategory.AddTask(testTask1);
      List<Task> savedTasks = testCategory.GetTasks();
      List<Task> testList = new List<Task> {testTask1};

      //Assert
      Assert.Equal(testList, savedTasks);
    }

    [Fact]
    public void Test_AddTask_AddsTaskToCategory()
    {
      Category.DeleteAll();
      //Arrange
      Category testCategory = new Category("Household chores");
      testCategory.Save();

      Task testTask = new Task("Mow the lawn");
      testTask.Save();

      Task testTask2 = new Task("Water the garden");
      testTask2.Save();

      //Act
      testCategory.AddTask(testTask);
      testCategory.AddTask(testTask2);

      List<Task> result = testCategory.GetTasks();
      List<Task> testList = new List<Task>{testTask, testTask2};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Delete_DeletesCategoryAssociationsFromDatabase()
    {
      //Arrange
      Task testTask = new Task("Mow the lawn");
      testTask.Save();

      Category testCategory = new Category("Home stuff");
      testCategory.Save();

      //Act
      testCategory.AddTask(testTask);
      testCategory.Delete();

      List<Category> resultTaskCategories = testTask.GetCategories();
      List<Category> testTaskCategories = new List<Category> {};

      //Assert
      Assert.Equal(testTaskCategories, resultTaskCategories);
    }

    public void Dispose()
    {
      Category.DeleteAll();
    }
  }
}
