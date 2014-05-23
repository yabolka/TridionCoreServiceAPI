using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Xml.Linq;
using TridionCoreServiceAPI.Models;
using TridionCoreServiceAPI.Repositories;

namespace TridionCoreServiceAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {
        private TridionItemRepository repository = new TridionItemRepository();

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [ActionName("GetItem")]
        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            TridionItem item = repository.GetByUri("tcm:" + id);
            if (!string.IsNullOrEmpty(item.Error))
            {
                return NotFound();
            }

            return Ok(item);
        }

        [ActionName("GetLocalized")]
        [HttpGet]
        public IHttpActionResult GetLocalizedItems(string id)
        {
            
            XElement localItem = repository.GetLocalizedItems("tcm:" + id);
            if (localItem == null)
            {
                return NotFound();
            }
            return Ok(localItem);
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/delete/5
        public HttpResponseMessage Delete(string id)
        {
            XElement localItem = repository.GetLocalizedItems("tcm:" + id);
            if (localItem == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            if (repository.IsPublished("tcm:" + id))
            {
                return new HttpResponseMessage(HttpStatusCode.Forbidden);
            }

            var childNodes = localItem.Elements();
            if (childNodes.Count() != 0)
            {
                bool hasPublishedChildren = false;
                foreach (var node in childNodes)
                {
                    if (repository.IsPublished(node.Attribute("ID").Value))
                    {
                        hasPublishedChildren = true;
                        break;
                    }
                }

                if (hasPublishedChildren)
                {
                    return new HttpResponseMessage(HttpStatusCode.Forbidden);
                }

                foreach (var node in childNodes)
                {
                    repository.UnlocalizeItem(node.Attribute("ID").Value);
                    repository.DeleteComponent(node.Attribute("ID").Value);
                }                           
            }

            repository.UnlocalizeItem("tcm:" + id);
            repository.DeleteComponent("tcm:" + id);
            
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
