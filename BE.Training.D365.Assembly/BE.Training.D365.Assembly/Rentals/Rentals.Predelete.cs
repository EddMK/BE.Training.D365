using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace BE.Training.D365.Assembly.Rentals
{
    internal class Rentals : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity target = (Entity)context.InputParameters["Target"];

                if (target.LogicalName == "training_rentals")
                {
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                    try
                    {
                        if (target.Contains("training_lk_videoid"))
                        {
                            EntityReference videoReference = (EntityReference)target.GetAttributeValue<EntityReference>("training_lk_videoid");

                            if (!videoReference.Equals(null))
                            {
                                tracingService.Trace("inside delete if");
                                Guid videoId = videoReference.Id;
                                Entity video = service.Retrieve("training_videos", videoId, new ColumnSet("training_nb_quantity", "training_str_title"));
                                int quantity = video.GetAttributeValue<int>("training_nb_quantity");
                                string title = video.GetAttributeValue<string>("training_str_title");
                                video["training_nb_quantity"] = quantity++;
                                service.Update(video);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                        throw;
                    }
                }
            }
        }
    }
}
