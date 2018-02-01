using System.Collections.Generic;
using System.ServiceModel;

namespace MyLib
{
    [ServiceContract]
    public interface IRemoteUpdate
    {
        /// <summary>
        /// Method for updating the project list.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<Project> updatedProjectList();

        [OperationContract]
        string updateProjectListAsString();
    }
}
