using Lab1.Domain.Models;
using Lab1.Domain.Entities;
using System.Collections.Generic;

namespace Lab1.Tests.Models
{
    // Test implementation of ListModel for testing
    public class TestListModel<T> : ListModel<T>
    {
        public TestListModel()
        {
            Items = new List<T>();
            TotalCount = 0;
            CurrentPage = 1;
            TotalPages = 0;
            PageSize = 3;
        }
    }

    // Test implementation of ResponseData for testing
    public class TestResponseData<T> : ResponseData<T>
    {
        public static TestResponseData<T> CreateSuccess(T data)
        {
            return new TestResponseData<T>
            {
                Successfull = true,
                Data = data,
                ErrorMessage = null
            };
        }

        public static TestResponseData<T> CreateError(string error)
        {
            return new TestResponseData<T>
            {
                Successfull = false,
                Data = default(T),
                ErrorMessage = error
            };
        }
    }
}