namespace QuizWiz.Filters
{
    using QuizWiz.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Threading;
    using System.Web.Mvc;

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeExamAttribute : ActionFilterAttribute
    {
        private static ExamInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        /// <summary>
        /// 
        /// </summary>
        private class ExamInitializer
        {
            /// <summary>
            /// 
            /// </summary>
            public ExamInitializer()
            {
                Database.SetInitializer<ExamContext>(null);

                try
                {
                    using (var context = new ExamContext())
                    {
                        if (!context.Database.Exists())
                        {
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The quizwiz database could not be initialized.", ex);
                }
            }
        }
    }
}
