using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;

namespace BE.Training.D365.Assembly
{
    public class RentalsPrecreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext) serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity target = (Entity)context.InputParameters["Target"];

                if(target.LogicalName == "training_rentals")
                {
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                    try
                    {
                        tracingService.Trace("DEBUT DU PLUGIN");
                        // Plug-in business logic goes here.
                        if (target.Contains("training_lk_videoid"))
                        {
                            Guid videoId = target.GetAttributeValue<EntityReference>("training_lk_videoid").Id;

                            if (!string.IsNullOrEmpty(videoId.ToString()))
                            {
                                Entity videoReference = service.Retrieve("training_videos", videoId, new ColumnSet("training_nb_quantity", "training_str_title"));
                                tracingService.Trace(videoId.ToString());
                                int quantity = videoReference.GetAttributeValue<int>("training_nb_quantity");
                                string title = videoReference.GetAttributeValue<string>("training_str_title");
                                //training_str_title
                                tracingService.Trace(""+quantity);
                                if ((quantity == 0))
                                {
                                    tracingService.Trace("Inside If");
                                    throw new InvalidPluginExecutionException($"No more items available for the selected movie : {title} !");
                                }
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
