using Microsoft.AspNetCore.Mvc;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Services
{
    public class ProblemDetailsException : Exception
    {
        public ProblemDetails ProblemDetails { get; }

        public ProblemDetailsException(ProblemDetails error)
        {
            ProblemDetails = error;
        }
    }
}
