namespace QuizWiz.Controllers
{
    using QuizWiz.Models;
    using System.Web.Mvc;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class QuestionController : Controller
    {
        private IContextFactory factory;

        /// <summary>
        /// 
        /// </summary>
        public QuestionController()
        {
            this.factory = new ContextFactory();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        public QuestionController(IContextFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(QuestionEditModel model)
        {
            using (var db = this.factory.GetExamContext())
            {
                var section = (from s in db.Sections.Include("Questions")
                               where s.SectionId == model.SectionId
                               select s).SingleOrDefault();

                var answers = new List<Answer>();

                if (model.Answers != null)
                {
                    foreach (var a in model.Answers)
                    {
                        answers.Add(new Answer { Text = a.Text });
                    }
                }

                section.Questions.Add(new Question { Text = model.Text, OrderIndex = 1, Answers = answers });
                db.SaveChanges();
            }

            return new EmptyResult();
        }
    }
}