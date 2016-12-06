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
      int result = Task.GetAll().Count;
      Assert.Equal(0,result);
    }

    [Fact]
    public void Test_Equal_ReturnsTrueIfDescriptionsAreTheSame()
    {
      //Arrange, Act
      Task firstTask = new Task("Mow the lawn", 1, new DateTime(2016, 11, 30));
      Task secondTask = new Task("Mow the lawn", 1, new DateTime(2016, 11, 30));

      //Assert
      Assert.Equal(firstTask, secondTask);
    }

    [Fact]
    public void Test_Save_SavesToDatabase()
    {
      //Arrange
      Task testTask = new Task("Mow the lawn", 1, new DateTime(2016, 11, 30));

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
      //Arrange
      Task testTask = new Task("Mow the lawn", 1, new DateTime(2016, 11, 30));
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
      //Arrange
      Task testTask = new Task("Mow the lawn", 1, new DateTime(2016, 11, 30));
      testTask.Save();

      //Act
      Task foundTask = Task.Find(testTask.GetId());

      //Assert
      Assert.Equal(testTask, foundTask);
    }

    [Fact]
    public void Test_Find_OrderByDueDate()
    {
      //Arrange
      Task firstTask = new Task("Mow the lawn", 1, new DateTime(2016, 11, 30));
      Task secondTask = new Task("Do the dishes", 1, new DateTime(2016, 12, 1));

      //Act
      secondTask.Save();
      firstTask.Save();
      List<Task> result = Task.GetAll();
      List<Task> testList = new List<Task>{firstTask, secondTask};

      //Assert
      Assert.Equal(testList, result);
    }

    public void Dispose()
    {
      Task.DeleteAll();
    }
  }
}
