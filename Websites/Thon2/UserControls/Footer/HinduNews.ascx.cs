﻿using System;
using System.Xml;
using System.Web.UI.HtmlControls;

public partial class HinduNewsAscx : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        System.Net.WebClient wc = new System.Net.WebClient();
        string eksemmelle = "";
        wc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
        try
        {
            eksemmelle = wc.DownloadString("http://www.hindu.com/rss/01hdline.xml");
        }
        catch (Exception e1)
        {
            e1 = null;
        }
        if (!string.IsNullOrEmpty(eksemmelle))
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(eksemmelle);
            XmlNodeList list = doc.SelectNodes("rss/channel/item");
            HtmlGenericControl hgc;
            HtmlAnchor anc;
            foreach (XmlNode node in list)
                RenderNewsItem(node.SelectSingleNode("title").InnerText, node.SelectSingleNode("link").InnerText, node.SelectSingleNode("description").InnerText, node.SelectSingleNode("pubDate").InnerText);
        }
    }

    private void RenderNewsItem(string title, string link, string content, string date)
    {
        NewsItemAscx item = (NewsItemAscx)LoadControl("~/UserControls/Footer/NewsItem.ascx");
        item.Title= title;
        item.Link =link;
        item.Content = content;
        item.Date = date;
        nph.Controls.Add(item);
    }

}


 
        