using System;
using System.Xml;
using System.Web;
using System.Web.Security;
using Thon.ZaszBlog.Support.CodedRepresentations;
using System.Threading;

namespace Thon.ZaszBlog.Support.Web.HttpHandlers
{
	// Exports all posts to the BlogML xml format
	public class BlogMLExportHandler : IHttpHandler
	{
		#region IHttpHandler Members
		public void ProcessRequest(HttpContext context)
		{
			if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
			{
				context.Response.ContentType = "text/xml";
				context.Response.AppendHeader("Content-Disposition", "attachment; filename=BlogML.xml");
				WriteXml(context);
			}
			else
			{
				context.Response.StatusCode = 403;//Access denied.
			}
		}
		public bool IsReusable
		{
			get { return false; }
		}
		#endregion

		#region XML creation

		private static void WriteXml(HttpContext context)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = System.Text.Encoding.UTF8;
			settings.Indent = true;

			using (XmlWriter writer = XmlWriter.Create(context.Response.OutputStream, settings))
			{
				writer.WriteStartElement("blog", "http://www.blogml.com/2006/09/BlogML");
				writer.WriteAttributeString("root-url", SupportUtilities.RelativeWebRoot.ToString());
				writer.WriteAttributeString("date-created", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
				writer.WriteAttributeString("xmlns", "xs", null, "http://www.w3.org/2001/XMLSchema");

				AddTitle(writer);
				AddSubTitle(writer);
				AddAuthors(writer);
				AddExtendedProperties(writer);
				AddCategories(writer);
				AddPosts(writer);

				writer.WriteEndElement();
			}
		}

		private static void AddTitle(XmlWriter writer)
		{
			writer.WriteStartElement("title");
			writer.WriteAttributeString("type", "text");
			writer.WriteCData(BlogSettings.Instance.Name);
			writer.WriteEndElement();
		}

		private static void AddSubTitle(XmlWriter writer)
		{
			writer.WriteStartElement("sub-title");
			writer.WriteAttributeString("type", "text");
			writer.WriteCData(BlogSettings.Instance.Description);
			writer.WriteEndElement();
		}

