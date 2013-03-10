using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.GData;
using Google.Picasa;
using Google.GData.Client;
using Google.GData.Photos;
using Google.GData.Extensions;
using Google.GData.Extensions.Location;

namespace jamescms.Models
{
    public class PhotoRepository : IPhotoRepository
    {
        private PicasaService service = new PicasaService("jamessantiago-jamescms-1");

        public PhotoRepository()
        {
            service.setUserCredentials(GetGUsername(), GetGPassword());
        }

        public AtomEntryCollection GetAllAlbums()
        {
            AlbumQuery query = new AlbumQuery(PicasaQuery.CreatePicasaUri(GetGUsername()));
            PicasaFeed feed = service.Query(query);
            return feed.Entries;
        }

        public AtomEntryCollection GetAllPhotos(string id)
        {
            PhotoQuery query = new PhotoQuery(PicasaQuery.CreatePicasaUri(GetGUsername(), id) + "?imgmax=576");
            PicasaFeed feed = service.Query(query);
            return feed.Entries;
        }

        public PicasaEntry GetPhoto(string albumId, string photoTitle)
        {
            PhotoQuery query = new PhotoQuery(PicasaQuery.CreatePicasaUri(GetGUsername(), albumId));
            query.ExtraParameters = "imgmax=576";
            query.KindParameter = "";
            PicasaFeed feed = service.Query(query);
            var entry = feed.Entries.Where(d => d.Title.Text.ToLower() == photoTitle.ToLower()).FirstOrDefault();
            return (PicasaEntry)entry;
        }

        public string GetAllComments(string albumid, string photoid)
        {
            CommentsQuery query = new CommentsQuery(PicasaQuery.CreatePicasaUri(GetGUsername(), albumid, photoid));
            PicasaFeed feed = service.Query(query);
            if (feed.Entries.Count != 0)
            {

                int count = 0;
                string results = "";
                foreach (PicasaEntry entry in feed.Entries)
                {
                    if (count < 20)
                    {
                        results += "<p>" + entry.Content.Content + "</p>";
                        count++;
                    }
                }
                return results;
            }
            return "No Comments";
        }

        public void AddComment(string albumid, string photoid, string comment)
        {
            Uri postUri = new Uri(PicasaQuery.CreatePicasaUri("default", albumid, photoid));
            CommentEntry entry = new CommentEntry();
            entry.Content.Content = comment;
            PicasaEntry createdEntry = (PicasaEntry)service.Insert(postUri, entry);
        }

        public void ModifyAlbumSummary(string albumid, string summary)
        {
            AlbumQuery query = new AlbumQuery();
            PicasaEntry album = (PicasaEntry)service.Get(
                string.Format("http://picasaweb.google.com/data/entry/api/user/{0}/albumid/{1}", GetGUsername(), albumid)
                );

            album.Summary.Text = summary;
            album.Update();
        }

        private string GetGUsername()
        {
            return System.Configuration.ConfigurationManager.AppSettings["GoogleUsername"];
        }

        private string GetGPassword()
        {
            return System.Configuration.ConfigurationManager.AppSettings["GooglePassword"];
        }

        public bool isAuthorized(System.Security.Principal.IPrincipal User, string albumid)
        {
            if (User.IsInRole("Friends"))
                return true;           

            AlbumQuery query = new AlbumQuery();
            PicasaEntry album = (PicasaEntry)service.Get(
                string.Format("http://picasaweb.google.com/data/entry/api/user/{0}/albumid/{1}", GetGUsername(), albumid)
                );

            string summary = album.Summary.Text;

            if (summary.ToLower() == "coworkers" && User.IsInRole("Coworkers"))
                return true;

            if (summary.ToLower() == "all")
                return true;

            return false;
        }
    }
}