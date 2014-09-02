namespace QuizWiz.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 
    /// </summary>
    public class QuestionEditModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int ExamId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOpenEnded { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<AnswerEditModel> Answers { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AnswerEditModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OrderIndex { get; set; }
    }
}