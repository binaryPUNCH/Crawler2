﻿using CrawlerLibrary.Models;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using SolrNet;
using SolrNet.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CrawlerMVC.Controllers.api
{
    public class Link
    {
        public string URL;
        public string Text;
        public string Title;
    }

    public class SearchController : ApiController
    {
        [Route("api/Search/{query}")]
        public IHttpActionResult Get(string query)
        {
            using (var client = new WebClient())
            {
                Uri uri = new Uri(string.Format("http://176.23.159.28:8983/solr/testcore/query?q=p:{0}&hl=true&hl.fl=p&hl.fragsize=500&fl=id+title+resourcename", query));
                try
                {
                    var jsonReturn = client.DownloadString(uri);
                    var dyn = JsonConvert.DeserializeObject<dynamic>(jsonReturn);
                    List<Link> linkList = new List<Link>();
                    foreach (dynamic i in dyn.response.docs)
                    {
                        Link temp = new Link();
                        temp.URL = i.resourcename[0];
                        temp.Title = i.title[0];
                        temp.Text = dyn.highlighting[i.id.ToString()].p[0];
                        linkList.Add(temp);
                    }
                    return Ok(linkList);
                }
                catch (WebException e)
                {
                    return BadRequest();
                }
            }
        }
    }
}