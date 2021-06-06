using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Extensions
{
    public class ResponseExtension
    {
        public ResponseExtension()
        {

        }
        public ResponseExtension(string message = null, object data = null, object errors = null, Paging paging = null)
        {
            Message = message;
            Data = data;
            Errors = errors;
            if (paging != null)
            {
                Paging = paging;
            }
        }
        public string Message { get; set; }
        public object Data { get; set; }
        public object Errors { get; set; }
        public Paging Paging { get; set; }
    }

    public class Error
    {
        public Error(string message, string stackTrace)
        {
            Message = message;
            StackTrace = stackTrace;
        }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }

    public class Paging
    {        
        public Paging(int pageNumber, int pageSize, int totalPages, int totalRecords)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalPages;
            TotalRecords = totalRecords;
        }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
    }
}
