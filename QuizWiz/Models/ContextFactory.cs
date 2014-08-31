namespace QuizWiz.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// 
    /// </summary>
    public interface IContextFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ExamContext GetExamContext();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ConfigContext GetConfigContext();
    }

    /// <summary>
    /// 
    /// </summary>
    public class ContextFactory : IContextFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ExamContext GetExamContext()
        {
            return new ExamContext();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConfigContext GetConfigContext()
        {
            return new ConfigContext();
        }
    }
}