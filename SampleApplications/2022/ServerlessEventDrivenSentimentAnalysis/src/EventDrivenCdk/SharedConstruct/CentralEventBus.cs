using System.Collections.Generic;
using System.Reflection.Metadata;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.StepFunctions;
using Amazon.CDK.AWS.StepFunctions.Tasks;
using Constructs;
using EventBus = Amazon.CDK.AWS.Events.EventBus;

namespace EventDrivenCdk.SharedConstruct
{
    public static class CentralEventBus
    {
        private static EventBus _centralEventBus;

        public static void AddCentralEventBus(EventBus bus)
        {
            _centralEventBus = bus;
        }
        
        public static Rule AddRule(Construct scope, string ruleName, string eventSource, string eventType, StateMachine workflow)
        {
            return new Rule(scope, ruleName, new RuleProps()
            {
                EventBus = _centralEventBus,
                RuleName = ruleName,
                EventPattern = new EventPattern()
                {
                    DetailType = new string[1] {eventType},
                    Source = new string[1] {eventSource},
                },
                Targets = new IRuleTarget[1]
                {
                    new SfnStateMachine(workflow)
                }
            });
        }
        
        public static Rule AddRule(Construct scope, string ruleName, string[] eventSource, string[] eventType, StateMachine workflow)
        {
            return new Rule(scope, ruleName, new RuleProps()
            {
                EventBus = _centralEventBus,
                RuleName = ruleName,
                EventPattern = new EventPattern()
                {
                    DetailType = eventType,
                    Source = eventSource
                },
                Targets = new IRuleTarget[1]
                {
                    new SfnStateMachine(workflow)
                }
            });
        }
        
        public static Rule AddRule(Construct scope, string ruleName, string[] eventSource, StateMachine workflow)
        {
            return new Rule(scope, ruleName, new RuleProps()
            {
                EventBus = _centralEventBus,
                RuleName = ruleName,
                EventPattern = new EventPattern()
                {
                    Source = eventSource
                },
                Targets = new IRuleTarget[1]
                {
                    new SfnStateMachine(workflow)
                }
            });
        }
        
        public static EventBridgePutEvents PublishEvent(Construct scope, string stepName, string eventSource, string eventName, TaskInput eventDetail)
        {
            return new EventBridgePutEvents(scope, stepName, new EventBridgePutEventsProps()
            {
                Entries = new EventBridgePutEventsEntry[1]
                {
                    new EventBridgePutEventsEntry
                    {
                        Detail = eventDetail,
                        DetailType = eventName,
                        Source = eventSource,
                        EventBus = _centralEventBus
                    }
                },
                ResultPath = "$.eventPublishResult"
            });
        }
    }
}