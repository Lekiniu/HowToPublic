using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace HowToWebApplication.Models
{
    public class ArticlesCustomClass
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        //[Display(Name = "Title")]
        //[Required(ErrorMessageResourceType = typeof(Resources.Global),
        //          ErrorMessageResourceName = "NameRequired")]
        [Display(Name = "Title", ResourceType = typeof(Resources.Global))]
        public string Title { get; set; }


        [Required]
        [AllowHtml]
        //[Display(Name = "Content")]
        //[Required(ErrorMessageResourceType = typeof(Resources.Global),
        //          ErrorMessageResourceName = "CountryRequired")]
        [Display(Name = "Content", ResourceType = typeof(Resources.Global))]
        public string Content { get; set; }


        //[Display(Name = "Date")]
        [Display(Name = "Date", ResourceType = typeof(Resources.Global))]
        public DateTime Date { get; set; }

        //[Display(Name = "IsBlocked")]
        [Display(Name = "IsBlocked", ResourceType = typeof(Resources.Global))]
        public bool IsBlocked { get; set; }

        [Display(Name = "Authors", ResourceType = typeof(Resources.Global))]
        //[Display(Name = "User")]
        public users User { get; set; }

        //[Display(Name = "Ratings")]
        [Display(Name = "Ratings", ResourceType = typeof(Resources.Global))]
        public IEnumerable<ratings> Ratings { get; set; }

        public int UsersId { get; set; }

        [Display(Name = "Categories", ResourceType = typeof(Resources.Global))]
        public List<categories> Categories { get; set; }

        [Display(Name = "Requests", ResourceType = typeof(Resources.Global))]
        public List<requests> Requests { get; set; }

        [Display(Name = "Images", ResourceType = typeof(Resources.Global))]
        public HttpPostedFileBase[] Images { get; set; }

        public IEnumerable<images> ImagesList { get; set; }

        [Display(Name = "MainImagesList", ResourceType = typeof(Resources.Global))]
        public IEnumerable<images> MainImagesList { get; set; }

        [Display(Name = "MainImage", ResourceType = typeof(Resources.Global))]
        public images MainImage { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int[] CategoriesList { get; set; }


        [Display(Name = "Article title")]
        public List<int> RequestsList { get; set; }

    }

    //model for product List
    public class ArticlesListModel
    {
        public IEnumerable<ArticlesCustomClass> ArticlesList { get; set; }
    }
}