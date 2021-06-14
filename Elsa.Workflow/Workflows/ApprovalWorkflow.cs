using System.Net.Http;
using Elsa.Activities.Http;
using Elsa.Builders;
using Elsa.Serialization;
using Microsoft.Extensions.DependencyInjection;
namespace Elsa.Workflow.Workflows
{
    public class ApprovalWorkflow : IWorkflow
    {
        public void Build(IWorkflowBuilder builder)
        {
            var serializer = builder.ServiceProvider.GetRequiredService<IContentSerializer>();
            builder
                .WithDisplayName("Approval Workflow")
                .HttpEndpoint(activity => activity
                    .WithPath("/v1/approvals")
                    .WithMethod(HttpMethod.Post.Method)
                    .WithReadContent());
        }
    }
}
