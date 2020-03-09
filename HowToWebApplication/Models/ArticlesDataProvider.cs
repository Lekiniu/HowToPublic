﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using HowToWebApplication.Helpers;

namespace HowToWebApplication.Models
{
    public class ArticlesDataProvider
    {
        HowToDbEntities _db = new HowToDbEntities();

        


        public bool Exist(articles article)
        {
            return _db.articles.FirstOrDefault(e => e.title == article.title) == null ? false : true;
        }

        public bool ExistCustomArticle(ArticlesCustomClass article)
        {
            return _db.articles.FirstOrDefault(e => e.title == article.Title) == null ? false : true;
        }

        public articles GetArticlesById(int id)
        {
            return _db.articles.FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<ratings> GetRatingsByArticlesId(int id)
        {
            return _db.ratings.Where(e => e.articlesId == id);
        }

        //public IEnumerable<categories> GetCategoriesByArticlesId(int id)
        //{
        //    var article = GetArticlesById(id);
        //    foreach (var cat in article.articleCategories)
        //    {
        //        //     foreach (var category in _db.categories)
        //        //     {
        //        //     if (cat.categoriesId == category.Id)
        //        //      {

        //        //      }
        //        //}
        //        //   }
        //        return _db.categories.Where(e => e.Id == cat.categoriesId);
        //    }
        //}

        public IEnumerable<images> GetImagesByArticleId(int id)
        {
            return _db.images.Where(e => e.articlesId == id);
        }


        public images GetMainImageByArticleId(int id)
        {
            return _db.images.FirstOrDefault(e => e.articlesId == id && e.isMain == true);
        }

        public IEnumerable<images> GetMainImagesByArticleId(int id)
        {
            return _db.images.Where(e => e.articlesId == id && e.isMain==true);
        }


        //Create
        public void CreateArticles(ArticlesCustomClass article)
        {
            
            var newarticle = new articles();
           
            newarticle.title = article.Title;
            newarticle.content = article.Content;
            newarticle.date = DateTime.Now;
            newarticle.isBlocked = true;

            if (LoginHelper.IsLoggedIn())
            {
                newarticle.usersId = LoginHelper.CurrentUser().Id;
            }
            else
            {
                newarticle.usersId = 16; // რომ წავა დასასრულისკენ პროექტი, ეს იფ-ელსი წაიშლება და მარტო current user დარჩება 
            }


            if (!ExistCustomArticle(article))
            {
                _db.articles.Add(newarticle);

                //_db.SaveChanges();
                foreach (var categoryId in article.CategoriesList)
                {

                    _db.articleCategories.Add(
                       new articleCategories()
                       {
                           categoriesId = categoryId,
                           articlesId = newarticle.Id
                       });
                    _db.SaveChanges();
                }

                if (article.RequestsList != null)
                {
                    foreach (var requestID in article.RequestsList)
                    {

                        _db.requestsArticles.Add(
                           new requestsArticles()
                           {
                               requestsId = requestID,
                               articlesId = newarticle.Id
                           });
                        _db.SaveChanges();
                    }
                }


                var mapPath = HostingEnvironment.MapPath("~/images/");
                foreach (var file in article.Images)
                {
                    if (file != null)
                    {
                        file.SaveAs(mapPath + file.FileName);
                        _db.images.Add(new images()
                        {
                            name = file.FileName,
                            url = "~/images/" + file.FileName,
                            articlesId = newarticle.Id,
                            usersId = null,
                            isMain = false
                        });
                        _db.SaveChanges();
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        //selected categories helper function 
        public IEnumerable<articleCategories>  GetSelectedCatagories(int id)
        {
            var result = _db.articles.FirstOrDefault(e => e.Id == id);
            var categoryResult = _db.articleCategories.Where(e => e.articlesId == result.Id);
            return categoryResult;
        }

        //selected request helper function 
        public IEnumerable<requestsArticles> GetSelectedRequest(int id)
        {
            var result = _db.articles.FirstOrDefault(e => e.Id == id);
            var requestResult = _db.requestsArticles.Where(e => e.articlesId == result.Id);
            return requestResult;
        }

        public IEnumerable<articles> GetArticlesByCategoryId(int categoryId )
        {
            var result = _db.articleCategories.Where(e => e.categoriesId == categoryId);
            var articles = _db.articles.Where(e => result.Any(a => a.categoriesId == categoryId && a.articlesId == e.Id));
            return articles;
        }

        public IEnumerable<categories> GetCategoriesByArticleId(int articleId)
        {
            var result = _db.articleCategories.Where(e => e.articlesId == articleId);
            var categories = _db.categories.Where(e => result.Any(a => a.articlesId == articleId && a.categoriesId == e.Id));
            return categories;
        }

        //Edit 
        public void EditArticles(ArticlesCustomClass article)
        {
            var result = _db.articles.FirstOrDefault(e => e.Id == article.Id);
            var categoryResult = _db.articleCategories.Where(e => e.articlesId == article.Id);
            var requestResult = _db.requestsArticles.Where(e => e.articlesId == article.Id);
            if (!ExistCustomArticle(article) || result.title == article.Title)
            {         
                result.title = article.Title;
                result.content = article.Content;
                //result.usersId = article.UsersId;

            }
            _db.SaveChanges();


            //კატეგორიის ცვლილებას აკეთებს
            if (article.CategoriesList != null)
            {
                var notTheseCategoryIds = categoryResult.Select(e => e.categoriesId);
                var CategorySelects = article.CategoriesList.Where(f => !notTheseCategoryIds.Contains(f)).ToList();


                foreach (var categoryId in CategorySelects)
                {
                    _db.articleCategories.Add(
                   new articleCategories()
                   {
                       categoriesId = categoryId,
                       articlesId = result.Id
                   });
                    _db.SaveChanges();
                }
            }

            //რექუესტის ცვლილების აკეტებს
            if (article.RequestsList != null)
            {
                var notTheseRequestsIds = requestResult.Select(e => e.requestsId);
                var RequestsSelects = article.RequestsList.Where(f => !notTheseRequestsIds.Contains(f)).ToList();
                foreach (var requestID in RequestsSelects)
                {
                        _db.requestsArticles.Add(
                                   new requestsArticles()
                                   {
                                       requestsId = requestID,
                                       articlesId = result.Id
                                   });   
                }
                _db.SaveChanges();
            }


            if (article.Images != null)
            {
                var mapPath = HostingEnvironment.MapPath("~/images/");
                foreach (var file in article.Images)
                {
                    if (file != null)
                    {
                        file.SaveAs(mapPath + file.FileName);
                        _db.images.Add(new images() { name = file.FileName, url = "~/images/" + file.FileName,
                                                     articlesId = result.Id, usersId = null, isMain=false });
                        _db.SaveChanges();
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        // წაშლილი კატეგორიის ბაზიდან ამოშლის ფუნქცია
        public void DeleteUnselectedCategories(ArticlesCustomClass model, IEnumerable<int> PrevSelectedCategories)
        {
            var result = _db.articles.FirstOrDefault(e => e.Id == model.Id);
            var UnSelectedCategories = PrevSelectedCategories.Where(f => !model.CategoriesList.Contains(f)).ToList();
            var categoryResult = _db.articleCategories.Where(e => e.articlesId == result.Id);

            foreach (var category in UnSelectedCategories)
            {
                foreach (var item in categoryResult)
                {
                    if (category == item.categoriesId)
                    {
                        var UnSelectedCategoriesResult = _db.articleCategories.Where(e => e.categoriesId == category);
                        _db.articleCategories.RemoveRange(UnSelectedCategoriesResult);
                    }

                }
            }
            _db.SaveChanges();
        }

        // წაშლილი მოთხოვნის ბაზიდან ამოშლის ფუნქცია
        public void DeleteUnselectedRequest(ArticlesCustomClass model, IEnumerable<int> PrevSelectedRequests)
        {
            var result = _db.articles.FirstOrDefault(e => e.Id == model.Id);
            var requestResult = _db.requestsArticles.Where(e => e.articlesId == result.Id);
            if (model.RequestsList != null)
            {
                var UnSelectedRequests = PrevSelectedRequests.Where(f => !model.RequestsList.Contains(f)).ToList();
                foreach (var request in UnSelectedRequests)
                {
                    foreach (var item in requestResult)
                    {
                        if (request == item.requestsId)
                        {
                            var UnSelectedRequestsResult = _db.requestsArticles.Where(e => e.requestsId == request);
                            _db.requestsArticles.RemoveRange(UnSelectedRequestsResult);
                        }

                    }
                }
                _db.SaveChanges();
            }
            else // თუ არც ერთ მოთხოვნა აღარაა მულტისელექტში, ყველა მოთხოვნა იშლება  ბაზიდან
            {
                foreach (var request in PrevSelectedRequests)
                {
                    foreach (var item in requestResult)
                    {
                        if (request == item.requestsId)
                        {
                            var UnSelectedRequestsResult = _db.requestsArticles.Where(e => e.requestsId == request);
                            _db.requestsArticles.RemoveRange(UnSelectedRequestsResult);
                        }
                    }
                }
                _db.SaveChanges();
            }
        }



        ////Delete
        //public void deleteCategories(articles articles)
        //{
        //    var result = _db.articles.FirstOrDefault(e => e.Id == articles.Id);
        //    result.isBlocked = false;
        //    _db.SaveChanges();
        //}


        public void FullDeleteArticle(articles article)
        {
            //var article = _db.articles.FirstOrDefault(e => e.Id == model.Id);

            var result = _db.articleCategories.Where(e => e.categoriesId == article.Id).ToList();

            var deleteFavourite = _db.favourites.Where(e => e.articlesId == article.Id);
            var deleteImage = _db.images.Where(e => e.articlesId == article.Id);
            var deleteRating = _db.ratings.Where(e => e.articlesId == article.Id);
            var BlockArticlesRequests = _db.requestsArticles.Where(e => e.articlesId == article.Id);
            var deleteArticleTags = _db.articlesTags.Where(e => e.articlesId == article.Id);
            var deleteArticleCategories = _db.articleCategories.Where(e => e.articlesId == article.Id);
            var deleteArticle = _db.articles.Where(e => e.Id == article.Id);


            _db.favourites.RemoveRange(deleteFavourite);
            _db.images.RemoveRange(deleteImage);
            _db.ratings.RemoveRange(deleteRating);
          

            foreach(var artReq in BlockArticlesRequests)
            {
                artReq.articlesId = null;
            }

            _db.articlesTags.RemoveRange(deleteArticleTags);
            _db.articleCategories.RemoveRange(deleteArticleCategories);
            _db.articles.RemoveRange(deleteArticle);
            _db.SaveChanges();
        }


        public void DeleteImages(images img)
        {

           // var findImage = _db.images.FirstOrDefault(e => e.articlesId == article.Id);
            string fullPath = HostingEnvironment.MapPath(img.url);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            var deletableImages = _db.images.Where(e =>e.Id == img.Id);
            _db.images.RemoveRange(deletableImages);

            _db.SaveChanges();
        }

        public void MultiArticleDelete(int[] ids)
        {
            foreach(var id in ids)
            {
                
                var article = _db.articles.FirstOrDefault(e => e.Id ==id);
                FullDeleteArticle(article);
            }
        }

        public void BlockArticle(int id )
        {
            var article = _db.articles.FirstOrDefault(e => e.Id == id);
            article.isBlocked = true;
            _db.SaveChanges();
        }
        public void UnBlockArticle(int id)
        {
            var article = _db.articles.FirstOrDefault(e => e.Id == id);
            article.isBlocked = false;
            _db.SaveChanges();
        }

        public IEnumerable<articles> AllArticles()
        {
            return _db.articles;
        }

        public IEnumerable<articles> GetArticlesByUserId(int id)
        {
            var getUser = _db.users.FirstOrDefault(e => e.Id == id);
            return _db.articles.Where(e => e.usersId == getUser.Id);
        }

        public List<requests> GetRequestsByArticleId(int id)
        {
            var requestsResult = new List<requests>();
            var articleResult = _db.requestsArticles.Where(e => e.articlesId == id).ToList();

            if (articleResult.Count() != 0)
            {
                foreach (var req in _db.requests)
                {
                    foreach (var artReq in articleResult)
                    {
                        if (req.Id == artReq.requestsId)
                        {
                            requestsResult.Add(req);
                        }
                    }
                }
            }
            return requestsResult;
        }


        public List<categories> GetCategoriesByarticleId(int id)
        {
            var categoriesResult = new List<categories>();
            var result = GetArticlesById(id);
            var categoriesList = _db.categories.ToList();

            foreach (var cat in result.articleCategories)
                foreach (var category in categoriesList)
                {
                    if (cat.categoriesId == category.Id)
                    {
                        categoriesResult.Add(category);
                    }
                }
            return categoriesResult;
        }

        // data for products list model
        public IEnumerable<ArticlesCustomClass> GetArticleListModelData()
        {
            var model = new ArticlesListModel
            {
                ArticlesList = AllArticles().ToList().OrderByDescending(m => m.Id).Select(m => new ArticlesCustomClass()
                {
                    Id = m.Id,
                    Title = m.title,
                    Content = m.content,
                    Date = m.date,
                    User = m.users,
                    IsBlocked = m.isBlocked,
                    Ratings = GetRatingsByArticlesId(m.Id),
                    MainImagesList = GetMainImagesByArticleId(m.Id),
                    Categories = GetCategoriesByArticleId(m.Id).ToList(),
                }),

            };
            return model.ArticlesList;
        }


        // data for products list model
        public IEnumerable<ArticlesCustomClass> GetArticleListModelDataByCategory(int id)
        {
            var model = new ArticlesListModel
            {
                ArticlesList = GetArticlesByCategoryId(id).ToList().OrderByDescending(m => m.Id).Select(m => new ArticlesCustomClass()
                {
                    Id = m.Id,
                    Title = m.title,
                    Content = m.content,
                    Date = m.date,
                    User = m.users,
                    IsBlocked = m.isBlocked,
                    Ratings = GetRatingsByArticlesId(m.Id),
                    MainImagesList = GetMainImagesByArticleId(m.Id),
                    Categories = GetCategoriesByArticleId(m.Id).ToList(),
                }),

            };
            return model.ArticlesList;
        }

        // data for products list model
        public ArticlesCustomClass GetArticleModelData(int id)
        {
            //var model = new ArticlesCustomClass
            //{
            var result = GetArticlesById(id);
            var model= new ArticlesCustomClass()
            {
                Id = result.Id,
                Title = result.title,
                Content = result.content,
                Date = result.date,
                User = result.users,
                IsBlocked = result.isBlocked,
                Ratings = GetRatingsByArticlesId(result.Id),
                ImagesList = GetImagesByArticleId(result.Id),
                Categories = GetCategoriesByArticleId(result.Id).ToList(),
                MainImage= GetMainImageByArticleId(result.Id),
                Requests= GetRequestsByArticleId(result.Id)
            };
            //};
            return model;
        }

    }
}
