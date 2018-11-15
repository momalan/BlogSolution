using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BlogSolution.Models;

namespace BlogSolution.Controllers
{
    public class PostsController : ApiController
    {

        public IHttpActionResult Get(string tag = "")
        {
            using (BlogSolutionDBEntities enti = new BlogSolutionDBEntities())
            {
                List<Blog> blogs;
                if (string.IsNullOrEmpty(tag))
                {
                    blogs = enti.Blogs.ToList();
                }
                else
                {
                    blogs = enti.Blogs
                               .Include("Tags")
                               .Where(b => b.Tags.Any(t => t.nameOfTag == tag))
                               .OrderByDescending(t => t.createdAt)
                               .ToList();
                }
                List<BlogGetModel> list = blogs.Select(blog => new BlogGetModel
                {
                    slug = blog.slug,
                    title = blog.title,
                    body = blog.body,
                    description = blog.description,
                    createdAt = blog.createdAt,
                    updatedAt = blog.updatedAt,
                    tagList = blog.Tags.Select(t => t.nameOfTag).ToList()
                }).ToList();
                return Ok(list); ;



            }
        }

        public IHttpActionResult GetBlogBySlug(string slug)
        {
            using (BlogSolutionDBEntities enti = new BlogSolutionDBEntities())
            {
                var specificBlog = enti.Blogs.FirstOrDefault(x => x.slug == slug);
                if (specificBlog == null)
                {
                    return NotFound();
                }
                else
                {
                    BlogGetModel blogReturn = new BlogGetModel();
                    List<BlogGetModel> list = new List<BlogGetModel>();
                    List<string> listTags = new List<string>();
                    blogReturn.slug = specificBlog.slug;
                    blogReturn.title = specificBlog.title;
                    blogReturn.description = specificBlog.description;
                    blogReturn.body = specificBlog.body;
                    blogReturn.createdAt = specificBlog.createdAt;
                    blogReturn.updatedAt = specificBlog.updatedAt;
                    var tags = specificBlog.Tags.ToList();
                    foreach (var i in tags)
                    {
                        listTags.Add(i.nameOfTag);
                    }
                    blogReturn.tagList = listTags;
                    list.Add(blogReturn);
                    return Ok(list);
                }
            }
        }

        public IHttpActionResult Post([FromBody]JObject j)
        {
            using (BlogSolutionDBEntities enti = new BlogSolutionDBEntities())
            {
                dynamic x = JsonConvert.DeserializeObject<BlogGetModel>(j.ToString());
                Blog newBlog = new Blog();
                Tag tag = new Tag();
                newBlog.slug = Slug(x.title);
                newBlog.title = x.title;
                newBlog.description = x.description;
                newBlog.body = x.body;
                newBlog.createdAt = DateTime.Now;
                newBlog.updatedAt = DateTime.Now;
                var existingSlug = enti.Blogs.Where(s => s.slug == newBlog.slug).FirstOrDefault();
                if (existingSlug == null)
                {
                    enti.Blogs.Add(newBlog);
                    enti.SaveChanges();

                    for (int i = 0; i < x.tagList.Count; i++)
                    {
                        tag.nameOfTag = x.tagList[i];
                        var existingTag = enti.Tags.FirstOrDefault(y => y.nameOfTag == tag.nameOfTag);
                        if(existingTag == null)
                        {
                            tag.Blogs.Add(newBlog);
                            enti.Tags.Add(tag);
                            enti.SaveChanges();
                        }
                        else if (!existingTag.Blogs.Any(xx => xx.id == newBlog.id))
                        {
                            existingTag.Blogs.Add(newBlog);
                            enti.SaveChanges();
                        }
                    }
                }
                else
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "You already have the same title"));
                }
                return Ok();
            }
        }

        public IHttpActionResult Put(string slug, [FromBody]JObject j)
        {
            using (BlogSolutionDBEntities enti = new BlogSolutionDBEntities())
            {
                dynamic x = JsonConvert.DeserializeObject<BlogGetModel>(j.ToString());
                var blogForUpdate = enti.Blogs.FirstOrDefault(y => y.slug == slug);
                if (blogForUpdate == null)
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Blog for update not found."));
                Blog updatedBlog = new Blog();
                Tag t = new Tag();
                if (x.title != null)
                {
                    blogForUpdate.slug = Slug(x.title);
                    blogForUpdate.title = x.title;
                    var existingTitle = enti.Blogs.Where(s => s.title == blogForUpdate.title).FirstOrDefault();
                    if (existingTitle == null)
                    {
                        if (x.description != null)
                        {
                            blogForUpdate.description = x.description;
                        }

                        if (x.body != null)
                        {
                            blogForUpdate.body = x.body;
                        }
                        blogForUpdate.updatedAt = DateTime.Now;
                        enti.SaveChanges();
                    }
                    else
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "You already have the same title"));
                    }

                }
                else
                {
                    if (x.description != null)
                    {
                        blogForUpdate.description = x.description;
                    }
                    if (x.body != null)
                    {
                        blogForUpdate.body = x.body;
                    }

                    blogForUpdate.updatedAt = DateTime.Now;
                }
                if (x.tagList == null)
                {
                    enti.SaveChanges();
                }
                else
                {
                    for (int i = 0; i < x.tagList.Count; i++)
                    {
                        string tag = x.tagList[i];
                        var existingTag = enti.Tags.FirstOrDefault(y => y.nameOfTag == tag);
                        if (existingTag == null)
                        {
                            t.nameOfTag = x.tagList[i];
                            t.Blogs.Add(blogForUpdate);
                            enti.Tags.Add(t);
                            enti.SaveChanges();
                        }
                        else if (!existingTag.Blogs.Any(b => b.id == blogForUpdate.id))
                        {
                            existingTag.Blogs.Add(blogForUpdate);
                            enti.SaveChanges();
                        }
                    }
                }

                return Ok();

            }
        }

        public IHttpActionResult Delete(string slug)
        {
            using (BlogSolutionDBEntities enti = new BlogSolutionDBEntities())
            {
                var blogForDelete = enti.Blogs.Include("Tags").FirstOrDefault(x => x.slug == slug);
                if (blogForDelete == null)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Slug not found."));
                }
                else
                {
                    enti.Blogs.Remove(blogForDelete);
                    enti.SaveChanges();
                    return Ok();
                }
            }
        }

        [Route("api/tags")]
        public IQueryable<GetTagsModel> Get()
        {
            using (BlogSolutionDBEntities enti = new BlogSolutionDBEntities())
            {
                var tags = enti.Tags.ToList();
                return tags
                 .Select(p => new GetTagsModel
                 {
                     nameOfTag = p.nameOfTag
                 }).AsQueryable();
            }
        
        }
        public string Slug(string title)
        {
            string slug = "";
            for (int i = 0; i < title.Length; i++)
            {
                if (title[i].Equals(' '))
                {
                    slug = slug + "-";
                }
                else if (title[i].Equals(','))
                {
                    slug = slug + "";
                }
                else if (title[i].Equals(':'))
                {
                    slug = slug + "";
                }
                else if (title[i].Equals('/'))
                {
                    slug = slug + "";
                }
                else if (title[i].Equals('\''))
                {
                    slug = slug + "";
                }
                else
                {
                    slug += title[i];
                }
            }
            return slug.ToLower();
        }
    }
}
