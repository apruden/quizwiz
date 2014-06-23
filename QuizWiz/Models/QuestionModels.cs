namespace QuizWiz.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// 
    /// </summary>
    public class QuestionEditModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int SectionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Text { get; set; }

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