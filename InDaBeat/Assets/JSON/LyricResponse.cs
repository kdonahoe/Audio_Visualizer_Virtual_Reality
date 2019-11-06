using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LyricResponse
{
    public Meta meta { get; set; }
    public Response response { get; set; }

    public class Meta
    {
        public int status { get; set; }
    }

    public class Stats
    {
        public int unreviewed_annotations { get; set; }
        public int concurrents { get; set; }
        public bool hot { get; set; }
        public int pageviews { get; set; }
    }

    public class PrimaryArtist
    {
        public string api_path { get; set; }
        public string header_image_url { get; set; }
        public int id { get; set; }
        public string image_url { get; set; }
        public bool is_meme_verified { get; set; }
        public bool is_verified { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public int iq { get; set; }
    }

    public class LyricObject
    {
        public int annotation_count { get; set; }
        public string api_path { get; set; }
        public string full_title { get; set; }
        public string header_image_thumbnail_url { get; set; }
        public string header_image_url { get; set; }
        public int id { get; set; }
        public int lyrics_owner_id { get; set; }
        public string lyrics_state { get; set; }
        public string path { get; set; }
        public string pyongs_count { get; set; }
        public string song_art_image_thumbnail_url { get; set; }
        public string song_art_image_url { get; set; }
        public Stats stats { get; set; }
        public string title { get; set; }
        public string title_with_featured { get; set; }
        public string url { get; set; }
        public PrimaryArtist primary_artist { get; set; }
    }

    public class Hit
    {
        public List<object> highlights { get; set; }
        public string index { get; set; }
        public string type { get; set; }
        public LyricObject result { get; set; }
    }

    public class Response
    {
        public List<Hit> hits { get; set; }
    }
}
