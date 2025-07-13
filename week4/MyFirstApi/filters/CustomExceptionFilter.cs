 using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting; 
using System;
using System.IO;

namespace MyFirstAPI.filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilter> _logger;
        private readonly string _logFilePath;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            // Define a path for the log file.
            string logDirectory = Path.Combine(env.ContentRootPath, "ErrorLogs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            _logFilePath = Path.Combine(logDirectory, "application_errors.log");
        }

        public void OnException(ExceptionContext context)
        {
            // Log the exception details to a file
            LogExceptionToFile(context.Exception);

            // Log the exception using the injected logger
            _logger.LogError(context.Exception, "An unhandled exception occurred.");

            // Set the Result property to a standardized response
            context.Result = new ObjectResult("An unexpected error occurred. Please try again later.")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            // Mark the exception as handled
            context.ExceptionHandled = true;
        }

        private void LogExceptionToFile(Exception ex)
        {
            try
            {
                string logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} - ERROR: {ex.Message}\n";
                logMessage += $"Stack Trace: {ex.StackTrace}\n";
                if (ex.InnerException != null)
                {
                    logMessage += $"Inner Exception: {ex.InnerException.Message}\n";
                    logMessage += $"Inner Stack Trace: {ex.InnerException.StackTrace}\n";
                }
                logMessage += "------------------------------------------------------------\n";

                File.AppendAllText(_logFilePath, logMessage);
            }
            catch (Exception logEx)
            {
                _logger.LogError(logEx, "Failed to write exception to log file.");
            }
        }
    }
}