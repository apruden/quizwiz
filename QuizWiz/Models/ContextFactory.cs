using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizWiz.Models
{
    public interface IContextFactory
    {
        ExamContext GetExamContext();
    }

    public class ContextFactory : IContextFactory
    {
        public ExamContext GetExamContext()
        {
            return new ExamContext();
        }
    }
}