using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.GData.Client;

namespace jamescms.Models
{
    public interface IPhotoRepository
    {
        AtomEntryCollection GetAllAlbums();
        AtomEntryCollection GetAllPhotos(String id);
        AtomEntryCollection GetPhoto(string albumId, string photoId);
        String GetAllComments(String albumid, String photoid);
        void AddComment(String albumid, String photoid, String comment);
        void ModifyAlbumSummary(string albumid, string summary);
    }
}