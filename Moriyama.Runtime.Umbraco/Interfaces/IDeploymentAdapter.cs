using Moriyama.Runtime.Models;

namespace Moriyama.Runtime.Umbraco.Interfaces
{
    public enum DeploymentAction
    {
        Deploy, Delete
    }

    public interface IDeploymentAdapter
    {
        void DeployContent(RuntimeContentModel model, DeploymentAction action);
    }
}