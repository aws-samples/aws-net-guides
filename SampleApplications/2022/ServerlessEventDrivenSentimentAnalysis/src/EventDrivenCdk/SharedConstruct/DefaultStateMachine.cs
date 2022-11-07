using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.StepFunctions;
using Constructs;

namespace EventDrivenCdk.SharedConstruct
{
    public class DefaultStateMachine: StateMachine
    {
        public DefaultStateMachine(Construct scope, string id, IChainable definition, StateMachineType type) : base(scope, id, new StateMachineProps()
        {
            Definition = definition,
            Logs = new LogOptions()
            {
                Destination = new LogGroup(scope, $"{id}LogGroup", new LogGroupProps()
                {
                    Retention = RetentionDays.ONE_DAY,
                    LogGroupName = $"{id}LogGroup"
                }),
                Level = LogLevel.ALL
            },
            TracingEnabled = true,
            StateMachineType = type,
        })
        {
        }
    }
}