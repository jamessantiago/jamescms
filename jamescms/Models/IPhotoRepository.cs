using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.GData.Client;
using Google.GData.Photos;

namespace jamescms.Models
{
    public interface IPhotoRepository
    {
        AtomEntryCollection GetAllAlbums();
        AtomEntryCollection GetAllPhotos(string id);
        PicasaEntry GetPhoto(string albumId, string photoTitle);
        String GetAllComments(String albumid, String photoid);
        void AddComment(String albumid, String photoid, String comment);
        void ModifyAlbumSummary(string albumid, string summary);
    }
}