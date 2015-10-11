using Moriyama.Runtime.Models;
using Moriyama.Runtime.Umbraco.Interfaces;

namespace Moriyama.Runtime.Umbraco.Application.DeploymentAdapter
{
    public class DummyDeploymentAdapter : IDeploymentAdapter
    {
        public void DeployContent(RuntimeContentModel model, DeploymentAction action)
        {
            
        }
    }
}
