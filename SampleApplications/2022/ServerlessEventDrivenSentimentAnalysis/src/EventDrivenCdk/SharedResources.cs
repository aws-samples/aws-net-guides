using Amazon.CDK;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.EventSchemas;
using Constructs;

namespace EventDrivenCdk
{
    public class SharedResources : Construct
    {
        public EventBus CentralEventBus { get; private set; }
        
        public SharedResources(Construct scope, string id) : base(scope, id)
        {
            var centralEventBridge = new EventBus(this, "CentralEventBridge", new EventBusProps()
            {
                EventBusName = "CentralEventBus",
            });

            this.CentralEventBus = centralEventBridge;

            var schemaDiscovery = new CfnDiscoverer(this, "EventBridgeDiscovery", new CfnDiscovererProps()
            {
                SourceArn = centralEventBridge.EventBusArn,
                CrossAccount = false,
                Description = "Discovery for central event bus"
            });

            SharedConstruct.CentralEventBus.AddCentralEventBus(this.CentralEventBus);
        }
    }
}