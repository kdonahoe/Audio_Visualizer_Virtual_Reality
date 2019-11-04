using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using Newtonsoft.Json;
using UnityEngine;
using static LyricResponse;

public class WebUtils
{
    public LyricResponse searchSongs(string song)
    {
        try
        {
            string response = SendRequestToSpotify("GET", Properties.genius_search_url + song, "");
            return JsonConvert.DeserializeObject<LyricResponse>(response);
        }
        catch
        {
            return null;
        }
    }

    public List<string> getTopLyrics(string song)
    {
        List<string> lyrics = new List<string>(); 
        try
        {
            string txtResponse = SendRequestToSpotify("GET", Properties.genius_search_url + song, "");
            LyricResponse response = JsonConvert.DeserializeObject<LyricResponse>(txtResponse);
            if (response != null)
            {
                if (response.response.hits.Count > 0)
                {
                    HtmlWeb web = new HtmlWeb();
                    HtmlDocument doc = web.Load(response.response.hits[0].result.url);
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class='lyrics']//p").Elements())
                    {
                        if (!string.IsNullOrWhiteSpace(node.InnerHtml))
                        {
                            lyrics.Add(node.InnerHtml);
                        }
                    }
                    return lyrics;
                }
            }
            return null;
        }
        catch(Exception ex)
        {
            Debug.Log(ex.Message);
            return null;
        }
    }

    public string SendRequestToSpotify(String method, String url, String jsonBody)
    {
        HttpWebRequest request;
        request = (HttpWebRequest)WebRequest.Create(url);
        // Add the method to authentication the user
        request.Method = method;
        request.Accept = "application/json";
        request.Headers.Add("Authorization", "Bearer " + Properties.access_token);
        request.ContentLength = jsonBody.Length;
        request.Timeout = 14400000;
        request.KeepAlive = false;
        string body = jsonBody;

        byte[] bodyBytes = Encoding.UTF8.GetBytes(body);

        if (!string.IsNullOrEmpty(body))
        {
            request.ContentType = "application/json";
            request.ContentLength = bodyBytes.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bodyBytes, 0, bodyBytes.Length);
            requestStream.Close();
        }
        try
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
