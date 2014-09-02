namespace QuizWiz.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using QuizWiz.Controllers;
    using QuizWiz.Models;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class TestExamsController
    {
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestGetQuestion()
        {
            var question = new Question { OrderIndex = 1, QuestionId = 1, Text = "test", Answers = new List<Answer>() };
            var exam = new Exam { ExamId = 1, Name = "test", Questions = new List<Question> { question } };
            var submission = new Submission { Exam = exam, SubmissionId = 1};
            var data = new List<Exam>{ exam }.AsQueryable();
            var dataSubmissions = new List<Submission> { submission }.AsQueryable();

            var mockSet = new Mock<DbSet<Exam>>();
            mockSet.SetupQueryable<Exam>(data);
            mockSet.Setup(m => m.Include("Questions")).Callback<string>(
                i =>
                {
                    //nothing to be done.
                }).Returns(mockSet.Object);

            var mockSetSubmission = new Mock<DbSet<Submission>>();
            mockSetSubmission.SetupQueryable<Submission>(dataSubmissions);

            var mockContext = new Mock<ExamContext>();
            mockContext.Setup(c => c.Exams).Returns(mockSet.Object);
            mockContext.Setup(c => c.Submissions).Returns(mockSetSubmission.Object);

            var mockContextFactory = new Mock<IContextFactory>();
            mockContextFactory.Setup<ExamContext>(f => f.GetExamContext()).Returns(mockContext.Object);

            var controller = new ExamsController(mockContextFactory.Object);
            var result = (JsonResult)controller.GetQuestion(1, 1);
            var actual = (ExamQuestionModel)result.Data;

            Assert.AreEqual(question.QuestionId, actual.Question.QuestionId);
        }

        [TestMethod]
        public void TestDeleteExam()
        {
            var question = new Question { OrderIndex = 1, QuestionId = 1, Text = "test", Answers = new List<Answer>() };
            var exam = new Exam { ExamId = 1, Name = "test", Questions = new List<Question> { question } };
            var submission = new Submission { Exam = exam, SubmissionId = 1 };
            var data = new List<Exam> { exam }.AsQueryable();
            var dataSubmissions = new List<Submission> { submission }.AsQueryable();

            var mockSet = new Mock<DbSet<Exam>>();
            mockSet.SetupQueryable<Exam>(data);
            mockSet.Setup(m => m.Include("Questions")).Callback<string>(
                i =>
                {
                    //nothing to be done.
                }).Returns(mockSet.Object);

            var mockSetSubmission = new Mock<DbSet<Submission>>();
            mockSetSubmission.SetupQueryable<Submission>(dataSubmissions);

            var mockContext = new Mock<ExamContext>();
            mockContext.Setup(c => c.Exams).Returns(mockSet.Object);
            mockContext.Setup(c => c.Submissions).Returns(mockSetSubmission.Object);

            var mockContextFactory = new Mock<IContextFactory>();
            mockContextFactory.Setup<ExamContext>(f => f.GetExamContext()).Returns(mockContext.Object);

            var controller = new ExamsController(mockContextFactory.Object);
            controller.Delete(1);
        }
    }
}