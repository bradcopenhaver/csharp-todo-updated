using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace ToDoList.Objects
{
  public class ToDoTest : IDisposable
  {
    public ToDoTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=todo_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_DatabaseEmptyAtFirst()
    {
      Task.DeleteAll();
      int result = Task.GetAll().Count;
      Assert.Equal(0,result);
    }

    [Fact]
    public void Test_Equal_ReturnsTrueIfDescriptionsAreTheSame()
    {
      Task.DeleteAll();
      //Arrange, Act
      Task firstTask = new Task("Mow the lawn");
      Task secondTask = new Task("Mow the lawn");

      //Assert
      Assert.Equal(firstTask, secondTask);
    }

    [Fact]
    public void Test_Save_SavesToDatabase()
    {
      Task.DeleteAll();
      //Arrange
      Task testTask = new Task("Mow the lawn");

      //Act
      testTask.Save();
      List<Task> result = Task.GetAll();
      List<Task> testList = new List<Task>{testTask};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Save_AssignsIdToObject()
    {
      Task.DeleteAll();
      //Arrange
      Task testTask = new Task("Mow the lawn");
      //Act
      testTask.Save();
      Task savedTask = Task.GetAll()[0];

      int result = savedTask.GetId();
      int testId = testTask.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_Find_FindsTaskInDatabase()
    {
      Task.DeleteAll();
      //Arrange
      Task testTask = new Task("Mow the lawn");
      testTask.Save();

      //Act
      Task foundTask = Task.Find(testTask.GetId());

      //Assert
      Assert.Equal(testTask, foundTask);
    }

    [Fact]
    public void Test_AddCategory_AddsCategoryToTask()
    {
      Task.DeleteAll();
      //Arrange
      Task testTask = new Task("Mow the lawn");
      testTask.Save();

      Category testCategory = new Category("Home stuff");
      testCategory.Save();

      //Act
      testTask.AddCategory(testCategory);

      List<Category> result = testTask.GetCategories();
      List<Category> testList = new List<Category>{testCategory};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_GetCategories_ReturnsAllTaskCategories()
    {
      Task.DeleteAll();
      //Arrange
      Task testTask = new Task("Mow the lawn");
      testTask.Save();

      Category testCategory1 = new Category("Home stuff");
      testCategory1.Save();

      Category testCategory2 = new Category("Work stuff");
      testCategory2.Save();

      //Act
      testTask.AddCategory(testCategory1);
      List<Category> result = testTask.GetCategories();
      List<Category> testList = new List<Category> {testCategory1};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Delete_DeletesTaskAssociationsFromDatabase()
    {
      //Arrange
      Category testCategory = new Category("Home stuff");
      testCategory.Save();

      Task testTask = new Task("Mow the Lawn");
      testTask.Save();

      //Act
      testTask.AddCategory(testCategory);
      Task.DeleteTask(testTask.GetId());

      List<Task> resultCategoryTasks = testCategory.GetTasks();
      List<Task> testCategoryTasks = new List<Task>{};

      //Assert
      Assert.Equal(testCategoryTasks, resultCategoryTasks);
    }
    [Fact]
    public void ToggleCompleted_ChangeTaskCompletedStatus_true()
    {
      //Arrange
      Task newTask = new Task("water the dog");
      newTask.Save();

      //Act
      newTask.ToggleCompleted();
      //newTask.ChangeCompleted();
      Task foundTask = Task.Find(newTask.GetId());

      //Assert
      Assert.Equal(foundTask, newTask);
    }

    public void Dispose()
    {
      Task.DeleteAll();
    }
  }
}
