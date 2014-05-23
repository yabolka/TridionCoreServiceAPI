using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Tridion.ContentManager.CoreService.Client;
using TridionCoreServiceAPI.Models;

namespace TridionCoreServiceAPI.Repositories
{
    public class TridionItemRepository
    {
        private readonly string endpointName = "netTcp_2013";

        public TridionItemRepository()
        {
            this.Client = new SessionAwareCoreServiceClient(endpointName);
        }

        public TridionItemRepository(string endpointName)
        {
            this.endpointName = endpointName;
            this.Client = new SessionAwareCoreServiceClient(endpointName);
        }

        private SessionAwareCoreServiceClient Client { get; set; }

        public TridionItem GetByUri(string uri)
        {
            TridionItem tridionItem = new TridionItem();

            try
            {
                IdentifiableObjectData objectData = this.Client.Read(uri, null) as IdentifiableObjectData;
                FullVersionInfo versionInfo = objectData.VersionInfo as FullVersionInfo;

                tridionItem.Title = objectData.Title;
                tridionItem.Uri = uri;
                tridionItem.LastModifiedBy = versionInfo.Revisor.Title;
                tridionItem.IsPublished = IsPublished(uri);
            }
            catch (Exception ex)
            {

                tridionItem.Error = ex.Source + ", " + ex.Message + ", " + ex.ToString();
            }            

            return tridionItem;
        }

        public XElement GetLocalizedItems(string uri)
        {
            XElement usingXML = null;
            try
            {
                // original code from http://www.tridiondeveloper.com/getting-used-items-using-the-core-service
                // Create a filter
                UsingItemsFilterData usingItemsFilterData = new UsingItemsFilterData
                {
                    BaseColumns = ListBaseColumns.IdAndTitle, // to specify the detail in the XML
                    IncludeLocalCopies = true,
                    ItemTypes = new[] { ItemType.Component }
                };
                // Get the XML by calling .GetListXml on the client (assumes you have a 'client' object already)
                usingXML = this.Client.GetListXml(uri, usingItemsFilterData);

            }
            catch (Exception ex)
            {
            }
            return usingXML;
        }

        public void UnlocalizeItem(string uri)
        {
            RepositoryLocalObjectData objectData = this.Client.Read(uri, null) as RepositoryLocalObjectData;

            if (objectData.BluePrintInfo.IsLocalized != null && (bool)objectData.BluePrintInfo.IsLocalized)
            {
                this.Client.UnLocalize(uri, null);
            }
        }

        public void DeleteComponent(string uri)
        {
            try
            {
                this.Client.Delete(uri);
            }
            catch (Exception ex)
            {                
                throw new ArgumentException("The component with id {0} could not be deleted. Make sure it is a valid ID, or that the component is not published!", ex.InnerException);
            }
            
        }

        public bool IsPublished(string uri)
        {
            bool componentIsPublished = false;
            var publicationTargets = Client.GetSystemWideList(new PublicationTargetsFilterData());
            foreach (var target in publicationTargets)
            {
                if (this.Client.IsPublished(uri, target.Id, false))
                {
                    componentIsPublished = true;
                    break;
                }
            }

            return componentIsPublished;
        }
    }
}