		private static void AddAuthors(XmlWriter writer)
		{
			writer.WriteStartElement("authors");

			foreach (MembershipUser user in Membership.GetAllUsers())
			{
				writer.WriteStartElement("author");

				writer.WriteAttributeString("id", user.UserName);
				writer.WriteAttributeString("date-created", user.CreationDate.ToString("yyyy-MM-ddTHH:mm:ss"));
				writer.WriteAttributeString("date-modified", user.CreationDate.ToString("yyyy-MM-ddTHH:mm:ss"));
				writer.WriteAttributeString("approved", "true");
				writer.WriteAttributeString("email", user.Email);

				writer.WriteStartElement("title");
				writer.WriteAttributeString("type", "text");
				writer.WriteCData(user.UserName);
				writer.WriteEndElement();

				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}

		private static void AddExtendedProperties(XmlWriter writer)
		{
			writer.WriteStartElement("extended-properties");

			writer.WriteStartElement("property");
			writer.WriteAttributeString("name", "CommentModeration");
			writer.WriteAttributeString("value", "Anonymous");
			writer.WriteEndElement();

			writer.WriteStartElement("property");
			writer.WriteAttributeString("name", "SendTrackback");
			writer.WriteAttributeString("value", BlogSettings.Instance.EnableTrackBackSend ? "Yes" : "No");
			writer.WriteEndElement();

			writer.WriteEndElement();
		}

		private static void AddCategories(XmlWriter writer)
		{
			writer.WriteStartElement("categories");

			foreach (Category category in Category.Categories)
			{
				writer.WriteStartElement("category");

				writer.WriteAttributeString("id", category.Id.ToString());
				writer.WriteAttributeString("date-created", category.DateCreated.ToString("yyyy-MM-ddTHH:mm:ss"));
				writer.WriteAttributeString("date-modified", category.DateModified.ToString("yyyy-MM-ddTHH:mm:ss"));
				writer.WriteAttributeString("approved", "true");
				writer.WriteAttributeString("parentref", "0");

				if (!String.IsNullOrEmpty(category.Description))
				{
					writer.WriteAttributeString("description", category.Description);
				}

				writer.WriteStartElement("title");
				writer.WriteAttributeString("type", "text");
				writer.WriteCData(category.Title);
				writer.WriteEndElement();

				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}

		private static void AddPosts(XmlWriter writer)
		{
			writer.WriteStartElement("posts");

			foreach (Post post in Post.Posts)
			{
				writer.WriteStartElement("post");

				writer.WriteAttributeString("id", post.Id.ToString());
				writer.WriteAttributeString("date-created", post.DateCreated.ToString("yyyy-MM-ddTHH:mm:ss"));
				writer.WriteAttributeString("date-modified", post.DateModified.ToString("yyyy-MM-ddTHH:mm:ss"));
				writer.WriteAttributeString("approved", "true");
				writer.WriteAttributeString("post-url", post.RelativeLink.ToString());
				writer.WriteAttributeString("type", "normal");
				writer.WriteAttributeString("hasexcerpt", (!string.IsNullOrEmpty(post.Description)).ToString().ToLowerInvariant());
				writer.WriteAttributeString("views", "0");

				AddPostTitle(writer, post);
				AddPostContent(writer, post);
				AddPostName(writer, post);
				AddPostExcerpt(writer, post);
				AddPostAuthor(writer, post);
				AddPostCategories(writer, post);
				AddPostTags(writer, post);
				AddPostComments(writer, post);
				AddPostTrackbacks(writer, post);

				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}

		#region Add post elements

		private static void AddPostTitle(XmlWriter writer, Post post)
		{
			writer.WriteStartElement("title");
			writer.WriteAttributeString("type", "text");
			writer.WriteCData(post.Title);
			writer.WriteEndElement();
		}

		private static void AddPostContent(XmlWriter writer, Post post)
		{
			writer.WriteStartElement("content");
			writer.WriteAttributeString("type", "text");
			writer.WriteCData(post.Content);
			writer.WriteEndElement();
		}

		private static void AddPostName(XmlWriter writer, Post post)
		{
			writer.WriteStartElement("post-name");
			writer.WriteAttributeString("type", "text");
			writer.WriteCData(post.Title);
			writer.WriteEndElement();
		}

		private static void AddPostExcerpt(XmlWriter writer, Post post)
		{
			if (!String.IsNullOrEmpty(post.Description))
			{
				writer.WriteStartElement("excerpt");
				writer.WriteAttributeString("type", "text");
				writer.WriteCData(post.Description);
				writer.WriteEndElement();
			}
		}

		private static void AddPostAuthor(XmlWriter writer, Post post)
		{
			writer.WriteStartElement("authors");
			writer.WriteStartElement("author");
			writer.WriteAttributeString("ref", post.Author);
			writer.WriteEndElement();
			writer.WriteEndElement();
		}

		private static void AddPostCategories(XmlWriter writer, Post post)
		{
			if (post.Categories.Count == 0)
				return;

			writer.WriteStartElement("categories");
			foreach (Category category in post.Categories)
			{
				writer.WriteStartElement("category");
				writer.WriteAttributeString("ref", category.Id.ToString());
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		private static void AddPostTags(XmlWriter writer, Post post)
		{
			if (post.Tags.Count == 0)
				return;

			writer.WriteStartElement("tags");
			foreach (string tag in post.Tags)
			{
				writer.WriteStartElement("tag");
				writer.WriteAttributeString("ref", tag);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		private static void AddPostComments(XmlWriter writer, Post post)
		{
			if (post.Comments.Count == 0)
				return;

			writer.WriteStartElement("comments");
			foreach (Comment comment in post.Comments)
			{
				if (comment.Email == "trackback" || comment.Email == "pingback")
					continue;

				writer.WriteStartElement("comment");
				writer.WriteAttributeString("id", comment.Id.ToString());
				writer.WriteAttributeString("date-created", comment.DateCreated.ToString("yyyy-MM-ddTHH:mm:ss"));
				writer.WriteAttributeString("date-modified", comment.DateCreated.ToString("yyyy-MM-ddTHH:mm:ss"));
				writer.WriteAttributeString("approved", comment.IsApproved.ToString().ToLowerInvariant());
				writer.WriteAttributeString("user-name", comment.Author);

				if (comment.Website != null)
				{
					writer.WriteAttributeString("user-url", comment.Website.ToString());
				}
				else
				{
					writer.WriteAttributeString("user-url", "");
				}

				writer.WriteStartElement("title");
				writer.WriteAttributeString("type", "text");
				writer.WriteCData("re: " + post.Title);
				writer.WriteEndElement();

				writer.WriteStartElement("content");
				writer.WriteAttributeString("type", "text");
				writer.WriteCData(comment.Content);
				writer.WriteEndElement();

				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		private static void AddPostTrackbacks(XmlWriter writer, Post post)
		{
			if (post.Comments.Count == 0)
				return;

			writer.WriteStartElement("trackbacks");
			foreach (Comment comment in post.Comments)
			{
				if (comment.Email != "trackback" || comment.Email != "pingback")
					continue;

				writer.WriteStartElement("trackback");
				writer.WriteAttributeString("id", comment.Id.ToString());
				writer.WriteAttributeString("date-created", comment.DateCreated.ToString("yyyy-MM-ddTHH:mm:ss"));
				writer.WriteAttributeString("date-modified", comment.DateCreated.ToString("yyyy-MM-ddTHH:mm:ss"));
				writer.WriteAttributeString("approved", comment.IsApproved.ToString().ToLowerInvariant());
				writer.WriteAttributeString("url", comment.Website.ToString());

				writer.WriteStartElement("title");
				writer.WriteAttributeString("type", "text");
				writer.WriteCData(comment.Content);
				writer.WriteEndElement();

				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		#endregion

		#endregion

	}
}